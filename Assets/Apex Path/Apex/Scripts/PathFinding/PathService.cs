/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Threading;
    using Apex.DataStructures;
    using Apex.Utilities;
    using Apex.WorldGeometry;

    /// <summary>
    /// The path service. This service will satisfy <see cref="IPathRequest"/>s
    /// </summary>
    public sealed class PathService : IPathService
    {
        private const int StartQueueSize = 10;

        private Stopwatch _stopwatch;
        private PriorityQueueFifo<IPathRequest> _queue;
        private IPathingEngine _engine;
        private IGridManager _gridManager;
        private IDirectPather _directPather;
        private IThreadFactory _threadFactory;
        private bool _threadPoolSupported = true;

        private Thread _dedicatedThread;
        private AutoResetEvent _waitHandle;
        private bool _processingActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathService"/> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="directPather">The direct pather.</param>
        /// <param name="gridManager">The grid manager.</param>
        /// <param name="threadFactory">The thread factory.</param>
        public PathService(IPathingEngine engine, IDirectPather directPather, IGridManager gridManager, IThreadFactory threadFactory)
            : this(engine, directPather, gridManager, threadFactory, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathService"/> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="directPather">The direct pather.</param>
        /// <param name="gridManager">The grid manager.</param>
        /// <param name="threadFactory">The thread factory.</param>
        /// <param name="useThreadPoolForAsync">if set to <c>true</c> the thread pool will be used for async operation.</param>
        public PathService(IPathingEngine engine, IDirectPather directPather, IGridManager gridManager, IThreadFactory threadFactory, bool useThreadPoolForAsync)
        {
            Ensure.ArgumentNotNull(engine, "engine");
            Ensure.ArgumentNotNull(directPather, "directPather");
            Ensure.ArgumentNotNull(gridManager, "gridManager");
            Ensure.ArgumentNotNull(threadFactory, "threadFactory");

            _stopwatch = new Stopwatch();
            _queue = new PriorityQueueFifo<IPathRequest>(StartQueueSize, QueueType.Max);
            _engine = engine;
            _gridManager = gridManager;
            _directPather = directPather;
            _threadFactory = threadFactory;

            _threadPoolSupported = useThreadPoolForAsync;
        }

        /// <summary>
        /// Occurs if asynchronous processing failed.
        /// </summary>
        public event EventHandler AsyncFailed;

        /// <summary>
        /// Gets or sets a value indicating whether to run asynchronously.
        /// </summary>
        /// <value>
        ///   <c>true</c> if to run asynchronously; otherwise, <c>false</c>.
        /// </value>
        public bool runAsync
        {
            get;
            set;
        }

        /// <summary>
        /// Queues a request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void QueueRequest(IPathRequest request)
        {
            QueueRequest(request, 0);
        }

        /// <summary>
        /// Queues a request with a priority. Higher values take priority.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="priority">The priority.</param>
        public void QueueRequest(IPathRequest request, int priority)
        {
            Ensure.ArgumentNotNull(request, "request");

            lock (_queue)
            {
                _queue.Enqueue(request, priority);

                if (this.runAsync && !_processingActive)
                {
                    StartAyncProcessing();
                }
            }
        }

        /// <summary>
        /// Processes queued requests as a coroutine.
        /// </summary>
        /// <param name="maxMillisecondsPerFrame">The maximum milliseconds per frame before yielding.</param>
        /// <returns>
        /// coroutine enumerator
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Cannot process as coroutine when set to async operation.</exception>
        public IEnumerator ProcessRequests(int maxMillisecondsPerFrame)
        {
            if (this.runAsync)
            {
                throw new InvalidOperationException("Cannot process as coroutine when set to async operation.");
            }

            while (!this.runAsync)
            {
                var next = GetNext();
                if (next == null)
                {
                    yield return null;
                }
                else
                {
                    _stopwatch.Start();

                    next = _directPather.ResolveDirectPath(next);

                    if (_stopwatch.ElapsedMilliseconds > maxMillisecondsPerFrame)
                    {
                        _stopwatch.Reset();
                        yield return null;
                    }

                    if (next == null)
                    {
                        continue;
                    }

                    var run = true;
                    var subIter = _engine.ProcessRequestCoroutine(next);

                    while (run)
                    {
                        //Start is called multiple places, due to the enumeration going on in various loops. Start is safe to call multiple times, it will simply do nothing if already started.
                        _stopwatch.Start();
                        run = subIter.MoveNext();

                        if (_stopwatch.ElapsedMilliseconds > maxMillisecondsPerFrame)
                        {
                            _stopwatch.Reset();
                            yield return null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Processes queued requests synchronously.
        /// </summary>
        public void ProcessRequests()
        {
            var next = GetNext();
            while (next != null)
            {
                next = _directPather.ResolveDirectPath(next);
                if (next != null)
                {
                    _engine.ProcessRequest(next);
                }

                next = GetNext();
            }
        }

        private void ProcessRequestsAsync(object ignored)
        {
            ProcessRequests();
        }

        private void ProcessRequestsAsyncDedicated(object ignored)
        {
            try
            {
                while (this.runAsync)
                {
                    ProcessRequests();
                    _waitHandle.WaitOne();
                }
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch
            {
                //Since this isn't exactly something we expect to happen, just null the thread ref so that the next time a request is made the thread is tarted again.
                _processingActive = false;
                _dedicatedThread = null;
            }
        }

        private void StartAyncProcessing()
        {
            _processingActive = true;

            if (_threadPoolSupported)
            {
                try
                {
                    _threadPoolSupported = ThreadPool.QueueUserWorkItem(ProcessRequestsAsync);
                }
                catch (NotSupportedException)
                {
                    _threadPoolSupported = false;
                }
            }

            if (!_threadPoolSupported)
            {
                if (_dedicatedThread != null)
                {
                    _waitHandle.Set();
                    return;
                }

                try
                {
                    if (_waitHandle == null)
                    {
                        _waitHandle = new AutoResetEvent(true);
                    }

                    var t = _threadFactory.CreateThread("Pathing", ProcessRequestsAsyncDedicated);
                    t.Priority = ThreadPriority.BelowNormal;
                    t.IsBackground = true;

                    _dedicatedThread = t;
                    t.Start();
                }
                catch
                {
                    this.runAsync = false;

                    if (_waitHandle != null)
                    {
                        _waitHandle.Close();
                    }

                    _processingActive = false;
                    _dedicatedThread = null;
                    _waitHandle = null;

                    var e = AsyncFailed;
                    if (e != null)
                    {
                        e(this, EventArgs.Empty);
                    }
                }
            }
        }

        private IPathRequest GetNext()
        {
            lock (_queue)
            {
                while (_queue.hasNext)
                {
                    var next = _queue.Dequeue();

                    if (!next.hasDecayed)
                    {
                        //resolve the grid
                        if (next.fromGrid == null || next.toGrid == null)
                        {
                            _gridManager.InjectGrids(next.from, next.to, next);
                        }

                        return next;
                    }

                    next.Complete(PathingStatus.Decayed, null);
                }

                _processingActive = false;
            }

            return null;
        }

        /// <summary>
        /// Releases handles. The path service is void after being disposed.
        /// </summary>
        public void Dispose()
        {
            lock (_queue)
            {
                _queue.Clear();
                this.runAsync = false;
            }

            if (_waitHandle != null)
            {
                _waitHandle.Set();
                _waitHandle.Close();
            }

            _dedicatedThread = null;
            _waitHandle = null;
        }
    }
}

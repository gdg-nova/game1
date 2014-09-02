/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    using System;
    using Apex.PathFinding.MoveCost;
    using Apex.Services;
    using Apex.Utilities;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Component for configuring the <see cref="PathService"/>
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Basics/Path Service")]
    public sealed class PathServiceComponent : SingleInstanceComponent<PathServiceComponent>
    {
        /// <summary>
        /// The pathing engine to use
        /// </summary>
        public PathingEngineType engineType;

        /// <summary>
        /// The initial heap size. The optimal size depends on the size of the grid, and there is no static approximate factor, since the heap use percentage related to grid size diminishes as the grid gets bigger.
        /// </summary>
        public int initialHeapSize = 100;

        /// <summary>
        /// Whether the path finding should be done asynchronously, i.e. on a separate thread.
        /// </summary>
        public bool runAsync = true;

        /// <summary>
        /// Use thread pool for async request if available.
        /// </summary>
        public bool useThreadPoolForAsyncOperations = false;

        /// <summary>
        /// The maximum milliseconds per frame to use for path finding
        /// </summary>
        public int maxMillisecondsPerFrame = 5;

        private IPathService _pathService;

        /// <summary>
        /// Called on awake.
        /// </summary>
        protected sealed override void OnAwake()
        {
            //Determine the cost and path smoothing providers to use
            ISmoothPaths pathSmoother;
            var pathSmoothProvider = this.As<IPathSmootherFactory>();
            if (pathSmoothProvider == null)
            {
                pathSmoother = new PathSmoother();
            }
            else
            {
                pathSmoother = pathSmoothProvider.CreateSmoother();
            }

            IMoveCost moveCostProvider;
            var moveCostProviderFactory = this.As<IMoveCostFactory>();
            if (moveCostProviderFactory == null)
            {
                moveCostProvider = new DiagonalDistance(10);
            }
            else
            {
                moveCostProvider = moveCostProviderFactory.CreateMoveCostProvider();
            }

            //Setup the pathing engine to use
            IPathingEngine engine;
            if (this.engineType == PathingEngineType.Astar)
            {
                engine = new PathingAStar(this.initialHeapSize, moveCostProvider, pathSmoother);
            }
            else
            {
                engine = new PathingJumpPointSearch(this.initialHeapSize, moveCostProvider, pathSmoother);
            }

            _pathService = new PathService(engine, new DirectPather(), GridManager.instance, new ThreadFactory(), this.useThreadPoolForAsyncOperations);
            _pathService.runAsync = this.runAsync;
            GameServices.pathService = _pathService;
        }

        private void Start()
        {
            _pathService.AsyncFailed += OnAsyncFailed;

            if (!this.runAsync)
            {
                StartCoroutine(_pathService.ProcessRequests(maxMillisecondsPerFrame));
            }
        }

        /// <summary>
        /// Called when destroyed.
        /// </summary>
        protected sealed override void OnDestroy()
        {
            if (_pathService != null)
            {
                _pathService.AsyncFailed -= OnAsyncFailed;
                _pathService.Dispose();
            }

            base.OnDestroy();
        }

        private void OnAsyncFailed(object sender, EventArgs e)
        {
            this.runAsync = false;
            _pathService.runAsync = false;
            StartCoroutine(_pathService.ProcessRequests(maxMillisecondsPerFrame));
        }
    }
}

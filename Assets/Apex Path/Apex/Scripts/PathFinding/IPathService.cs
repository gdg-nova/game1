/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    using System;
    using System.Collections;

    /// <summary>
    /// Interface for the path service
    /// </summary>
    public interface IPathService : IDisposable
    {
        /// <summary>
        /// Occurs if asynchronous processing failed.
        /// </summary>
        event EventHandler AsyncFailed;

        /// <summary>
        /// Gets or sets a value indicating whether to run asynchronously.
        /// </summary>
        /// <value>
        ///   <c>true</c> if to run asynchronously; otherwise, <c>false</c>.
        /// </value>
        bool runAsync { get; set; }

        /// <summary>
        /// Processes queued requests as a coroutine.
        /// </summary>
        /// <param name="maxMillisecondsPerFrame">The maximum milliseconds per frame before yielding.</param>
        /// <returns>coroutine enumerator</returns>
        IEnumerator ProcessRequests(int maxMillisecondsPerFrame);

        /// <summary>
        /// Processes queued requests synchronously.
        /// </summary>
        void ProcessRequests();

        /// <summary>
        /// Queues a request.
        /// </summary>
        /// <param name="request">The request.</param>
        void QueueRequest(IPathRequest request);

        /// <summary>
        /// Queues a request with a priority. Higher values take priority.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="priority">The priority.</param>
        void QueueRequest(IPathRequest request, int priority);
    }
}

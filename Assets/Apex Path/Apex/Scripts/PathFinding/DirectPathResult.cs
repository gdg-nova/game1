/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    using Apex.DataStructures;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// The result of a path request that includes off grid navigation.
    /// </summary>
    public class DirectPathResult : PathResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectPathResult"/> class.
        /// </summary>
        /// <param name="from">Moving from.</param>
        /// <param name="to">Robing to.</param>
        /// <param name="originalRequest">The original request.</param>
        public DirectPathResult(Vector3 from, Vector3 to, IPathRequest originalRequest)
        {
            this.path = new StackWithLookAhead<IPositioned>(2);
            this.path.Push(new Position(to));
            this.path.Push(new Position(from));

            this.originalRequest = originalRequest;
            this.status = PathingStatus.Complete;
        }

        private DirectPathResult(PathingStatus status, IPathRequest originalRequest)
            : base(status, null, originalRequest)
        {
        }

        private DirectPathResult()
        {
        }

        /// <summary>
        /// Factory method to create a failure result
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="originalRequest">The original request.</param>
        /// <returns>The result</returns>
        public static DirectPathResult Fail(PathingStatus status, IPathRequest originalRequest)
        {
            return new DirectPathResult(status, originalRequest);
        }

        /// <summary>
        /// Factory method to create a result with a path
        /// </summary>
        /// <param name="from">Moving from.</param>
        /// <param name="to">Moving to.</param>
        /// <param name="endWaypoint">The end way point.</param>
        /// <param name="originalRequest">The original request.</param>
        /// <returns>The result</returns>
        public static DirectPathResult CreateWithPath(Vector3 from, Vector3 to, Vector3 endWaypoint, IPathRequest originalRequest)
        {
            return CreateWithPath(new Vector3[] { from, to }, endWaypoint, originalRequest);
        }

        /// <summary>
        /// Factory method to create a result with a path
        /// </summary>
        /// <param name="pathPoints">The path points.</param>
        /// <param name="endWaypoint">The end way point.</param>
        /// <param name="originalRequest">The original request.</param>
        /// <returns>The result</returns>
        public static DirectPathResult CreateWithPath(Vector3[] pathPoints, Vector3 endWaypoint, IPathRequest originalRequest)
        {
            var res = new DirectPathResult();

            var path = new StackWithLookAhead<IPositioned>(pathPoints.Length);

            for (int i = pathPoints.Length - 1; i >= 0; i--)
            {
                path.Push(new Position(pathPoints[i]));
            }

            res.path = path;
            res.pendingWaypoints = new[] { endWaypoint };
            res.originalRequest = originalRequest;
            res.status = PathingStatus.Complete;

            return res;
        }
    }
}

namespace Apex.PathFinding
{
    using Apex.DataStructures;
    using Apex.Utilities;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Represents a manual path, i.e. user defined with no involvement of the path finder.
    /// </summary>
    public sealed class ManualPath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManualPath"/> class.
        /// </summary>
        /// <param name="replanCallback">The replan callback.</param>
        /// <param name="path">The path.</param>
        public ManualPath(Replan replanCallback, params Vector3[] path)
        {
            Ensure.ArgumentNotNull(path, "path");

            this.onReplan = replanCallback;

            var stack = new StackWithLookAhead<IPositioned>(path.Length);
            for (int i = path.Length - 1; i >= 0; i--)
            {
                stack.Push(path[i].AsPositioned());
            }

            this.path = stack;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualPath"/> class.
        /// </summary>
        /// <param name="replanCallback">The replan callback.</param>
        /// <param name="path">The path.</param>
        public ManualPath(Replan replanCallback, IIndexable<Vector3> path)
        {
            Ensure.ArgumentNotNull(path, "path");

            this.onReplan = replanCallback;

            var stack = new StackWithLookAhead<IPositioned>(path.count);
            for (int i = path.count - 1; i >= 0; i--)
            {
                stack.Push(path[i].AsPositioned());
            }

            this.path = stack;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualPath"/> class.
        /// </summary>
        /// <param name="replanCallback">The replan callback.</param>
        /// <param name="path">The path.</param>
        public ManualPath(Replan replanCallback, StackWithLookAhead<IPositioned> path)
        {
            Ensure.ArgumentNotNull(path, "path");

            this.onReplan = replanCallback;
            this.path = path;
        }

        /// <summary>
        /// Call back delegate when the unit needs to replan.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="currentDestination">The current destination.</param>
        /// <param name="path">The path.</param>
        public delegate void Replan(GameObject unit, Vector3 currentDestination, ManualPath path);

        /// <summary>
        /// Gets or sets the replan callback.
        /// </summary>
        public Replan onReplan
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public StackWithLookAhead<IPositioned> path
        {
            get;
            set;
        }

        /// <summary>
        /// Updates the path.
        /// </summary>
        /// <param name="path">The path points.</param>
        public void Update(params Vector3[] path)
        {
            var stack = this.path;
            stack.Clear();

            for (int i = path.Length - 1; i >= 0; i--)
            {
                stack.Push(path[i].AsPositioned());
            }
        }

        /// <summary>
        /// Updates the path.
        /// </summary>
        /// <param name="path">The path points.</param>
        public void Update(IIndexable<Vector3> path)
        {
            var stack = this.path;
            stack.Clear();

            for (int i = path.count - 1; i >= 0; i--)
            {
                stack.Push(path[i].AsPositioned());
            }
        }
    }
}

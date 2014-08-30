/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    using Apex.Common;
    using Apex.PathFinding;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Interface for objects that can move
    /// </summary>
    public interface IMovable : IPositioned, IHaveAttributes
    {
        /// <summary>
        /// Gets the current destination.
        /// </summary>
        /// <value>
        /// The current destination.
        /// </value>
        IPositioned currentDestination { get; }

        /// <summary>
        /// Asks the object to move to the specified position
        /// </summary>
        /// <param name="position">The position to move to.</param>
        /// <param name="append">if set to <c>true</c> the destination is added as a way point.</param>
        void MoveTo(Vector3 position, bool append);

        /// <summary>
        /// Asks the object to move along the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        void MoveAlong(ManualPath path);

        /// <summary>
        /// Waits the specified seconds before continuing the move.
        /// </summary>
        /// <param name="seconds">The seconds to wait or null to wait until explicitly <see cref="Resume"/>d.</param>
        void Wait(float? seconds);

        /// <summary>
        /// Resumes movements after a <see cref="Wait" />.
        /// </summary>
        void Resume();

        /// <summary>
        /// Enables the movement orders following a call to <see cref="DisableMovementOrders"/>.
        /// </summary>
        void EnableMovementOrders();

        /// <summary>
        /// Disables movement orders, i.e. calls to <see cref="MoveTo"/> will be ignored until <see cref="EnableMovementOrders"/> is called.
        /// </summary>
        void DisableMovementOrders();
    }
}

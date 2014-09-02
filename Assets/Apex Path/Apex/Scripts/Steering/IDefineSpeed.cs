/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    using UnityEngine;

    /// <summary>
    /// Interface for components that define speed for an entity type.
    /// </summary>
    public interface IDefineSpeed
    {
        /// <summary>
        /// Gets the minimum speed of the unit.
        /// </summary>
        /// <value>
        /// The minimum speed.
        /// </value>
        float minimumSpeed { get; }

        /// <summary>
        /// Gets the maximum speed of the unit.
        /// </summary>
        /// <value>
        /// The maximum speed.
        /// </value>
        float maximumSpeed { get; }

        /// <summary>
        /// Signal that the unit has stopped.
        /// </summary>
        void SignalStop();

        /// <summary>
        /// Gets the preferred speed of the unit.
        /// </summary>
        /// <param name="currentMovementDirection">The current movement direction.</param>
        /// <returns>The preferred speed</returns>
        float GetPreferredSpeed(Vector3 currentMovementDirection);
    }
}

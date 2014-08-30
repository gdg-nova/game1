/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    using UnityEngine;

    /// <summary>
    /// Interface for objects on the move
    /// </summary>
    public interface IMovingObject
    {
        /// <summary>
        /// Gets the velocity of the object.
        /// </summary>
        /// <value>
        /// The velocity.
        /// </value>
        Vector3 velocity { get; }

        /// <summary>
        /// Stops the object's movement.
        /// </summary>
        void Stop();
    }
}

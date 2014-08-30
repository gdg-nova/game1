/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    using UnityEngine;

    /// <summary>
    /// Interface for steering components that adjust the default update interval.
    /// </summary>
    public interface IAdjustUpdateInterval
    {
        /// <summary>
        /// Gets the update interval to use for the next update.
        /// </summary>
        /// <param name="expectedVelocity">The expected velocity the unit will have between now and the next update if no adjustment is made.</param>
        /// <param name="unadjustedInterval">The unadjusted interval, i.e. the time which will pass before the next update if no adjustment is made.</param>
        /// <returns>The adjusted update interval. If no adjustment is needed, simply return <paramref name="unadjustedInterval"/>.</returns>
        float GetUpdateInterval(Vector3 expectedVelocity, float unadjustedInterval);
    }
}

namespace Apex.Steering
{
    using UnityEngine;

    /// <summary>
    /// Interface for custom unit turn implementations. Implement this if you want to apply your own turn logic to the <see cref="TurnableUnitComponent"/>.
    /// </summary>
    public interface ITurnUnits
    {
        /// <summary>
        /// Turns the unit to face the specified heading.
        /// </summary>
        /// <param name="desiredHeading">The desired heading.</param>
        /// <param name="desiredTurnSpeed">The desired speed by which to rotate.</param>
        void Turn(Vector3 desiredHeading, float desiredTurnSpeed);
    }
}

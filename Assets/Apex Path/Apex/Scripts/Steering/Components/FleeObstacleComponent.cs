/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering.Components
{
    using Apex;
    using Apex.Messages;
    using Apex.Services;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// A steering component that will cause the unit to move away from obstacles that encroach on its position.
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Steering/Steer to flee obstacle")]
    public class FleeObstacleComponent : SteeringComponent, IAdjustUpdateInterval
    {
        /// <summary>
        /// The maximum cell radius to look for a new position
        /// </summary>
        public int fleeMaxRadius = 5;

        private Cell _targetCell;

        /// <summary>
        /// Called on Awake
        /// </summary>
        protected override void Awake()
        {
            this.WarnIfMultipleInstances();

            base.Awake();
        }

        /// <summary>
        /// Gets the movement vector.
        /// </summary>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <returns>
        /// The movement vector
        /// </returns>
        protected override Vector3 GetMovementVector(Vector3 currentVelocity)
        {
            var pos = this.transformCached.position;

            var grid = GridManager.instance.GetGrid(pos);
            if (grid == null)
            {
                _targetCell = null;
                return Vector3.zero;
            }

            var cell = grid.GetCell(pos);
            if (cell.isWalkable(this.attributes))
            {
                if (_targetCell != null)
                {
                    var moveVector = (_targetCell.position - pos).AdjustAxis(0.0f, this.excludedAxis);
                    var dist = moveVector.magnitude + this.radius;
                    if (dist > grid.cellSize / 2.0f)
                    {
                        return moveVector;
                    }
                }

                _targetCell = null;
                return Vector3.zero;
            }

            if (_targetCell == null || _targetCell == cell)
            {
                _targetCell = grid.GetNearestWalkableCell(pos, pos, true, this.fleeMaxRadius, this.attributes);
            }

            return (_targetCell.position - pos).AdjustAxis(0.0f, this.excludedAxis);
        }

        /// <summary>
        /// Gets the speed adjustment factor, used to increase or decrease the speed of the unit under certain circumstances.
        /// </summary>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <returns>
        /// The speed adjustment factor
        /// </returns>
        public override float GetSpeedAdjustmentFactor(Vector3 currentVelocity)
        {
            if (_targetCell != null)
            {
                //Get the hell out of dodge (this will be clamped to max speed, so it just need to be high enough to reach max speed or above)
                return 10.0f;
            }

            return 1.0f;
        }

        float IAdjustUpdateInterval.GetUpdateInterval(Vector3 expectedVelocity, float unadjustedInterval)
        {
            if (_targetCell == null)
            {
                return unadjustedInterval;
            }

            //If the expected distance that would be traveled until the next update is greater than what remains to reach the current destination, adjust the interval accordingly
            var pos = this.transformCached.position.AdjustAxis(_targetCell.position.y, this.excludedAxis);

            return GetAppropriateUpdateInterval(expectedVelocity, (_targetCell.position - pos).sqrMagnitude, unadjustedInterval);
        }
    }
}

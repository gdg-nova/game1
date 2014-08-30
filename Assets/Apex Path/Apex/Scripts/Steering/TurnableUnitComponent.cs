/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    using System.Collections.Generic;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Turns a unit to face a direction. Where to face is governed by attached <see cref="IProvideFacingOrientation"/> components.
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Steering/Turnable Unit")]
    public class TurnableUnitComponent : MonoBehaviour, IControlFacingOrientation
    {
        /// <summary>
        /// The ignored axis
        /// </summary>
        public Axis ignoreAxis = Axis.Y;

        private List<IProvideFacingOrientation> _providers = new List<IProvideFacingOrientation>();
        private ITurnUnits _turner;

        /// <summary>
        /// Registers an orientation provider.
        /// </summary>
        /// <param name="p">The provider.</param>
        public void RegisterOrientationProvider(IProvideFacingOrientation p)
        {
            _providers.AddUnique(p);
        }

        /// <summary>
        /// Unregisters the orientation provider.
        /// </summary>
        /// <param name="p">The provider.</param>
        public void UnregisterOrientationProvider(IProvideFacingOrientation p)
        {
            _providers.Remove(p);
        }

        private void Awake()
        {
            this.WarnIfMultipleInstances();

            //Resolve the turner
            _turner = this.As<ITurnUnits>();
            if (_turner == null)
            {
                var fact = this.As<ITurnUnitsFactory>();
                if (fact != null)
                {
                    _turner = fact.Create();
                }
                else
                {
                    _turner = new DefaultTurner(this.transform);
                }
            }
        }

        private void Update()
        {
            FacingOrientation winnerOrientation = null;
            int highestPrio = 0;
            for (int i = 0; i < _providers.Count; i++)
            {
                var o = _providers[i].GetOrientation();
                if (o != null && o.priority > highestPrio)
                {
                    highestPrio = o.priority;
                    winnerOrientation = o;
                }
            }

            if (winnerOrientation == null || winnerOrientation.orientation == Vector3.zero)
            {
                return;
            }

            var rotationDirection = winnerOrientation.orientation.AdjustAxis(0.0f, this.ignoreAxis);

            _turner.Turn(rotationDirection, winnerOrientation.turnSpeed);
        }

        private class DefaultTurner : ITurnUnits
        {
            private Transform _transform;

            public DefaultTurner(Transform transform)
            {
                _transform = transform;
            }

            public void Turn(Vector3 desiredHeading, float desiredTurnSpeed)
            {
                var newOrientation = Quaternion.LookRotation(desiredHeading, Vector3.up);
                if (desiredTurnSpeed > 0.0f)
                {
                    newOrientation = Quaternion.Slerp(_transform.rotation, newOrientation, Time.deltaTime * desiredTurnSpeed);
                }

                _transform.rotation = newOrientation;
            }
        }
    }
}

/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering.Behaviours
{
    using System.Collections;
    using Apex.Messages;
    using Apex.Services;
    using Apex.Steering.Components;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// A steering behaviour that will make the unit to which it is attached, wander around within a certain radius.
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Behaviours/Wander")]
    public class WanderBehaviour : ExtendedMonoBehaviour, IHandleMessage<UnitNavigationEventMessage>
    {
        /// <summary>
        /// The radius from the starting position within which to wander
        /// </summary>
        public float radius = 10.0f;

        /// <summary>
        /// The minimum distance of a wander route
        /// </summary>
        public float minimumDistance = 4.0f;

        /// <summary>
        /// The time in seconds that the unit will linger after each wander route before moving on.
        /// </summary>
        public float lingerForSeconds = 0.0f;

        /// <summary>
        /// If unable to find a spot to wander to after having tried <see cref="bailAfterFailedAttempts"/> no more attempts will be made.
        /// </summary>
        public int bailAfterFailedAttempts = 100;

        private IMovable _mover;
        private Vector3 _startPos;

        private void Awake()
        {
            this.WarnIfMultipleInstances();

            _mover = this.As<IMovable>();
            if (_mover == null)
            {
                Debug.LogError("WanderBehaviour requires a component that implements IMovable.");
                this.enabled = false;
            }
        }

        /// <summary>
        /// Called on Start and OnEnable, but only one of the two, i.e. at startup it is only called once.
        /// </summary>
        protected override void OnStartAndEnable()
        {
            GameServices.messageBus.Subscribe(this);
            _startPos = this.transform.position;

            MoveNext(false);
            if (this.lingerForSeconds == 0.0f)
            {
                MoveNext(true);
            }
        }

        private void OnDisable()
        {
            GameServices.messageBus.Unsubscribe(this);
        }

        void IHandleMessage<UnitNavigationEventMessage>.Handle(UnitNavigationEventMessage message)
        {
            if (message.entity != this.gameObject || message.isHandled)
            {
                return;
            }

            if (message.eventCode == UnitNavigationEventMessage.Event.WaypointReached)
            {
                message.isHandled = true;

                MoveNext(true);
            }
            else if (message.eventCode == UnitNavigationEventMessage.Event.DestinationReached)
            {
                message.isHandled = true;

                StartCoroutine(DelayedMove());
            }
            else if (message.eventCode == UnitNavigationEventMessage.Event.StoppedNoRouteExists)
            {
                message.isHandled = true;

                MoveNext(false);
                if (this.lingerForSeconds == 0.0f)
                {
                    MoveNext(true);
                }
            }
        }

        private IEnumerator DelayedMove()
        {
            yield return new WaitForSeconds(this.lingerForSeconds);
            MoveNext(false);
        }

        private void MoveNext(bool append)
        {
            var unitMask = _mover.attributes;

            Vector3 pos = Vector3.zero;
            bool pointFound = false;
            int attempts = 0;

            while (!pointFound && attempts < this.bailAfterFailedAttempts)
            {
                pos = _startPos + (Random.insideUnitSphere * Random.Range(1.0f, this.radius)).AdjustAxis(0.0f, Axis.Y);

                var dir = (pos - this.transform.position.AdjustAxis(0.0f, Axis.Y));
                var dist = dir.magnitude;
                if (dist < this.minimumDistance)
                {
                    pos = this.transform.position + ((dir / dist) * this.minimumDistance);
                }

                var grid = GridManager.instance.GetGrid(pos);
                if (grid != null)
                {
                    var cell = grid.GetCell(pos);
                    pointFound = cell.isWalkable(unitMask);
                }
                else
                {
                    pointFound = true;
                }

                attempts++;
            }

            _mover.MoveTo(pos, append);
        }
    }
}

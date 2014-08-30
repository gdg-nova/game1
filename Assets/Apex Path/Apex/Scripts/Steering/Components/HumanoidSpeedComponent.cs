/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering.Components
{
    using UnityEngine;

    /// <summary>
    /// A component to define the speed settings for a humanoid unit.
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Steering/Humanoid Speed")]
    public class HumanoidSpeedComponent : MonoBehaviour, IDefineSpeed
    {
        /// <summary>
        /// The minimum speed ever, regardless of movement form. Any speed below this will mean a stop.
        /// </summary>
        public float minimumSpeedThreshold = 0.5f;

        /// <summary>
        /// The crawl speed
        /// </summary>
        public float crawlSpeed = 1.0f;

        /// <summary>
        /// The crouched speed
        /// </summary>
        public float crouchedSpeed = 1.5f;

        /// <summary>
        /// The walk speed
        /// </summary>
        public float walkSpeed = 3.0f;

        /// <summary>
        /// The run speed
        /// </summary>
        public float runSpeed = 6.0f;

        /// <summary>
        /// The strafe maximum speed, that is the highest speed possible when moving side ways
        /// </summary>
        public float strafeMaxSpeed = 4.0f;

        /// <summary>
        /// The back pedal maximum speed, that is the highest speed possible when moving backwards
        /// </summary>
        public float backPedalMaxSpeed = 3.0f;

        private Transform _transform;
        private float _preferredSpeed;
        private SpeedIndex _preferredSpeedIndex;
        private SpeedIndex _curSpeedIndex;

        /// <summary>
        /// An index that indicates what speed the unit is currently aiming for. Can be used to control animations.
        /// </summary>
        public enum SpeedIndex
        {
            /// <summary>
            /// The unit is stopped
            /// </summary>
            Stopped = 0,

            /// <summary>
            /// The unit is crawling
            /// </summary>
            Crawl = 1,

            /// <summary>
            /// The unit is crouched
            /// </summary>
            Crouched = 2,

            /// <summary>
            /// The unit is moving backwards
            /// </summary>
            BackPedal = 3,

            /// <summary>
            /// The unit is strafing
            /// </summary>
            Strafe = 4,

            /// <summary>
            /// The unit is walking
            /// </summary>
            Walk = 5,

            /// <summary>
            /// The unit is running
            /// </summary>
            Run = 6
        }

        /// <summary>
        /// Gets the minimum speed of the unit.
        /// </summary>
        /// <value>
        /// The minimum speed.
        /// </value>
        public float minimumSpeed
        {
            get { return this.minimumSpeedThreshold; }
        }

        /// <summary>
        /// Gets the maximum speed of the unit.
        /// </summary>
        /// <value>
        /// The maximum speed.
        /// </value>
        public float maximumSpeed
        {
            get { return this.runSpeed; }
        }

        /// <summary>
        /// Gets the current speed index. Can be used to control animations.
        /// </summary>
        public SpeedIndex currentSpeedIndex
        {
            get { return _curSpeedIndex; }
        }

        private void Awake()
        {
            this.WarnIfMultipleInstances();

            _transform = this.transform;
            Walk();
        }

        /// <summary>
        /// Signal that the unit has stopped.
        /// </summary>
        public void SignalStop()
        {
            _curSpeedIndex = SpeedIndex.Stopped;
        }

        /// <summary>
        /// Makes the unit's preferred speed that of <see cref="crawlSpeed"/>
        /// </summary>
        public void Crawl()
        {
            _preferredSpeedIndex = SpeedIndex.Crawl;
            _preferredSpeed = this.crawlSpeed;
        }

        /// <summary>
        /// Makes the unit's preferred speed that of <see cref="crouchedSpeed"/>
        /// </summary>
        public void Crouch()
        {
            _preferredSpeedIndex = SpeedIndex.Crouched;
            _preferredSpeed = this.crouchedSpeed;
        }

        /// <summary>
        /// Makes the unit's preferred speed that of <see cref="walkSpeed"/>
        /// </summary>
        public void Walk()
        {
            _preferredSpeedIndex = SpeedIndex.Walk;
            _preferredSpeed = this.walkSpeed;
        }

        /// <summary>
        /// Makes the unit's preferred speed that of <see cref="runSpeed"/>
        /// </summary>
        public void Run()
        {
            _preferredSpeedIndex = SpeedIndex.Run;
            _preferredSpeed = this.runSpeed;
        }

        /// <summary>
        /// Refreshes the preferred speed setting after changes made to the speed properties
        /// </summary>
        public void Refresh()
        {
            switch (_preferredSpeedIndex)
            {
                case SpeedIndex.Crawl:
                {
                    Crawl();
                    break;
                }

                case SpeedIndex.Crouched:
                {
                    Crouch();
                    break;
                }

                case SpeedIndex.Walk:
                {
                    Walk();
                    break;
                }

                case SpeedIndex.Run:
                {
                    Run();
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the preferred speed of the unit.
        /// </summary>
        /// <param name="currentMovementDirection">The current movement direction.</param>
        /// <returns>
        /// The preferred speed
        /// </returns>
        public float GetPreferredSpeed(Vector3 currentMovementDirection)
        {
            var dp = Vector3.Dot(currentMovementDirection, _transform.forward);

            if (dp < 0.5f)
            {
                //Between 60 and 120 degrees movement is considered a strafe
                if ((_preferredSpeed > this.strafeMaxSpeed) && (dp > -0.5f))
                {
                    _curSpeedIndex = SpeedIndex.Strafe;
                    return this.strafeMaxSpeed;
                }

                //Beyond 120 degrees its a backpedal.
                if (_preferredSpeed > this.backPedalMaxSpeed)
                {
                    _curSpeedIndex = SpeedIndex.BackPedal;
                    return this.backPedalMaxSpeed;
                }
            }

            _curSpeedIndex = _preferredSpeedIndex;
            return _preferredSpeed;
        }
    }
}

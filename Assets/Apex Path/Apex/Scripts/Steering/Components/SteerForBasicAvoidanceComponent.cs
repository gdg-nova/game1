namespace Apex.Steering.Components
{
    using Apex.Steering;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Basic steering component that ensures collision free movement of units.
    /// </summary>
    [AddComponentMenu("")]
    public class SteerForBasicAvoidanceComponent : SteeringComponent
    {
        private BasicScanner _scanner;
        private float _minSpeedSquared;
        private float _fovReverseAngleCos;
        private float _omniAwareRadius;

        /// <summary>
        /// Called on Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            _scanner = GetComponent<BasicScanner>();
            _fovReverseAngleCos = Mathf.Cos(((360f - this.unit.fieldOfView) / 2f) * Mathf.Deg2Rad);
            _omniAwareRadius = this.unit.radius * 2f;

            var speeder = this.As<IDefineSpeed>();
            if (speeder != null)
            {
                _minSpeedSquared = speeder.minimumSpeed * speeder.minimumSpeed;
            }

            var goLayer = 1 << this.gameObject.layer;
            if ((goLayer & Layers.units) == 0)
            {
                Debug.LogWarning(this.gameObject.name + " : For local avoidance to work, your unit must be put in the unit layer(s) mapped on the Game World.");
            }
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
            Vector3 moveVector = Vector3.zero;

            if (currentVelocity.sqrMagnitude < _minSpeedSquared)
            {
                return moveVector;
            }

            var otherUnits = _scanner.Units;
            float maxAdj = 0f;
            float adjSum = 0f;

            for (int i = 0; i < otherUnits.Length; i++)
            {
                var other = otherUnits[i];
                if (other.Equals(this.collider))
                {
                    continue;
                }

                Vector3 evadePos;
                var otherVelo = other.GetComponent<SteerableUnitComponent>();

                if (otherVelo == null)
                {
                    evadePos = other.transform.position;
                }
                else
                {
                    var otherPos = otherVelo.unit.position;
                    var distToOther = (otherPos - this.position).magnitude;
                    var otherSpeed = otherVelo.speed;

                    var predictionTime = 0.1f;
                    if (otherSpeed > 0f)
                    {
                        //Half the prediction time for better behavior
                        predictionTime = (distToOther / otherSpeed) / 2f;
                    }

                    evadePos = otherPos + (otherVelo.velocity * predictionTime);
                }

                var offset = this.position - evadePos;
                var offsetMagnitude = offset.magnitude;

                //Only do avoidance if inside vision cone or very close to the unit
                if (offsetMagnitude > _omniAwareRadius && Vector3.Dot(this.transformCached.forward, offset / offsetMagnitude) > _fovReverseAngleCos)
                {
                    continue;
                }

                //The adjustment normalizes the offset and adjusts its impact according to the offset length, i.e. the further the other unit is away the less it will impact the steering
                var adj = 1f / (offsetMagnitude * offsetMagnitude);
                adjSum += adj;
                if (adj > maxAdj)
                {
                    maxAdj = adj;
                }

                moveVector += (offset * adj);
            }

            if (maxAdj > 0f)
            {
                //Lastly we average out the move vector based on adjustments
                return moveVector / (adjSum / maxAdj);
            }

            return moveVector;
        }
    }
}

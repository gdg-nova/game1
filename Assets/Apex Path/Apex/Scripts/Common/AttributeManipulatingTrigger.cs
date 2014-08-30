/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Common
{
    using UnityEngine;

    /// <summary>
    /// A trigger behaviour that can apply and/or remove one or more attributes from an entity when the entity enters and/or exits the trigger area.
    /// </summary>
    [AddComponentMenu("Apex/Triggers/Attribute Manipulating Trigger")]
    public class AttributeManipulatingTrigger : MonoBehaviour
    {
        /// <summary>
        /// When the trigger behaviour should happen, on entry, exit or both.
        /// </summary>
        public Trigger updateOn = Trigger.Both;

        [SerializeField, AttributeProperty]
        private int _applies;

        [SerializeField, AttributeProperty]
        private int _removes;

        /// <summary>
        /// The criteria for when the trigger should 'trigger'
        /// </summary>
        public enum Trigger
        {
            /// <summary>
            /// Triggers on enter
            /// </summary>
            OnEnter = 1,

            /// <summary>
            /// Triggers on exit
            /// </summary>
            OnExit = 2,

            /// <summary>
            /// Triggers on both enter and exit
            /// </summary>
            Both = OnEnter | OnExit
        }

        /// <summary>
        /// Gets or sets the attributes to apply to units when the trigger triggers.
        /// </summary>
        public AttributeMask applies
        {
            get { return _applies; }
            set { _applies = value; }
        }

        /// <summary>
        /// Gets or sets the attributes to remove to units when the trigger triggers.
        /// </summary>
        public AttributeMask removes
        {
            get { return _removes; }
            set { _removes = value; }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((this.updateOn & Trigger.OnEnter) > 0)
            {
                Apply(other, _applies, _removes);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (this.updateOn == Trigger.OnExit)
            {
                Apply(other, _applies, _removes);
            }
            else if ((this.updateOn & Trigger.OnExit) > 0)
            {
                Apply(other, _removes, _applies);
            } 
        }

        private void Apply(Collider other, int apply, int remove)
        {
            var entity = other.GetComponent<AttributedComponent>();
            if (entity == null)
            {
                return;
            }

            entity.attributes |= apply;
            entity.attributes &= ~remove;
        }
    }
}

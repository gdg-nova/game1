/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.WorldGeometry
{
    using System;
    using Apex.Common;
    using UnityEngine;

    /// <summary>
    /// Component for setting up <see cref="GridPortal" />s at design time.
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Portals/Portal")]
    public class GridPortalComponent : ExtendedMonoBehaviour
    {
        /// <summary>
        /// The portal name
        /// </summary>
        public string portalName;

        /// <summary>
        /// The type of the portal, which determines how it affects path finding.
        /// </summary>
        public PortalType type;

        /// <summary>
        /// The bounds of the first portal
        /// </summary>
        public Bounds portalOne;

        /// <summary>
        /// The bounds of the second portal
        /// </summary>
        public Bounds portalTwo;

        /// <summary>
        /// The color to use when drawing portal gizmos.
        /// </summary>
        public Color portalColor = new Color(0f, 150f / 255f, 211f / 255f, 100f / 255f);

        [SerializeField, AttributeProperty]
        private int _exclusiveTo;
        private GridPortal _portal;

        /// <summary>
        /// Gets or sets the attribute mask that defines which units can use this portal.
        /// If set to a value other than <see cref="AttributeMask.None"/> only units with at least one of the specified attributes can use the portal.
        /// </summary>
        /// <value>
        /// The exclusive mask.
        /// </value>
        public AttributeMask exclusiveTo
        {
            get { return _exclusiveTo; }
            set { _exclusiveTo = value; }
        }

        private void Awake()
        {
            var action = this.As<IPortalAction>();
            if (action == null)
            {
                var fact = this.As<IPortalActionFactory>();
                if (fact != null)
                {
                    action = fact.Create();
                }
            }

            if (action == null)
            {
                Debug.LogError("A portal must have an accompanying portal action component, please add one.");
                this.enabled = false;
                return;
            }

            try
            {
                _portal = GridPortal.Create(this.portalName, this.type, this.portalOne, this.portalTwo, this.exclusiveTo, action);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                this.enabled = false;
            }
        }

        /// <summary>
        /// Called on Start and OnEnable, but only one of the two, i.e. at startup it is only called once.
        /// </summary>
        protected override void OnStartAndEnable()
        {
            if (_portal != null)
            {
                _portal.enabled = true;
            }
        }

        private void OnDisable()
        {
            if (_portal != null)
            {
                _portal.enabled = false;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = this.portalColor;
            Gizmos.DrawCube(this.portalOne.center, this.portalOne.size);
            Gizmos.DrawCube(this.portalTwo.center, this.portalTwo.size);
        }
    }
}

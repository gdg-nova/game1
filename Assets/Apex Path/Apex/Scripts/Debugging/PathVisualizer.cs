/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Debugging
{
    using Apex.PathFinding;
    using Apex.Steering;
    using Apex.Steering.Components;
    using UnityEngine;

    /// <summary>
    /// Visualization component that draws gizmos to represent a moving unit's current path.
    /// </summary>
    [AddComponentMenu("Apex/Debugging/Path Visualizer")]
    public class PathVisualizer : Visualizer
    {
        /// <summary>
        /// The route color
        /// </summary>
        public Color routeColor = new Color(148f / 255f, 214f / 255f, 53f / 255f);

        /// <summary>
        /// The way point color
        /// </summary>
        public Color waypointColor = new Color(0f, 150f / 255f, 211f / 255f);

        /// <summary>
        /// Whether to show segment markers
        /// </summary>
        public bool showSegmentMarkers = false;

        private ISteerable _target;

        /// <summary>
        /// Called on start
        /// </summary>
        protected override void Start()
        {
            _target = this.As<ISteerable>();
        }

        /// <summary>
        /// Draws the actual visualization.
        /// </summary>
        protected override void DrawVisualization()
        {
            if (_target == null)
            {
                return;
            }

            Vector3 heightAdj = new Vector3(0.0f, 0.2f, 0.0f);
            Gizmos.color = this.routeColor;

            var prev = _target.position;
            if (_target.currentDestination != null)
            {
                prev = _target.currentDestination.position;
                Gizmos.DrawLine(_target.position + heightAdj, prev + heightAdj);
            }

            if (_target.currentPath != null)
            {
                foreach (var n in _target.currentPath)
                {
                    if (n is IPortalNode)
                    {
                        continue;
                    }

                    if (showSegmentMarkers)
                    {
                        Gizmos.DrawSphere(prev, 0.2f);
                    }

                    Gizmos.DrawLine(prev + heightAdj, n.position + heightAdj);
                    prev = n.position;
                }
            }

            Gizmos.color = this.waypointColor;
            if (_target.currentWaypoints != null)
            {
                heightAdj.y = 1.0f;

                foreach (var wp in _target.currentWaypoints)
                {
                    var pinHead = wp + heightAdj;
                    Gizmos.DrawLine(wp, pinHead);
                    Gizmos.DrawSphere(pinHead, 0.3f);
                }
            }
        }
    }
}

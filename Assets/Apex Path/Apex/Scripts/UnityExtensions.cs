/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex
{
    using Apex.PathFinding;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Various extension to Unity types.
    /// </summary>
    public static class UnityExtensions
    {
        private static readonly Plane _xzPlane = new Plane(Vector3.up, Vector3.zero);

        /// <summary>
        /// Gets the collider at position.
        /// </summary>
        /// <param name="cam">The camera.</param>
        /// <param name="screenPos">The screen position.</param>
        /// <param name="layerMask">The layer mask.</param>
        /// <param name="maxDistance">The maximum distance.</param>
        /// <returns>The first collider found in the game world at the specified screen position.</returns>
        public static Collider GetColliderAtPosition(this Camera cam, Vector3 screenPos, LayerMask layerMask, float maxDistance = 1000.0f)
        {
            RaycastHit hit;
            var ray = cam.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                return hit.collider;
            }

            return null;
        }

        /// <summary>
        /// Casts a ray from the camera to the specified position.
        /// </summary>
        /// <param name="cam">The camera.</param>
        /// <param name="screenPos">The screen position.</param>
        /// <param name="layerMask">The layer mask.</param>
        /// <param name="maxDistance">The maximum distance.</param>
        /// <param name="hit">The hit details.</param>
        /// <returns><c>true</c> if the ray hit something, otherwise <c>false</c></returns>
        public static bool ScreenToLayerHit(this Camera cam, Vector3 screenPos, LayerMask layerMask, float maxDistance, out RaycastHit hit)
        {
            var ray = cam.ScreenPointToRay(screenPos);
            return Physics.Raycast(ray, out hit, maxDistance, layerMask);
        }

        /// <summary>
        /// Casts a ray from the camera to the xz plane through the specified screen point and returns the point the ray intersects the xz plane in world coordinates.
        /// </summary>
        /// <param name="cam">The camera.</param>
        /// <param name="screenPos">The screen position.</param>
        /// <returns>The intersection point on the xz plane in world coordinates</returns>
        public static Vector3 ScreenToGroundPoint(this Camera cam, Vector3 screenPos)
        {
            var ray = cam.ScreenPointToRay(screenPos);

            float d;
            if (_xzPlane.Raycast(ray, out d))
            {
                return ray.GetPoint(d);
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Casts a ray from the camera to the xz plane through the specified screen point and returns the point the ray intersects the xz plane in world coordinates.
        /// </summary>
        /// <param name="cam">The camera.</param>
        /// <param name="screenPos">The screen position.</param>
        /// <param name="groundHeight">Height (y-coordinate) that the ground level is at.</param>
        /// <returns>The intersection point on the xz plane in world coordinates</returns>
        public static Vector3 ScreenToGroundPoint(this Camera cam, Vector3 screenPos, float groundHeight)
        {
            var ray = cam.ScreenPointToRay(screenPos);
            var xzElevatedPlane = new Plane(Vector3.up, new Vector3(0f, groundHeight, 0f));

            float d;
            if (xzElevatedPlane.Raycast(ray, out d))
            {
                return ray.GetPoint(d);
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Adjusts an axis.
        /// </summary>
        /// <param name="target">The target to adjust.</param>
        /// <param name="source">The source used for the adjust.</param>
        /// <param name="targetAxis">The target axis.</param>
        /// <returns>The target vector with <paramref name="targetAxis"/> changed to that of <paramref name="source"/></returns>
        public static Vector3 AdjustAxis(this Vector3 target, Vector3 source, Axis targetAxis)
        {
            switch (targetAxis)
            {
                case Axis.Y:
                {
                    target.y = source.y;
                    break;
                }

                case Axis.X:
                {
                    target.x = source.x;
                    break;
                }

                case Axis.Z:
                {
                    target.z = source.z;
                    break;
                }
            }

            return target;
        }

        /// <summary>
        /// Adjusts an axis.
        /// </summary>
        /// <param name="target">The target to adjust.</param>
        /// <param name="value">The adjustment.</param>
        /// <param name="targetAxis">The target axis.</param>
        /// <returns>The target vector with <paramref name="targetAxis"/> changed to <paramref name="value"/></returns>
        public static Vector3 AdjustAxis(this Vector3 target, float value, Axis targetAxis)
        {
            switch (targetAxis)
            {
                case Axis.Y:
                {
                    target.y = value;
                    break;
                }

                case Axis.X:
                {
                    target.x = value;
                    break;
                }

                case Axis.Z:
                {
                    target.z = value;
                    break;
                }
            }

            return target;
        }

        /// <summary>
        /// Checks if one vector is approximately equal to another
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="other">The other.</param>
        /// <param name="allowedDifference">The allowed difference.</param>
        /// <returns><c>true</c> if the are approximately equal, otherwise <c>false</c></returns>
        public static bool Approximately(this Vector3 me, Vector3 other, float allowedDifference)
        {
            var dx = me.x - other.x;
            if (dx < -allowedDifference || dx > allowedDifference)
            {
                return false;
            }

            var dy = me.y - other.y;
            if (dy < -allowedDifference || dy > allowedDifference)
            {
                return false;
            }

            var dz = me.z - other.z;

            return (dz >= -allowedDifference) && (dz <= allowedDifference);
        }

        /// <summary>
        /// Wraps the vector in an IPositioned structure
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns>The wrapped position</returns>
        public static IPositioned AsPositioned(this Vector3 pos)
        {
            return new Position(pos);
        }

        /// <summary>
        /// Gets the first MonoBehavior on the component's game object that is of type T. This is different from GetComponent in that the type can be an interface or class that is not itself a component.
        /// It is however a relatively slow operation, and should not be used in actions that happen frequently, e.g. Update.
        /// </summary>
        /// <typeparam name="T">The type of behavior to look for</typeparam>
        /// <param name="c">The component whose siblings will be inspected if they are of type T</param>
        /// <param name="searchParent">if set to <c>true</c> the parent transform will also be inspected if no match is found on the current component's transform.</param>
        /// <returns>
        /// The T behavior sibling of the component or null if not found.
        /// </returns>
        public static T As<T>(this Component c, bool searchParent = false) where T : class
        {
            if (c.Equals(null))
            {
                return null;
            }

            return As<T>(c.gameObject, searchParent);
        }

        /// <summary>
        /// Gets the first MonoBehavior on the component's game object that is of type T. This is different from GetComponent in that the type can be an interface or class that is not itself a component.
        /// It is however a relatively slow operation, and should not be used in actions that happen frequently, e.g. Update.
        /// </summary>
        /// <typeparam name="T">The type of behavior to look for</typeparam>
        /// <param name="c">The component whose siblings will be inspected if they are of type T</param>
        /// <param name="searchParent">if set to <c>true</c> the parent transform will also be inspected if no match is found on the current component's transform.</param>
        /// <returns>
        /// The T behavior sibling of the component or null if not found.
        /// </returns>
        public static T As<T>(this IGameObjectComponent c, bool searchParent = false) where T : class
        {
            if (c.Equals(null))
            {
                return null;
            }

            return As<T>(c.gameObject, searchParent);
        }

        /// <summary>
        /// Gets the first MonoBehavior on the game object that is of type T. This is different from GetComponent in that the type can be an interface or class that is not itself a component.
        /// It is however a relatively slow operation, and should not be used in actions that happen frequently, e.g. Update.
        /// </summary>
        /// <typeparam name="T">The type of behavior to look for</typeparam>
        /// <param name="go">The game object whose components will be inspected if they are of type T</param>
        /// <param name="searchParent">if set to <c>true</c> the parent transform will also be inspected if no match is found on the current game object.</param>
        /// <returns>
        /// The T behavior or null if not found.
        /// </returns>
        public static T As<T>(this GameObject go, bool searchParent = false) where T : class
        {
            if (go.Equals(null))
            {
                return null;
            }

            //while linq would produce much leaner code, this performs better by several factors
            var components = go.GetComponents<MonoBehaviour>();
            for (int i = 0; i < components.Length; i++)
            {
                var v = components[i] as T;
                if (v != null)
                {
                    return v;
                }
            }

            if (searchParent && go.transform.parent != null)
            {
                return As<T>(go.transform.parent.gameObject, false);
            }

            return null;
        }

        /// <summary>
        /// Warns if multiple instances of the component exists on its game object.
        /// </summary>
        /// <param name="component">The component.</param>
        public static void WarnIfMultipleInstances(this MonoBehaviour component)
        {
            var t = component.GetType();

            if (component.GetComponents(t).Length > 1)
            {
                Debug.LogWarning(string.Format("GameObject '{0}' defines multiple instances of '{1}' which is not recommended.", component.gameObject.name, t.Name));
            }
        }
    }
}

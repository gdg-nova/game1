/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.QuickStarts
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Base class for quick starts.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class QuickStartBase : MonoBehaviour
    {
        private void Awake()
        {
            Setup();

            DestroyImmediate(this);
        }

        /// <summary>
        /// Sets up component on which the quick start is attached.
        /// </summary>
        protected abstract void Setup();

        /// <summary>
        /// Finds the first component of the specified type in the project.
        /// </summary>
        /// <typeparam name="T">The type of component to look for</typeparam>
        /// <returns>The component or null i not found</returns>
        protected T FindComponent<T>() where T : Component
        {
            var res = Resources.FindObjectsOfTypeAll<T>();

            if (res != null && res.Length > 0)
            {
                return res[0];
            }

            return null;
        }

        /// <summary>
        /// Adds a component of the specified type if it does not already exist
        /// </summary>
        /// <typeparam name="T">The type of component to add</typeparam>
        /// <param name="target">The target to which the component will be added.</param>
        /// <param name="globalSearch">if set to <c>true</c> the check to see if the component already exists will be done in the entire project, otherwise it will check the <paramref name="target"/>.</param>
        /// <param name="component">The component that was added.</param>
        /// <returns><c>true</c> if the component was added, otherwise <c>false</c></returns>
        protected bool AddIfMissing<T>(GameObject target, bool globalSearch, out T component) where T : Component
        {
            if (globalSearch)
            {
                component = FindComponent<T>();
            }
            else
            {
                component = target.GetComponent<T>();
            }

            if (component == null)
            {
                component = target.AddComponent<T>();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a component of the specified type if it does not already exist
        /// </summary>
        /// <typeparam name="T">The type of component to add</typeparam>
        /// <param name="target">The target to which the component will be added.</param>
        /// <param name="globalSearch">if set to <c>true</c> the check to see if the component already exists will be done in the entire project, otherwise it will check the <paramref name="target"/>.</param>
        /// <returns><c>true</c> if the component was added, otherwise <c>false</c></returns>
        protected bool AddIfMissing<T>(GameObject target, bool globalSearch) where T : Component
        {
            T component;
            return AddIfMissing(target, globalSearch, out component);
        }

        /// <summary>
        /// Adds a component of the specified type if it does not already exist
        /// </summary>
        /// <typeparam name="T">>The type of component to add</typeparam>
        /// <typeparam name="TMarkerAttribute">The type of the marker attribute that identifies the component type</typeparam>
        /// <param name="target">The target to which the component will be added.</param>
        /// <param name="globalSearch">if set to <c>true</c> the check to see if the component already exists will be done in the entire project, otherwise it will check the <paramref name="target"/>.</param>
        /// <param name="component">The component that was added.</param>
        /// <returns><c>true</c> if the component was added, otherwise <c>false</c></returns>
        protected bool AddIfMissing<T, TMarkerAttribute>(GameObject target, bool globalSearch, out T component)
            where T : Component
            where TMarkerAttribute : Attribute
        {
            component = null;
            MonoBehaviour[] candidates;
            if (globalSearch)
            {
                candidates = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            }
            else
            {
                candidates = target.GetComponents<MonoBehaviour>();
            }

            foreach (var mb in candidates)
            {
                if (Attribute.IsDefined(mb.GetType(), typeof(TMarkerAttribute)))
                {
                    return false;
                }
            }

            component = target.AddComponent<T>();
            return true;
        }

        /// <summary>
        /// Adds a component of the specified type if it does not already exist
        /// </summary>
        /// <typeparam name="T">>The type of component to add</typeparam>
        /// <typeparam name="TMarkerAttribute">The type of the marker attribute that identifies the component type</typeparam>
        /// <param name="target">The target to which the component will be added.</param>
        /// <param name="globalSearch">if set to <c>true</c> the check to see if the component already exists will be done in the entire project, otherwise it will check the <paramref name="target"/>.</param>
        /// <returns><c>true</c> if the component was added, otherwise <c>false</c></returns>
        protected bool AddIfMissing<T, TMarkerAttribute>(GameObject target, bool globalSearch)
            where T : Component
            where TMarkerAttribute : Attribute
        {
            T component;
            return AddIfMissing<T, TMarkerAttribute>(target, globalSearch, out component);
        }
    }
}

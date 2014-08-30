/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex
{
    using System;
    using System.Collections.Generic;
    using Apex.Common;
    using Apex.DataStructures;

    /// <summary>
    /// Exposes various common extensions
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Returns the item scoring the highest in a given list
        /// </summary>
        /// <typeparam name="T">The type of the item to score</typeparam>
        /// <typeparam name="TResult">The score type</typeparam>
        /// <param name="list">The list of items</param>
        /// <param name="scorer">The scoring function</param>
        /// <returns>The item attaining the highest score</returns>
        public static T MaxScored<T, TResult>(this IEnumerable<T> list, Func<T, TResult> scorer) where TResult : IComparable<TResult>
        {
            TResult maxScore = default(TResult);
            T result = default(T);

            foreach (var item in list)
            {
                var score = scorer(item);

                if (score.CompareTo(maxScore) > 0)
                {
                    maxScore = score;
                    result = item;
                }
            }

            return result;
        }

        /// <summary>
        /// Applies the specified action to all elements in a list.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="action">The action.</param>
        public static void Apply<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// Applies the specified action to all elements in a list.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="action">The action.</param>
        public static void Apply<T>(this IList<T> list, Action<T> action)
        {
            var count = list.Count;
            for (int i = 0; i < count; i++)
            {
                action(list[i]);
            }
        }

        /// <summary>
        /// Applies the specified action to all elements in a list.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="action">The action.</param>
        public static void Apply<T>(this IIndexable<T> list, Action<T> action)
        {
            var count = list.count;
            for (int i = 0; i < count; i++)
            {
                action(list[i]);
            }
        }

        /// <summary>
        /// Adds the range of elements.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="items">The items.</param>
        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Utility enhancement to ordinary lists to allow them to work as a set. Only use this on small lists otherwise a dedicated data structure should be employed.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="item">The item.</param>
        public static void AddUnique<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Utility enhancement to ordinary lists to allow them to work as a set. Only use this on small lists otherwise a dedicated data structure should be employed.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="items">The items.</param>
        public static void AddRangeUnique<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.AddUnique(item);
            }
        }

        /// <summary>
        /// Determines whether the specified entity has any of the specified attributes.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="attributes">The attributes to check for.</param>
        /// <returns><c>true</c> if the entity has at least one of the specified attributes, otherwise, <c>false</c></returns>
        public static bool HasAny(this IHaveAttributes entity, AttributeMask attributes)
        {
            return (entity.attributes & attributes) > 0;
        }

        /// <summary>
        /// Determines whether the specified entity has all of the specified attributes.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="attributes">The attributes to check for.</param>
        /// <returns><c>true</c> if the entity has all of the specified attributes, otherwise, <c>false</c></returns>
        public static bool HasAll(this IHaveAttributes entity, AttributeMask attributes)
        {
            return (entity.attributes & attributes) == attributes;
        }
    }
}

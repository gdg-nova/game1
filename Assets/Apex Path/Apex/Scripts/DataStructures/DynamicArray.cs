/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.DataStructures
{
    using System;

    /// <summary>
    /// Ultra basic implementation of a dynamic array that forgoes most safety checks and relies on a certain usage pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicArray<T> : IIndexable<T>
    {
        private T[] _items;
        private int _capacity;
        private int _used;

        internal DynamicArray(int capacity)
        {
            _items = new T[capacity];
            _capacity = capacity;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int count
        {
            get { return _used; }
        }

        /// <summary>
        /// Gets the value with the specified index. There is no bounds checking on get.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <returns>The value at the index</returns>
        public T this[int idx]
        {
            get
            {
                return _items[idx];
            }
        }

        internal void Add(T item)
        {
            if (_used == _capacity)
            {
                _capacity *= 2;
                _capacity = Math.Max(_capacity, 1);
                var tmp = new T[_capacity];
                Array.Copy(_items, 0, tmp, 0, _items.Length);
                _items = tmp;
            }

            _items[_used++] = item;
        }

        internal void Clear()
        {
            Array.Clear(_items, 0, _used);
            _used = 0;
        }
    }
}

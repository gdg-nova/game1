/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.LoadBalancing
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Apex.DataStructures;
    using Apex.Services;
    using UnityEngine;

    /// <summary>
    /// A special queue which updates a certain (max) number of items each time its <see cref="Update"/> method is called.
    /// </summary>
    public sealed class LoadBalancedQueue : ILoadBalancer
    {
        private BinaryHeap<LoadBalancerItem> _queue;
        private Stopwatch _watch;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancedQueue"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public LoadBalancedQueue(int capacity)
            : this(capacity, 0.1f, 20, 4, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancedQueue"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="defaultUpdateInterval">The default update interval to use for items where a specific interval is not specified.</param>
        /// <param name="autoAdjust">Controls whether to automatically adjust <see cref="maxUpdatesPerInterval"/> and <see cref="maxUpdateTimeInMillisecondsPerUpdate"/>,
        /// such that all queued items will be evenly spread across the <see cref="defaultUpdateInterval"/>.</param>
        public LoadBalancedQueue(int capacity, float defaultUpdateInterval, bool autoAdjust)
            : this(capacity, defaultUpdateInterval, 20, 4, autoAdjust)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancedQueue"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="defaultUpdateInterval">The default update interval to use for items where a specific interval is not specified.</param>
        /// <param name="maxUpdatesPerInterval">The maximum number of items to update on each call to <see cref="Update"/>.</param>
        /// <param name="maxUpdateTimeInMillisecondsPerUpdate">The maximum update time in milliseconds that each call to <see cref="Update"/> is allowed to take.</param>
        public LoadBalancedQueue(int capacity, float defaultUpdateInterval, int maxUpdatesPerInterval, int maxUpdateTimeInMillisecondsPerUpdate)
            : this(capacity, defaultUpdateInterval, maxUpdatesPerInterval, maxUpdateTimeInMillisecondsPerUpdate, false)
        {
        }

        private LoadBalancedQueue(int capacity, float defaultUpdateInterval, int maxUpdatesPerInterval, int maxUpdateTimeInMillisecondsPerUpdate, bool autoAdjust)
        {
            this.defaultUpdateInterval = defaultUpdateInterval;
            this.maxUpdatesPerInterval = maxUpdatesPerInterval;
            this.maxUpdateTimeInMillisecondsPerUpdate = maxUpdateTimeInMillisecondsPerUpdate;
            this.autoAdjust = autoAdjust;

            _queue = new BinaryHeap<LoadBalancerItem>(capacity, LoadBalanceItemComparer.instance);
            _watch = new Stopwatch();
        }

        /// <summary>
        /// Gets or sets the default update interval to use for items where a specific interval is not specified.
        /// </summary>
        /// <value>
        /// The default update interval.
        /// </value>
        public float defaultUpdateInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum number of items to update on each call to <see cref="Update"/>.
        /// </summary>
        /// <value>
        /// The maximum updates per interval.
        /// </value>
        public int maxUpdatesPerInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum update time in milliseconds that each call to <see cref="Update"/> is allowed to take.
        /// </summary>
        /// <value>
        /// The maximum update time in milliseconds per update.
        /// </value>
        public int maxUpdateTimeInMillisecondsPerUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically adjust <see cref="maxUpdatesPerInterval"/> and <see cref="maxUpdateTimeInMillisecondsPerUpdate"/>,
        /// such that all queued items will be evenly spread across the <see cref="defaultUpdateInterval"/>.
        /// </summary>
        /// <value>
        ///   <c>true</c> to automatically adjust; otherwise, <c>false</c>.
        /// </value>
        public bool autoAdjust
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the accumulated number of seconds the updates were overdue this frame, i.e. sum of all updates.
        /// </summary>
        public float updatesOverdueByTotal
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the time used on the last update.
        /// </summary>
        /// <value>
        /// The update milliseconds used.
        /// </value>
        public long updateMillisecondsUsed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the updated items count, i.e. how many items were updated last frame
        /// </summary>
        /// <value>
        /// The updated items count.
        /// </value>
        public int updatedItemsCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(ILoadBalanced item)
        {
            Add(item, this.defaultUpdateInterval, 0.0f);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="delayFirstUpdate">if set to <c>true</c> the item will be scheduled for its first update <see cref="defaultUpdateInterval"/> into the future, otherwise its first update will be as soon as possible.</param>
        public void Add(ILoadBalanced item, bool delayFirstUpdate)
        {
            Add(item, this.defaultUpdateInterval, delayFirstUpdate);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="interval">The interval between updates. This overrides the <see cref="defaultUpdateInterval"/>.</param>
        public void Add(ILoadBalanced item, float interval)
        {
            Add(item, interval, 0.0f);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="interval">The interval between updates. This overrides the <see cref="defaultUpdateInterval"/>.</param>
        /// <param name="delayFirstUpdate">if set to <c>true</c> the item will be scheduled for its first update <paramref name="interval"/> into the future, otherwise its first update will be as soon as possible.</param>
        public void Add(ILoadBalanced item, float interval, bool delayFirstUpdate)
        {
            var delay = delayFirstUpdate ? interval : 0.0f;
            Add(item, interval, delay);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="interval">The interval between updates. This overrides the <see cref="defaultUpdateInterval"/>.</param>
        /// <param name="delayFirstUpdateBy">The delay by which the first update of the item will be scheduled.</param>
        public void Add(ILoadBalanced item, float interval, float delayFirstUpdateBy)
        {
            var now = UnityServices.time.time;
            var queueItem = new LoadBalancerItem
            {
                lastUpdate = now,
                nextUpdate = now + delayFirstUpdateBy,
                interval = interval,
                item = item
            };

            _queue.Add(queueItem);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(ILoadBalanced item)
        {
            _queue.Remove(o => o.item == item);
        }

        /// <summary>
        /// Updates as many items as possible within the constraints of <see cref="maxUpdatesPerInterval"/> and <see cref="maxUpdateTimeInMillisecondsPerUpdate"/>.
        /// Items are updated only when their time is up, that is when they have not been updated for the interval with which they where added.
        /// </summary>
        public void Update()
        {
            if (!_queue.hasNext)
            {
                return;
            }

            var now = UnityServices.time.time;
            _watch.Reset();
            _watch.Start();

            var maxUpdates = this.maxUpdatesPerInterval;
            int updateCount = 0;
            float overDue = 0.0f;

            if (autoAdjust)
            {
                var framesPerInterval = this.defaultUpdateInterval / UnityServices.time.deltaTime;
                maxUpdates = Mathf.CeilToInt(_queue.count / framesPerInterval);
            }

            var item = _queue.Peek();
            while ((updateCount++ < maxUpdates) && (item.nextUpdate <= now) && (this.autoAdjust || (_watch.ElapsedMilliseconds < this.maxUpdateTimeInMillisecondsPerUpdate)))
            {
                var deltaTime = now - item.lastUpdate;
                overDue += (deltaTime - item.interval);

                var nextInterval = item.item.ExecuteUpdate(deltaTime, item.interval).GetValueOrDefault(item.interval);

                if (item.item.repeat)
                {
                    //Next interval is the suggested interval or the default. It cannot be 0 since that would lead to continuous updates in this loop.
                    nextInterval = Mathf.Max(nextInterval, 0.01f);

                    item.lastUpdate = now;
                    item.nextUpdate = now + nextInterval;
                    _queue.ReheapifyDownFrom(0);
                }
                else
                {
                    _queue.Remove();
                }

                if (!_queue.hasNext)
                {
                    break;
                }

                item = _queue.Peek();
            }

            this.updatedItemsCount = updateCount - 1;
            this.updatesOverdueByTotal = overDue;
            this.updateMillisecondsUsed = _watch.ElapsedMilliseconds;
        }

        private class LoadBalancerItem
        {
            public float nextUpdate { get; set; }

            public float lastUpdate { get; set; }

            public float interval { get; set; }

            public ILoadBalanced item { get; set; }
        }

        private sealed class LoadBalanceItemComparer : IComparer<LoadBalancerItem>
        {
            public static readonly IComparer<LoadBalancerItem> instance = new LoadBalanceItemComparer();

            public int Compare(LoadBalancerItem x, LoadBalancerItem y)
            {
                return y.nextUpdate.CompareTo(x.nextUpdate);
            }
        }
    }
}

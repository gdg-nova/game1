/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Services
{
    using Apex.GameState;
    using Apex.PathFinding;
    using UnityEngine;

    /// <summary>
    /// Provides access to all game wide services.
    /// </summary>
    public static partial class GameServices
    {
        private static IMessageBus _messageBus;
        private static IPathService _pathService;

        /// <summary>
        /// Gets or sets the game state manager.
        /// </summary>
        /// <value>
        /// The game state manager.
        /// </value>
        public static GameStateManager gameStateManager
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message bus.
        /// </summary>
        /// <value>
        /// The message bus.
        /// </value>
        /// <exception cref="UnityEngine.MissingComponentException">No message bus has been initialized, please ensure that you have a Game Services Initializer component in the game world.\nThis may also be caused by script files having been recompiled while the scene is running, if so restart the scene.</exception>
        public static IMessageBus messageBus
        {
            get
            {
                if (_messageBus == null)
                {
                    throw new MissingComponentException("No message bus has been initialized, please ensure that you have a Game Services Initializer component in the game world.\nThis may also be caused by script files having been recompiled while the scene is running, if so restart the scene.");
                }

                return _messageBus;
            }

            set
            {
                _messageBus = value;
            }
        }

        /// <summary>
        /// Gets or sets the path service.
        /// </summary>
        /// <value>
        /// The path service.
        /// </value>
        /// <exception cref="UnityEngine.MissingComponentException">No Path Service has been initialized, please ensure that you have a Path Service component in the game world.\nThis may also be caused by script files having been recompiled while the scene is running, if so restart the scene.</exception>
        public static IPathService pathService
        {
            get
            {
                if (_pathService == null)
                {
                    throw new MissingComponentException("No Path Service has been initialized, please ensure that you have a Path Service component in the game world.\nThis may also be caused by script files having been recompiled while the scene is running, if so restart the scene.");
                }

                return _pathService;
            }

            set
            {
                _pathService = value;
            }
        }
    }
}

/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.GameState
{
    using System.Collections.Generic;
    using Apex.Units;
    using UnityEngine;

    /// <summary>
    /// Class for managing various game state related data.
    /// </summary>
    public partial class GameStateManager
    {
        private Selections _unitSelection;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameStateManager"/> class.
        /// </summary>
        public GameStateManager()
        {
            _unitSelection = new Selections();
        }

        /// <summary>
        /// Gets the unit selections.
        /// </summary>
        /// <value>
        /// The unit selections.
        /// </value>
        public Selections unitSelection
        {
            get { return _unitSelection; }
        }

        /// <summary>
        /// Registers a selectable unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public void RegisterSelectable(ISelectable unit)
        {
            _unitSelection.RegisterSelectable(unit);
        }
    }
}

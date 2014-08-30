/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Units
{
    using Apex.Services;
    using UnityEngine;

    /// <summary>
    /// Default implementation of <see cref="ISelectable"/>. Attaching this component to a unit makes it selectable.
    /// </summary>
    [AddComponentMenu("Apex/Units/Selectable Unit")]
    public class SelectableUnitComponent : MonoBehaviour, ISelectable
    {
        /// <summary>
        /// The visual used to indicate whether the unit is selected or not.
        /// </summary>
        public GameObject selectionVisual;

        private bool _isSelected;
        private bool? _selectPending;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ISelectable" /> is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if selected; otherwise, <c>false</c>.
        /// </value>
        public bool selected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _selectPending = null;

                if (_isSelected != value)
                {
                    _isSelected = value;
                    if (this.selectionVisual != null)
                    {
                        this.selectionVisual.SetActive(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the position of the unit.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector3 position
        {
            get { return this.transform.position; }
        }

        private void Awake()
        {
            this.WarnIfMultipleInstances();
        }

        private void Start()
        {
            //Ensure selection is toggled off
            _isSelected = true;
            this.selected = false;

            GameServices.gameStateManager.RegisterSelectable(this);
        }

        /// <summary>
        /// Marks the unit as pending for selection. This is used to indicate a selection is progress, before the actual selection occurs.
        /// </summary>
        /// <param name="pending">if set to <c>true</c> the unit is pending for selection otherwise it is not.</param>
        public void MarkSelectPending(bool pending)
        {
            if (pending != _selectPending)
            {
                _selectPending = pending;

                if (this.selectionVisual != null)
                {
                    this.selectionVisual.SetActive(pending);
                }
            }
        }
    }
}

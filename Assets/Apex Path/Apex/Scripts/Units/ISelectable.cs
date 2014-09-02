/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Units
{
    using Apex.WorldGeometry;

    /// <summary>
    /// Interface for selectable units
    /// </summary>
    public interface ISelectable : IPositioned, IGameObjectComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ISelectable"/> is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if selected; otherwise, <c>false</c>.
        /// </value>
        bool selected { get; set; }

        /// <summary>
        /// Marks the unit as pending for selection. This is used to indicate a selection is progress, before the actual selection occurs.
        /// </summary>
        /// <param name="pending">if set to <c>true</c> the unit is pending for selection otherwise it is not.</param>
        void MarkSelectPending(bool pending);
    }
}

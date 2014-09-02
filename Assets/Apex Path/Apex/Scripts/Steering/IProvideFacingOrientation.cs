/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    /// <summary>
    /// Interface for components that provide a facing orientation for use by <see cref="IControlFacingOrientation"/> components.
    /// </summary>
    public interface IProvideFacingOrientation
    {
        /// <summary>
        /// Gets the orientation this component would like to face.
        /// </summary>
        /// <returns>The facing orientation</returns>
        FacingOrientation GetOrientation();
    }
}

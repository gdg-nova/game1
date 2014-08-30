/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering
{
    /// <summary>
    /// Interface for components that turn a unit to face a specific direction.
    /// </summary>
    public interface IControlFacingOrientation
    {
        /// <summary>
        /// Registers an orientation provider.
        /// </summary>
        /// <param name="p">The provider.</param>
        void RegisterOrientationProvider(IProvideFacingOrientation p);

        /// <summary>
        /// Unregisters the orientation provider.
        /// </summary>
        /// <param name="p">The provider.</param>
        void UnregisterOrientationProvider(IProvideFacingOrientation p);
    }
}

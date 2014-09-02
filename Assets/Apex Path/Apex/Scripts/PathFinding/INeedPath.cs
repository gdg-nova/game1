/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    using Apex.Common;

    /// <summary>
    /// Interface to be implemented by entities that want to issue path requests
    /// </summary>
    public interface INeedPath
    {
        /// <summary>
        /// Gets the radius of the entity.
        /// </summary>
        float radius { get; }

        /// <summary>
        /// Gets the attributes of the entity.
        /// </summary>
        AttributeMask attributes { get; }

        /// <summary>
        /// Consumes the path result.
        /// </summary>
        /// <param name="result">The result.</param>
        void ConsumePathResult(PathResult result);
    }
}

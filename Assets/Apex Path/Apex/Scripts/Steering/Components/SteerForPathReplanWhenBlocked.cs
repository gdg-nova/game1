/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Steering.Components
{
    using Apex.PathFinding;
    using Apex.Services;
    using Apex.WorldGeometry;
    using UnityEngine;

    /// <summary>
    /// Processes path results and tries to replan to a new destination as close to the original if the status of the result is <see cref="Apex.PathFinding.PathingStatus.DestinationBlocked"/>.
    /// </summary>
    [AddComponentMenu("Apex/Navigation/Steering/Steer for Path Processing/Replan when Blocked")]
    public class SteerForPathReplanWhenBlocked : SteerForPathResultProcessorComponent
    {
        /// <summary>
        /// The maximum cell distance for new destination
        /// </summary>
        public int maxCellDistanceForNewDestination = 3;

        /// <summary>
        /// Processes the result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="steerer">The steerer.</param>
        /// <returns>
        ///   <c>true</c> if the result was handled by this processor, otherwise <c>false</c>
        /// </returns>
        public override bool HandleResult(PathResult result, SteerForPathComponent steerer)
        {
            switch (result.status)
            {
                case PathingStatus.DestinationBlocked:
                {
                    var request = result.originalRequest;

                    //Try to find an unobstructed cell as close to the original destination as possible
                    var newDestination = request.toGrid.GetNearestWalkableCell(
                                            request.to,
                                            this.transform.position,
                                            false,
                                            this.maxCellDistanceForNewDestination,
                                            request.requester.attributes);

                    if (newDestination != null)
                    {
                        request.type = RequestType.Normal;
                        request.to = newDestination.position;
                        steerer.RequestPath(request);
                        return true;
                    }

                    break;
                }
            }

            return false;
        }
    }
}

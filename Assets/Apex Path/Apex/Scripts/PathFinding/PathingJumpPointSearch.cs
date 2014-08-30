/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.PathFinding
{
    using System;
    using System.Collections.Generic;
    using Apex.DataStructures;
    using Apex.PathFinding.MoveCost;

    /// <summary>
    /// Pathing engine using the Jump Point Search algorithm
    /// </summary>
    public class PathingJumpPointSearch : PathingAStar
    {
        private DynamicArray<IPathNode> _neighbours;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathingJumpPointSearch"/> class.
        /// </summary>
        /// <param name="heapInitialSize">Initial size of the heap.</param>
        public PathingJumpPointSearch(int heapInitialSize)
            : base(heapInitialSize)
        {
            _neighbours = new DynamicArray<IPathNode>(8);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathingJumpPointSearch"/> class.
        /// </summary>
        /// <param name="heapInitialSize">Initial size of the heap.</param>
        /// <param name="moveCostProvider">The move cost provider.</param>
        /// <param name="pathSmoother">The path smoother to use.</param>
        public PathingJumpPointSearch(int heapInitialSize, IMoveCost moveCostProvider, ISmoothPaths pathSmoother)
            : base(heapInitialSize, moveCostProvider, pathSmoother)
        {
            _neighbours = new DynamicArray<IPathNode>(8);
        }

        /// <summary>
        /// Gets the walkable successors of the specified node.
        /// </summary>
        /// <param name="current">The current node.</param>
        /// <param name="successorArray">The array to fill with successors.</param>
        /// <returns>
        /// All walkable successors of the node.
        /// </returns>
        protected override void GetWalkableSuccessors(IPathNode current, DynamicArray<IPathNode> successorArray)
        {
            _neighbours.Clear();

            if (current.predecessor == null)
            {
                current.GetWalkableNeighbours(_neighbours, _requesterAttributes, _cutCorners, false);
            }
            else if (current is IPortalNode)
            {
                current.GetWalkableNeighbours(successorArray, _requesterAttributes, _cutCorners, false);
                return;
            }
            else
            {
                var dirFromPredecessor = current.predecessor.GetDirectionTo(current);
                PruneNeighbours(current, dirFromPredecessor);
            }

            var neighbourCount = _neighbours.count;
            for (int i = 0; i < neighbourCount; i++)
            {
                var n = _neighbours[i];
                if (n == null)
                {
                    break;
                }

                var dirToNeighbour = current.GetDirectionTo(n);
                var jp = Jump(current, dirToNeighbour);
                if (jp != null)
                {
                    successorArray.Add(jp);
                }
            }
        }

        private IPathNode Jump(IPathNode current, VectorXZ direction)
        {
            var next = current.GetNeighbour(direction.x, direction.z);

            if (next == null || !next.isWalkableFrom(current, _requesterAttributes))
            {
                return null;
            }

            if (next == this.goal)
            {
                return next;
            }

            if (HasForcedNeighbour(next, direction))
            {
                return next;
            }

            if (direction.x != 0 && direction.z != 0)
            {
                if (Jump(next, new VectorXZ(direction.x, 0)) != null || Jump(next, new VectorXZ(0, direction.z)) != null)
                {
                    return next;
                }

                //If both or either neighbours (depending on cut corner setting) in a diagonal move are blocked, the diagonal neighbour is not reachable since the passage is blocked
                var n1 = next.GetNeighbour(direction.x, 0);
                var n2 = next.GetNeighbour(0, direction.z);

                bool jumpOn = _cutCorners ? ((n1 != null && n1.isWalkableFrom(current, _requesterAttributes)) || (n2 != null && n2.isWalkableFrom(current, _requesterAttributes))) : ((n1 != null && n1.isWalkableFrom(current, _requesterAttributes)) && (n2 != null && n2.isWalkableFrom(current, _requesterAttributes)));
                if (jumpOn)
                {
                    return Jump(next, direction);
                }
            }
            else
            {
                return Jump(next, direction);
            }

            return null;
        }

        private bool HasForcedNeighbour(IPathNode current, VectorXZ direction)
        {
            if (current.hasVirtualNeighbour)
            {
                return true;
            }

            bool hasForced = false;

            if (direction.x != 0)
            {
                if (direction.z != 0)
                {
                    if (_cutCorners)
                    {
                        var nLeft = current.GetNeighbour(-direction.x, 0);
                        if (nLeft != null && !nLeft.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            var fn = current.GetNeighbour(-direction.x, direction.z);
                            hasForced |= (fn != null && fn.isWalkableFrom(current, _requesterAttributes));
                        }

                        var nDown = current.GetNeighbour(0, -direction.z);
                        if (nDown != null && !nDown.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            var fn = current.GetNeighbour(direction.x, -direction.z);
                            hasForced |= (fn != null && fn.isWalkableFrom(current, _requesterAttributes));
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (_cutCorners)
                    {
                        var nUp = current.GetNeighbour(0, 1);
                        if (nUp != null && !nUp.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            var fn = current.GetNeighbour(direction.x, 1);
                            hasForced |= (fn != null && fn.isWalkableFrom(current, _requesterAttributes));
                        }

                        var nDown = current.GetNeighbour(0, -1);
                        if (nDown != null && !nDown.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            var fn = current.GetNeighbour(direction.x, -1);
                            hasForced |= (fn != null && fn.isWalkableFrom(current, _requesterAttributes));
                        }
                    }
                    else
                    {
                        var nUpBack = current.GetNeighbour(-direction.x, 1);
                        if (nUpBack != null && !nUpBack.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            var fn = current.GetNeighbour(0, 1);
                            hasForced |= (fn != null && fn.isWalkableFrom(current, _requesterAttributes));
                        }

                        var nDownBack = current.GetNeighbour(-direction.x, -1);
                        if (nDownBack != null && !nDownBack.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            var fn = current.GetNeighbour(0, -1);
                            hasForced |= (fn != null && fn.isWalkableFrom(current, _requesterAttributes));
                        }
                    }
                }
            }
            else
            {
                if (_cutCorners)
                {
                    var nLeft = current.GetNeighbour(-1, 0);
                    if (nLeft != null && !nLeft.isWalkableFromAllDirections(_requesterAttributes))
                    {
                        var fn = current.GetNeighbour(-1, direction.z);
                        hasForced |= (fn != null && fn.isWalkableFrom(current, _requesterAttributes));
                    }

                    var nRight = current.GetNeighbour(1, 0);
                    if (nRight != null && !nRight.isWalkableFromAllDirections(_requesterAttributes))
                    {
                        var fn = current.GetNeighbour(1, direction.z);
                        hasForced |= (fn != null && fn.isWalkableFrom(current, _requesterAttributes));
                    }
                }
                else
                {
                    var nLeftDown = current.GetNeighbour(-1, -direction.z);
                    if (nLeftDown != null && !nLeftDown.isWalkableFromAllDirections(_requesterAttributes))
                    {
                        var fn = current.GetNeighbour(-1, 0);
                        hasForced |= (fn != null && fn.isWalkableFrom(current, _requesterAttributes));
                    }

                    var nRightDown = current.GetNeighbour(1, -direction.z);
                    if (nRightDown != null && !nRightDown.isWalkableFromAllDirections(_requesterAttributes))
                    {
                        var fn = current.GetNeighbour(1, 0);
                        hasForced |= (fn != null && fn.isWalkableFrom(current, _requesterAttributes));
                    }
                }
            }

            return hasForced;
        }

        private void PruneNeighbours(IPathNode current, VectorXZ direction)
        {
            if (direction.x != 0)
            {
                if (direction.z != 0)
                {
                    //Natural neighbours
                    var nTop = current.TryGetWalkableNeighbour(0, direction.z, _requesterAttributes, _neighbours);
                    var nRight = current.TryGetWalkableNeighbour(direction.x, 0, _requesterAttributes, _neighbours);

                    if (_cutCorners)
                    {
                        if (nTop || nRight)
                        {
                            current.TryGetWalkableNeighbour(direction.x, direction.z, _requesterAttributes, _neighbours);
                        }

                        //Forced neighbours? The left/down is as seen from a normal view of the grid, i.e. not seen from the direction of movement (well direction left corner diagonal)
                        var nLeft = current.GetNeighbour(-direction.x, 0);
                        if (nLeft != null && !nLeft.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            current.TryGetWalkableNeighbour(-direction.x, direction.z, _requesterAttributes, _neighbours);
                        }

                        var nDown = current.GetNeighbour(0, -direction.z);
                        if (nDown != null && !nDown.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            current.TryGetWalkableNeighbour(direction.x, -direction.z, _requesterAttributes, _neighbours);
                        }
                    }
                    else
                    {
                        if (nTop && nRight)
                        {
                            current.TryGetWalkableNeighbour(direction.x, direction.z, _requesterAttributes, _neighbours);
                        }
                    }
                }
                else
                {
                    //Natural neighbour
                    current.TryGetWalkableNeighbour(direction.x, 0, _requesterAttributes, _neighbours);

                    //Forced neighbours?
                    if (_cutCorners)
                    {
                        var nUp = current.GetNeighbour(0, 1);
                        if (nUp != null && !nUp.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            current.TryGetWalkableNeighbour(direction.x, 1, _requesterAttributes, _neighbours);
                        }

                        var nDown = current.GetNeighbour(0, -1);
                        if (nDown != null && !nDown.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            current.TryGetWalkableNeighbour(direction.x, -1, _requesterAttributes, _neighbours);
                        }
                    }
                    else
                    {
                        var nUpBack = current.GetNeighbour(-direction.x, 1);
                        if (nUpBack != null && !nUpBack.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            current.TryGetWalkableNeighbour(0, 1, _requesterAttributes, _neighbours);
                            current.TryGetWalkableNeighbour(direction.x, 1, _requesterAttributes, _neighbours);
                        }

                        var nDownBack = current.GetNeighbour(-direction.x, -1);
                        if (nDownBack != null && !nDownBack.isWalkableFromAllDirections(_requesterAttributes))
                        {
                            current.TryGetWalkableNeighbour(0, -1, _requesterAttributes, _neighbours);
                            current.TryGetWalkableNeighbour(direction.x, -1, _requesterAttributes, _neighbours);
                        }
                    }
                }
            }
            else
            {
                //Portals return Vector3.zero as the direction, and for those we need to start over on the new grid.
                if (direction.z == 0)
                {
                    current.GetWalkableNeighbours(_neighbours, _requesterAttributes, _cutCorners, false);
                    return;
                }

                //Natural neighbour
                current.TryGetWalkableNeighbour(0, direction.z, _requesterAttributes, _neighbours);

                //Forced neighbours? The left/right is as seen from a normal view of the grid, i.e. not seen from the direction of movement (well direction bottom up)
                if (_cutCorners)
                {
                    var nLeft = current.GetNeighbour(-1, 0);
                    if (nLeft != null && !nLeft.isWalkableFromAllDirections(_requesterAttributes))
                    {
                        current.TryGetWalkableNeighbour(-1, direction.z, _requesterAttributes, _neighbours);
                    }

                    var nRight = current.GetNeighbour(1, 0);
                    if (nRight != null && !nRight.isWalkableFromAllDirections(_requesterAttributes))
                    {
                        current.TryGetWalkableNeighbour(1, direction.z, _requesterAttributes, _neighbours);
                    }
                }
                else
                {
                    var nLeftDown = current.GetNeighbour(-1, -direction.z);
                    if (nLeftDown != null && !nLeftDown.isWalkableFromAllDirections(_requesterAttributes))
                    {
                        current.TryGetWalkableNeighbour(-1, 0, _requesterAttributes, _neighbours);
                        current.TryGetWalkableNeighbour(-1, direction.z, _requesterAttributes, _neighbours);
                    }

                    var nRightDown = current.GetNeighbour(1, -direction.z);
                    if (nRightDown != null && !nRightDown.isWalkableFromAllDirections(_requesterAttributes))
                    {
                        current.TryGetWalkableNeighbour(1, 0, _requesterAttributes, _neighbours);
                        current.TryGetWalkableNeighbour(1, direction.z, _requesterAttributes, _neighbours);
                    }
                }
            }
        }
    }
}

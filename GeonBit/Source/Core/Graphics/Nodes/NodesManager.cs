#region LICENSE
//-----------------------------------------------------------------------------
// For the purpose of making video games, educational projects or gamification,
// GeonBit is distributed under the MIT license and is totally free to use.
// To use this source code or GeonBit as a whole for other purposes, please seek 
// permission from the library author, Ronen Ness.
// 
// Copyright (c) 2017 Ronen Ness [ronenness@gmail.com].
// Do not remove this license notice.
//-----------------------------------------------------------------------------
#endregion
#region File Description
//-----------------------------------------------------------------------------
// Static manager for nodes transformations and caching.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;

namespace GeonBit.Core.Graphics
{
    /// <summary>
    /// Static class to manage node updates and caching.
    /// </summary>
    public static class NodesManager
    {
        /// <summary>
        /// Frame identifier. Used for internal caching mechanisms.
        /// </summary>
        public static uint CurrFrame { get; private set; } = 0;

        /// <summary>
        /// Queue of nodes that require update at the end of the loop.
        /// </summary>
        static List<Node> _nodesUpdateQueue = new List<Node>();

        /// <summary>
        /// Start drawing frame (call this at the begining of your drawing loop, before drawing any nodes).
        /// </summary>
        public static void StartFrame()
        {
            // increase frame id
            CurrFrame++;
        }

        /// <summary>
        /// End drawing frame (call this at the end of your drawing loop, after drawing all nodes).
        /// </summary>
        public static void EndFrame()
        {
            // update nodes
            foreach (var node in _nodesUpdateQueue)
            {
                node.UpdateTransformations(true);
            }

            // clear nodes update queue
            _nodesUpdateQueue.Clear();
        }

        /// <summary>
        /// Add this node to a queue of nodes that will do transformations update at the end of the frame.
        /// </summary>
        /// <param name="node">Node to update when frame ends.</param>
        public static void AddNodeToUpdateQueue(Node node)
        {
            _nodesUpdateQueue.Add(node);
        }
    }
}

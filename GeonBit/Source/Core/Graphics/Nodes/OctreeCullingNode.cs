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
// A scene node that uses octree culling.
// This is the node type you'd want to use for most 3D cases.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using GeonBit.Core.Utils;

namespace GeonBit.Core.Graphics
{
    /// <summary>
    /// An octree culling node.
    /// This node will divide the screen into bounding boxes recursively, until reaching a minimal size.
    /// When testing for culling we first test top-level bounding boxes, then those contained by them, and so forth.
    /// This is the optimal culling node that covers most basic 3d use-cases.
    /// </summary>
    class OctreeCullingNode : Node
    {
        /// <summary>
        /// Data that only the top octree holds.
        /// </summary>
        protected class OctreeData
        {
            /// <summary>
            /// Root octree node.
            /// </summary>
            public OctreeCullingNode Root;

            /// <summary>
            /// Maximum number of divisions into sub-octrees.
            /// </summary>
            public uint MaxDivisions;
        }

        // octree settings (shared between all octree nodes inside the tree).
        OctreeData _octreeData;

        // parent pointer and index in parent children
        OctreeCullingNode _parentOctree;
        Vector3 _indexInParent;

        // debug render octree nodes
        public static bool DebugRenderOctreeParts = false;

        /// <summary>
        /// The child octrees under this octree node.
        /// </summary>
        OctreeCullingNode[,,] _childOctrees = null;

        /// <summary>
        /// The child octrees bounding boxes.
        /// We use these to test where we need to push nodes into, before we create the actual child octrees.
        /// </summary>
        BoundingBox[,,] _childBoundingBoxes = null;

        // nodes that are actually inside this octree node, and not under one of its children
        // this is for child nodes that are too big for the subdivision nodes.
        List<Node> _nodesUnderThisOctreeBox = new List<Node>();

        /// <summary>
        /// Cache of nodes directly under this octree node.
        /// </summary>
        Node[] _nodesUnderThisOctreeBoxArray;
        
        /// <summary>
        /// Do we need to update array with nodes directly under this octree?
        /// </summary>
        bool _isNodesListDirty = true;

        /// <summary>
        /// Bounding box entity for debug rendering.
        /// </summary>
        BoundingBoxEntity _debugBoundingBoxEntity = null;

        /// <summary>
        /// How many times we can divide this node and its children.
        /// </summary>
        uint _divisionsLeft = 0;

        // half length of this node bounding box diagonal
        float _boxHalfLength;

        // when this counter > 0, this node is counting until destroying self, after it was found to be empty.
        // the reason we use counter and not remove immediately is to prevent garbage generation due to rapid remove/add on moving objects.
        int _countToRemoveSelf = 0;

        /// <summary>
        /// Frames to count until removing an empty node.
        /// If an entity is added to the node during thi grace time, it will not be removed.
        /// </summary>
        public static int GraceTimeToRemoveEmptyNodes = 500;

        /// <summary>
        /// Return if this octree node is the octree root.
        /// </summary>
        public bool IsOctreeRoot
        {
            get {return  _octreeData.Root == this; }
        }

        /// <summary>
        /// Create the octree at top level, eg covering the whole screen.
        /// </summary>
        /// <param name="boundingBox">The starting bounding box of the entire octree. Node: nodes that are outside this box will not be culled properly.</param>
        /// <param name="maxDivisions">How many times we can divide this octree until reaching minumum size.</param>
        public OctreeCullingNode(BoundingBox boundingBox, uint maxDivisions = 6)
        {
            // create octree data
            _octreeData = new OctreeData();
            _octreeData.Root = this;
            _octreeData.MaxDivisions = maxDivisions;
            _divisionsLeft = maxDivisions;

            // init octree
            InitOctreeBox(boundingBox);
        }

        /// <summary>
        /// Create internal octree node (eg not the root octree node).
        /// </summary>
        internal OctreeCullingNode(OctreeCullingNode parent, Vector3 index, BoundingBox boundingBox, uint divisionsLeft)
        {
            // store octree data and init
            _parentOctree = parent;
            _indexInParent = index;
            _octreeData = parent._octreeData;
            _divisionsLeft = divisionsLeft;
            InitOctreeBox(boundingBox);
        }

        /// <summary>
        /// Clone this scene node.
        /// </summary>
        /// <returns>Node copy.</returns>
        public override Node Clone()
        {
            return new OctreeCullingNode(LastBoundingBox, _octreeData.MaxDivisions);
        }

        /// <summary>
        /// Initialize this octree node.
        /// </summary>
        void InitOctreeBox(BoundingBox boundingBox)
        {
            // set bounding box and sphere
            LastBoundingBox = boundingBox;
            LastBoundingSphere = BoundingSphere.CreateFromBoundingBox(LastBoundingBox);

            // if not root, remove transformations
            if (!IsOctreeRoot)
            {
                Transformations = null;
            }

            // calculate half length
            _boxHalfLength = (boundingBox.Max - boundingBox.Min).Length() * 0.5f;

            // create the octree branches (subdivisions).
            if (_divisionsLeft > 0)
            {
                // create matrix of child trees and their bounding boxes
                _childOctrees = new OctreeCullingNode[2, 2, 2];
                _childBoundingBoxes = new BoundingBox[2, 2, 2];

                // get min, max, and step
                Vector3 min = boundingBox.Min;
                Vector3 max = boundingBox.Max;
                Vector3 step = (max - min) * 0.5f;

                // init bounding boxes
                Vector3 center = (LastBoundingBox.Max - LastBoundingBox.Min);
                for (int x = 0; x < 2; ++x)
                {
                    for (int y = 0; y < 2; ++y)
                    {
                        for (int z = 0; z < 2; ++z)
                        {
                            Vector3 currMin = min + step * new Vector3(x, y, z);
                            Vector3 currMax = currMin + step;
                            _childBoundingBoxes[x, y, z] = new BoundingBox(currMin, currMax);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called every time one of the child nodes recalculate world transformations.
        /// </summary>
        /// <param name="node">The child node that updated.</param>
        public override void OnChildWorldMatrixChange(Node node)
        {
            // call base world matrix change
            base.OnChildWorldMatrixChange(node);

            // remove and add the node again to tree (because it might changed its position etc).
            RemoveFromTree(node);
            PushToTree(node);
        }

        /// <summary>
        /// Called whenever an entity was added / removed from this node.
        /// </summary>
        /// <param name="node">Node that was added / removed.</param>
        /// <param name="wasAdded">If true its a node that was added, if false, a node that was removed.</param>
        override protected void OnChildNodesListChange(Node node, bool wasAdded)
        {
            // call base child node list change
            base.OnChildNodesListChange(node, wasAdded);

            // if added, push to tree
            if (wasAdded)
            {
                PushToTree(node);
            }
            // if removed, remove from tree
            else
            {
                RemoveFromTree(node);
            }
        }

        /// <summary>
        /// Called whenever we need to push a node to the octree.
        /// </summary>
        /// <param name="node">Node to push into tree.</param>
        protected void PushToTree(Node node)
        {
            // since we just got a child node, reset the counter until destroying self
            _countToRemoveSelf = 0;

            // get the new node bounding box
            BoundingBox bb = node.GetBoundingBox();

            // nothing in it? don't add
            if (bb.Min == bb.Max)
            {
                return;
            }

            // if this is the smallest division possible, just add to node list and return
            if (_divisionsLeft == 0)
            {
                AddToSelf(node);
                return;
            }

            // if its too big to fit into one subdivision, add directly to this octree node's list
            if ((bb.Max - bb.Min).Length() > _boxHalfLength)
            {
                AddToSelf(node);
                return;
            }

            // if got here it means we need to push this into one of the subdivisions
            for (int x = 0; x < 2; ++x)
            {
                for (int y = 0; y < 2; ++y)
                {
                    for (int z = 0; z < 2; ++z)
                    {
                        if (_childBoundingBoxes[x, y, z].Contains(bb) != ContainmentType.Disjoint)
                        {
                            AddToSubTree(node, x, y, z);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add a new child node into one of the subdivision branches.
        /// This also create the subdivision octree if needed.
        /// </summary>
        protected void AddToSubTree(Node node, int x, int y, int z)
        {
            // create the subdivision branch if needed
            if (_childOctrees[x, y, z] == null)
            {
                _childOctrees[x, y, z] = new OctreeCullingNode(this, new Vector3(x, y, z), _childBoundingBoxes[x, y, z], _divisionsLeft - 1);
            }

            // add child node to branch
            _childOctrees[x, y, z].PushToTree(node);
        }

        /// <summary>
        /// Add a node to be directly under this octree node, and not under one of its children.
        /// </summary>
        /// <param name="node"></param>
        private void AddToSelf(Node node)
        {
            // add to list of nodes under this bounding box
            _nodesUnderThisOctreeBox.Add(node);
            _isNodesListDirty = true;

            // link self to node
            node.LinkToNode(this);
        }

        /// <summary>
        /// Called whenever we need to remove a node from the octree.
        /// </summary>
        /// <param name="node">Node to push into tree.</param>
        protected void RemoveFromTree(Node node)
        {
            // remove from all octree nodes
            foreach (Node linked in node.LinkedNodes)
            {
                OctreeCullingNode octreeNode = linked as OctreeCullingNode;
                if (octreeNode != null)
                {
                    octreeNode.RemoveFromSelf(node);
                }
            }
        }

        /// <summary>
        /// Return if this octree have subtrees under it.
        /// </summary>
        public bool HaveSubTrees
        {
            get
            {
                if (_childOctrees == null) { return false; }
                for (int x = 0; x < 2; ++x)
                {
                    for (int y = 0; y < 2; ++y)
                    {
                        for (int z = 0; z < 2; ++z)
                        {
                            if (_childOctrees[x, y, z] != null)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Remove a node from this octree list.
        /// </summary>
        void RemoveFromSelf(Node node)
        {
            // remove from list
            _nodesUnderThisOctreeBox.Remove(node);
            _isNodesListDirty = true;

            // check if we need to remove ourselves from parent
            // if so, start a timer that will do it after few frames (this is to reduce garbage generation due to rapidly moving objects)
            if (Empty && _nodesUnderThisOctreeBox.Count == 0 && !HaveSubTrees)
            {
                _countToRemoveSelf = 1;
            }
        }

        /// <summary>
        /// Get if this node is currently visible in camera.
        /// </summary>
        public bool IsInScreen
        {
            get
            {
                return (CullingNode.CurrentCameraFrustum.Contains(LastBoundingBox) != ContainmentType.Disjoint);
            }
        }

        /// <summary>
        /// Draw the node and its children.
        /// </summary>
        /// <param name="forceEvenIfAlreadyDrawn">If true, will draw this node even if it was already drawn in current frame.</param>
        protected override void DrawSpecific(bool forceEvenIfAlreadyDrawn = false)
        {
            // if we are in countdown to remove self mode, increase counter and remove if needed
            if (_countToRemoveSelf != 0)
            {
                if (_countToRemoveSelf++ > GraceTimeToRemoveEmptyNodes)
                {
                    _parentOctree._childOctrees[(int)_indexInParent.X, (int)_indexInParent.Y, (int)_indexInParent.Z] = null;
                }
                return;
            }

            // if camera frustum is not defined or culling disabled, draw this node without culling
            if (DisableCulling || CullingNode.CurrentCameraFrustum == null)
            {
                DrawOctreeNodeAfterVisibleCheck(forceEvenIfAlreadyDrawn);
                return;
            }

            // update transformations (only if needed, testing logic is inside)
            DoTransformationsUpdateIfNeeded();

            // check culling
            if (!IsInScreen)
            {
                return;
            }

            // draw the octree itself
            DrawOctreeNodeAfterVisibleCheck(forceEvenIfAlreadyDrawn);
        }


        /// <summary>
        /// Get if this node can hold renderable entities (if not, it means this node is just for child nodes, and not for entities.
        /// Note: an octree cannot hold entities. Only nodes.
        /// </summary>
        public override bool CanHoldEntities
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Calc final transformations for current frame.
        /// This uses an indicator to know if an update is needed, so no harm is done if you call it multiple times.
        /// </summary>
        protected override void DoTransformationsUpdateIfNeeded()
        {
            // do update only if root
            if (IsOctreeRoot)
            {
                base.DoTransformationsUpdateIfNeeded();
            }
        }

        /// <summary>
        /// Draw this octree node, after checking for culling etc.
        /// If this function is called, it means this node was tested and visible.
        /// </summary>
        protected void DrawOctreeNodeAfterVisibleCheck(bool forceEvenIfAlreadyDrawn = false)
        {
            // update transformations if needed
            DoTransformationsUpdateIfNeeded();

            // if debug mode - draw octree bounding box
            if (DebugRenderOctreeParts)
            {
                if (_debugBoundingBoxEntity == null)
                {
                    _debugBoundingBoxEntity = new BoundingBoxEntity();
                    _debugBoundingBoxEntity.Box = LastBoundingBox;
                }
                _debugBoundingBoxEntity.BoxEffect.DiffuseColor = _nodesUnderThisOctreeBox.Count > 0 ? Color.Yellow.ToVector3() : Color.Gray.ToVector3();
                _debugBoundingBoxEntity.Draw(this, ref _localTransform, ref _worldTransform);
            }

            // check if we need to extract array of nodes under this octree
            if (_isNodesListDirty)
            {
                _nodesUnderThisOctreeBoxArray = _nodesUnderThisOctreeBox.ToArray();
                _isNodesListDirty = false;
            }

            // draw nodes that are directly under this octree box.
            // remember - nodes that are small enough got into one of the subdivisions of this node, but larget nodes are 
            // directly under this tree leaf and needs to be drawn.
            foreach (Node child in _nodesUnderThisOctreeBoxArray)
            {
                child.Draw(forceEvenIfAlreadyDrawn);
            }

            // trigger draw event
            __OnNodeDraw?.Invoke(this);

            // draw the octree subdivisions (but only if we have them, eg if this is not the smallest octree node)
            if (_childOctrees != null)
            {
                for (int x = 0; x < 2; ++x)
                {
                    for (int y = 0; y < 2; ++y)
                    {
                        for (int z = 0; z < 2; ++z)
                        {
                            if (_childOctrees[x, y, z] != null)
                            {
                                _childOctrees[x, y, z].Draw(forceEvenIfAlreadyDrawn);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get up-to-date bounding box of this node and all its child nodes, and recalculate it if needed.
        /// </summary>
        /// <returns>Bounding box of the node and its children.</returns>
        public override BoundingBox GetBoundingBox()
        {
            // note: the bounding box in octree is const
            return LastBoundingBox;
        }

        /// <summary>
        /// Recalculate bounding box of this node and all its child nodes.
        /// </summary>
        /// <returns>Bounding box of the node and its children.</returns>
        public override BoundingBox UpdateBoundingBox()
        {
            // note: the bounding box in octree is const
            return LastBoundingBox;
        }

        /// <summary>
        /// Get up-to-date bounding sphere of this node and all its child nodes, and recalculate it if needed.
        /// </summary>
        /// <returns>Bounding sphere of the node and its children.</returns>
        public override BoundingSphere GetBoundingSphere()
        {
            // note: the bounding sphere in octree is const
            return LastBoundingSphere;
        }

        /// <summary>
        /// Calculate bounding sphere and return results.
        /// This also set internal caching.
        /// </summary>
        /// <returns>Bounding sphere of the node and its children.</returns>
        public override BoundingSphere UpdateBoundingSphere()
        {
            // note: the bounding sphere in octree is const
            return LastBoundingSphere;
        }
    }
}

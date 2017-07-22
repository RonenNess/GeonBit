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
// A scene node with basic Bounding-Box based culling.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GeonBit.Core.Graphics
{
    /// <summary>
    /// Bounding-Box culling node will calculate the bounding box of the node and its children, and will cull out 
    /// if it doesn't intersect with the camera frustum.
    /// </summary>
    public class BoundingBoxCullingNode : CullingNode
    {
        /// <summary>
        /// Clone this scene node.
        /// </summary>
        /// <returns>Node copy.</returns>
        public override Node Clone()
        {
            BoundingBoxCullingNode ret = new BoundingBoxCullingNode();
            ret.Transformations = Transformations.Clone();
            ret.LastBoundingBox = LastBoundingBox;
            ret.Visible = Visible;
            return ret;
        }

        /// <summary>
        /// Get if this node is currently visible in camera.
        /// </summary>
        override public bool IsInScreen
        {
            get
            {
                var bb = GetBoundingBox();
                return (bb.Min != bb.Max && CameraFrustum.Contains(bb) != ContainmentType.Disjoint);
            }
        }

        /// <summary>
        /// Get if this node is partly inside screen (eg intersects with camera frustum).
        /// </summary>
        override public bool IsPartlyInScreen
        {
            get
            {
                var bb = GetBoundingBox();
                return (bb.Min != bb.Max && CameraFrustum.Contains(GetBoundingBox()) == ContainmentType.Intersects);
            }
        }

        /// <summary>
        /// Update culling test / cached data.
        /// This is called whenever trying to draw this node after transformations update
        /// </summary>
        override protected void UpdateCullingData()
        {
        }
    }
}

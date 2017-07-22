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
// A scene node with basic Bounding-sphere based culling.
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
    /// Bounding-Sphere culling node will calculate the bounding sphere of the node and its children, and will cull out 
    /// if it doesn't intersect with the camera frustum.
    /// </summary>
    public class BoundingSphereCullingNode : CullingNode
    {
        /// <summary>
        /// Clone this scene node.
        /// </summary>
        /// <returns>Node copy.</returns>
        public override Node Clone()
        {
            BoundingSphereCullingNode ret = new BoundingSphereCullingNode();
            ret.Transformations = Transformations.Clone();
            ret.LastBoundingSphere = LastBoundingSphere;
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
                var bs = GetBoundingSphere();
                return (bs.Radius > 0f && CameraFrustum.Contains(bs) != ContainmentType.Disjoint);
            }
        }

        /// <summary>
        /// Get if this node is partly inside screen (eg intersects with camera frustum).
        /// </summary>
        override public bool IsPartlyInScreen
        {
            get
            {
                var bs = GetBoundingSphere();
                return (bs.Radius > 0f && CameraFrustum.Contains(bs) == ContainmentType.Intersects);
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

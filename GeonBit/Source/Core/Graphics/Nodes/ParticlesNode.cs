#region LICENSE
/**
 * For the purpose of making video games only, GeonBit is distributed under the MIT license.
 * to use this source code or GeonBit as a whole for any other purpose, please seek written 
 * permission from the library author.
 * 
 * Copyright (c) 2017 Ronen Ness [ronenness@gmail.com].
 * You may not remove this license notice.
 */
#endregion
#region File Description
//-----------------------------------------------------------------------------
// Scene node optimized for particle systems.
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
    /// A scene node optimized for particles.
    /// </summary>
    public class ParticleNode : BoundingSphereCullingNode
    {
        /// <summary>
        /// Clone this scene node.
        /// </summary>
        /// <returns>Node copy.</returns>
        public override Node Clone()
        {
            ParticleNode ret = new ParticleNode();
            ret.Transformations = Transformations.Clone();
            ret.LastBoundingBox = LastBoundingBox;
            ret.Visible = Visible;
            return ret;
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

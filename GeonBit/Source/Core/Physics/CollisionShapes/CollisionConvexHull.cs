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
// Collision shape for a convex hull.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Physics.CollisionShapes
{

    /// <summary>
    /// Convex-Hull collision shape.
    /// </summary>
    public class CollisionConvexHull : ICollisionShape
    {
        /// <summary>
        /// Create the collision convext hull.
        /// </summary>
        /// <param name="points">Points to create convex hull from.</param>
        public CollisionConvexHull(Vector3[] points)
        {
            // convert to bullet vectors and create the shape
            BulletSharp.Math.Vector3[] bvectors = ToBullet.Vectors(points);
            _shape = new BulletSharp.ConvexHullShape(bvectors);
        }
    }
}

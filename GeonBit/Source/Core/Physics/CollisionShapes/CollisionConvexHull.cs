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

        /// <summary>
        /// Clone the physical shape.
        /// </summary>
        /// <returns>Cloned shape.</returns>
        protected override ICollisionShape CloneImp()
        {
            // extract points from shape
            var shape = _shape as BulletSharp.ConvexHullShape;
            Vector3[] points = new Vector3[shape.NumPoints];
            int i = 0;
            foreach (var point in shape.Points)
            {
                points[i++] = ToMonoGame.Vector(point);
            }

            // create and return clone
            return new CollisionConvexHull(points);
        }
    }
}

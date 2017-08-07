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
// Collision shape for a box.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Physics.CollisionShapes
{
    /// <summary>
    /// Triangle collision shape.
    /// </summary>
    public class CollisionTriangle : ICollisionShape
    {
        /// <summary>
        /// Create the collision triangle.
        /// </summary>
        /// <param name="p1">Triangle point 1.</param>
        /// <param name="p2">Triangle point 2.</param>
        /// <param name="p3">Triangle point 2.</param>
        public CollisionTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            _shape = new BulletSharp.TriangleShape(ToBullet.Vector(p1), ToBullet.Vector(p2), ToBullet.Vector(p3));
        }

        /// <summary>
        /// Clone the physical shape.
        /// </summary>
        /// <returns>Cloned shape.</returns>
        protected override ICollisionShape CloneImp()
        {
            var shape = _shape as BulletSharp.TriangleShape;
            return new CollisionTriangle(ToMonoGame.Vector(shape.Vertices[0]), ToMonoGame.Vector(shape.Vertices[1]), ToMonoGame.Vector(shape.Vertices[2]));
        }
    }
}

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
// Collision shape for a an endless plane.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Physics.CollisionShapes
{
    /// <summary>
    /// Endless plane collision shape.
    /// </summary>
    public class CollisionEndlessPlane : ICollisionShape
    {
        /// <summary>
        /// Create the collision plane.
        /// </summary>
        /// <param name="normal">Plane normal vector.</param>
        public CollisionEndlessPlane(Vector3? normal = null)
        {
            // default normal
            if (normal == null) { normal = Vector3.Up; }

            // create the plane shape
            _shape = new BulletSharp.StaticPlaneShape(ToBullet.Vector((Vector3)normal), 1);
        }

        /// <summary>
        /// Clone the physical shape.
        /// </summary>
        /// <returns>Cloned shape.</returns>
        protected override ICollisionShape CloneImp()
        {
            var shape = _shape as BulletSharp.StaticPlaneShape;
            return new CollisionEndlessPlane(ToMonoGame.Vector(shape.PlaneNormal));
        }
    }
}

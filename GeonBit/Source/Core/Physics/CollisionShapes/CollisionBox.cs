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
    /// Box collision shape.
    /// </summary>
    public class CollisionBox : ICollisionShape
    {
        /// <summary>
        /// Create the collision box.
        /// </summary>
        /// <param name="width">Box base width (X axis).</param>
        /// <param name="height">Box base height (Y axis).</param>
        /// <param name="depth">Bow base depth (Z axis).</param>
        public CollisionBox(float width = 1f, float height = 1f, float depth = 1f)
        {
            _shape = new BulletSharp.BoxShape(width / 2f, height / 2f, depth / 2f);
        }

        /// <summary>
        /// Clone the physical shape.
        /// </summary>
        /// <returns>Cloned shape.</returns>
        protected override ICollisionShape CloneImp()
        {
            var shape = _shape as BulletSharp.BoxShape;
            Vector3 halfExtent = ToMonoGame.Vector(shape.HalfExtentsWithoutMargin);
            return new CollisionBox(halfExtent.X * 2f, halfExtent.Y * 2f, halfExtent.Z * 2f);
        }
    }
}

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
// Collision shape for a capsule.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Physics.CollisionShapes
{
    /// <summary>
    /// Which axis the capsule is aligned with.
    /// </summary>
    public enum CapsuleDirectionAxis
    {
        /// <summary>
        /// Capsule aligned with X axis.
        /// </summary>
        X,

        /// <summary>
        /// Capsule aligned with Y axis.
        /// </summary>
        Y,

        /// <summary>
        /// Capsule aligned with Z axis.
        /// </summary>
        Z,
    }

    /// <summary>
    /// Capsule collision shape.
    /// </summary>
    public class CollisionCapsule : ICollisionShape
    {
        /// <summary>
        /// Create the collision capsule.
        /// </summary>
        /// <param name="radius">Capsule radius.</param>
        /// <param name="height">Capsule height.</param>
        /// <param name="axis">Capsule axis direction.</param>
        public CollisionCapsule(float radius = 1f, float height = 1f, CapsuleDirectionAxis axis = CapsuleDirectionAxis.Y)
        {
            switch (axis)
            {
                case CapsuleDirectionAxis.X:
                    _shape = new BulletSharp.CapsuleShapeX(radius, height);
                    break;

                case CapsuleDirectionAxis.Y:
                    _shape = new BulletSharp.CapsuleShape(radius, height);
                    break;

                case CapsuleDirectionAxis.Z:
                    _shape = new BulletSharp.CapsuleShapeZ(radius, height);
                    break;
            }
        }
    }
}

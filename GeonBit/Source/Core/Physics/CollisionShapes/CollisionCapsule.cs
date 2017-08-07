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
        // cylinder axis type
        private CapsuleDirectionAxis _axisType;

        /// <summary>
        /// Create the collision capsule.
        /// </summary>
        /// <param name="radius">Capsule radius.</param>
        /// <param name="height">Capsule height.</param>
        /// <param name="axis">Capsule axis direction.</param>
        public CollisionCapsule(float radius = 1f, float height = 1f, CapsuleDirectionAxis axis = CapsuleDirectionAxis.Y)
        {
            _axisType = axis;
            switch (_axisType)
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

        /// <summary>
        /// Clone the physical shape.
        /// </summary>
        /// <returns>Cloned shape.</returns>
        protected override ICollisionShape CloneImp()
        {
            var shape = _shape as BulletSharp.CapsuleShape;
            return new CollisionCapsule(shape.Radius, shape.HalfHeight * 2f, _axisType);
        }
    }
}

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
// Collision shape for a cone.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Physics.CollisionShapes
{
    /// <summary>
    /// Which axis the cone is aligned with.
    /// </summary>
    public enum ConeDirectionAxis
    {
        /// <summary>
        /// Cone aligned with X axis.
        /// </summary>
        X,

        /// <summary>
        /// Cone aligned with Y axis.
        /// </summary>
        Y,

        /// <summary>
        /// Cone aligned with Z axis.
        /// </summary>
        Z,
    }

    /// <summary>
    /// Cone collision shape.
    /// </summary>
    public class CollisionCone : ICollisionShape
    {
        // cylinder axis type
        private ConeDirectionAxis _axisType;

        /// <summary>
        /// Create the collision cone.
        /// </summary>
        /// <param name="radius">Cone radius.</param>
        /// <param name="height">Cone height.</param>
        /// <param name="axis">Cone axis direction.</param>
        public CollisionCone(float radius = 1f, float height = 1f, ConeDirectionAxis axis = ConeDirectionAxis.Y)
        {
            _axisType = axis;
            switch (_axisType)
            {
                case ConeDirectionAxis.X:
                    _shape = new BulletSharp.ConeShapeX(radius, height);
                    break;

                case ConeDirectionAxis.Y:
                    _shape = new BulletSharp.ConeShape(radius, height);
                    break;

                case ConeDirectionAxis.Z:
                    _shape = new BulletSharp.ConeShapeZ(radius, height);
                    break;
            }
        }

        /// <summary>
        /// Clone the physical shape.
        /// </summary>
        /// <returns>Cloned shape.</returns>
        protected override ICollisionShape CloneImp()
        {
            var shape = _shape as BulletSharp.ConeShape;
            return new CollisionCone(shape.Radius, shape.Height, _axisType);
        }
    }
}

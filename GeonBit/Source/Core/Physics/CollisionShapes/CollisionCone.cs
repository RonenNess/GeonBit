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
        /// <summary>
        /// Create the collision cone.
        /// </summary>
        /// <param name="radius">Cone radius.</param>
        /// <param name="height">Cone height.</param>
        /// <param name="height">Cone axis direction.</param>
        public CollisionCone(float radius = 1f, float height = 1f, ConeDirectionAxis axis = ConeDirectionAxis.Y)
        {
            switch (axis)
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
    }
}

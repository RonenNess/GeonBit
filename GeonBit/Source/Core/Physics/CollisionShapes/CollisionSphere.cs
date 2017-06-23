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
// Collision shape for a sphere.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Physics.CollisionShapes
{
    /// <summary>
    /// Sphere shape.
    /// </summary>
    public class CollisionSphere : ICollisionShape
    {
        /// <summary>
        /// Create the collision sphere.
        /// </summary>
        /// <param name="radius">Sphere radius.</param>
        public CollisionSphere(float radius = 1f)
        {
            _shape = new BulletSharp.SphereShape(radius);
        }
    }
}

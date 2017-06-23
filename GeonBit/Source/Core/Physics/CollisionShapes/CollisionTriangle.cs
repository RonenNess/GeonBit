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
    }
}

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
// Interface for a physical collision shape.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

namespace GeonBit.Core.Physics.CollisionShapes
{
    /// <summary>
    /// The interface of a physical collision shape.
    /// </summary>
    public class ICollisionShape
    {
        /// <summary>
        /// Get the bullet collision shape.
        /// </summary>
        internal BulletSharp.CollisionShape BulletCollisionShape { get { return _shape; } }

        /// <summary>
        /// Bullet shape instance (must be set by the inheriting class).
        /// </summary>
        protected BulletSharp.CollisionShape _shape;
    }
}

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
// Compound collision shape.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Physics.CollisionShapes
{
    /// <summary>
    /// A compound shape, made of one or more collision shapes combined together.
    /// This is a great way to optimize static objects.
    /// </summary>
    public class CollisionCompoundShape : ICollisionShape
    {
        /// <summary>
        /// Create the compound shape.
        /// </summary>
        public CollisionCompoundShape()
        {
            _shape = new BulletSharp.CompoundShape();
        }

        /// <summary>
        /// Clear the compound shape and remove all children from it.
        /// </summary>
        public void Clear()
        {
            // note: while its tempting to just do '_shape = new BulletSharp.CompoundShape();' its not recommended,
            // as it will not apply bodies already using this shape!
            var shape = _shape as BulletSharp.CompoundShape;
            foreach (var child in shape.ChildList)
            {
                shape.RemoveChildShape(child.ChildShape);
            }
        }

        /// <summary>
        /// Add a child shape to this compound shape.
        /// </summary>
        /// <param name="shape">Collision shape to add.</param>
        /// <param name="transform">Transformations for child shape.</param>
        public void AddShape(ICollisionShape shape, Matrix? transform = null)
        {
            var comShape = _shape as BulletSharp.CompoundShape;
            comShape.AddChildShape(ToBullet.Matrix(transform ?? Matrix.Identity), shape.BulletCollisionShape);
        }

        /// <summary>
        /// Remove a child shape from this compound shape.
        /// </summary>
        /// <param name="shape">Collision shape to remove.</param>
        public void RemoveShape(ICollisionShape shape)
        {
            var comShape = _shape as BulletSharp.CompoundShape;
            comShape.RemoveChildShape(shape.BulletCollisionShape);
        }

        /// <summary>
        /// Clone the physical shape.
        /// </summary>
        /// <returns>Cloned shape.</returns>
        protected override ICollisionShape CloneImp()
        {
            // create a new, empty compound shape
            var ret = new CollisionCompoundShape();

            // get our compound shape
            var shape = _shape as BulletSharp.CompoundShape;

            // iterate our children and add them to the new shape
            foreach (var child in shape.ChildList)
            {
                // get shape and transformations
                var collShape = child.ChildShape;
                var transform = child.Transform;
                ((BulletSharp.CompoundShape)ret._shape).AddChildShape(transform, collShape);
            }

            // return the new compound shape
            return ret;
        }
    }
}

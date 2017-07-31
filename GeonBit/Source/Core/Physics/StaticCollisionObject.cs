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
// A static collision object.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using BulletSharp;
using Microsoft.Xna.Framework;
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("World")]

namespace GeonBit.Core.Physics
{
    /// <summary>
    /// A static collision object that don't support events and cannot be moved.
    /// This is useful for really static things that can only block and collide, but don't really do anything.
    /// </summary>
    public class StaticCollisionObject
    {
        // the collision object itself
        BulletSharp.CollisionObject _body = null;

        // collision shape
        CollisionShapes.ICollisionShape _shape;

        /// <summary>
        /// Get the rigid body in bullet format.
        /// </summary>
        internal BulletSharp.CollisionObject BulletCollisionObject { get { return _body; } }

        /// <summary>
        /// Get / set object world transformations.
        /// </summary>
        public Matrix WorldTransformations
        {
            get
            {
                return ToMonoGame.Matrix(_body.WorldTransform);
            }
            set
            {
                _body.WorldTransform = ToBullet.Matrix(value);
            }
        }

        /// <summary>
        /// Create the static collision object from shape.
        /// </summary>
        /// <param name="shape">Collision shape that define this body.</param>
        /// <param name="transformations">Starting transformations.</param>
        public StaticCollisionObject(CollisionShapes.ICollisionShape shape, Matrix? transformations = null)
        {
            // create the collision object
            _shape = shape;
            _body = new CollisionObject();
            _body.CollisionShape = shape.BulletCollisionShape;
            if (transformations != null) _body.WorldTransform = ToBullet.Matrix(transformations.Value);
        }
    }
}

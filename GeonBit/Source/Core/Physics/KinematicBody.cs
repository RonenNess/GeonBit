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
// A Kinematic Body object.
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
    public class KinematicBody : BasicPhysicalBody
    {
        /// <summary>
        /// Return the bullet 3d entity.
        /// </summary>
        internal override BulletSharp.CollisionObject _BulletEntity { get { return BulletCollisionObject; } }

        /// <summary>
        /// Get the rigid body in bullet format.
        /// </summary>
        internal BulletSharp.CollisionObject BulletCollisionObject { get; private set; }

        /// <summary>
        /// Get / set object world transformations.
        /// Get the rigid body in bullet format.
        /// </summary>
        public override Matrix WorldTransform
        {
            get
            {
                return ToMonoGame.Matrix(_BulletEntity.WorldTransform);
            }
            set
            {
                _BulletEntity.WorldTransform = ToBullet.Matrix(value);
            }
        }

        /// <summary>
        /// Create the static collision object from shape.
        /// </summary>
        /// <param name="shape">Collision shape that define this body.</param>
        /// <param name="transformations">Starting transformations.</param>
        public KinematicBody(CollisionShapes.ICollisionShape shape, Matrix? transformations = null)
        {
            // create the collision object
            _shape = shape;
            BulletCollisionObject = new CollisionObject();
            BulletCollisionObject.CollisionShape = shape.BulletCollisionShape;

            // by default turn off activation and collision events
            BulletCollisionObject.ActivationState = ActivationState.DisableSimulation;
            InvokeCollisionEvents = false;

            // if provided, set transformations
            if (transformations != null) BulletCollisionObject.WorldTransform = ToBullet.Matrix(transformations.Value);
        }

        /// <summary>
        ///  Attach self to a bullet3d physics world.
        /// </summary>
        /// <param name="world"></param>
        internal override void AddSelfToBulletWorld(BulletSharp.DynamicsWorld world)
        {
            world.AddCollisionObject(BulletCollisionObject, CollisionGroup, CollisionMask);
        }

        /// <summary>
        /// Remove self from a bullet3d physics world.
        /// </summary>
        /// <param name="world">World to remove from.</param>
        internal override void RemoveSelfFromBulletWorld(BulletSharp.DynamicsWorld world)
        {
            world.RemoveCollisionObject(BulletCollisionObject);
        }
    }
}

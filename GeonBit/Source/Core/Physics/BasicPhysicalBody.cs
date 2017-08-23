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
// Basic physical body entity.
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
    /// A Physical body is the basic object of the physics world.
    /// They are affected by forces and collide with each other.
    /// Physical Entity = Rigid Body + Collision Shape.
    /// </summary>
    public class BasicPhysicalBody
    {
        /// <summary>
        /// Return the bullet 3d entity.
        /// </summary>
        internal virtual BulletSharp.CollisionObject _BulletEntity { get; }

        /// <summary>
        /// The collision shape used with this body.
        /// </summary>
        protected CollisionShapes.ICollisionShape _shape;

        // containing world instance.
        internal PhysicsWorld _world;

        /// <summary>
        /// Get the collision shape.
        /// </summary>
        public CollisionShapes.ICollisionShape Shape { get { return _shape; } }

        /// <summary>
        /// Return if the physical body is currently active.
        /// </summary>
        public bool IsActive
        {
            get { return _BulletEntity.IsActive; }
        }

        /// <summary>
        /// Get world transform from physical body.
        /// </summary>
        virtual public Matrix WorldTransform
        {
            // get world transformations
            get
            {
                // get the bullet transform matrix
                BulletSharp.Math.Matrix btMatrix;
                _BulletEntity.GetWorldTransform(out btMatrix);

                // convert to MonoGame and return (also apply scale, which is stored seperately)
                return Matrix.CreateScale(Scale) * ToMonoGame.Matrix(btMatrix);
            }

            // set world transformations
            set
            {
                // convert to bullet matrix
                BulletSharp.Math.Matrix btMatrix = ToBullet.Matrix(value);

                // set motion state
                _BulletEntity.WorldTransform = btMatrix;

                // set scale
                Vector3 scale; Vector3 position; Quaternion rotation;
                value.Decompose(out scale, out rotation, out position);
                _BulletEntity.CollisionShape.LocalScaling = ToBullet.Vector(scale);
                UpdateAABB();
            }
        }
        
        /// <summary>
        /// If false, will not simulate forces on this body and will make it behave like a kinematic body.
        /// </summary>
        virtual public bool EnableSimulation
        {
            set
            {
                _BulletEntity.ActivationState = value ? ActivationState.ActiveTag : ActivationState.DisableSimulation;
            }
            get
            {
                return _BulletEntity.ActivationState != ActivationState.DisableSimulation;
            }
        }

        /// <summary>
        /// Get / set body restitution.
        /// </summary>
        public float Restitution
        {
            get
            {
                return _BulletEntity.Restitution;
            }
            set
            {
                _BulletEntity.Restitution = value;
            }
        }

        /// <summary>
        /// Get / Set body scale.
        /// </summary>
        public Vector3 Scale
        {
            get
            {
                return ToMonoGame.Vector(_BulletEntity.CollisionShape.LocalScaling);
            }
            set
            {
                _BulletEntity.CollisionShape.LocalScaling = ToBullet.Vector(value);
                UpdateAABB();
            }
        }

        /// <summary>
        /// Get / set body friction.
        /// </summary>
        public float Friction
        {
            get { return _BulletEntity.Friction; }
            set { _BulletEntity.Friction = Friction; }
        }

        /// <summary>
        /// Get / set body position.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                // get transform
                BulletSharp.Math.Matrix world;
                _BulletEntity.GetWorldTransform(out world);

                // return position
                return ToMonoGame.Vector(world.Origin);
            }
            set
            {
                // get transform
                BulletSharp.Math.Matrix world;
                _BulletEntity.GetWorldTransform(out world);

                // update origin
                world.Origin = ToBullet.Vector(value);

                // set position
                _BulletEntity.WorldTransform = world;
                UpdateAABB();
            }
        }

        /// <summary>
        /// Update axis-aligned-bounding-box, after transformations of this object were changed.
        /// </summary>
        virtual protected void UpdateAABB()
        {
            if (_world != null) { _world.UpdateSingleAabb(this); }
        }

        /// <summary>
        /// Return if this is a static object.
        /// </summary>
        virtual public bool IsStatic { get { return false; } }

        /// <summary>
        /// Return if this is a kinematic object.
        /// </summary>
        virtual public bool IsKinematic { get { return false; } }

        /// <summary>
        /// Get collision flags based on current state.
        /// </summary>
        protected CollisionFlags CollisionFlags
        {
            get
            {
                CollisionFlags ret = CollisionFlags.None;
                if (IsStatic) ret |= CollisionFlags.StaticObject;
                if (IsKinematic) ret |= CollisionFlags.KinematicObject;
                if (IsEthereal) ret |= CollisionFlags.NoContactResponse;
                if (InvokeCollisionEvents) ret |= CollisionFlags.CustomMaterialCallback;
                return ret;
            }
        }

        /// <summary>
        /// If true (default) will invoke collision events.
        /// You can turn this off for optimizations.
        /// </summary>
        public bool InvokeCollisionEvents
        {
            get
            {
                return _invokeCollisionEvents;
            }
            set
            {
                _invokeCollisionEvents = value;
                UpdateCollisionFlags();
            }
        }
        private bool _invokeCollisionEvents = true;

        /// <summary>
        /// If true, this object will not have a physical body (eg other objects will pass through it), but will still trigger contact events. 
        /// </summary>
        public bool IsEthereal
        {
            get
            {
                return _isEthereal;
            }
            set
            {
                _isEthereal = value;
                UpdateCollisionFlags();
            }
        }
        private bool _isEthereal = false;

        /// <summary>
        /// Update collision flags.
        /// </summary>
        protected void UpdateCollisionFlags()
        {
            _BulletEntity.CollisionFlags = CollisionFlags;
        }

        /// <summary>
        /// The component associated with this physical body.
        /// </summary>
        internal ECS.Components.Physics.BasePhysicsComponent EcsComponent;

        // current collision group
        short _collisionGroup = short.MaxValue;

        /// <summary>
        /// The collision group this body belongs to.
        /// Note: compare bits mask.
        /// </summary>
        public short CollisionGroup
        {
            get { return _collisionGroup; }
            set { _collisionGroup = value; AddBodyAgain(); }
        }

        // current collision mask
        short _collisionMask = short.MaxValue;
        
        /// <summary>
        /// With which collision groups this body will collide?
        /// Note: compare bits mask.
        /// </summary>
        public short CollisionMask
        {
            get { return _collisionMask; }
            set { _collisionMask = value; AddBodyAgain(); }
        }

        /// <summary>
        /// Called internally to remove and re-add this body again to the physical world.
        /// This action is required when some of the properties require an updated.
        /// </summary>
        virtual protected void AddBodyAgain()
        {
            if (_world != null)
            {
                PhysicsWorld world = _world;
                world.RemoveBody(this);
                world.AddBody(this);
            }
        }

        /// <summary>
        /// Called when this physical body start colliding with another body.
        /// </summary>
        /// <param name="other">The other body we collide with.</param>
        /// <param name="data">Extra collision data.</param>
        public void CallCollisionStart(BasicPhysicalBody other, ref CollisionData data)
        {
            EcsComponent.CallCollisionStart(other.EcsComponent, data);
        }

        /// <summary>
        /// Called when this physical body stop colliding with another body.
        /// </summary>
        /// <param name="other">The other body we collided with, but no longer.</param>
        public void CallCollisionEnd(BasicPhysicalBody other)
        {
            EcsComponent.CallCollisionEnd(other.EcsComponent);
        }

        /// <summary>
        /// Called while this physical body is colliding with another body.
        /// </summary>
        /// <param name="other">The other body we are colliding with.</param>
        public void CallCollisionProcess(BasicPhysicalBody other)
        {
            EcsComponent.CallCollisionProcess(other.EcsComponent);
        }

        /// <summary>
        /// Attach self to a bullet3d physics world.
        /// </summary>
        /// <param name="world">World to add to.</param>
        internal virtual void AddSelfToBulletWorld(BulletSharp.DynamicsWorld world)
        {
        }

        /// <summary>
        /// Remove self from a bullet3d physics world.
        /// </summary>
        /// <param name="world">World to remove from.</param>
        internal virtual void RemoveSelfFromBulletWorld(BulletSharp.DynamicsWorld world)
        {
        }
    }
}

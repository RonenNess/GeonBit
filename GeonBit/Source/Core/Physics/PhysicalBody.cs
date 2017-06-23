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
// Physical Body is the basic object of the physics world.
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
    public class PhysicalBody
    {
        // the rigid body itself
        RigidBody _body = null;

        // collision shape
        CollisionShapes.ICollisionShape _shape;

        // current state (position, rotation, scale).
        MotionState _state;

        /// <summary>
        /// Get the rigid body in bullet format.
        /// </summary>
        internal RigidBody BulletRigidBody { get { return _body; } }

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
            get { return _body.IsActive; }
        }

        /// <summary>
        /// Get world transform from physical body.
        /// </summary>
        public Matrix WorldTransform
        {
            // get world transformations
            get
            {
                // get the bullet transform matrix
                BulletSharp.Math.Matrix btMatrix;
                _body.GetWorldTransform(out btMatrix);

                // convert to MonoGame and return (also apply scale, which is stored seperately)
                return Matrix.CreateScale(Scale) * ToMonoGame.Matrix(btMatrix);
            }

            // set world transformations
            set
            {
                // convert to bullet matrix
                BulletSharp.Math.Matrix btMatrix = ToBullet.Matrix(value);

                // set motion state
               // _body.MotionState.WorldTransform = btMatrix;
                _body.WorldTransform = btMatrix;
                // _body.MotionState.SetWorldTransform(ref btMatrix);

                // set scale
                Scale = value.Scale;
            }
        }
        
        /// <summary>
        /// Get / Set body scale.
        /// </summary>
        public Vector3 Scale
        {
            get
            {
                return ToMonoGame.Vector(_body.CollisionShape.LocalScaling);
            }
            set
            {
                _body.CollisionShape.LocalScaling = ToBullet.Vector(value);
                if (_world != null) { _world._world.UpdateSingleAabb(_body); }
            }
        }

        /// <summary>
        /// Get if the physical body currently have any forces on it.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _body.AngularVelocity.Length != 0 || _body.LinearVelocity.Length != 0;
            }
        }

        /// <summary>
        /// Apply force on the body, from its center.
        /// </summary>
        /// <param name="force">Force to apply.</param>
        public void ApplyForce(Vector3 force)
        {
            _body.ApplyCentralForce(ToBullet.Vector(force));
            _body.Activate();
        }

        /// <summary>
        /// Apply force on the body, from a given position.
        /// </summary>
        /// <param name="force">Force to apply.</param>
        /// <param name="from">Force source position.</param>
        public void ApplyForce(Vector3 force, Vector3 from)
        {
            _body.ApplyForce(ToBullet.Vector(force), ToBullet.Vector(from));
            _body.Activate();
        }

        /// <summary>
        /// Apply impulse on the body, from its center.
        /// </summary>
        /// <param name="impulse">Impulse to apply.</param>
        public void ApplyImpulse(Vector3 impulse)
        {
            _body.ApplyCentralImpulse(ToBullet.Vector(impulse));
            _body.Activate();
        }

        /// <summary>
        /// Apply torque force on the body.
        /// </summary>
        /// <param name="torque">Torque force to apply.</param>
        /// <param name="asImpulse">If true, will apply torque as an impulse.</param>
        public void ApplyTorque(Vector3 torque, bool asImpulse = false)
        {
            if (asImpulse)
            {
                _body.ApplyTorqueImpulse(ToBullet.Vector(torque));
            }
            else
            {
                _body.ApplyTorque(ToBullet.Vector(torque));
            }
            _body.Activate();
        }

        /// <summary>
        /// Apply impulse on the body, from a given position.
        /// </summary>
        /// <param name="impulse">Impulse to apply.</param>
        /// <param name="from">Impulse source position.</param>
        public void ApplyImpulse(Vector3 impulse, Vector3 from)
        {
            _body.ApplyImpulse(ToBullet.Vector(impulse), ToBullet.Vector(from));
            _body.Activate();
        }

        /// <summary>
        /// Get the axis-aligned bounding box of this physical body.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                BulletSharp.Math.Vector3 min;
                BulletSharp.Math.Vector3 max;
                _body.GetAabb(out min, out max);
                return new BoundingBox(ToMonoGame.Vector(min), ToMonoGame.Vector(max));
            }
        }

        /// <summary>
        /// Get / set body friction.
        /// </summary>
        public float Friction
        {
            get { return _body.Friction; }
            set { _body.Friction = Friction; }
        }

        /// <summary>
        /// Clear all forces from the body.
        /// </summary>
        /// <param name="clearVelocity">If true, will also clear current velocity.</param>
        public void ClearForces(bool clearVelocity = true)
        {
            _body.ClearForces();
            if (clearVelocity)
            {
                ClearVelocity();
            }
        }

        /// <summary>
        /// Copy transformation, forces, velocity etc. from another physical body.
        /// </summary>
        /// <param name="other">Other physical body to copy condition from.</param>
        public void CopyConditionFrom(PhysicalBody other)
        {
            BulletSharp.Math.Matrix trans = _body.WorldTransform;
            other._body.GetWorldTransform(out trans);
            _body.MotionState.SetWorldTransform(ref trans);
            _body.WorldTransform = trans;
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
                _body.GetWorldTransform(out world);

                // return position
                return ToMonoGame.Vector(world.Origin);
            }
            set
            {
                // get transform
                BulletSharp.Math.Matrix world;
                _body.GetWorldTransform(out world);

                // update origin
                world.Origin = ToBullet.Vector(value);

                // set position
                _body.WorldTransform = world;
            }
        }

        /// <summary>
        /// Clear current velocity.
        /// </summary>
        public void ClearVelocity()
        {
            _body.AngularVelocity = _body.LinearVelocity = BulletSharp.Math.Vector3.Zero;
        }

        /// <summary>
        /// Get / set linear velocity.
        /// </summary>
        public Vector3 LinearVelocity
        {
            get { return ToMonoGame.Vector(_body.LinearVelocity); }
            set { _body.LinearVelocity = ToBullet.Vector(value); }
        }

        /// <summary>
        /// Get / set linear factor.
        /// </summary>
        public Vector3 LinearFactor
        {
            get { return ToMonoGame.Vector(_body.LinearFactor); }
            set { _body.LinearFactor = ToBullet.Vector(value); }
        }

        /// <summary>
        /// Get / set angular velocity.
        /// </summary>
        public Vector3 AngularVelocity
        {
            get { return ToMonoGame.Vector(_body.AngularVelocity); }
            set { _body.AngularVelocity = ToBullet.Vector(value); }
        }

        /// <summary>
        /// Get / set angular factor.
        /// </summary>
        public Vector3 AngularFactor
        {
            get { return ToMonoGame.Vector(_body.AngularFactor); }
            set { _body.AngularFactor = ToBullet.Vector(value); }
        }

        /// <summary>
        /// Set damping.
        /// </summary>
        /// <param name="linear">Linear damping.</param>
        /// <param name="angular">Angular damping.</param>
        public void SetDamping(float linear, float angular)
        {
            _body.SetDamping(linear, angular);
        }

        // alternative gravity specific for this object.
        // note: we need to store it internally because this must be set to the body AFTER its added to the 
        // world, otherwise this property is ignored. 
        Vector3? _bodyGravity = null;

        /// <summary>
        /// If true (default) will invoke collision events.
        /// You can turn this off for optimizations.
        /// </summary>
        public bool InvokeCollisionEvents
        {
            // get if this body invoke collision events
            get
            {
                return (_body.CollisionFlags & CollisionFlags.CustomMaterialCallback) != 0;
            }

            // set if this body invoke collision events
            set
            {
                if (value)
                {
                    _body.CollisionFlags |= CollisionFlags.CustomMaterialCallback;
                }
                else
                {
                    _body.CollisionFlags &=  ~CollisionFlags.CustomMaterialCallback;
                }
            }
        }

        /// <summary>
        /// If true, this object will not have a physical body (eg other objects will pass through it), but will still trigger contact events. 
        /// </summary>
        public bool IsEthereal
        {
            // get if this body is ethereal
            get
            {
                return (_body.CollisionFlags & CollisionFlags.NoContactResponse) != 0;
            }

            // set if this body is ethereal
            set
            {
                if (value)
                {
                    _body.CollisionFlags |= CollisionFlags.NoContactResponse;
                }
                else
                {
                    _body.CollisionFlags &= ~CollisionFlags.NoContactResponse;
                }
            }
        }

        /// <summary>
        /// Return true if this body has alternative gravity (instead of default world gravity).
        /// </summary>
        public bool HasCustomGravity
        {
            get { return _bodyGravity != null; }
        }

        /// <summary>
        /// Set / get body gravity factor.
        /// </summary>
        public Vector3? Gravity
        {
            // get body gravity
            get { return _bodyGravity; }

            // set body gravity
            set
            {
                // if we got a new valid value, set it on body
                if (value != null)
                {
                    _body.Gravity = ToBullet.Vector((Vector3)value);
                }
                // if got null, set body to be world default gravity
                else
                {
                    if (_world != null)
                    {
                        BulletSharp.Math.Vector3 grav = new BulletSharp.Math.Vector3();
                        _world._world.GetGravity(out grav);
                        _body.Gravity = grav;
                    }
                }

                // store gravity locally
                _bodyGravity = value;
            }
        }

        /// <summary>
        /// Get / set linear damping.
        /// </summary>
        public float LinearDamping
        {
            get { return _body.LinearDamping; }
            set { SetDamping(value, _body.AngularDamping); }
        }

        /// <summary>
        /// Get / set angular damping.
        /// </summary>
        public float AngularDamping
        {
            get { return _body.AngularDamping; }
            set { SetDamping(_body.LinearDamping, value); }
        }

        /// <summary>
        /// Update the mass and inertia of this body.
        /// </summary>
        /// <param name="mass">New body mass.</param>
        /// <param name="inertia">New body inertia.</param>
        public void SetMassAndInertia(float mass, float inertia)
        {
            _body.SetMassProps(mass, _shape.BulletCollisionShape.CalculateLocalInertia(mass) * inertia);
        }

        /// <summary>
        /// The physical body component associated with this physical body.
        /// </summary>
        internal ECS.Components.Physics.PhysicalBody EcsComponent;

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
        /// Called internally to remove and add this body again to the collision world.
        /// This action is required when some of the properties require an updated.
        /// </summary>
        protected void AddBodyAgain()
        {
            if (_world != null)
            {
                PhysicsWorld world = _world;
                world.RemoveBody(this);
                world.AddBody(this);
            }
        }

        /// <summary>
        /// Create the physical entity.
        /// </summary>
        /// <param name="shape">Collision shape that define this body.</param>
        /// <param name="mass">Body mass (in kg), or 0 for static.</param>
        /// <param name="inertia">Body inertia, or 0 for static.</param>
        /// <param name="transformations">Starting transformations.</param>
        public PhysicalBody(CollisionShapes.ICollisionShape shape, float mass = 10f, float inertia = 1f, Matrix? transformations = null)
        {
            // store collision shape
            _shape = shape;

            // set default transformations
            transformations = transformations ?? Matrix.Identity;

            // create starting state
            _state = new DefaultMotionState(ToBullet.Matrix((Matrix)(transformations)));

            // create the rigid body construction info
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(
                mass, 
                _state, 
                shape.BulletCollisionShape, 
                shape.BulletCollisionShape.CalculateLocalInertia(mass) * inertia);

            // create the rigid body itself and attach self to UserObject
            _body = new RigidBody(info);
            _body.UserObject = this;

            // set some defaults
            InvokeCollisionEvents = true;
            IsEthereal = false;
        }

        /// <summary>
        /// Called when this physical body start colliding with another body.
        /// </summary>
        /// <param name="other">The other body we collide with.</param>
        /// <param name="data">Extra collision data.</param>
        public void CallCollisionStart(PhysicalBody other, CollisionData data)
        {
            EcsComponent.CallCollisionStart(other.EcsComponent, data);
        }

        /// <summary>
        /// Called when this physical body stop colliding with another body.
        /// </summary>
        /// <param name="other">The other body we collided with, but no longer.</param>
        public void CallCollisionEnd(PhysicalBody other)
        {
            EcsComponent.CallCollisionEnd(other.EcsComponent);
        }

        /// <summary>
        /// Called while this physical body is colliding with another body.
        /// </summary>
        /// <param name="other">The other body we are colliding with.</param>
        public void CallCollisionProcess(PhysicalBody other)
        {
            EcsComponent.CallCollisionProcess(other.EcsComponent);
        }
    }
}

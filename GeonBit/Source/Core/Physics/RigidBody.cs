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
// Rigid body object.
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
    /// A rigid body is the basic dynamic object of the physics world.
    /// They are affected by forces, collide with each other, and trigger different events.
    /// </summary>
    public class RigidBody : BasicPhysicalBody
    {
        /// <summary>
        /// Return the bullet 3d entity.
        /// </summary>
        internal override BulletSharp.CollisionObject _BulletEntity { get { return BulletRigidBody; } }

        // current state (position, rotation, scale).
        MotionState _state;

        /// <summary>
        /// Get the rigid body in bullet format.
        /// </summary>
        internal BulletSharp.RigidBody BulletRigidBody { get; private set; }

        /// <summary>
        /// Get if the physical body currently have any forces on it.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return BulletRigidBody.AngularVelocity.Length != 0 || BulletRigidBody.LinearVelocity.Length != 0;
            }
        }

        /// <summary>
        /// Apply force on the body, from its center.
        /// </summary>
        /// <param name="force">Force to apply.</param>
        public void ApplyForce(Vector3 force)
        {
            BulletRigidBody.ApplyCentralForce(ToBullet.Vector(force));
            BulletRigidBody.Activate();
        }

        /// <summary>
        /// Apply force on the body, from a given position.
        /// </summary>
        /// <param name="force">Force to apply.</param>
        /// <param name="from">Force source position.</param>
        public void ApplyForce(Vector3 force, Vector3 from)
        {
            BulletRigidBody.ApplyForce(ToBullet.Vector(force), ToBullet.Vector(from));
            BulletRigidBody.Activate();
        }

        /// <summary>
        /// Apply impulse on the body, from its center.
        /// </summary>
        /// <param name="impulse">Impulse to apply.</param>
        public void ApplyImpulse(Vector3 impulse)
        {
            BulletRigidBody.ApplyCentralImpulse(ToBullet.Vector(impulse));
            BulletRigidBody.Activate();
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
                BulletRigidBody.ApplyTorqueImpulse(ToBullet.Vector(torque));
            }
            else
            {
                BulletRigidBody.ApplyTorque(ToBullet.Vector(torque));
            }
            BulletRigidBody.Activate();
        }

        /// <summary>
        /// Apply impulse on the body, from a given position.
        /// </summary>
        /// <param name="impulse">Impulse to apply.</param>
        /// <param name="from">Impulse source position.</param>
        public void ApplyImpulse(Vector3 impulse, Vector3 from)
        {
            BulletRigidBody.ApplyImpulse(ToBullet.Vector(impulse), ToBullet.Vector(from));
            BulletRigidBody.Activate();
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
                BulletRigidBody.GetAabb(out min, out max);
                return new BoundingBox(ToMonoGame.Vector(min), ToMonoGame.Vector(max));
            }
        } 

        /// <summary>
        /// Clear all forces from the body.
        /// </summary>
        /// <param name="clearVelocity">If true, will also clear current velocity.</param>
        public void ClearForces(bool clearVelocity = true)
        {
            BulletRigidBody.ClearForces();
            if (clearVelocity)
            {
                ClearVelocity();
            }
        }

        /// <summary>
        /// Copy transformation, forces, velocity etc. from another physical body.
        /// </summary>
        /// <param name="other">Other physical body to copy condition from.</param>
        public void CopyConditionFrom(RigidBody other)
        {
            BulletSharp.Math.Matrix trans = BulletRigidBody.WorldTransform;
            other.BulletRigidBody.GetWorldTransform(out trans);
            BulletRigidBody.MotionState.SetWorldTransform(ref trans);
            BulletRigidBody.WorldTransform = trans;
        }
        
        /// <summary>
        /// Clear current velocity.
        /// </summary>
        public void ClearVelocity()
        {
            BulletRigidBody.AngularVelocity = BulletRigidBody.LinearVelocity = BulletSharp.Math.Vector3.Zero;
        }

        /// <summary>
        /// Get / set linear velocity.
        /// </summary>
        public Vector3 LinearVelocity
        {
            get { return ToMonoGame.Vector(BulletRigidBody.LinearVelocity); }
            set { BulletRigidBody.LinearVelocity = ToBullet.Vector(value); }
        }

        /// <summary>
        /// Get / set linear factor.
        /// </summary>
        public Vector3 LinearFactor
        {
            get { return ToMonoGame.Vector(BulletRigidBody.LinearFactor); }
            set { BulletRigidBody.LinearFactor = ToBullet.Vector(value); }
        }

        /// <summary>
        /// Get / set angular velocity.
        /// </summary>
        public Vector3 AngularVelocity
        {
            get { return ToMonoGame.Vector(BulletRigidBody.AngularVelocity); }
            set { BulletRigidBody.AngularVelocity = ToBullet.Vector(value); }
        }

        /// <summary>
        /// Get / set angular factor.
        /// </summary>
        public Vector3 AngularFactor
        {
            get { return ToMonoGame.Vector(BulletRigidBody.AngularFactor); }
            set { BulletRigidBody.AngularFactor = ToBullet.Vector(value); }
        }

        /// <summary>
        /// Set damping.
        /// </summary>
        /// <param name="linear">Linear damping.</param>
        /// <param name="angular">Angular damping.</param>
        public void SetDamping(float linear, float angular)
        {
            BulletRigidBody.SetDamping(linear, angular);
        }

        // alternative gravity specific for this object.
        // note: we need to store it internally because this must be set to the body AFTER its added to the 
        // world, otherwise this property is ignored. 
        Vector3? _bodyGravity = null;

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
                    BulletRigidBody.Gravity = ToBullet.Vector((Vector3)value);
                }
                // if got null, set body to be world default gravity
                else
                {
                    if (_world != null)
                    {
                        BulletSharp.Math.Vector3 grav = new BulletSharp.Math.Vector3();
                        _world._world.GetGravity(out grav);
                        BulletRigidBody.Gravity = grav;
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
            get { return BulletRigidBody.LinearDamping; }
            set { SetDamping(value, BulletRigidBody.AngularDamping); }
        }

        /// <summary>
        /// Get / set angular damping.
        /// </summary>
        public float AngularDamping
        {
            get { return BulletRigidBody.AngularDamping; }
            set { SetDamping(BulletRigidBody.LinearDamping, value); }
        }

        /// <summary>
        /// Update the mass and inertia of this body.
        /// </summary>
        /// <param name="mass">New body mass.</param>
        /// <param name="inertia">New body inertia.</param>
        public void SetMassAndInertia(float mass, float inertia)
        {
            BulletRigidBody.SetMassProps(mass, _shape.BulletCollisionShape.CalculateLocalInertia(mass) * inertia);
        }

        /// <summary>
        /// Create the physical entity.
        /// </summary>
        /// <param name="shape">Collision shape that define this body.</param>
        /// <param name="mass">Body mass (in kg), or 0 for static.</param>
        /// <param name="inertia">Body inertia, or 0 for static.</param>
        /// <param name="transformations">Starting transformations.</param>
        public RigidBody(CollisionShapes.ICollisionShape shape, float mass = 10f, float inertia = 1f, Matrix? transformations = null)
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
            BulletRigidBody = new BulletSharp.RigidBody(info);
            BulletRigidBody.UserObject = this;

            // set default group and mask
            CollisionGroup = CollisionGroups.DynamicObjects;
            CollisionMask = CollisionMasks.Targets;

            // set some defaults
            InvokeCollisionEvents = true;
            IsEthereal = false;
        }

        /// <summary>
        ///  Attach self to a bullet3d physics world.
        /// </summary>
        /// <param name="world"></param>
        internal override void AddSelfToBulletWorld(BulletSharp.DynamicsWorld world)
        {
            // update flags
            UpdateCollisionFlags();

            // add to world
            world.AddRigidBody(BulletRigidBody, CollisionGroup, CollisionMask);

            // if this rigid body has custom gravity, override it by self value after adding to world.
            // this is because when you add a body to Bullet3d world its gravity gets overriden.
            if (HasCustomGravity)
            {
                Gravity = Gravity;
            }
        }

        /// <summary>
        /// Remove self from a bullet3d physics world.
        /// </summary>
        /// <param name="world">World to remove from.</param>
        internal override void RemoveSelfFromBulletWorld(BulletSharp.DynamicsWorld world)
        {
            world.RemoveRigidBody(BulletRigidBody);
        }
    }
}

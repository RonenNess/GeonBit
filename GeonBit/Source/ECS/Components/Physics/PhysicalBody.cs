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
// Physical body component.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Physics
{
    /// <summary>
    /// A physical body component.
    /// </summary>
    public class PhysicalBody : BaseComponent
    {
        /// <summary>
        /// The physical body in the core layer.
        /// </summary>
        internal Core.Physics.PhysicalBody _body = null;

        /// <summary>
        /// The shape used for this physical body.
        /// </summary>
        private Core.Physics.CollisionShapes.ICollisionShape _shape = null;

        /// <summary>
        /// Set / get body scale.
        /// </summary>
        public Vector3 Scale
        {
            get { return _body.Scale; }
            set { _body.Scale = value; }
        }

        // body mass
        float _mass;

        // body inertia
        float _intertia;

        // did we already updated body in current frame?
        // this is needed for optimization because draw calls and update calls are not 1:1.
        bool _alreadyUpdatedBodyInFrame = false;

        /// <summary>
        /// Get / set body mass.
        /// </summary>
        public float Mass
        {
            get { return _mass; }
            set { _mass = value; _body.SetMassAndInertia(_mass, _intertia); }
        }

        /// <summary>
        /// Get / set body inertia.
        /// </summary>
        public float Inertia
        {
            get { return _intertia; }
            set { _intertia = value; _body.SetMassAndInertia(_mass, _intertia); }
        }

        /// <summary>
        /// Get / set physical body world transformation.
        /// </summary>
        public Matrix WorldTransform
        {
            get { return _body.WorldTransform; }
            set { _body.WorldTransform = value; }
        }

        /// <summary>
        /// Get / set current body position.
        /// </summary>
        public Vector3 Position
        {
            get { return _body.Position; }
            set { _body.Position = value; }
        }

        /// <summary>
        /// The collision group this body belongs to.
        /// Note: compare bits mask.
        /// </summary>
        public short CollisionGroup
        {
            get { return _body.CollisionGroup; }
            set { _body.CollisionGroup = value; }
        }

        /// <summary>
        /// With which collision groups this body will collide?
        /// Note: compare bits mask.
        /// </summary>
        public short CollisionMask
        {
            get { return _body.CollisionMask; }
            set { _body.CollisionMask = value; }
        }

        /// <summary>
        /// If true (default) will invoke collision events.
        /// You can turn this off for optimizations.
        /// </summary>
        public bool InvokeCollisionEvents
        {
            get { return _body.InvokeCollisionEvents; }
            set { _body.InvokeCollisionEvents = value; }
        }

        /// <summary>
        /// If ethereal, other bodies will be able to pass through this object, but it will still trigger contact events.
        /// </summary>
        public bool IsEthereal
        {
            get { return _body.IsEthereal; }
            set { _body.IsEthereal = value; }
        }

        /// <summary>
        /// Get / set body restitution.
        /// </summary>
        public float Restitution
        {
            get { return _body.Restitution; }
            set { _body.Restitution = value; }
        }

        /// <summary>
        /// Optional constant velocity to set for this physical body.
        /// </summary>
        public Vector3? ConstVelocity = null;

        /// <summary>
        /// Optional constant velocity to set for this physical body.
        /// </summary>
        public Vector3? ConstAngularVelocity = null;

        /// <summary>
        /// Optional constant force to set for this physical body.
        /// </summary>
        public Vector3? ConstForce = null;

        /// <summary>
        /// Optional constant angular force to set for this physical body.
        /// </summary>
        public Vector3? ConstTorqueForce = null;

        // are we currently in physics world?
        bool _isInWorld = false;

        /// <summary>
        /// Create the physical body.
        /// </summary>
        /// <param name="shapeInfo">Body shape info.</param>
        /// <param name="mass">Body mass (0 for static).</param>
        /// <param name="inertia">Body inertia (0 for static).</param>
        /// <param name="friction">Body friction.</param>
        public PhysicalBody(IBodyShapeInfo shapeInfo, float mass = 0f, float inertia = 0f, float friction = 1f)
        {
            CreateBody(shapeInfo.CreateShape(), mass, inertia, friction);
        }

        /// <summary>
        /// Create the physical body from shape instance.
        /// </summary>
        /// <param name="shape">Physical shape to use.</param>
        /// <param name="mass">Body mass (0 for static).</param>
        /// <param name="inertia">Body inertia (0 for static).</param>
        /// <param name="friction">Body friction.</param>
        public PhysicalBody(Core.Physics.CollisionShapes.ICollisionShape shape, float mass = 0f, float inertia = 0f, float friction = 1f)
        {
            CreateBody(shape, mass, inertia, friction);
        }

        /// <summary>
        /// Create the actual collision body.
        /// </summary>
        /// <param name="shape">Collision shape.</param>
        /// <param name="mass">Body mass.</param>
        /// <param name="inertia">Body inertia.</param>
        /// <param name="friction">Body friction.</param>
        private void CreateBody(Core.Physics.CollisionShapes.ICollisionShape shape, float mass, float inertia, float friction)
        {
            // store params and create the body
            _mass = mass;
            _intertia = inertia;
            _body = new Core.Physics.PhysicalBody(shape, mass, inertia);
            _body.Friction = friction;
            _shape = shape;

            // set self as attached data (needed for collision events)
            _body.EcsComponent = this;
        }

        /// <summary>
        /// Called when this physical body start colliding with another body.
        /// </summary>
        /// <param name="other">The other body we collide with.</param>
        /// <param name="data">Extra collision data.</param>
        public void CallCollisionStart(PhysicalBody other, Core.Physics.CollisionData data)
        {
            if (_GameObject != null)
            {
                _GameObject.CallCollisionStart(other._GameObject, data);
            }
        }
        
        /// <summary>
        /// Called when this physical body stop colliding with another body.
        /// </summary>
        /// <param name="other">The other body we collided with, but no longer.</param>
        public void CallCollisionEnd(PhysicalBody other)
        {
            if (_GameObject != null)
            {
                _GameObject.CallCollisionEnd(other._GameObject);
            }
        }

        /// <summary>
        /// Called while this physical body is colliding with another body.
        /// </summary>
        /// <param name="other">The other body we are colliding with.</param>
        public void CallCollisionProcess(PhysicalBody other)
        {
            if (_GameObject != null)
            {
                _GameObject.CallCollisionProcess(other._GameObject);
            }
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            // create cloned component to return
            PhysicalBody ret = (PhysicalBody)CopyBasics(new PhysicalBody(_shape, Mass, Inertia, _body.Friction));

            // copy current state
            ret._body.CopyConditionFrom(_body);
            ret.InvokeCollisionEvents = InvokeCollisionEvents;
            ret.IsEthereal = IsEthereal;
            ret.Gravity = Gravity;
            ret.CollisionGroup = CollisionGroup;
            ret.CollisionMask = CollisionMask;
            ret.ConstForce = ConstForce;
            ret.ConstVelocity = ConstVelocity;
            ret.ConstTorqueForce = ConstTorqueForce;
            ret.ConstAngularVelocity = ConstAngularVelocity;
            ret.Restitution = Restitution;

            // return the cloned body
            return ret;
        }

        /// <summary>
        /// Called every frame in the Update() loop.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected override void OnUpdate()
        {
            // set const velocity
            if (ConstVelocity != null)
            {
                _body.LinearVelocity = ConstVelocity.Value;
            }
            // set const angular velocity
            if (ConstAngularVelocity != null)
            {
                _body.AngularVelocity = ConstAngularVelocity.Value;
            }
            // set const force
            if (ConstForce != null)
            {
                _body.ApplyForce(ConstForce.Value);
            }
            // set const torque force
            if (ConstTorqueForce != null)
            {
                _body.ApplyTorque(ConstTorqueForce.Value);
            }

            // normally, we want to update node before drawing entity.
            // but if our node is currently not visible or culled out, we still want to update it.
            // for that purpose we do the test below - if node was not drawn, update from within update() call.
            if (!_GameObject.SceneNode.WasDrawnThisFrame && NeedToUpdataNode)
            {
                UpdateNodeTransforms();
            } 
            _alreadyUpdatedBodyInFrame = false;
        }

        /// <summary>
        /// Called every frame before the scene renders.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected override void OnBeforeDraw()
        { 
            // update scene node
            if (!_alreadyUpdatedBodyInFrame && NeedToUpdataNode)
            {
                UpdateNodeTransforms();
            }
        }

        /// <summary>
        /// Return if the physical body was updated and currently need to update the scene node.
        /// </summary>
        bool NeedToUpdataNode
        {
            get { return _body.IsActive && _body.IsDirty; }
        }

        /// <summary>
        /// Update scene node transformations.
        /// </summary>
        internal void UpdateNodeTransforms()
        {
            Matrix newTrans = _body.WorldTransform;
            _GameObject.SceneNode.SetWorldTransforms(ref newTrans);
            _alreadyUpdatedBodyInFrame = true;
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
        /// Set damping.
        /// </summary>
        /// <param name="linear">Linear damping.</param>
        /// <param name="angular">Angular damping.</param>
        public void SetDamping(float linear, float angular)
        {
            _body.SetDamping(linear, angular);
        }

        /// <summary>
        /// Get / set linear damping.
        /// </summary>
        public float LinearDamping
        {
            get { return _body.LinearDamping; }
            set { _body.LinearDamping = value; }
        }

        /// <summary>
        /// Get / set angular damping.
        /// </summary>
        public float AngularDamping
        {
            get { return _body.AngularDamping; }
            set { _body.AngularDamping = value; }
        }

        /// <summary>
        /// Clear all forces from the body.
        /// </summary>
        /// <param name="clearVelocity">If true, will also clear velocity.</param>
        public void ClearForces(bool clearVelocity = true)
        {
            _body.ClearForces(clearVelocity);
        }

        /// <summary>
        /// Clear velocity from the body.
        /// </summary>
        public void ClearVelocity()
        {
            _body.ClearVelocity();
        }

        /// <summary>
        /// Apply force on the physical body, from its center.
        /// </summary>
        /// <param name="force">Force vector to apply.</param>
        public void ApplyForce(Vector3 force)
        {
            _body.ApplyForce(force);
        }

        /// <summary>
        /// Apply force on the physical body, from given position.
        /// </summary>
        /// <param name="force">Force vector to apply.</param>
        /// <param name="from">Force source position.</param>
        public void ApplyForce(Vector3 force, Vector3 from)
        {
            _body.ApplyForce(force, from);
        }

        /// <summary>
        /// Apply force on the physical body, from its center.
        /// </summary>
        /// <param name="impulse">Impulse vector to apply.</param>
        public void ApplyImpulse(Vector3 impulse)
        {
            _body.ApplyImpulse(impulse);
        }

        /// <summary>
        /// Apply impulse on the physical body, from given position.
        /// </summary>
        /// <param name="impulse">Impulse vector to apply.</param>
        /// <param name="from">Impulse source position.</param>
        public void ApplyImpulse(Vector3 impulse, Vector3 from)
        {
            _body.ApplyImpulse(impulse, from);
        }

        /// <summary>
        /// Apply torque force on the body.
        /// </summary>
        /// <param name="torque">Torque force to apply.</param>
        /// <param name="asImpulse">If true, will apply torque as an impulse.</param>
        public void ApplyTorque(Vector3 torque, bool asImpulse = false)
        {
            _body.ApplyTorque(torque, asImpulse);
        }

        /// <summary>
        /// Get / set current body linear velocity.
        /// </summary>
        public Vector3 LinearVelocity
        {
            get { return _body.LinearVelocity; }
            set { _body.LinearVelocity = value; }
        }

        /// <summary>
        /// Get / set linear factor.
        /// </summary>
        public Vector3 LinearFactor
        {
            get { return _body.LinearFactor; }
            set { _body.LinearFactor = value; }
        }

        /// <summary>
        /// Get / set current body angular velocity.
        /// </summary>
        public Vector3 AngularVelocity
        {
            get { return _body.AngularVelocity; }
            set { _body.AngularVelocity = value; }
        }

        /// <summary>
        /// Get / set angular factor.
        /// </summary>
        public Vector3 AngularFactor
        {
            get { return _body.AngularFactor; }
            set { _body.AngularFactor = value; }
        }

        /// <summary>
        /// Set / get body gravity (if undefined, will use world default).
        /// </summary>
        public Vector3? Gravity
        {
            get { return _body.Gravity; }
            set { _body.Gravity = value; }
        }

        /// <summary>
        /// Get the bounding box of the physical body.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get { return _body.BoundingBox; }
        }

        /// <summary>
        /// Force the scene node to calculate world transformations and copy them to the physical body matrix.
        /// This will make the scene node world transform override the current physical body state.
        /// </summary>
        /// <param name="clearForces">If true, will also clear all forces and velocity currently applied on body.</param>
        public void CopyNodeWorldMatrix(bool clearForces = true)
        {
            // clear forces (if needed)
            if (clearForces) { _body.ClearForces(true); }

            // note: we can't just use SceneNode.WorldTransformations because its calculated differently because there's a physical body attached (ourselves..)
            WorldTransform = _GameObject.SceneNode.BuildTransformationsMatrix() * 
                (_GameObject.SceneNode.Parent != null ? _GameObject.SceneNode.Parent.WorldTransformations : Matrix.Identity);
        }

        /// <summary>
        /// Called when parent GameObject changes (after the change).
        /// </summary>
        /// <param name="prevParent">Previous parent.</param>
        /// <param name="newParent">New parent.</param>
        protected override void OnParentChange(GameObject prevParent, GameObject newParent)
        {
            // make previous parent scene node no longer use external transformations
            if (prevParent != null)
            {
                prevParent.SceneNode.UseExternalTransformations = false;
            }

            // reset the already-updated flag
            _alreadyUpdatedBodyInFrame = false;

            // if we got a new parent:
            if (newParent != null)
            {
                // make sure it doesn't already have a physical body
                if (newParent.PhysicalBody != null && newParent.PhysicalBody != this)
                {
                    throw new Exceptions.InvalidActionException("Cannot add multiple physical bodies to a single Game Object!");
                }

                // set its node to relay on external transformations.
                newParent.SceneNode.UseExternalTransformations = true;
            }
        }

        /// <summary>
        /// Called when this component is effectively removed from scene, eg when removed
        /// from a GameObject or when its GameObject is removed from scene.
        /// </summary>
        protected override void OnRemoveFromScene()
        {
            // remove from physics world
            if (_isInWorld)
            {
                _GameObject.ParentScene.Physics.RemoveBody(_body);
                _isInWorld = false;
            }
        }

        /// <summary>
        /// Called when this component is effectively added to scene, eg when added
        /// to a GameObject currently in scene or when its GameObject is added to scene.
        /// </summary>
        protected override void OnAddToScene()
        {
            // add to physics world
            if (!_isInWorld)
            {
                _GameObject.ParentScene.Physics.AddBody(_body);
                _isInWorld = true;
            }
        }
    }
}

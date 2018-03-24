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
// Basic physical component.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Physics
{
    /// <summary>
    /// Basic physical component.
    /// All physical components, such as Rigid body, Kinematic body etc, should inherit from this class.
    /// </summary>
    public abstract class BasePhysicsComponent : BaseComponent
    {
        /// <summary>
        /// The physical body in the core layer.
        /// </summary>
        internal virtual Core.Physics.BasicPhysicalBody _PhysicalBody { get; }

        /// <summary>
        /// Return if this is a static object.
        /// </summary>
        public bool IsStatic { get { return _PhysicalBody.IsStatic; } }

        /// <summary>
        /// Return if this is a kinematic object.
        /// </summary>
        public bool IsKinematic { get { return _PhysicalBody.IsKinematic; } }

        /// <summary>
        /// Return true if you want this physical body to take over node transformations.
        /// </summary>
        protected virtual bool TakeOverNodeTransformations { get { return false; } }

        /// <summary>
        /// Optional user data you can attach to this physical component.
        /// </summary>
        public object UserData = null;

        /// <summary>
        /// Set / get body scale.
        /// </summary>
        public Vector3 Scale
        {
            get { return _PhysicalBody.Scale; }
            set { _PhysicalBody.Scale = value; }
        }

        /// <summary>
        /// Get / set simulation state.
        /// If true, will simulate forces etc on this body (default).
        /// If false, will not simulate forces and basically behave like a kinematic body.
        /// </summary>
        public bool EnableSimulation
        {
            get { return _PhysicalBody.EnableSimulation; }
            set { _PhysicalBody.EnableSimulation = value; }
        }
        
        /// <summary>
        /// Get / set physical body world transformation.
        /// </summary>
        public Matrix WorldTransform
        {
            get { return _PhysicalBody.WorldTransform; }
            set { _PhysicalBody.WorldTransform = value; }
        }

        /// <summary>
        /// Get / set current body position.
        /// </summary>
        public Vector3 Position
        {
            get { return _PhysicalBody.Position; }
            set { _PhysicalBody.Position = value; }
        }

        /// <summary>
        /// The collision group this body belongs to.
        /// Note: compare bits mask.
        /// </summary>
        public short CollisionGroup
        {
            get { return _PhysicalBody.CollisionGroup; }
            set { _PhysicalBody.CollisionGroup = value; }
        }

        /// <summary>
        /// With which collision groups this body will collide?
        /// Note: compare bits mask.
        /// </summary>
        public short CollisionMask
        {
            get { return _PhysicalBody.CollisionMask; }
            set { _PhysicalBody.CollisionMask = value; }
        }

        /// <summary>
        /// If true (default) will invoke collision events.
        /// You can turn this off for optimizations.
        /// </summary>
        public bool InvokeCollisionEvents
        {
            get { return _PhysicalBody.InvokeCollisionEvents; }
            set { _PhysicalBody.InvokeCollisionEvents = value; }
        }

        /// <summary>
        /// If ethereal, other bodies will be able to pass through this object, but it will still trigger contact events.
        /// </summary>
        public bool IsEthereal
        {
            get { return _PhysicalBody.IsEthereal; }
            set { _PhysicalBody.IsEthereal = value; }
        }

        /// <summary>
        /// Get / set body restitution.
        /// </summary>
        public float Restitution
        {
            get { return _PhysicalBody.Restitution; }
            set { _PhysicalBody.Restitution = value; }
        }

        // are we currently in physics world?
        bool _isInWorld = false;
        
        /// <summary>
        /// Called when this physical body start colliding with another body.
        /// </summary>
        /// <param name="other">The other body we collide with.</param>
        /// <param name="data">Extra collision data.</param>
        public void CallCollisionStart(BasePhysicsComponent other, Core.Physics.CollisionData data)
        {
            if (_GameObject != null && other._GameObject != null)
            {
                _GameObject.CallCollisionStart(other._GameObject, data);
            }
        }

        /// <summary>
        /// Update scene node transformations.
        /// </summary>
        internal virtual void UpdateNodeTransforms()
        {
        }

        /// <summary>
        /// Called when this physical body stop colliding with another body.
        /// </summary>
        /// <param name="other">The other body we collided with, but no longer.</param>
        public void CallCollisionEnd(BasePhysicsComponent other)
        {
            if (_GameObject != null && other._GameObject != null)
            {
                _GameObject.CallCollisionEnd(other._GameObject);
            }
        }

        /// <summary>
        /// Called while this physical body is colliding with another body.
        /// </summary>
        /// <param name="other">The other body we are colliding with.</param>
        public void CallCollisionProcess(BasePhysicsComponent other)
        {
            if (_GameObject != null && other._GameObject != null)
            {
                _GameObject.CallCollisionProcess(other._GameObject);
            }
        }  
        
        /// <summary>
        /// Get / set body friction.
        /// </summary>
        public float Friction
        {
            get { return _PhysicalBody.Friction; }
            set { _PhysicalBody.Friction = Friction; }
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
                prevParent.SceneNode.UseExternalTransformations = TakeOverNodeTransformations;
            }

            // if we got a new parent:
            if (newParent != null)
            {
                // make sure it doesn't already have a physical body
                if (newParent.PhysicalBody != null && newParent.PhysicalBody != this)
                {
                    throw new Exceptions.InvalidActionException("Cannot add multiple physical bodies to a single Game Object!");
                }

                // set its node to relay on external transformations.
                newParent.SceneNode.UseExternalTransformations = TakeOverNodeTransformations;
                if (TakeOverNodeTransformations) { UpdateNodeTransforms(); }
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
                _GameObject.ParentScene.Physics.RemoveBody(_PhysicalBody);
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
                _GameObject.ParentScene.Physics.AddBody(_PhysicalBody);
                _isInWorld = true;
            }
        }

        /// <summary>
        /// Copy basic properties to another component (helper function to help with Cloning).
        /// </summary>
        /// <param name="copyTo">Other component to copy values to.</param>
        /// <returns>The object we are copying properties to.</returns>
        protected override BaseComponent CopyBasics(BaseComponent copyTo)
        {
            BasePhysicsComponent ret = copyTo as BasePhysicsComponent;
            ret.InvokeCollisionEvents = InvokeCollisionEvents;
            ret.IsEthereal = IsEthereal;
            ret.CollisionGroup = CollisionGroup;
            ret.CollisionMask = CollisionMask;
            ret.EnableSimulation = EnableSimulation;
            ret.Restitution = Restitution;
            ret.WorldTransform = WorldTransform;
            ret.Scale = Scale;
            ret.UserData = UserData;
            return base.CopyBasics(ret);
        }
    }
}

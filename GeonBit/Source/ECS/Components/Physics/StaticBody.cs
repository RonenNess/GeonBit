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
// Static body component.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Physics
{
    /// <summary>
    /// A Static Body component.
    /// This body will not respond to forces, and will always copy the transformations of the parent Game Object.
    /// It is optimized for static, constant things like terrain, trees, rocks, walls, etc.
    /// </summary>
    public class StaticBody : BasePhysicsComponent
    {
        // the core static body
        Core.Physics.StaticBody _body;

        /// <summary>
        /// The physical body in the core layer.
        /// </summary>
        internal override Core.Physics.BasicPhysicalBody _PhysicalBody { get { return _body; } }

        /// <summary>
        /// The shape used for this physical body.
        /// </summary>
        private Core.Physics.CollisionShapes.ICollisionShape _shape = null;
        
        // are we currently in physics world?
        bool _isInWorld = false;

        /// <summary>
        /// Create the static collision body from shape info.
        /// </summary>
        /// <param name="shapeInfo">Body shape info.</param>
        public StaticBody(IBodyShapeInfo shapeInfo)
        {
            CreateBody(shapeInfo.CreateShape());
        }

        /// <summary>
        /// Create the static collision body from shape instance.
        /// </summary>
        /// <param name="shape">Shape to use.</param>
        public StaticBody(Core.Physics.CollisionShapes.ICollisionShape shape)
        {
            CreateBody(shape);
        }

        /// <summary>
        /// Create the actual collision body.
        /// </summary>
        /// <param name="shape">Collision shape.</param>
        private void CreateBody(Core.Physics.CollisionShapes.ICollisionShape shape)
        {
            _shape = shape;
            _body = new Core.Physics.StaticBody(shape);
            _body.EcsComponent = this;
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            // create cloned component to return
            KinematicBody ret = (KinematicBody)CopyBasics(new KinematicBody(_shape.Clone()));

            // return the cloned object
            return ret;
        }

        /// <summary>
        /// Called every time scene node transformation updates.
        /// Note: this is called only if GameObject is enabled and have Update events enabled.
        /// </summary>
        protected override void OnTransformationUpdate()
        {
            _body.WorldTransform = _GameObject.SceneNode.WorldTransformations;
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
            
            // transform to match game object transformations
            OnTransformationUpdate();
        }
    }
}

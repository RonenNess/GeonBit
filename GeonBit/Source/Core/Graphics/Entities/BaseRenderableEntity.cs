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
// Basic functionality for all renderable entities.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GeonBit.Core.Graphics
{
    /// <summary>
    /// A callback function you can register on different entity-related events.
    /// </summary>
    /// <param name="node">The entity instance the event was called from.</param>
    public delegate void EntityEventCallback(BaseRenderableEntity node);

    /// <summary>
    /// A basic renderable entity.
    /// </summary>
    public class BaseRenderableEntity : IEntity
    {
        // is this entity currently visible.
        private bool _visible = true;

        /// <summary>
        /// Add bias to distance from camera when sorting by distance from camera.
        /// This is useful for particles, sprites etc.
        /// </summary>
        virtual public float CameraDistanceBias { get { return 0f; } }

        /// <summary>
        /// Callback that triggers every time an entity is rendered.
        /// Note: entities that are culled out will not trigger this event.
        /// </summary>
        public static EntityEventCallback OnDraw;

        /// <summary>
        /// Which rendering queue to use for this entity.
        /// Rendering queues determine different rendering settings (like opacity support, blending, sorting etc) + the order
        /// in which the entities are drawn.
        /// </summary>
        public RenderingQueue RenderingQueue = RenderingQueue.Solid;

        /// <summary>
        /// Blending state for this entity.
        /// </summary>
        public BlendState BlendingState = BlendState.AlphaBlend;

        /// <summary>
        /// If true, will draw just the wireframe of the entity.
        /// Note: settings this property will change the rendering queue property.
        /// </summary>
        public bool WireFrame
        {
            get
            {
                return RenderingQueue == RenderingQueue.Wireframe;
            }
            set
            {
                RenderingQueue = value ? RenderingQueue.Wireframe : RenderingQueue.Solid;
            }
        }

        // last transformation version of parent node that we calculated bounding box for.
        uint _lastWorldTransformForBoundingBox;

        /// <summary>
        /// Last bounding box we calculated for this entity.
        /// </summary>
        protected BoundingBox _lastBoundingBox;

        // last transformation version of parent node that we calculated bounding sphere for.
        uint _lastWorldTransformForBoundingSphere;

        /// <summary>
        /// Last bounding sphere we calculated for this entity.
        /// </summary>
        protected BoundingSphere _lastBoundingSphere;

        /// <summary>
        /// Get / Set if this entity is visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        // an empty value for bounding box calculation. this is just an optimization.
        static readonly BoundingBox EmptyBoundingBox = new BoundingBox();

        // an empty value for bounding sphere calculations. this is just an optimization.
        static readonly BoundingSphere EmptyBoundingSphere = new BoundingSphere();

        /// <summary>
        /// If true, this entity will only show in debug / editor mode.
        /// </summary>
        public virtual bool IsDebugEntity
        {
            get { return false; }
        }

        /// <summary>
        /// Create the renderable entity.
        /// </summary>
        public BaseRenderableEntity()
        {
            // count the object creation
            Utils.CountAndAlert.Count(Utils.CountAndAlert.PredefAlertTypes.AddedOrCreated);
        }

        /// <summary>
        /// Draw the entity.
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public virtual void Draw(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            // not visible / no active camera? skip
            if (!Visible || GraphicsManager.ActiveCamera == null)
            {
                return;
            }

            // call draw callback
            OnDraw?.Invoke(this);

            // make sure we got up-to-date bounding sphere, which is important for lightings and other optimizations
            GetBoundingSphere(parent, ref localTransformations, ref worldTransformations);

            // call to draw this entity - this will either add to the corresponding rendering queue, or draw immediately if have no drawing queue.
            GraphicsManager.DrawEntity(this, worldTransformations);
        }

        /// <summary>
        /// The per-entity drawing function, must be implemented by child entities.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public virtual void DoEntityDraw(ref Matrix worldTransformations)
        {
            // set blend state
            GraphicsManager.GraphicsDevice.BlendState = BlendingState;
        }

        /// <summary>
        /// Get the bounding box of this entity, either from cache or calculate it.
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding box of the entity.</returns>
        public BoundingBox GetBoundingBox(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            // if transformations changed since last time we calculated bounding box, recalc it
            if (_lastWorldTransformForBoundingBox != parent.TransformVersion)
            {
                Utils.CountAndAlert.Count(Utils.CountAndAlert.PredefAlertTypes.HeavyUpdate);
                _lastWorldTransformForBoundingBox = parent.TransformVersion;
                _lastBoundingBox = CalcBoundingBox(parent, ref localTransformations, ref worldTransformations);
            }

            // return bounding box
            return _lastBoundingBox;
        }

        /// <summary>
        /// Return the last calculated bounding box.
        /// Note: this value may be out-of-date if transformations changed since last calculation.
        /// </summary>
        /// <returns>Last known bounding box.</returns>
        public BoundingBox GetLastBoundingBox()
        {
            return _lastBoundingBox;
        }

        /// <summary>
        /// Calculate and return the bounding box of this entity (in world space).
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding box of the entity.</returns>
        protected virtual BoundingBox CalcBoundingBox(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            return EmptyBoundingBox;
        }

        /// <summary>
        /// Get the bounding sphere of this entity, either from cache or calculate it.
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding sphere of the entity.</returns>
        public BoundingSphere GetBoundingSphere(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            // if transformations changed since last time we calculated bounding sphere, recalc it
            if (_lastWorldTransformForBoundingSphere != parent.TransformVersion)
            {
                Utils.CountAndAlert.Count(Utils.CountAndAlert.PredefAlertTypes.HeavyUpdate);
                _lastWorldTransformForBoundingSphere = parent.TransformVersion;
                _lastBoundingSphere = CalcBoundingSphere(parent, ref localTransformations, ref worldTransformations);
            }

            // return bounding sphere
            return _lastBoundingSphere;
        }

        /// <summary>
        /// Return the last calculated bounding sphere.
        /// Note: this value may be out-of-date if transformations changed since last calculation.
        /// </summary>
        /// <returns>Last known bounding sphere.</returns>
        public BoundingSphere GetLastBoundingSphere()
        {
            return _lastBoundingSphere;
        }

        /// <summary>
        /// Calculate and return the bounding sphere of this entity (in world space).
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding sphere of the entity.</returns>
        protected virtual BoundingSphere CalcBoundingSphere(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            return EmptyBoundingSphere;
        }
    }
}

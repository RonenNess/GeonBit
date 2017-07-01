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
// Implement basic functionality for components that render stuff.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.ECS.Components.Graphics
{
    /// <summary>
    /// Base implementation for most graphics-related components.
    /// </summary>
    public abstract class BaseRendererComponent : BaseComponent
    {
        /// <summary>
        /// Get the main entity instance of this renderer.
        /// </summary>
        protected abstract Core.Graphics.BaseRenderableEntity Entity { get; }

        /// <summary>
        /// Set / get Entity blending state.
        /// </summary>
        public BlendState BlendingState
        {
            set { Entity.BlendingState = value; }
            get { return Entity.BlendingState; }
        }

        /// <summary>
        /// Set / get the rendering queue of this entity.
        /// </summary>
        virtual public Core.Graphics.RenderingQueue RenderingQueue
        {
            get { return Entity.RenderingQueue; }
            set { Entity.RenderingQueue = value; }
        }

        /// <summary>
        /// Copy basic properties to another component (helper function to help with Cloning).
        /// </summary>
        /// <param name="copyTo">Other component to copy values to.</param>
        /// <returns>The object we are copying properties to.</returns>
        protected override BaseComponent CopyBasics(BaseComponent copyTo)
        {
            base.CopyBasics(copyTo);
            BaseRendererComponent otherRenderer = copyTo as BaseRendererComponent;
            otherRenderer.RenderingQueue = RenderingQueue;
            otherRenderer.BlendingState = BlendingState;
            return copyTo;
        }

        /// <summary>
        /// Called when GameObject turned disabled.
        /// </summary>
        protected override void OnDisabled()
        {
            Entity.Visible = false;
        }

        /// <summary>
        /// Called when GameObject is enabled.
        /// </summary>
        protected override void OnEnabled()
        {
            Entity.Visible = true;
        }

        /// <summary>
        /// Change component parent GameObject.
        /// </summary>
        /// <param name="prevParent">Previous parent.</param>
        /// <param name="newParent">New parent.</param>
        override protected void OnParentChange(GameObject prevParent, GameObject newParent)
        {
            // remove from previous parent
            if (prevParent != null && prevParent.SceneNode != null)
            {
                prevParent.SceneNode.RemoveEntity(Entity);
            }

            // add model entity to new parent
            if (newParent != null && newParent.SceneNode != null)
            {
                newParent.SceneNode.AddEntity(Entity);
            }
        }
    }
}

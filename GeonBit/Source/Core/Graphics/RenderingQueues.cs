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
// Define the rendering queues.
// Rendering queues are lists of items to draw with specific device settings and
// order. Its important in order to handle effects, opacity, etc.
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
    /// Pre-defined rendering queue.
    /// Every rendering queue have drawing settings and their order determine the order in which object batches will be drawn.
    /// </summary>
    public enum RenderingQueue
    {
        /// <summary>
        /// Will not use rendering queue, but simply draw this entity the moment the draw function is called.
        /// This does not guarantee any specific order and will use default device settings.
        /// </summary>
        NoQueue = -1,

        /// <summary>
        /// Draw solid entities without depth buffer and without any culling. 
        /// Everything drawn in the other queues will cover entities in this queue.
        /// </summary>
        SolidBackNoCull,

        /// <summary>
        /// Draw solid entities with depth buffer. 
        /// This is the default queue for simple 3D meshes without alpha channels.
        /// </summary>
        Solid,

        /// <summary>
        /// Drawing settings for solid terrain meshes.
        /// </summary>
        Terrain,

        /// <summary>
        /// Drawing settings for billboards.
        /// </summary>
        Billboards,

        /// <summary>
        /// Draw after all the solid queues, without affecting the depth buffer.
        /// This means it draw things in the background that will not hide any other objects.
        /// </summary>
        Background,

        /// <summary>
        /// Draw after all the solid queues, without affecting the depth buffer and without culling.
        /// This means it draw things in the background that will not hide any other objects.
        /// </summary>
        BackgroundNoCull,

        /// <summary>
        /// For entities with opacity, but does not order by distance from camera.
        /// This means its a good queue for entities with alpha channels on top of solid items, but its not suitable if entities with alpha may cover each other.
        /// </summary>
        OpacityUnordered,

        /// <summary>
        /// For entities with opacity, order renderings by distance from camera.
        /// This is the best queue to use for dynamic entities with alpha channels that might cover each other.
        /// </summary>
        Opacity,

        /// <summary>
        /// For entities that are mostly solid and opaque, but have some transparent elements in them.
        /// </summary>
        Mixed,

        /// <summary>
        /// Special queue that draws everything as wireframe.
        /// </summary>
        Wireframe,

        /// <summary>
        /// For special effects and particles, but will still use depth buffer, and will not sort by distance from camera.
        /// </summary>
        EffectsUnordered,

        /// <summary>
        /// For special effects and particles, but will still use depth buffer, and will sort by distance from camera.
        /// </summary>
        Effects,

        /// <summary>
        /// For special effects and particles, does not use depth buffer (eg will always be rendered on top).
        /// </summary>
        EffectsOverlay,

        /// <summary>
        /// Renders last, on top of everything, without using depth buffer.
        /// </summary>
        Overlay,

        /// <summary>
        /// Render queue for debug purposes.
        /// Note: this queue only draws when in debug mode!
        /// </summary>
        Debug,
    }

    /// <summary>
    /// A single entity in a rendering queue.
    /// </summary>
    class EntityInQueue
    {
        /// <summary>
        /// The renderable entity.
        /// </summary>
        public BaseRenderableEntity Entity;

        /// <summary>
        /// World transformations to draw with.
        /// </summary>
        public Matrix World;

        /// <summary>
        /// Create the entity-in-queue entry.
        /// </summary>
        /// <param name="entity">Entity to draw.</param>
        /// <param name="world">World transformations.</param>
        public EntityInQueue(BaseRenderableEntity entity, Matrix world)
        {
            Entity = entity;
            World = world;
        }
    }

    /// <summary>
    /// Rendering queue settings and entities.
    /// </summary>
    class RenderingQueueInstance
    {
        /// <summary>
        /// Current entities in queue.
        /// </summary>
        public List<EntityInQueue> Entities = new List<EntityInQueue>();

        /// <summary>
        /// Rasterizer settings of this queue.
        /// </summary>
        public RasterizerState RasterizerState = new RasterizerState();

        /// <summary>
        /// Depth stencil settings for this queue.
        /// </summary>
        public DepthStencilState DepthStencilState = new DepthStencilState();

        /// <summary>
        /// If true, will sort entities by distance from camera.
        /// </summary>
        public bool SortByCamera = false;
    }

    /// <summary>
    /// Manage and draw the rendering queues.
    /// </summary>
    internal static class RenderingQueues
    {
        // List of built-in rendering queues.
        static List<RenderingQueueInstance> _renderingQueues = new List<RenderingQueueInstance>();

        /// <summary>
        /// Init all built-in rendering queues.
        /// </summary>
        public static void Initialize()
        {
            // SolidBackNoCull
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.None;
                queue.RasterizerState.DepthClipEnable = false;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                _renderingQueues.Add(queue);
            }

            // Solid
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = true;
                _renderingQueues.Add(queue);
            }

            // Terrain
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = true;
                _renderingQueues.Add(queue);
            }

            // Billboards
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.None;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = true;
                _renderingQueues.Add(queue);
            }

            // Background
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                _renderingQueues.Add(queue);
            }

            // BackgroundNoCull
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.None;
                queue.RasterizerState.DepthClipEnable = false;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                _renderingQueues.Add(queue);
            }

            // OpacityUnordered
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.None;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                _renderingQueues.Add(queue);
            }

            // Opacity
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.None;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                queue.SortByCamera = true;
                _renderingQueues.Add(queue);
            }

            // Mixed
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = true;
                queue.SortByCamera = true;
                _renderingQueues.Add(queue);
            }

            // Wireframe
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.None;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.WireFrame;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                _renderingQueues.Add(queue);
            }

            // EffectsUnordered
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.None;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                queue.SortByCamera = false;
                _renderingQueues.Add(queue);
            }

            // Effects
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.None;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = true;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                queue.SortByCamera = true;
                _renderingQueues.Add(queue);
            }

            // EffectsOverlay
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.None;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = false;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                queue.SortByCamera = false;
                _renderingQueues.Add(queue);
            }

            // Overlay
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = false;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                queue.SortByCamera = false;
                _renderingQueues.Add(queue);
            }

            // debug stuff
            {
                RenderingQueueInstance queue = new RenderingQueueInstance();
                queue.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
                queue.RasterizerState.DepthClipEnable = true;
                queue.RasterizerState.FillMode = FillMode.Solid;
                queue.DepthStencilState.DepthBufferEnable = false;
                queue.DepthStencilState.DepthBufferWriteEnable = false;
                queue.SortByCamera = false;
                _renderingQueues.Add(queue);
            }
        }

        // default rasterizer state to reset to after every frame.
        static RasterizerState _defaultRasterizerState = new RasterizerState();

        /// <summary>
        /// Draw rendering queues.
        /// </summary>
        public static void DrawQueues()
        {
            // iterate drawing queues
            foreach (var queue in _renderingQueues)
            {
                // if no entities in queue, skip
                if (queue.Entities.Count == 0)
                {
                    continue;
                }

                // apply queue states
                GraphicsManager.GraphicsDevice.RasterizerState = queue.RasterizerState;
                GraphicsManager.GraphicsDevice.DepthStencilState = queue.DepthStencilState;

                // if need to sort by distance from camera, do the sorting
                if (queue.SortByCamera)
                {
                    Vector3 camPos = GraphicsManager.ActiveCamera.Position;
                    queue.Entities.Sort(delegate (EntityInQueue x, EntityInQueue y)
                    {
                        return  (int)(Vector3.Distance(camPos, y.World.Translation) * 100f - System.Math.Floor(y.Entity.CameraDistanceBias)) - 
                                (int)(Vector3.Distance(camPos, x.World.Translation) * 100f - System.Math.Floor(x.Entity.CameraDistanceBias));
                    });
                }

                // draw all entities in queue
                foreach (var entityData in queue.Entities)
                {
                    entityData.Entity.DoEntityDraw(ref entityData.World);
                }

                // clear queue
                queue.Entities.Clear();
            }

            // reset device states
            GraphicsManager.GraphicsDevice.RasterizerState = _defaultRasterizerState;
            GraphicsManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        /// <summary>
        /// Add entity to its rendering queue.
        /// </summary>
        /// <param name="entity">Entity to push to queue.</param>
        /// <param name="world">World transformations.</param>
        public static void AddEntity(BaseRenderableEntity entity, Matrix world)
        {
            // special case - skip debug if not in debug mode
            if (entity.RenderingQueue == RenderingQueue.Debug && !GeonBitMain.Instance.DebugMode)
            {
                return;
            }

            // add to the rendering queue
            _renderingQueues[(int)entity.RenderingQueue].Entities.Add(new EntityInQueue(entity, world));
        }
    }
}
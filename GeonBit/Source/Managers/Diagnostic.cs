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
// Provide diagnostic and performance test utilities.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using GeonBit.Core.Utils;

namespace GeonBit.Managers
{
    /// <summary>
    /// Provide diagnostic-related utilities.
    /// </summary>
    public class Diagnostic : IManager
    {
        // singleton instance
        static Diagnostic _instance = null;

        /// <summary>
        /// Enable / Disable diagnostics.
        /// </summary>
        public bool Enabled
        {
            set
            {
                _enabled = value;
                UpdateEnabledState();
            }
            get
            {
                return _enabled;
            }
        }
        private bool _enabled = true;

        /// <summary>
        /// Get diagnostics utils instance.
        /// </summary>
        public static Diagnostic Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Diagnostic();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Init diagnostics manager.
        /// </summary>
        public void Initialize()
        {
            // update enabled / disabled state
            UpdateEnabledState();
        }

        /// <summary>
        /// Enable / disable diagnostics recording.
        /// </summary>
        private void UpdateEnabledState()
        {
            // enabled diagnostics
            if (_enabled)
            {
                // register callback to count transformations update
                Core.Graphics.Node.__OnNodeTransformationsUpdate = (Core.Graphics.Node node) =>
                {
                    _transformUpdated++;
                };

                // register callback to count node draw calls
                Core.Graphics.Node.__OnNodeDraw = (Core.Graphics.Node node) =>
                {
                    _renderedNodes++;
                    if (node.HaveEntities)
                    {
                        _renderedNodesWithEntities++;
                    }
                };

                // register callback to count entities draw calls
                Core.Graphics.BaseRenderableEntity.OnDraw = (Core.Graphics.BaseRenderableEntity entity) =>
                {
                    // count rendered entities, unless its the debug bounding box entity which is used internally
                    if (!(entity is Core.Graphics.BoundingBoxEntity))
                    {
                        _renderedEntities++;
                    }
                };
            }
            // disable diagnostics
            else
            {
                // clear callbacks
                Core.Graphics.Node.__OnNodeDraw = null;
                Core.Graphics.Node.__OnNodeTransformationsUpdate = null;
                Core.Graphics.BaseRenderableEntity.OnDraw = null;

            }
        }

        /// <summary>
        /// Get main diagnostics as string.
        /// </summary>
        /// <returns>Diagnostic string.</returns>
        public string GetReportString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append("Debug mode: ").Append(GeonBitMain.Instance.DebugMode.ToString()).Append("\n");
            builder.Append("FPS: ").Append(FpsCount.ToString()).Append("\n");
            builder.Append("Entities drawn: ").Append(_renderedEntities.ToString()).Append("\n");
            builder.Append("Nodes drawn: ").Append(_renderedNodes.ToString()).Append("\n");
            builder.Append("Nodes with entities drawn: ").Append(_renderedNodesWithEntities.ToString()).Append("\n");
            builder.Append("Nodes updated: ").Append(_transformUpdated.ToString()).Append("\n");
            builder.Append("Objects Alive: ").Append(ECS.GameObject.Count.ToString()).Append("\n");
            builder.Append("Components Alive: ").Append(ECS.Components.BaseComponent.Count.ToString());
            return builder.ToString();
        }

        /// <summary>
        /// To make it a true singleton.
        /// </summary>
        private Diagnostic() { }

        // to count fps
        private int _currFps = 0;
        private int _actualFps = 0;
        private float _secondCount = 0;

        /// <summary>
        /// Set / get if we want to debug render octree parts.
        /// </summary>
        public bool DebugRenderOctree
        {
            get { return Core.Graphics.OctreeCullingNode.DebugRenderOctreeParts; }
            set { Core.Graphics.OctreeCullingNode.DebugRenderOctreeParts = value; }
        }

        /// <summary>
        /// Set / get if we want to debug render physics.
        /// </summary>
        public bool DebugRenderPhysics
        {
            get { return ECS.GameScene.DebugRenderPhysics; }
            set { ECS.GameScene.DebugRenderPhysics = value; }
        }

        /// <summary>
        /// Get current FPS count.
        /// </summary>
        public int FpsCount { get { return _actualFps; } }

        /// <summary>
        /// Update current time.
        /// </summary>
        public void Update(GameTime time)
        {
            // count fps
            _currFps++;
            _secondCount += TimeManager.Instance.TimeFactor;
            if (_secondCount >= 1.0f)
            {
                _actualFps = _currFps;
                _currFps = 0;
                _secondCount = 0;
            }

            // update alerts
            CountAndAlert.Update();
        }

        /// <summary>
        /// Called every frame during the Draw() process.
        /// </summary>
        public void Draw(GameTime time)
        {
        }

        // count how many entities were rendered this frame.
        private int _renderedEntities = 0;

        // count how many nodes were rendered this frame.
        private int _renderedNodes = 0;

        // count how many nodes did transformation updates this frame.
        private int _transformUpdated = 0;

        // count how many nodes with entities were rendered this frame.
        private int _renderedNodesWithEntities = 0;

        /// <summary>
        /// Get how many entities were rendered this frame.
        /// </summary>
        public int EntitiesDrawCount
        {
            get { return _renderedEntities; }
        }

        /// <summary>
        /// Get how many nodes were rendered this frame.
        /// </summary>
        public int NodesDrawCount
        {
            get { return _renderedNodes; }
        }

        /// <summary>
        /// Get how many nodes with entities were rendered this frame.
        /// </summary>
        public int NodesWithEntitiesDrawCount
        {
            get { return _renderedNodesWithEntities; }
        }

        /// <summary>
        /// Get how many nodes did transformations update this frame.
        /// </summary>
        public int TransformationsUpdateCount
        {
            get { return _transformUpdated; }
        }

        /// <summary>
        /// Call this when starting drawing frame.
        /// </summary>
        public void ResetDrawCounters()
        {
            _renderedEntities = 0;
            _transformUpdated = 0;
            _renderedNodes = 0;
            _renderedNodesWithEntities = 0;
        }

        /// <summary>
        /// Called every constant X seconds during the Update() phase.
        /// </summary>
        /// <param name="interval">Time since last FixedUpdate().</param>
        public void FixedUpdate(float interval)
        {
        }
    }
}

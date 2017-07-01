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
// Provide in-game utilities.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace GeonBit.Managers
{
    /// <summary>
    /// Provide game-related utilities.
    /// </summary>
    public class GraphicsManager : IManager
    {
        // singleton instance
        static GraphicsManager _instance = null;

        /// <summary>
        /// Get instance.
        /// </summary>
        public static GraphicsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GraphicsManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Get current viewport size.
        /// </summary>
        public Point ViewportSize
        {
            get
            {
                return new Point(
                    Core.Graphics.GraphicsManager.GraphicsDevice.Viewport.Width, 
                    Core.Graphics.GraphicsManager.GraphicsDevice.Viewport.Height
                    );
            }
        }

        /// <summary>
        /// Get current resolution size.
        /// </summary>
        public Point ScreenSize
        {
            get
            {
                return new Point(
                    Core.Graphics.GraphicsManager.GraphicsDevice.DisplayMode.Width,
                    Core.Graphics.GraphicsManager.GraphicsDevice.DisplayMode.Height
                    );
            }
        }

        /// <summary>
        /// To make it a true singleton.
        /// </summary>
        private GraphicsManager() { }

        /// <summary>
        /// Update manager.
        /// </summary>
        /// <param name="time">GameTime, as provided by MonoGame.</param>
        public void Update(GameTime time)
        {
        }

        /// <summary>
        /// Create and return a GameObject with skybox attached to it.
        /// </summary>
        /// <param name="texture">Skybox texture (leave null for default texture).</param>
        /// <param name="parent">Optional GameObject to attach add skybox to (as a child). If null, will add to active scene's root.</param>
        /// <returns>GameObject containing skybox.</returns>
        public ECS.GameObject CreateSkybox(string texture = null, ECS.GameObject parent = null)
        {
            ECS.GameObject skybox = new ECS.GameObject("skybox", ECS.SceneNodeType.Simple);
            skybox.AddComponent(new ECS.Components.Graphics.SkyBox(texture));
            skybox.Parent = parent != null ? parent : Application.Instance.ActiveScene.Root;
            return skybox;
        }

        /// <summary>
        /// Create and return a GameObject with background attached to it.
        /// Note: also attach it to either the active scene root, or a given GameObject.
        /// </summary>
        /// <param name="texture">Skybox texture (leave null for default texture).</param>
        /// <param name="parent">Optional GameObject to attach add skybox to (as a child). If null, will add to active scene's root.</param>
        /// <returns>GameObject containing background.</returns>
        public ECS.GameObject CreateBackground(string texture, ECS.GameObject parent = null)
        {
            ECS.GameObject background = new ECS.GameObject("background", ECS.SceneNodeType.Simple);
            background.AddComponent(new ECS.Components.Graphics.SceneBackground(texture));
            background.Parent = parent != null ? parent : Application.Instance.ActiveScene.Root;
            return background;
        }

        /// <summary>
        /// Set the default material generator for a material type.
        /// This is used to determine how to generate materials for loaded models (will apply only on models not loaded yet).
        /// For example, if you set the Default material generator function, it means that whenever GeonBit loads a model with BasicEffect,
        /// it will call this function to generate a corresponding material for it.
        /// </summary>
        /// <param name="type">Material type to set.</param>
        /// <param name="generator">Generator function to use on this material.</param>
        public void SetDefaultMaterialGenerator(Core.Graphics.Materials.MaterialTypes type, Core.Graphics.Materials.MaterialGenerator generator)
        {
            Core.Graphics.Materials.DefaultMaterialsFactory.SetDefaultMaterialGenerator(type, generator);
        }

        /// <summary>
        /// Called every frame during the Draw() process.
        /// </summary>
        public void Draw(GameTime time)
        {
        }

        /// <summary>
        /// Init Game Utils manager.
        /// </summary>
        public void Initialize()
        {
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

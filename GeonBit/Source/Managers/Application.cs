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
// Provide general application utils.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Managers
{
    /// <summary>
    /// Provide application-level utilities.
    /// </summary>
    public class Application : IManager
    {
        // singleton instance
        static Application _instance = null;

        /// <summary>
        /// Get application utils instance.
        /// </summary>
        public static Application Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Application();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Get the currently active scene.
        /// </summary>
        public ECS.GameScene ActiveScene { get; private set; }

        /// <summary>
        /// Get if application is currently active / focused.
        /// </summary>
        public bool IsActive
        {
            get { return _game.IsActive; }
        }

        /// <summary>
        /// To make it a true singleton.
        /// </summary>
        private Application() { }

        /// <summary>
        /// Set scene instance as active scene and activate() it when done.
        /// </summary>
        /// <param name="scene">Scene to set as the activa scene.</param>
        public void LoadScene(ECS.GameScene scene)
        {
            // deactivate previous scene
            if (ActiveScene != null)
            {
                ActiveScene.Deactivate();
            }

            // set new active scene
            ActiveScene = scene;

            // load active scene
            if (ActiveScene != null)
            {
                ActiveScene.Activate();
            }
        }

        /// <summary>
        /// Update manager.
        /// </summary>
        /// <param name="time">GameTime, as provided by MonoGame.</param>
        public void Update(GameTime time)
        {
        }

        /// <summary>
        /// Called every frame during the Draw() process.
        /// </summary>
        public void Draw(GameTime time)
        {
        }

        /// <summary>
        /// Set the main 'Game' class instance.
        /// </summary>
        /// <param name="game">MonoGame 'Game' instance.</param>
        internal void SetGameInstance(Game game)
        {
            _game = game;
        }

        /// <summary>
        /// Init application manager.
        /// </summary>
        public void Initialize()
        {
            // create empty scene with starting test camera
            ActiveScene = new ECS.GameScene();
            ECS.GameObject camera = new ECS.GameObject("camera", ECS.SceneNodeType.Simple);
            camera.AddComponent(new ECS.Components.Graphics.Camera());
            camera.SceneNode.Position = new Vector3(0, 0, 10);
        }

        // main game instance
        private Game _game;

        /// <summary>
        /// Exit game.
        /// </summary>
        public void Exit()
        {
            ActiveScene.Destroy();
            _game.Exit();
        }

        /// <summary>
        /// Called every constant X seconds during the Update() phase.
        /// </summary>
        /// <param name="interval">Time since last FixedUpdate().</param>
        public void FixedUpdate(float interval)
        {
            ActiveScene.FixedUpdate();
        }
    }
}

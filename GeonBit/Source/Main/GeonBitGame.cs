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
// Base class for your main Game class implementation.
//
// Author: Ronen Ness.
// Since: 2016.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using GeonBit.Managers;

namespace GeonBit
{
    /// <summary>
    /// GeonBit's basic 'Game' class, that replaces MonoGame 'Game'.
    /// To use GeonBit, you need to implement a type of this class and implement your Draw(), Initialize() and Update() functions.
    /// Note: since GeonBit already handles rendering and main loop internally, usually you only need to implement Initialize() and
    /// let the components do all the logic.
    /// 
    /// After creating your GeonBitGame class, use it from your Program file like this:
    /// GeonBitMain.Instance.Run(new EmptyGeonBitMain());
    /// </summary>
    abstract public class GeonBitGame
    {
        /// <summary>
        /// Provide an easy access to all GeonBit managers.
        /// </summary>
        protected readonly EasyManagersGetters Managers = new EasyManagersGetters();

        // pointer to the monogame game wrapper class.
        internal MonoGameGameWrapper _gameWrapper;

        /// <summary>
        /// Get GeonBitMain instance.
        /// </summary>
        protected GeonBitMain GeonBitMain { get { return GeonBitMain.Instance; } }

        /// <summary>
        /// Get currently active scene.
        /// </summary>
        protected ECS.GameScene ActiveScene { get { return GeonBitMain.ActiveScene; } }

        /// <summary>
        /// Get currently active camera.
        /// </summary>
        protected ECS.Components.Graphics.Camera ActiveCamera { get { return GeonBitMain.ActiveScene.ActiveCamera; } }

        /// <summary>How many seconds passed since last frame, in real seconds.</summary>
        protected float _timeFactor = 0.0f;

        /// <summary>
        /// Get GeonBit custom content class.
        /// </summary>
        public Core.ResourcesManager Resources
        {
            get
            {
                return Core.ResourcesManager.Instance;
            }
        }

        /// <summary>
        /// Should we enable vsync to sync FPS with monitor refresh rate?
        /// Note: you must set this property before Initialize is called, eg inside the constructor.
        /// </summary>
        public bool EnableVsync { protected set; get; } = true;

        /// <summary>
        /// Which UI theme to use (or null, if you don't want to use GeonBit built-in UI system).
        /// Note: you must set this property before Initialize is called, eg inside the constructor.
        /// </summary>
        public string UiTheme { protected set; get; } = "editor";
        
        /// <summary>
        /// Should we run in debug mode? This will show and render some extra data, but should not
        /// use for released games.
        /// Note: you must set this property before Initialize is called, eg inside the constructor.
        /// </summary>
        public bool DebugMode { protected set; get; } = true;

        /// <summary>
        /// Create the main game class.
        /// </summary>
        public GeonBitGame()
        {
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Allows the game to run custom frame-based logic.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// This is called when the game should draw itself.
        /// You don't need to call clear, draw, etc. Just add custom drawing logic if needed.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public abstract void Draw(GameTime gameTime);

        /// <summary>
        /// Exit the application.
        /// </summary>
        public void Exit()
        {
            Managers.Application.Exit();
        }

        /// <summary>
        /// Get if geonbit UI currently enabled
        /// </summary>
        public bool UiEnabled
        {
            get { return GeonBitMain.Instance.UiEnabled; }
        }

        /// <summary>
        /// Get current FPS count (updates once per second).
        /// </summary>
        protected int FpsCount
        {
            get { return Diagnostic.Instance.FpsCount; }
        }


        /// <summary>
        /// Return diagnostic data to show for debug text.
        /// </summary>
        virtual protected string GetDiagnosticDataAsString()
        {
            return Diagnostic.Instance.GetReportString();
        }

        /// <summary>
        /// Make game work in fullscreen mode.
        /// </summary>
        /// <param name="framed">If true, will make fullscreen but with window frame.</param>
        public void MakeFullscreen(bool framed = false)
        {
            _gameWrapper.MakeFullscreen(framed);
        }
    }
}

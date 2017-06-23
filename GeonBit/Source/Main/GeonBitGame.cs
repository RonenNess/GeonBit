#region LICENSE
/**
 * For the purpose of making video games only, GeonBit is distributed under the MIT license.
 * to use this source code or GeonBit as a whole for any other purpose, please seek written 
 * permission from the library author.
 * 
 * Copyright (c) 2017 Ronen Ness [ronenness@gmail.com].
 * You may not remove this license notice.
 */
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
using Microsoft.Xna.Framework.Graphics;
using GeonBit.Managers;

namespace GeonBit
{
    /// <summary>
    /// Base 'Game' class with GeonBit integrated, plus few helper functions.
    /// You can inherit from this class instead of the native MonoGame 'Game' class, and have
    /// GeonBit properly initialized and updated without extra work.
    /// 
    /// To use this class simply inherit from it, and implement DoDraw(), DoUpdate() and DoInitialize().
    /// You don't need to call GeonBitMain.Initialize() / Update() / Draw(), it happens automatically.
    /// In addition, you have all the managers available with the 'Managers' directive.
    /// </summary>
    abstract public class GeonBitGame
    {
        /// <summary>
        /// Provide an easy access to all GeonBit managers.
        /// </summary>
        protected readonly Managers.EasyManagersGetters Managers = new Managers.EasyManagersGetters();

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

        // optional paragraph to show diagnostic data
        GeonBit.UI.Entities.Paragraph _showDiagnosticData;

        /// <summary>How many seconds passed since last frame, in real seconds.</summary>
        protected float _timeFactor = 0.0f;

        /// <summary>
        /// Get GeonBit custom content class.
        /// </summary>
        public GeonBit.Core.ResourcesManager Resources
        {
            get
            {
                return GeonBit.Core.ResourcesManager.Instance;
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
        /// Show / hide diagnostic data.
        /// </summary>
        /// <param name="show"></param>
        /// <param name="position"></param>
        public void ShowDiagnosticData(bool show, UI.Entities.Anchor position = UI.Entities.Anchor.BottomLeft)
        {
            if (show && !UiEnabled)
            {
                throw new System.Exception("Cannot show built-in diagnostic with GeonBit.UI disabled. To show diagnostic data on external UI system, use GetDiagnosticDataAsString().");
            }
            _showDiagnosticData = show ? new UI.Entities.Paragraph("", position, Color.White, scale: 0.8f) : null;
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

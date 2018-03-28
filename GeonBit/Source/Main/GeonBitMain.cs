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
// Main class that manage the engine.
// This is the most top object that you access from your Game main class.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GeonBit
{
    /// <summary>
    /// GeonBit root namespace.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// GeonBit engine main manager.
    /// This singleton class contains misc configuration for GeonBit, the main Initialize function,
    /// and Draw() / Update() functions that roll the entire engine.
    /// 
    /// To use GeonBit, you need to create a Game class that do the following:
    /// 1. call GeonBitMain.Instance.Initialize() from its Initialize() function.
    /// 2. call GeonBitMain.Instance.Update() from its Update() function.
    /// 3. call GeonBitMain.Instance.Draw() from its Draw() function.
    /// 
    /// 
    /// </summary>
    public class GeonBitMain : Managers.EasyManagersGetters
    {
        /// <summary>Current GeonBit version identifier.</summary>
        public const string VERSION = "0.1.0.8";

        // singleton instance
        static GeonBitMain _instance;

        /// <summary>
        /// Get main instance.
        /// </summary>
        public static GeonBitMain Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GeonBitMain();
                }
                return _instance;
            }
        }

        // list of managers to update and manage
        List<Managers.IManager> _managers = new List<Managers.IManager>();

        // indicate if Initialize() was already called or not.
        bool _wasInit = false;

        /// <summary>
        /// Get if the built-in UI system currently enabled.
        /// </summary>
        public bool UiEnabled { get; private set; } = true;

        /// <summary>
        /// Time until next fixed update event.
        /// </summary>
        float _timeForNextFixedUpdate = 0f;

        /// <summary>
        /// If time factor in current frame (eg seconds passed from last frame) is bigger than this number,
        /// skip current frame. This is to prevent buggy behavior when getting focus back / continue after
        /// debugging for too long.
        /// </summary>
        public float MaxTimeFactorToRun = 10f;

        /// <summary>
        /// If true, will not run when application is not focused.
        /// </summary>
        public bool PauseWhenNotFocused = true;

        /// <summary>
        /// SpriteBatch for UI drawing.
        /// </summary>
        SpriteBatch _spriteBatch;

        /// <summary>
        /// To make it a singleton.
        /// </summary>
        private GeonBitMain()
        {
        }

        /// <summary>
        /// Return if we are currently in debug mode.
        /// </summary>
        public new bool DebugMode
        {
            get { return _debug; }
        }

        // are we initialized in debug mode?
#if DEBUG
        private bool _debug = true;
#else
        private bool _debug = false;
#endif

        /// <summary>
        /// Run GeonBit engine.
        /// Call this function from your program Main() function, with the implementation of your GeonBitGame class.
        /// </summary>
        /// <param name="game">Your main 'game' class.</param>
        public void Run(GeonBitGame game)
        {
            using (var wrapper = new MonoGameGameWrapper(game))
                wrapper.Run();
        }

        /// <summary>
        /// Initialize engine.
        /// </summary>
        /// <param name="game">Main game class instance.</param>
        /// <param name="graphics">Graphic device manager.</param>
        /// <param name="uiSkin">Which UI skin to use. If null, will not init GeonBit.UI.</param>
        /// <param name="debugMode">Should we run in debug mode (show debug components etc.)</param>
        public void Initialize(Game game, GraphicsDeviceManager graphics, string uiSkin = "hd", bool? debugMode = null)
        {
            // sanity check
            if (_wasInit)
            {
                throw new Exceptions.InvalidActionException("GeonBit main was already init!");
            }

            // register all the core managers
            RegisterManager(Managers.GraphicsManager.Instance);
            RegisterManager(Managers.SoundManager.Instance);
            RegisterManager(Managers.Application.Instance);
            RegisterManager(Managers.Diagnostic.Instance);
            RegisterManager(Managers.GameInput.Instance);
            RegisterManager(Managers.Prototypes.Instance);
            RegisterManager(Managers.TimeManager.Instance);
            RegisterManager(Managers.Plugins.Instance);
            RegisterManager(Managers.GameFiles.Instance);
            RegisterManager(Managers.ConfigStorage.Instance);

            // before starting to initialize ask the plugins manager to load all plugins
            // this is because the plugins may add additional managers we'll want to init in this call.
            Managers.Plugins.Instance.LoadAll();

            // set debug mode
            if (debugMode != null)
                _debug = debugMode.Value;

            // set that GeonBit main was init
            _wasInit = true;

            // initialize UI
            UiEnabled = uiSkin != null;
            if (UiEnabled)
            {
                UI.UserInterface.Initialize(game.Content, uiSkin);
            }

            // initialize core stuff
            Core.CoreManager.Initialize(graphics, game.Content);

            // create spritebatch for the UI rendering
            _spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            // set 'game' instance in Application manager.
            Application.SetGameInstance(game);

            // init all external managers
            foreach (var manager in _managers)
            {
                manager.Initialize();
            }

            // set default empty scene
            Application.LoadScene(new ECS.GameScene());
        }

        /// <summary>
        /// Register a manager class.
        /// Note: must be called before calling Initialize().
        /// </summary>
        /// <param name="manager">Manager class to register.</param>
        public void RegisterManager(Managers.IManager manager)
        {
            // we cannot register managers after init is called
            if (_wasInit)
            {
                throw new Exceptions.InvalidActionException("Cannot register managers after Initialize was called!");
            }

            // add manager
            _managers.Add(manager);
        }

        /// <summary>
        /// Do per-frame events (call this inside your Update() function).
        /// </summary>
        /// <param name="gameTime">MonoGame gameTime object.</param>
        public void Update(GameTime gameTime)
        {
            // skip if not focused
            if (PauseWhenNotFocused && !Application.IsActive)
            {
                return;
            }

            // if time passed since last update was too long, skip
            // this is to prevent gliches in graphics / physics if the game window regain focus after a long
            // time being hidden. You can remove this mechanism by setting MaxTimeFactorToRun to 0f.
            if (MaxTimeFactorToRun > 0 && gameTime.ElapsedGameTime.TotalSeconds > MaxTimeFactorToRun)
            {
                return;
            }

            // GeonBit.UI update UI manager
            if (UiEnabled)
            {
                UI.UserInterface.Active.Update(gameTime);
            }

            // update graphics
            Core.Graphics.GraphicsManager.Update(TimeManager.TimeFactor);

            // update active scene
            ActiveScene.Update();

            // update all managers
            foreach (var manager in _managers)
            {
                manager.Update(gameTime);
            }

            // check if time for fixed-update event
            _timeForNextFixedUpdate -= TimeManager.TimeFactor;
            if (_timeForNextFixedUpdate <= 0f)
            {
                _timeForNextFixedUpdate = TimeManager.FixedUpdateInterval;
                foreach (var manager in _managers)
                {
                    manager.FixedUpdate(TimeManager.FixedTimeFactor);
                }
            }
        }

        /// <summary>
        /// Draw everything.
        /// Note: need to be called after clearing the device.
        /// </summary>
        /// <param name="clearBuffer">If true, will also clear buffer.</param>
        public void Draw(bool clearBuffer = true)
        {
            // get graphic device
            GraphicsDevice device = Core.Graphics.GraphicsManager.GraphicsDevice;

            // call the before-draw update function
            ActiveScene.BeforeDraw();

            // start drawing frame
            Core.Graphics.GraphicsManager.StartDrawFrame();

            // reset diagnostic drawings count
            Diagnostic.ResetDrawCounters();

            // render everything
            ActiveScene.Draw();

            // end drawing frame (will also draw all rendering queues)
            Core.Graphics.GraphicsManager.EndDrawFrame();

            // reset stencil state
            device.DepthStencilState = DepthStencilState.Default;

            // draw ui
            if (UiEnabled)
            {
                UI.UserInterface.Active.Draw(_spriteBatch);
            }
        }
    }
}

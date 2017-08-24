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
// A class to wrap MonoGame 'Game' class and invoke GeonBit functions.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeonBit.Managers;

namespace GeonBit
{
    /// <summary>
    /// A class that wraps MonoGame 'Game' class and implements all the basic required functions.
    /// It then invoke the corresponding functions in GeonBitGame class provided by the user.
    /// </summary>
    internal class MonoGameGameWrapper : Game
    {
        /// <summary>
        /// Graphics manager.
        /// </summary>
        protected GraphicsDeviceManager _graphics;

        /// <summary>
        /// Sprite batch - used for UI and 2d rendering on screen.
        /// </summary>
        protected SpriteBatch _spriteBatch;

        /// <summary>
        /// The GeonBitGame class provided by the user.
        /// </summary>
        GeonBitGame _game;

        /// <summary>
        /// Create the Game wrapper class.
        /// </summary>
        /// <param name="game">GeonBit game instance (provided by the user).</param>
        public MonoGameGameWrapper(GeonBitGame game)
        {
            // store game class and set this pointer
            _game = game;
            _game._gameWrapper = this;

            // create graphics and init content root directory
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // if vsync disabled
            if (!_game.InitParams.EnableVsync)
            {
                _graphics.SynchronizeWithVerticalRetrace = false;
                IsFixedTimeStep = false;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected sealed override void Initialize()
        {
            // initialize engine
            GeonBitMain.Instance.Initialize(this, _graphics, _game.InitParams.UiTheme, _game.InitParams.DebugMode);

            // do pre-initialize stuff
            _game.PreInit();

            // call the _game init
            _game.Initialize();

            // call base initialize
            base.Initialize();
        }
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected sealed override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected sealed override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected sealed override void Update(GameTime gameTime)
        {
            // if not currently active skip
            if (!IsActive)
                return;

            // do main update
            GeonBitMain.Instance.Update(gameTime);

            // call the GeonBitGame Update() function
            _game.Update(gameTime);

            // call base update function
            base.Update(gameTime);
        }

        /// <summary>
        /// Set window title.
        /// </summary>
        /// <param name="title">New title to set.</param>
        public void SetTitle(string title)
        {
            Window.Title = title;
        }

        /// <summary>
        /// Resize window.
        /// </summary>
        /// <param name="size">New window size.</param>
        public void Resize(Point size)
        {
            _graphics.PreferredBackBufferWidth = size.X;
            _graphics.PreferredBackBufferHeight = size.Y;
            _graphics.ApplyChanges();
            OnResize();
        }

        /// <summary>
        /// Make game work in fullscreen mode.
        /// </summary>
        /// <param name="framed">If true, will make fullscreen but with window frame.</param>
        /// <param name="resolution">Resolution to use. If not provided, will use current device resolution.</param>
        public void MakeFullscreen(bool framed = false, Point? resolution = null)
        {
            // get default resolution
            Point size = resolution ?? GetDisplayModeResolution();

            // "fullscreen" with frame and title bar
            if (framed)
            {
                _graphics.PreferredBackBufferWidth = size.X;
                _graphics.PreferredBackBufferHeight = size.Y;
                _graphics.IsFullScreen = false;
                _graphics.ApplyChanges();
            }
            // really fullscreen
            else
            {
                _graphics.PreferredBackBufferWidth = size.X;
                _graphics.PreferredBackBufferHeight = size.Y;
                Window.AllowUserResizing = true;
                Window.IsBorderless = true;
                _graphics.ApplyChanges();
                _graphics.ToggleFullScreen();
            }

            // notify resize
            OnResize();
        }

        /// <summary>
        /// Handle resize events.
        /// </summary>
        private void OnResize()
        {
            Core.Graphics.GraphicsManager.HandleResize();
        }

        /// <summary>
        /// Get current display mode resolution.
        /// </summary>
        /// <returns>Current display mode resolution (or in other words, desktop's resolution).</returns>
        public Point GetDisplayModeResolution()
        {
            return new Point(_graphics.GraphicsDevice.DisplayMode.Width, _graphics.GraphicsDevice.DisplayMode.Height);
        }

        /// <summary>
        /// Get supported resolutions.
        /// </summary>
        /// <param name="onlyWithSameRatio">If true, will only return supported resolutions with the same ratio as current resolution.</param>
        /// <returns>Array of supported resolutions.</returns>
        public Point[] GetSupportedResolutions(bool onlyWithSameRatio = false)
        {
            // get native resulotion
            Point nativeResolution = GetDisplayModeResolution();
            float nativeRatio = (float)nativeResolution.X / (float)nativeResolution.Y;

            // iterate supported resolutions
            List<Point> ret = new List<Point>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                // filter by ratio
                if (onlyWithSameRatio)
                {
                    if ((float)mode.Width / (float)mode.Height != nativeRatio)
                    {
                        continue;
                    }
                }

                // add to return list
                ret.Add(new Point(mode.Width, mode.Height));
            }
            return ret.ToArray();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected sealed override void Draw(GameTime gameTime)
        {
            // clear buffers
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw everything
            GeonBitMain.Instance.Draw();

            // call the _game draw function
            _game.Draw(gameTime);

            // call base draw function
            base.Draw(gameTime);
        }
    }
}

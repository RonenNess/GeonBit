﻿#region LICENSE
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
// A class to wrap MonoGame 'Game' class and invoke GeonBit functions.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

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
            if (!_game.EnableVsync)
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
            GeonBitMain.Instance.Initialize(this, _graphics, _game.UiTheme, _game.DebugMode);

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
        /// Make game work in fullscreen mode.
        /// </summary>
        /// <param name="framed">If true, will make fullscreen but with window frame.</param>
        public void MakeFullscreen(bool framed = false)
        {
            // "fullscreen" with frame and title bar
            if (framed)
            {
                _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.DisplayMode.Width;
                _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.DisplayMode.Height;
                _graphics.IsFullScreen = false;
                _graphics.ApplyChanges();
                Window.AllowUserResizing = false;
            }
            // really fullscreen
            else
            {
                _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.DisplayMode.Width;
                _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.DisplayMode.Height;
                _graphics.ApplyChanges();
                _graphics.ToggleFullScreen();
            }
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

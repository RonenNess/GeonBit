// This file shows an example of a Program.cs file and allow us to execute GeonBit as binary.
// This project should be build as a lib, this file is for testing only.
using System;
using Microsoft.Xna.Framework;

namespace GeonBit
{
    /// <summary>
    /// Empty GeonBitGame implementation just so we can execute binary.
    /// </summary>
    internal class EmptyGeonBitMain : GeonBitGame
    {
        // create the game class
        public EmptyGeonBitMain()
        {
            // disable ui, since this project don't contain geonbit ui content
            InitParams.UiTheme = null;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        override public void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Initialize to implement per main type.
        /// </summary>
        override public void Initialize()
        { 
        }

        /// <summary>
        /// Draw function to implement per main type.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        override public void Draw(GameTime gameTime)
        {
        }
    }

    /// <summary>
    /// Program to execute GeonBit as a binary file, for test purposes.
    /// Don't use this class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GeonBitMain.Instance.Run(new EmptyGeonBitMain());
        }
    }
}

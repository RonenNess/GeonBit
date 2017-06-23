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
// Implement a scene background component.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.ECS.Components.Graphics
{
    /// <summary>
    /// Different modes to draw scene background.
    /// </summary>
    public enum BackgroundDrawMode
    {
        /// <summary>
        /// Will draw background as repeating tiles.
        /// </summary>
        Tiled,

        /// <summary>
        /// Will stretch the texture over the entire viewport.
        /// </summary>
        Stretched,

        /// <summary>
        /// Will try to fit the background to the viewport size ratio with minimal stretching.
        /// This means some parts of the texture will be out of screen (can either be from top / bottom, or left / right).
        /// </summary>
        Cover,
    }

    /// <summary>
    /// Add a simple texture-based background to the scene.
    /// </summary>
    public class SceneBackground : BaseComponent
    {
        /// <summary>
        /// Texture to draw.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// Texture drawing mode.
        /// </summary>
        public BackgroundDrawMode DrawMode = BackgroundDrawMode.Stretched;

        /// <summary>
        /// When in Tiled draw mode only, this will be the source texture tile offset.
        /// </summary>
        public Point TileOffset = Point.Zero;

        /// <summary>
        /// Create the scene background component.
        /// </summary>
        /// <param name="texture">Background texture to draw.</param>
        public SceneBackground(Texture2D texture)
        {
            Texture = texture;
        }

        /// <summary>
        /// Create the scene background component.
        /// </summary>
        /// <param name="path">Background texture path.</param>
        public SceneBackground(string path) : this(Resources.GetTexture(path))
        {
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            SceneBackground ret = new SceneBackground(Texture);
            CopyBasics(ret);
            ret.TileOffset = TileOffset;
            ret.DrawMode = DrawMode;
            return ret;
        }

        /// <summary>
        /// Called every frame before the scene renders.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected override void OnBeforeDraw()
        {
            // draw background based on draw mode
            switch (DrawMode)
            {
                // tiled background
                case BackgroundDrawMode.Tiled:
                    Core.Graphics.GraphicsManager.DrawTiledTexture(Texture, Vector2.Zero,
                        new Rectangle(TileOffset, Core.Graphics.GraphicsManager.ViewportSize));
                    break;

                // stretched background
                case BackgroundDrawMode.Stretched:
                    Core.Graphics.GraphicsManager.DrawTexture(Texture,
                        Texture.Bounds, new Rectangle(Point.Zero, Core.Graphics.GraphicsManager.ViewportSize));
                    break;

                // cover background
                case BackgroundDrawMode.Cover:

                    // get how much we need to scale for X / Y to fit
                    float needScaleX = (float)Core.Graphics.GraphicsManager.ViewportSize.X / (float)Texture.Width;
                    float needScaleY = (float)Core.Graphics.GraphicsManager.ViewportSize.Y / (float)Texture.Height;

                    // take the bigger scale as the base scale to use for both X and Y (to maintain texture ratio)
                    float scale = System.Math.Max(needScaleX, needScaleY);

                    // draw the background
                    Core.Graphics.GraphicsManager.DrawTexture(Texture, Texture.Bounds, Vector2.Zero, scale, Vector2.One * 0.5f);
                    break;
            }
        }
    }
}

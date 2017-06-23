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
// A component that renders a texture always facing camera.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeonBit.Core.Graphics;
using GeonBit.Core.Graphics.Materials;

namespace GeonBit.ECS.Components.Graphics
{
    /// <summary>
    /// This component renders a 3D quad that always faces the active camera.
    /// </summary>
    public class BillboardRenderer : BaseRendererComponent
    {
        /// <summary>
        /// The entity from the core layer used to draw the model.
        /// </summary>
        protected SpriteEntity _entity;

        /// <summary>
        /// Get the main entity instance of this renderer.
        /// </summary>
        protected override BaseRenderableEntity Entity { get { return _entity; } }

        /// <summary>
        /// Entity blending state.
        /// </summary>
        public BlendState BlendingState
        {
            set { _entity.BlendingState = value; }
            get { return _entity.BlendingState; }
        }

        /// <summary>
        /// Override material default settings for this specific model instance.
        /// </summary>
        public MaterialOverrides MaterialOverride
        {
            get { return _entity.MaterialOverride; }
            set { _entity.MaterialOverride = value; }
        }

        /// <summary>
        /// Set / get optional axis to lock rotation to.
        /// </summary>
        public Vector3? LockedAxis
        {
            get { return _entity.LockedAxis; }
            set { _entity.LockedAxis = value; }
        }

        /// <summary>
        /// Set / get the material of this sprite.
        /// </summary>
        public MaterialAPI Material
        {
            get { return _entity.Material; }
            set { _entity.Material = value; }
        }

        // spritesheet used for billboards (1 step only that coveres the entire texture).
        static SpriteSheet _billboardSpritesheet = new SpriteSheet(new Point(1, 1));

        /// <summary>
        /// Create the billboard renderer component.
        /// </summary>
        /// <param name="texture">Texture to use for this sprite with a default material.</param>
        public BillboardRenderer(Texture2D texture = null)
        {
            _entity = new SpriteEntity(_billboardSpritesheet, texture);
        }

        /// <summary>
        /// Create the billboard renderer component.
        /// </summary>
        /// <param name="material">Material to use with this sprite.</param>
        public BillboardRenderer(MaterialAPI material)
        {
            _entity = new SpriteEntity(_billboardSpritesheet, material);
        }

        /// <summary>
        /// Create the billboard renderer component.
        /// </summary>
        /// <param name="texturePath">Texture to use for this sprite with a new default material.</param>
        public BillboardRenderer(string texturePath = null) : this(Resources.GetTexture(texturePath))
        {
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            BillboardRenderer ret = new BillboardRenderer(_entity.Material);
            CopyBasics(ret);
            ret._entity.CopyStep(_entity);
            ret.MaterialOverride = _entity.MaterialOverride.Clone();
            ret.LockedAxis = LockedAxis;
            ret.BlendingState = BlendingState;
            return ret;
        }
    }
}

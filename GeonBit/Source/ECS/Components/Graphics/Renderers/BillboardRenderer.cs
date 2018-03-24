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
    public class BillboardRenderer : BaseRendererWithOverrideMaterial
    {
        /// <summary>
        /// The entity from the core layer used to draw the model.
        /// </summary>
        protected SpriteEntity _entity;

        /// <summary>
        /// Optional position offset.
        /// </summary>
        public Vector3 PositionOffset
        {
            get { return _entity.PositionOffset; }
            set { _entity.PositionOffset = value; }
        }

        /// <summary>
        /// Get the main entity instance of this renderer.
        /// </summary>
        protected override BaseRenderableEntity Entity { get { return _entity; } }

        /// <summary>
        /// Override material default settings for this specific model instance.
        /// </summary>
        public override MaterialOverrides MaterialOverride
        {
            get { return _entity.MaterialOverride; }
            set { _entity.MaterialOverride = value; }
        }

        /// <summary>
        /// Set / get optional axis to lock rotation to (when facing camera).
        /// </summary>
        public Vector3? LockedAxis
        {
            get { return _entity.LockedAxis; }
            set { _entity.LockedAxis = value; }
        }

        /// <summary>
        /// If true, sprite will always face camera. If false will just use node's rotation.
        /// </summary>
        public bool FaceCamera
        {
            get { return _entity.FaceCamera; }
            set { _entity.FaceCamera = value; }
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
        /// <param name="faceCamera">If true, will always face camera. If false will use node's rotation.</param>
        public BillboardRenderer(Texture2D texture = null, bool faceCamera = true)
        {
            _entity = new SpriteEntity(_billboardSpritesheet, texture);
            FaceCamera = faceCamera;
        }

        /// <summary>
        /// Create the billboard renderer component.
        /// </summary>
        /// <param name="material">Material to use with this sprite.</param>
        /// <param name="faceCamera">If true, will always face camera. If false will use node's rotation.</param>
        public BillboardRenderer(MaterialAPI material, bool faceCamera = true)
        {
            _entity = new SpriteEntity(_billboardSpritesheet, material);
            FaceCamera = faceCamera;
        }

        /// <summary>
        /// Create the billboard renderer component.
        /// </summary>
        /// <param name="texturePath">Texture to use for this sprite with a new default material.</param>
        /// <param name="faceCamera">If true, will always face camera. If false will use node's rotation.</param>
        public BillboardRenderer(string texturePath = null, bool faceCamera = true) : this(Resources.GetTexture(texturePath), faceCamera)
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
            ret.PositionOffset = PositionOffset;
            ret.FaceCamera = FaceCamera;
            return ret;
        }
    }
}

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
// A test material that uses MonoGame default effect with default lightings.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace GeonBit.Core.Graphics.Materials
{
    /// <summary>
    /// A basic material with default lightings.
    /// </summary>
    public class BasicMaterial : MaterialAPI
    {
        // the effect instance of this material.
        BasicEffect _effect;

        /// <summary>
        /// Get the effect instance.
        /// </summary>
        public override Effect Effect { get { return _effect; } }

        // empty effect instance to clone when creating new material
        static BasicEffect _emptyEffect = new BasicEffect(GraphicsManager.GraphicsDevice);

        /// <summary>
        /// Create the default material from empty effect.
        /// </summary>
        public BasicMaterial() : this(_emptyEffect, true)
        {
        }

        /// <summary>
        /// Create the material from another material instance.
        /// </summary>
        /// <param name="other">Other material to clone.</param>
        public BasicMaterial(BasicMaterial other)
        {
            _effect = other._effect.Clone() as BasicEffect;
            MaterialAPI asBase = this;
            other.CloneBasics(ref asBase);
        }

        /// <summary>
        /// Create the default material.
        /// </summary>
        /// <param name="fromEffect">Effect to create material from.</param>
        /// <param name="copyEffectProperties">If true, will copy initial properties from effect.</param>
        public BasicMaterial(BasicEffect fromEffect, bool copyEffectProperties = true)
        {
            // store effect and set default properties
            _effect = fromEffect.Clone() as BasicEffect;
            SetDefaults();

            // copy properties from effect itself
            if (copyEffectProperties)
            {
                // set effect defaults
                Texture = fromEffect.Texture;
                TextureEnabled = fromEffect.TextureEnabled;
                Alpha = fromEffect.Alpha;
                AmbientLight = new Color(fromEffect.AmbientLightColor.X, fromEffect.AmbientLightColor.Y, fromEffect.AmbientLightColor.Z);
                DiffuseColor = new Color(fromEffect.DiffuseColor.X, fromEffect.DiffuseColor.Y, fromEffect.DiffuseColor.Z);
                SpecularColor = new Color(fromEffect.SpecularColor.X, fromEffect.SpecularColor.Y, fromEffect.SpecularColor.Z);
                SpecularPower = fromEffect.SpecularPower;

                // enable lightings by default
                _effect.EnableDefaultLighting();
                _effect.LightingEnabled = true;
            }
        }

        /// <summary>
        /// Apply this material.
        /// </summary>
        override protected void MaterialSpecificApply(bool wasLastMaterial)
        {
            // set world matrix
            if (IsDirty(MaterialDirtyFlags.World))
            {
                _effect.World = World;
            }

            // if it was last material used, stop here - no need for the following settings
            if (wasLastMaterial) { return; }

            // set all effect params
            if (IsDirty(MaterialDirtyFlags.TextureParams))
            {
                _effect.Texture = Texture;
                _effect.TextureEnabled = TextureEnabled;
            }
            if (IsDirty(MaterialDirtyFlags.Alpha))
            {
                _effect.Alpha = Alpha;
            }
            if (IsDirty(MaterialDirtyFlags.AmbientLight))
            {
                _effect.AmbientLightColor = AmbientLight.ToVector3();
            }
            if (IsDirty(MaterialDirtyFlags.EmissiveLight))
            {
                _effect.EmissiveColor = EmissiveLight.ToVector3();
            }
            if (IsDirty(MaterialDirtyFlags.MaterialColors))
            {
                _effect.DiffuseColor = DiffuseColor.ToVector3();
                _effect.SpecularColor = SpecularColor.ToVector3();
                _effect.SpecularPower = SpecularPower;
            }
        }

        /// <summary>
        /// Update material view matrix.
        /// </summary>
        /// <param name="view">New view to set.</param>
        override protected void UpdateView(ref Matrix view)
        {
            _effect.View = View;
        }

        /// <summary>
        /// Update material projection matrix.
        /// </summary>
        /// <param name="projection">New projection to set.</param>
        override protected void UpdateProjection(ref Matrix projection)
        {
            _effect.Projection = Projection;
        }

        /// <summary>
        /// Clone this material.
        /// </summary>
        /// <returns>Copy of this material.</returns>
        public override MaterialAPI Clone()
        {
            return new BasicMaterial(this);
        }
    }
}

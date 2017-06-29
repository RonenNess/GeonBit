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
// A test material that uses MonoGame default skinned effect with default lightings.
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
    /// A test material with default lightings and default MonoGame effects.
    /// </summary>
    public class SkinnedMaterial : MaterialAPI
    {
        // the effect instance of this material.
        SkinnedEffect _effect;

        /// <summary>
        /// Get the effect instance.
        /// </summary>
        public override Effect Effect { get { return _effect; } }

        /// <summary>
        /// Current bone transformations
        /// </summary>
        Matrix[] _currBones;

        /// <summary>
        /// Set bone transforms for an animated material.
        /// Useable only for materials that implement skinned animation in shader.
        /// </summary>
        /// <param name="bones"></param>
        override public void SetBoneTransforms(Matrix[] bones)
        {
            _currBones = bones;
        }

        /// <summary>
        /// Create the material.
        /// </summary>
        /// <param name="fromEffect">Effect to create material from.</param>
        /// <param name="copyEffectProperties">If true, will copy initial properties from effect.</param>
        public SkinnedMaterial(SkinnedEffect fromEffect, bool copyEffectProperties = true)
        {
            // store effect and set default properties
            _effect = fromEffect; // new SkinnedEffect(fromEffect.GraphicsDevice);
            SetDefaults();

            // copy properties from effect itself
            if (copyEffectProperties)
            {
                // set effect defaults
                Texture = fromEffect.Texture;
                TextureEnabled = fromEffect.Texture != null;
                Alpha = fromEffect.Alpha;
                AmbientLight = new Color(fromEffect.AmbientLightColor.X, fromEffect.AmbientLightColor.Y, fromEffect.AmbientLightColor.Z);
                DiffuseColor = new Color(fromEffect.DiffuseColor.X, fromEffect.DiffuseColor.Y, fromEffect.DiffuseColor.Z);
                SmoothLighting = fromEffect.PreferPerPixelLighting;
                SpecularColor = new Color(fromEffect.SpecularColor.X, fromEffect.SpecularColor.Y, fromEffect.SpecularColor.Z);
                SpecularPower = fromEffect.SpecularPower;

                // enable lightings by default
                LightingEnabled = true;
                SmoothLighting = true;
                _effect.EnableDefaultLighting();
            }
        }

        /// <summary>
        /// Create the material from another material instance.
        /// </summary>
        /// <param name="other">Other material to clone.</param>
        public SkinnedMaterial(SkinnedMaterial other)
        {
            _effect = other._effect;
            MaterialAPI asBase = this;
            other.CloneBasics(ref asBase);
        }

        /// <summary>
        /// Apply this material.
        /// </summary>
        override protected void MaterialSpecificApply(bool wasLastMaterial)
        {
            // set world matrix
            _effect.World = World;

            // if it was last material used, stop here - no need for the following settings
            if (wasLastMaterial) { return; }

            // set bone transforms
            if (_currBones != null) { _effect.SetBoneTransforms(_currBones); }

            // set all effect params
            _effect.View = View;
            _effect.Projection = Projection;
            _effect.Texture = Texture;
            _effect.Alpha = Alpha;
            _effect.AmbientLightColor = LightingEnabled ? AmbientLight.ToVector3() : Color.White.ToVector3();
            _effect.DiffuseColor = DiffuseColor.ToVector3();
            _effect.PreferPerPixelLighting = SmoothLighting;
            _effect.SpecularColor = SpecularColor.ToVector3();
            _effect.SpecularPower = SpecularPower;
            GraphicsManager.GraphicsDevice.SamplerStates[0] = SamplerState;
        }

        /// <summary>
        /// Clone this material.
        /// </summary>
        /// <returns>Copy of this material.</returns>
        public override MaterialAPI Clone()
        {
            return new SkinnedMaterial(this);
        }
    }
}

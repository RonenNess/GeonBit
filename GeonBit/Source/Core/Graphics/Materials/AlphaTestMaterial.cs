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
// A material with basic alpha test (invisble pixels are ommitted).
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
    /// Material with alpha test.
    /// </summary>
    public class AlphaTestMaterial : MaterialAPI
    {
        // the effect instance of this material.
        AlphaTestEffect _effect;

        /// <summary>
        /// Get the effect instance.
        /// </summary>
        public override Effect Effect { get { return _effect; } }

        /// <summary>
        /// The function used to decide which pixels are visible and which are not.
        /// </summary>
        public CompareFunction AlphaFunction = CompareFunction.GreaterEqual;

        /// <summary>
        /// Alpha value to compare with the AlphaFunction, to decide which pixels are visible and which are not.
        /// </summary>
        public int ReferenceAlpha = 128;

        /// <summary>
        /// Create the default material from empty effect.
        /// </summary>
        public AlphaTestMaterial() : this(new AlphaTestEffect(GraphicsManager.GraphicsDevice), true)
        {
        }

        /// <summary>
        /// Create the material.
        /// </summary>
        /// <param name="effect">Effect to use.</param>
        /// <param name="copyEffectProperties">If true, will copy initial properties from effect.</param>
        public AlphaTestMaterial(AlphaTestEffect effect, bool copyEffectProperties = true)
        {
            // store effect and set default properties
            _effect = effect;
            SetDefaults();

            // copy properties from effect itself
            if (copyEffectProperties)
            {
                // set effect defaults
                Texture = _effect.Texture;
                Alpha = _effect.Alpha;

                // enable lightings by default
                LightingEnabled = true;
                SmoothLighting = true;
            }
        }

        /// <summary>
        /// Create the material from another material instance.
        /// </summary>
        /// <param name="other">Other material to clone.</param>
        public AlphaTestMaterial(AlphaTestMaterial other)
        {
            _effect = other._effect;
            MaterialAPI asBase = this;
            other.CloneBasics(ref asBase);
            AlphaFunction = other.AlphaFunction;
            ReferenceAlpha = other.ReferenceAlpha;
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

            // set all effect params
            _effect.View = View;
            _effect.Projection = Projection;
            _effect.Texture = Texture;
            _effect.Alpha = Alpha;
            _effect.DiffuseColor = DiffuseColor.ToVector3();
            _effect.AlphaFunction = AlphaFunction;
            _effect.ReferenceAlpha = ReferenceAlpha;
            GraphicsManager.GraphicsDevice.SamplerStates[0] = SamplerState;
        }

        /// <summary>
        /// Clone this material.
        /// </summary>
        /// <returns>Copy of this material.</returns>
        public override MaterialAPI Clone()
        {
            return new AlphaTestMaterial(this);
        }
    }
}

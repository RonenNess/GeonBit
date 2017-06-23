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
// A material for sprites and billboards with alpha-test.
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
    /// Material for sprites and billboards.
    /// </summary>
    public class SpriteMaterial : AlphaTestMaterial
    {
        /// <summary>
        /// Create the default material from empty effect.
        /// </summary>
        public SpriteMaterial() : base()
        {
            SamplerState = SamplerState.PointClamp;
            TextureEnabled = true;
        }

        /// <summary>
        /// Create the default material.
        /// </summary>
        /// <param name="effect">Effect to use.</param>
        /// <param name="copyEffectProperties">If true, will copy initial properties from effect.</param>
        public SpriteMaterial(AlphaTestEffect effect, bool copyEffectProperties = true) : base(effect, copyEffectProperties)
        {
            SamplerState = SamplerState.PointClamp;
            TextureEnabled = true;
        }
    }
}

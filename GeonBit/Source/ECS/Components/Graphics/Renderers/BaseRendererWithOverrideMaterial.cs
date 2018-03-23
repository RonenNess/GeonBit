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
// Define a base renderer with override material options.
//
// Author: Ronen Ness.
// Since: 2018.
//-----------------------------------------------------------------------------
#endregion
using GeonBit.Core.Graphics;


namespace GeonBit.ECS.Components.Graphics
{
    /// <summary>
    /// A base renderer with override material options.
    /// </summary>
    public abstract class BaseRendererWithOverrideMaterial : BaseRendererComponent
    {
        /// <summary>
        /// Override material default settings for this specific renderer instance.
        /// </summary>
        public abstract MaterialOverrides MaterialOverride { get; set; }
    }
}

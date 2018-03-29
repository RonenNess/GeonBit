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
// Define the interface for lights manager.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Graphics.Lights
{
    /// <summary>
    /// Interface for lights manager.
    /// These objects manage lights and provide them to materials when trying to render.
    /// </summary>
    public interface ILightsManager
    {
        /// <summary>
        /// Get / set ambient light.
        /// </summary>
        /// <returns></returns>
        Color AmbientLight { get; set; }

        /// <summary>
        /// Add a light source to the lights manager.
        /// </summary>
        /// <param name="light">Light to add.</param>
        void AddLight(LightSource light);

        /// <summary>
        /// Remove a light source from the lights manager.
        /// </summary>
        /// <param name="light">Light to remove.</param>
        void RemoveLight(LightSource light);
        
        /// <summary>
        /// Enable / disable all lights.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Get all lights for a given bounding sphere and material.
        /// </summary>
        /// <param name="material">Material to get lights for.</param>
        /// <param name="boundingSphere">Rendering bounding sphere.</param>
        /// <param name="maxLights">Maximum lights count to return.</param>
        /// <returns>Array of lights to apply on this material and drawing. Note: directional lights must always come first!</returns>
        LightSource[] GetLights(Materials.MaterialAPI material, ref BoundingSphere boundingSphere, int maxLights);

        /// <summary>
        /// Update the transformations of a light inside this manager.
        /// </summary>
        /// <param name="light">Light to update.</param>
        void UpdateLightTransform(LightSource light);
    }
}

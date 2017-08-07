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
// Lights manager.
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
    /// Lights Manager manage all lights and serve them to the materials when they need to render.
    /// </summary>
    public class LightsManager
    {
        /// <summary>
        /// Ambient light.
        /// </summary>
        public Color AmbientLight = Color.Gray;

        /// <summary>
        /// Add light source to lights manager.
        /// </summary>
        /// <param name="light">Light to add.</param>
        public void AddLight(LightSource light)
        {
            // if light already got parent, assert
            if (light._manager != null)
            {
                throw new Exceptions.InvalidActionException("Light to add is already inside lights manager!");
            }

            // set light's manager to self
            light._manager = this;

            // add light to lights map
            UpdateLightTransform(light);
        }

        /// <summary>
        /// Get all lights for a given bounding sphere.
        /// </summary>
        /// <param name="boundingSphere"></param>
        /// <returns></returns>
        public LightSource[] GetLights(BoundingSphere boundingSphere)
        {
            return null;
        }

        /// <summary>
        /// Update the transformations of a light inside this manager.
        /// </summary>
        /// <param name="light">Light to update.</param>
        internal void UpdateLightTransform(LightSource light)
        {

        }
    }
}

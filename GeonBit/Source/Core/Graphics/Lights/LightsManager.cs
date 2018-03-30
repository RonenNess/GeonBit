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
// Default basic lights manager.
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
    /// Implement a default, basic lights manager.
    /// </summary>
    public class LightsManager : ILightsManager
    {
        /// <summary>
        /// Contains metadata about lights in this lights manager.
        /// </summary>
        private struct LightSourceMD
        {
            /// <summary>
            /// Min lights region index this light is currently in.
            /// </summary>
            public Vector3 MinRegionIndex;

            /// <summary>
            /// Max lights region index this light is currently in.
            /// </summary>
            public Vector3 MaxRegionIndex;

            /// <summary>
            /// If true, it means the light is infinite (eg have no range).
            /// </summary>
            public bool Infinite;
        }

        // ambient light value
        Color _ambient = Color.Gray;

        /// <summary>
        /// Ambient light.
        /// </summary>
        public Color AmbientLight {
            get { return Enabled ? _ambient : Color.White; }
            set { _ambient = value; }
        }

        // the size of a batch / region containing lights.
        Vector3 _regionSize = new Vector3(250, 250, 250);

        // dictionary of regions and the lights they contain.
        Dictionary<Vector3, List<LightSource>> _regions = new Dictionary<Vector3, List<LightSource>>();

        // list of lights that are infinite, eg have no range limit.
        List<LightSource> _infiniteLights = new List<LightSource>();

        // data about lights in this lights manager
        Dictionary<LightSource, LightSourceMD> _lightsData = new Dictionary<LightSource, LightSourceMD>();

        // list with all lights currently in manager.
        List<LightSource> _lights = new List<LightSource>();

        // to return empty lights array.
        static LightSource[] EmptyLightsArray = new LightSource[0];

        /// <summary>
        /// Enable / disable all lights.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Lights manager divide the world into segments, or regions, that contain lights.
        /// When drawing entities we get the entity bounding sphere and and all the 
        /// </summary>
        public Vector3 LightsRegionSize
        {
            get { return _regionSize; }
            set { _regionSize = value; UpdateLightsRegionSize(); }
        }

        /// <summary>
        /// Add a light source to lights manager.
        /// </summary>
        /// <param name="light">Light to add.</param>
        public void AddLight(LightSource light)
        {
            // if light already got parent, assert
            if (light.LightsManager != null)
            {
                throw new Exceptions.InvalidActionException("Light to add is already inside a lights manager!");
            }

            // set light's manager to self
            light.LightsManager = this;

            // add to list of lights
            _lights.Add(light);

            // add light to lights map
            light.RecalcBoundingSphere(false);
            UpdateLightTransform(light);
        }

        /// <summary>
        /// Remove a light source from lights manager.
        /// </summary>
        /// <param name="light">Light to remove.</param>
        public void RemoveLight(LightSource light)
        {
            // if lights don't belong to this manager, assert
            if (light.LightsManager != this)
            {
                throw new Exceptions.InvalidActionException("Light to remove is not inside this lights manager!");
            }

            // remove light's manager pointer
            light.LightsManager = null;

            // remove from list of lights
            RemoveLightFromItsRegions(light);
            _lights.Remove(light);
            _lightsData.Remove(light);
        }

        /// <summary>
        /// Get min region index for a given bounding sphere.
        /// </summary>
        /// <param name="boundingSphere">Bounding sphere to get min region for.</param>
        /// <returns>Min lights region index.</returns>
        private Vector3 GetMinRegionIndex(ref BoundingSphere boundingSphere)
        {
            Vector3 ret = boundingSphere.Center - Vector3.One * boundingSphere.Radius;
            ret /= LightsRegionSize;
            ret.X = (float)System.Math.Floor(ret.X);
            ret.Y = (float)System.Math.Floor(ret.Y);
            ret.Z = (float)System.Math.Floor(ret.Z);
            return ret;
        }

        /// <summary>
        /// Get max region index for a given bounding sphere.
        /// </summary>
        /// <param name="boundingSphere">Bounding sphere to get min region for.</param>
        /// <returns>Min lights region index.</returns>
        private Vector3 GetMaxRegionIndex(ref BoundingSphere boundingSphere)
        {
            Vector3 ret = boundingSphere.Center + Vector3.One * boundingSphere.Radius;
            ret /= LightsRegionSize;
            ret.X = (float)System.Math.Floor(ret.X);
            ret.Y = (float)System.Math.Floor(ret.Y);
            ret.Z = (float)System.Math.Floor(ret.Z);
            return ret;
        }

        /// <summary>
        /// Get all lights for a given bounding sphere.
        /// </summary>
        /// <param name="material">Material to get lights for.</param>
        /// <param name="boundingSphere">Rendering bounding sphere.</param>
        /// <param name="maxLights">Maximum lights count to return.</param>
        /// <returns>Array of lights to apply on this material and drawing. Note: directional lights must always come first!</returns>
        public LightSource[] GetLights(Materials.MaterialAPI material, ref BoundingSphere boundingSphere, int maxLights)
        {
            // if disabled return empty lights array
            if (!Enabled)
            {
                return EmptyLightsArray;
            }

            // if no lights at all, skip
            if (_regions.Count == 0 && _infiniteLights.Count == 0) { return EmptyLightsArray; }

            // get min and max points of this bounding sphere
            Vector3 min = GetMinRegionIndex(ref boundingSphere);
            Vector3 max = GetMaxRegionIndex(ref boundingSphere);

            // build array to return
            Utils.ResizableArray<LightSource> retLights = new Utils.ResizableArray<LightSource>();

            // add all infinite lights first (directional lights etc)
            foreach (var light in _infiniteLights)
            {
                retLights.Add(light);
            }

            // iterate regions and add lights
            bool isFirstRegionWeCheck = true;
            Vector3 index = new Vector3();
            for (int x = (int)min.X; x <= max.X; ++x)
            {
                index.X = x;
                for (int y = (int)min.Y; y <= max.Y; ++y)
                {
                    index.Y = y;
                    for (int z = (int)min.Z; z <= max.Z; ++z)
                    {
                        index.Z = z;

                        // try to fetch region lights
                        List<LightSource> regionLights;
                        if (_regions.TryGetValue(index, out regionLights))
                        {
                            // iterate lights in region
                            foreach (var light in _regions[index])
                            {
                                // if light not visible, skip
                                if (!light.Visible)
                                    continue;

                                // if its not first region we fetch, test against duplications
                                if (!isFirstRegionWeCheck && System.Array.IndexOf(retLights.InternalArray, light) != -1)
                                    continue;

                                // make sure light really touch object
                                if (!boundingSphere.Intersects(light.BoundingSphere))
                                    continue;

                                // if light is out of camera, skip it
                                if (!GraphicsManager.ActiveCamera.ViewFrustum.Intersects(light.BoundingSphere))
                                    continue;

                                // add light to return array
                                retLights.Add(light);

                                // if exceeded max lights stop here
                                if (retLights.Count >= maxLights)
                                    break;
                            }

                            // no longer first region we test
                            isFirstRegionWeCheck = false;
                        }
                    }
                }
            }

            // return the results array
            retLights.Trim();
            return retLights.InternalArray;
        }

        /// <summary>
        /// Called after user changed the lights region size.
        /// </summary>
        private void UpdateLightsRegionSize()
        {
            // clear regions dictionary
            _regions.Clear();

            // re-add all lights
            foreach (var light in _lights)
            {
                UpdateLightTransform(light);
            }
        }

        /// <summary>
        /// Remove a light from all the regions its in (assuming light is inside this lights manager).
        /// </summary>
        /// <param name="light">Light to remove from regions.</param>
        protected void RemoveLightFromItsRegions(LightSource light)
        {
            // if we don't have any metadata on this light it means its probably a new light. do nothing.
            if (!_lightsData.ContainsKey(light))
            {
                return;
            }

            // get light metadata
            var lightMd = _lightsData[light];
            var min = lightMd.MinRegionIndex;
            var max = lightMd.MaxRegionIndex;

            // if infinite light remove from infinite lights list and stop here
            if (lightMd.Infinite)
            {
                _infiniteLights.Remove(light);
                return;
            }

            // remove light from previous regions
            Vector3 index = new Vector3();
            for (int x = (int)min.X; x <= max.X; ++x)
            {
                index.X = x;
                for (int y = (int)min.Y; y <= max.Y; ++y)
                {
                    index.Y = y;
                    for (int z = (int)min.Z; z <= max.Z; ++z)
                    {
                        index.Z = z;

                        // remove light from region
                        List<LightSource> region;
                        if (_regions.TryGetValue(index, out region))
                        {
                            region.Remove(light);

                            // if region is now empty, remove it
                            if (region.Count == 0)
                            {
                                _regions.Remove(index);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update the transformations of a light inside this manager.
        /// </summary>
        /// <param name="light">Light to update.</param>
        public void UpdateLightTransform(LightSource light)
        {
            // remove light from previous regions
            RemoveLightFromItsRegions(light);

            // if its infinite light add to infinite list
            if (light.IsInfinite)
            {
                // add to infinite lights
                _infiniteLights.Add(light);

                // update light's metadata
                LightSourceMD newMd = new LightSourceMD();
                newMd.Infinite = true;
                _lightsData[light] = newMd;

                // stop here
                return;
            }

            // calc new min and max for the light
            var boundingSphere = light.BoundingSphere;
            var min = GetMinRegionIndex(ref boundingSphere);
            var max = GetMaxRegionIndex(ref boundingSphere);

            // add light to new regions
            Vector3 index = new Vector3();
            for (int x = (int)min.X; x <= max.X; ++x)
            {
                index.X = x;
                for (int y = (int)min.Y; y <= max.Y; ++y)
                {
                    index.Y = y;
                    for (int z = (int)min.Z; z <= max.Z; ++z)
                    {
                        index.Z = z;

                        // if region don't exist, create it
                        if (!_regions.ContainsKey(index))
                        {
                            _regions[index] = new List<LightSource>();
                        }

                        // add light to region
                        _regions[index].Add(light);
                    }
                }
            }

            // update light's metadata
            {
                LightSourceMD newMd = new LightSourceMD();
                newMd.MinRegionIndex = min;
                newMd.MaxRegionIndex = max;
                _lightsData[light] = newMd;
            }
        }
    }
}

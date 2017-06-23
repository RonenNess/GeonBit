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
// A special class to hold per-entity material properties.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.Core.Graphics
{
    /// <summary>
    /// Helper class to manage materials override properties.
    /// You can use this class to set color, material, specular, etc for a specific model renderers, without having to create a
    /// new material for them.
    /// 
    /// This allow you to easily override material's defaults without having to define lots of different materials, but note that this method is
    /// slightly slower so don't use it on too many entities at the same time.
    /// </summary>
    public class MaterialOverrides
    {
        // optional per-entity properties that override material's defaults
        Color? _diffuseColor = null;
        Color? _specularColor = null;
        float? _alpha = null;
        Texture2D _texture = null;
        bool? _lightingEnabled = null;

        /// <summary>
        /// Override diffuse color for this specific entity.
        /// Note: will affect all parts of the model.
        /// </summary>
        public Color? DiffuseColor
        {
            get { return _diffuseColor; }
            set { _diffuseColor = value; UpdateOverridePropertiesState(); }
        }

        /// <summary>
        /// Override specular color for this specific entity.
        /// Note: will affect all parts of the model.
        /// </summary>
        public Color? SpecularColor
        {
            get { return _specularColor; }
            set { _specularColor = value; UpdateOverridePropertiesState(); }
        }

        /// <summary>
        /// Override Alpha value for this specific entity.
        /// Note: will affect all parts of the model.
        /// </summary>
        public float? Alpha
        {
            get { return _alpha; }
            set { _alpha = value; UpdateOverridePropertiesState(); }
        }

        /// <summary>
        /// Override material texture for this specific entity.
        /// Note: will affect all parts of the model.
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; UpdateOverridePropertiesState(); }
        }

        /// <summary>
        /// Override material lighting enabled property for this specific entity.
        /// </summary>
        public bool? LightingEnabled
        {
            get { return _lightingEnabled; }
            set { _lightingEnabled = value; UpdateOverridePropertiesState(); }
        }

        /// <summary>
        /// If true will use the override per-entity properties.
        /// </summary>
        public bool UsingOverrideProperties { get; protected set; }

        /// <summary>
        /// Return if this entity should use material override properties (properties like texture, color, etc
        /// which override the material defaults).
        /// </summary>
        private bool HaveOverrideProperties
        {
            get { return Alpha != null || DiffuseColor != null || Texture != null || SpecularColor != null || LightingEnabled != null; }
        }

        /// <summary>
        /// Update if currently using override properties or not.
        /// </summary>
        private void UpdateOverridePropertiesState()
        {
            UsingOverrideProperties = HaveOverrideProperties;
        }

        /// <summary>
        /// Clone custom render settings.
        /// </summary>
        /// <returns>Cloned settings.</returns>
        public MaterialOverrides Clone()
        {
            MaterialOverrides ret = new MaterialOverrides();
            ret._diffuseColor = _diffuseColor;
            ret._specularColor = _specularColor;
            ret._alpha = _alpha;
            ret._texture = _texture;
            ret._lightingEnabled = _lightingEnabled;
            ret.UpdateOverridePropertiesState();
            return ret;
        }

        /// <summary>
        /// Apply all custom render properties on a given material, and return either the given material or a clone of it, if needed.
        /// This will not do anything if there are no custom properties currently used.
        /// </summary>
        /// <param name="material">Effect to set properties.</param>
        /// <returns>Either the input material or a clone of it with applied properties.</returns>
        public Materials.MaterialAPI Apply(Materials.MaterialAPI material)
        {
            // if there's nothing to do just return the original material
            if (!UsingOverrideProperties)
            {
                return material;
            }

            // we need to apply custom properties. clone the material.
            material = material.Clone();

            // if got override diffuse color, set it
            if (DiffuseColor != null)
            {
                material.DiffuseColor = ((Color)DiffuseColor);
            }

            // if got override specular color, set it
            if (SpecularColor != null)
            {
                material.SpecularColor = ((Color)SpecularColor);
            }

            // if got override alpha, set it
            if (Alpha != null)
            {
                material.Alpha = (float)Alpha;
            }

            // if got override lighting, set it
            if (LightingEnabled != null)
            {
                material.LightingEnabled = (bool)LightingEnabled;
            }

            // if got override texture, set it
            if (Texture != null)
            {
                material.Texture = Texture;
                material.TextureEnabled = true;
            }

            // return the cloned material
            return material;
        }
    }
}

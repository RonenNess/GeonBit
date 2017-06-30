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
// Material base class.
// A material is a MonoGame effect wrapper + per-instance settings, such as 
// diffuse color, lightings, etc.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.Core.Graphics.Materials
{
    /// <summary>
    /// GeonBit.Core.Graphics.Materials contain all the built-in materials.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// A callback to call per technique pass when using material iterate.
    /// </summary>
    /// <param name="pass">Current pass.</param>
    public delegate void EffectPassCallback(EffectPass pass);

    /// <summary>
    /// The base class for a material.
    /// Note: for some material types one or more of the properties below may be ignored.
    /// For example, we might have a material that doesn't support lighting at all, and will ignore lighting-related properties.
    /// </summary>
    abstract public class MaterialAPI
    {
        // last material used
        internal static MaterialAPI _lastMaterialApplied = null;

        /// <summary>
        /// Get the effect instance.
        /// </summary>
        abstract public Effect Effect { get; }

        /// <summary>
        /// Is lightings enabled.
        /// </summary>
        virtual public bool LightingEnabled { get; set; }

        /// <summary>
        /// If true will use per-pixel smooth lighting.
        /// </summary>
        virtual public bool SmoothLighting { get; set; }

        /// <summary>
        /// Diffuse color.
        /// </summary>
        virtual public Color DiffuseColor { get; set; }

        /// <summary>
        /// Specular color.
        /// </summary>
        virtual public Color SpecularColor { get; set; }

        /// <summary>
        /// Ambient light color.
        /// </summary>
        virtual public Color AmbientLight { get; set; }

        /// <summary>
        /// Specular power.
        /// </summary>
        virtual public float SpecularPower { get; set; }

        /// <summary>
        /// Opacity levels (multiplied with color opacity).
        /// </summary>
        virtual public float Alpha { get; set; }

        /// <summary>
        /// Texture to draw.
        /// </summary>
        virtual public Texture2D Texture { get; set; }

        /// <summary>
        /// Is texture currently enabled.
        /// </summary>
        virtual public bool TextureEnabled { get; set; }

        /// <summary>
        /// Current world transformations.
        /// </summary>
        virtual public Matrix World { get; set; }

        // current view matrix (shared by all materials)
        static Matrix _view;

        // current projection matrix (shared by all materials)
        static Matrix _projection;

        /// <summary>
        /// Current view matrix.
        /// </summary>
        virtual public Matrix View { get { return _view; } }

        /// <summary>
        /// Current projection matrix.
        /// </summary>
        virtual public Matrix Projection { get { return _projection; } }

        /// <summary>
        /// Default sampler state.
        /// </summary>
        public static SamplerState DefaultSamplerState = SamplerState.LinearWrap;

        /// <summary>
        /// Sampler state.
        /// </summary>
        public SamplerState SamplerState = DefaultSamplerState;

        /// <summary>
        /// Set materials view and projection matrixes (shared by all materials).
        /// </summary>
        /// <param name="view">Current view matrix.</param>
        /// <param name="projection">Current projection matrix.</param>
        public static void SetViewProjection(Matrix view, Matrix projection)
        {
            _view = view;
            _projection = projection;
        }

        /// <summary>
        /// Clone this material.
        /// </summary>
        /// <returns>Copy of this material.</returns>
        abstract public MaterialAPI Clone();

        /// <summary>
        /// Apply all new properties on the material effect.
        /// Call this whenever you want to draw using this material.
        /// </summary>
        public void Apply(Matrix worldMatrix)
        {
            // set world matrix
            World = worldMatrix;

            // apply sampler state
            GraphicsManager.GraphicsDevice.SamplerStates[0] = SamplerState;

            // set effect tag to point on self, and call the per-effect specific apply
            if (Effect.Tag == null) { Effect.Tag = this; }
            MaterialSpecificApply(_lastMaterialApplied == this);

            // set last material applied to self
            _lastMaterialApplied = this;
        }

        /// <summary>
        /// Apply all new properties on the material effect, implemented per material type.
        /// </summary>
        /// <param name="wasLastMaterial">If true, it means this material was the last material applied. Useful for internal optimizations.</param>
        abstract protected void MaterialSpecificApply(bool wasLastMaterial);

        /// <summary>
        /// Set bone transforms for an animated material.
        /// Useable only for materials that implement skinned animation in shader.
        /// </summary>
        /// <param name="bones"></param>
        virtual public void SetBoneTransforms(Matrix[] bones)
        {
            throw new System.Exception("Material does not support bone transformations in GPU.");
        }

        /// <summary>
        /// Iterate over all passes in current technique and call the provided callback for each pass.
        /// You can use this function to draw stuff manually.
        /// </summary>
        /// <param name="callback">The callback to call for every pass.</param>
        public void IterateEffectPasses(EffectPassCallback callback)
        {
            // render the buffer with effect
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                // draw current pass
                pass.Apply();

                // call the draw callback
                callback(pass);
            }
        }

        /// <summary>
        /// Clone all the basic properties of a material.
        /// </summary>
        /// <param name="cloned">Cloned material to copy properties into.</param>
        protected void CloneBasics(ref MaterialAPI cloned)
        {
            cloned.World = World;
            cloned.TextureEnabled = TextureEnabled;
            cloned.Texture = Texture;
            cloned.Alpha = Alpha;
            cloned.LightingEnabled = LightingEnabled;
            cloned.SmoothLighting = SmoothLighting;
            cloned.DiffuseColor = DiffuseColor;
            cloned.SpecularColor = SpecularColor;
            cloned.SpecularPower = SpecularPower;
            cloned.AmbientLight = AmbientLight;
            cloned.SamplerState = SamplerState;
        }

        /// <summary>
        /// Set default value for all the basic properties.
        /// </summary>
        public void SetDefaults()
        {
            World = Matrix.Identity;
            TextureEnabled = false;
            Texture = null;
            Alpha = 1f;
            LightingEnabled = true;
            SmoothLighting = true;
            DiffuseColor = Color.White;
            SpecularColor = Color.White;
            SpecularPower = 1f;
            AmbientLight = Color.White;
            SamplerState = DefaultSamplerState;
        }
    }
}

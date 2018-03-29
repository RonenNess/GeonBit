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
    /// Track which material parameters need to be recomputed during the next OnApply.
    /// </summary>
    public enum MaterialDirtyFlags
    {
        /// <summary>
        /// Change in world matrix.
        /// </summary>
        World = 1 << 0,
        
        /// <summary>
        /// Change in light sources, not including ambient or emissive.
        /// </summary>
        LightSources = 1 << 1,

        /// <summary>
        /// Change in material color params (can be diffuse, specular, etc. This includes specular power as well.)
        /// </summary>
        MaterialColors = 1 << 2,
        
        /// <summary>
        /// Change in material alpha.
        /// </summary>
        Alpha = 1 << 3,

        /// <summary>
        /// Change in texture, texture enabled, or other texture-related params.
        /// </summary>
        TextureParams = 1 << 4,

        /// <summary>
        /// Lighting params changed (enabled disabled / smooth lighting mode).
        /// </summary>
        LightingParams = 1 << 5,

        /// <summary>
        /// Change in fog-related params.
        /// </summary>
        Fog = 1 << 6,

        /// <summary>
        /// Chage in alpha-test related params.
        /// </summary>
        AlphaTest = 1 << 7,

        /// <summary>
        /// Change in skinned mesh bone transformations.
        /// </summary>
        Bones = 1 << 8,

        /// <summary>
        /// Change in ambient light color.
        /// </summary>
        AmbientLight = 1 << 9,

        /// <summary>
        /// Change in emissive light color.
        /// </summary>
        EmissiveLight = 1 << 10,

        /// <summary>
        /// All dirty flags.
        /// </summary>
        All = int.MaxValue
    }

    /// <summary>
    /// A callback to call per technique pass when using material iterate.
    /// </summary>
    /// <param name="pass">Current pass.</param>
    public delegate void EffectPassCallback(EffectPass pass);

    /// <summary>
    /// Sampler states we can use for materials textures.
    /// </summary>
    static public class SamplerStates
    {
        /// <summary>
        /// AnisotropicClamp sampler state.
        /// </summary>
        public static SamplerState AnisotropicClamp { get { return SamplerState.AnisotropicClamp; } }

        /// <summary>
        /// AnisotropicWrap sampler state.
        /// </summary>
        public static SamplerState AnisotropicWrap { get { return SamplerState.AnisotropicWrap; } }

        /// <summary>
        /// LinearClamp sampler state.
        /// </summary>
        public static SamplerState LinearClamp { get { return SamplerState.LinearClamp; } }

        /// <summary>
        /// PointClamp sampler state.
        /// </summary>
        public static SamplerState PointClamp { get { return SamplerState.PointClamp; } }

        /// <summary>
        /// PointWrap sampler state.
        /// </summary>
        public static SamplerState PointWrap { get { return SamplerState.PointWrap; } }
    }


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
        /// Params dirty flags.
        /// </summary>
        int _dirtyFlags = (int)MaterialDirtyFlags.All;

        /// <summary>
        /// Path of GeonBit built-in effects.
        /// </summary>
        public static readonly string EffectsPath = "GeonBit.Core/Effects/";

        /// <summary>
        /// Get the effect instance.
        /// </summary>
        abstract public Effect Effect { get; }

        /// <summary>
        /// Get how many samplers this material uses.
        /// </summary>
        protected virtual int SamplersCount { get { return 1; } }

        /// <summary>
        /// Create the material object.
        /// </summary>
        public MaterialAPI()
        {
            // count the object creation
            Utils.CountAndAlert.Count(Utils.CountAndAlert.PredefAlertTypes.AddedOrCreated);
        }

        /// <summary>
        /// Return if this material support dynamic lighting.
        /// </summary>
        virtual public bool LightingEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Get how many lights this material support on the same render pass.
        /// </summary>
        virtual protected int MaxLights
        {
            get { return 7; }
        }

        /// <summary>
        /// Diffuse color.
        /// </summary>
        virtual public Color DiffuseColor
        {
            get { return _diffuseColor; }
            set { _diffuseColor = value; SetAsDirty(MaterialDirtyFlags.MaterialColors); }
        }
        Color _diffuseColor;

        /// <summary>
        /// Specular color.
        /// </summary>
        virtual public Color SpecularColor
        {
            get { return _specularColor; }
            set { _specularColor = value; SetAsDirty(MaterialDirtyFlags.MaterialColors); }
        }
        Color _specularColor;

        /// <summary>
        /// Ambient light color.
        /// </summary>
        virtual public Color AmbientLight
        {
            get { return _ambientLight; }
            set { _ambientLight = value; SetAsDirty(MaterialDirtyFlags.AmbientLight); }
        }
        Color _ambientLight;

        /// <summary>
        /// Emissive light color.
        /// </summary>
        virtual public Color EmissiveLight
        {
            get { return _emissiveLight; }
            set { _emissiveLight = value; SetAsDirty(MaterialDirtyFlags.EmissiveLight); }
        }
        Color _emissiveLight;

        /// <summary>
        /// Specular power.
        /// </summary>
        virtual public float SpecularPower
        {
            get { return _specularPower; }
            set { _specularPower = value; SetAsDirty(MaterialDirtyFlags.MaterialColors); }
        }
        float _specularPower;

        /// <summary>
        /// Opacity levels (multiplied with color opacity).
        /// </summary>
        virtual public float Alpha
        {
            get { return _alpha; }
            set { _alpha = value; SetAsDirty(MaterialDirtyFlags.Alpha); }
        }
        float _alpha;

        /// <summary>
        /// Texture to draw.
        /// </summary>
        virtual public Texture2D Texture
        { 
                get { return _texture; }
                set { _texture = value; SetAsDirty(MaterialDirtyFlags.TextureParams); }
        }
        Texture2D _texture;

        /// <summary>
        /// Is texture currently enabled.
        /// </summary>
        virtual public bool TextureEnabled
        {
            get { return _textureEnabled; }
            set { _textureEnabled = value; SetAsDirty(MaterialDirtyFlags.TextureParams); }
        }
        bool _textureEnabled;

        /// <summary>
        /// Current world transformations.
        /// </summary>
        virtual public Matrix World
        {
            get { return _world; }
            set { _world = value; SetAsDirty(MaterialDirtyFlags.World); }
        }
        Matrix _world;

        // current view matrix (shared by all materials)
        static Matrix _view;

        // current projection matrix (shared by all materials)
        static Matrix _projection;

        // view-projection matrix (multiply of view and projection)
        static Matrix _viewProjection;

        /// <summary>
        /// Current view matrix.
        /// </summary>
        virtual public Matrix View { get { return _view; } }

        /// <summary>
        /// Current projection matrix.
        /// </summary>
        virtual public Matrix Projection { get { return _projection; } }

        /// <summary>
        /// Current view-projection matrix.
        /// </summary>
        virtual public Matrix ViewProjection { get { return _viewProjection; } }

        // current view matrix version, used so we'll only update materials view when needed.
        static uint _globalViewMatrixVersion = 1;

        // current projection matrix version, used so we'll only update materials projection when needed.
        static uint _globalProjectionMatrixVersion = 1;

        // local view matrix version
        uint _viewMatrixVersion = 0;

        // local projection matrix version
        uint _projectionMatrixVersion = 0;

        /// <summary>
        /// Default sampler state.
        /// </summary>
        public static SamplerState DefaultSamplerState = SamplerState.LinearWrap;

        /// <summary>
        /// Sampler state.
        /// </summary>
        public SamplerState SamplerState = DefaultSamplerState;

        /// <summary>
        /// If true, will use the currently set lights manager in `Graphics.GraphicsManager.LightsManager` and call ApplyLights() with the lights from manager.
        /// </summary>
        protected virtual bool UseDefaultLightsManager { get { return false; } }

        /// <summary>
        /// Add to dirty flags.
        /// </summary>
        /// <param name="val">Value to add to dirty flags using the or operator.</param>
        protected void SetAsDirty(int val)
        {
            _dirtyFlags |= val;
        }

        /// <summary>
        /// Add to dirty flags.
        /// </summary>
        /// <param name="val">Value to add to dirty flags using the or operator.</param>
        protected void SetAsDirty(MaterialDirtyFlags val)
        {
            _dirtyFlags |= (int)val;
        }

        /// <summary>
        /// Check dirty flags.
        /// </summary>
        /// <param name="val">Value to test if dirty.</param>
        protected bool IsDirty(int val)
        {
            return (_dirtyFlags & (int)val) != 0;
        }

        /// <summary>
        /// Add to dirty flags.
        /// </summary>
        /// <param name="val">Value to add to dirty flags using the or operator.</param>
        protected bool IsDirty(MaterialDirtyFlags val)
        {
            return (_dirtyFlags & (int)val) != 0;
        }

        /// <summary>
        /// Set materials view and projection matrixes (shared by all materials).
        /// </summary>
        /// <param name="view">Current view matrix.</param>
        /// <param name="projection">Current projection matrix.</param>
        public static void SetViewProjection(Matrix view, Matrix projection)
        {
            // update view
            if (_view != view)
            {
                _view = view;
                _globalViewMatrixVersion++;
            }

            // update projection
            if (_projection != projection)
            {
                _projection = projection;
                _globalProjectionMatrixVersion++;
            }

            // update view/projection matrix
            _viewProjection = _view *_projection;
        }

        /// <summary>
        /// Clone this material.
        /// </summary>
        /// <returns>Copy of this material.</returns>
        abstract public MaterialAPI Clone();

        /// <summary>
        /// Apply sampler state of this material.
        /// </summary>
        protected virtual void ApplySamplerState()
        {
            var states = GraphicsManager.GraphicsDevice.SamplerStates;
            for (int i = 0; i < SamplersCount; ++i)
                if (states[i] != SamplerState) states[i] = SamplerState;
        }

        /// <summary>
        /// Apply all new properties on the material effect.
        /// Call this whenever you want to draw using this material.
        /// </summary>
        /// <param name="worldMatrix">The world transformations of the currently rendered entity.</param>
        /// <param name="boundingSphere">The bounding sphere (should be already transformed) of the rendered entity.</param>
        public void Apply(ref Matrix worldMatrix, ref BoundingSphere boundingSphere)
        {
            // set world matrix
            World = worldMatrix;

            // apply sampler state
            ApplySamplerState();

            // update view if needed
            if (_viewMatrixVersion != _globalViewMatrixVersion)
            {
                UpdateView(ref _view);
                _viewMatrixVersion = _globalViewMatrixVersion;
            }

            // update projection if needed
            if (_projectionMatrixVersion != _globalProjectionMatrixVersion)
            {
                UpdateProjection(ref _projection);
                _projectionMatrixVersion = _globalProjectionMatrixVersion;
            }

            // if support light get lights and set them
            if (LightingEnabled && UseDefaultLightsManager)
            {
                // get lights in rendering range
                var lightsManager = GraphicsManager.ActiveLightsManager;
                var lights = lightsManager.GetLights(this, ref boundingSphere, MaxLights);
                AmbientLight = lightsManager.AmbientLight;
                ApplyLights(lights, ref worldMatrix, ref boundingSphere);
            }

            // set effect tag to point on self, and call the per-effect specific apply
            if (Effect.Tag == null) { Effect.Tag = this; }
            MaterialSpecificApply(_lastMaterialApplied == this);

            // set last material applied to self
            _lastMaterialApplied = this;

            // clear flags
            _dirtyFlags = 0;
        }

        /// <summary>
        /// Apply light sources on this material.
        /// </summary>
        /// <param name="lights">Array of light sources to apply.</param>
        /// <param name="worldMatrix">World transforms of the rendering object.</param>
        /// <param name="boundingSphere">Bounding sphere (after world transformation applied) of the rendering object.</param>
        virtual protected void ApplyLights(Lights.LightSource[] lights, ref Matrix worldMatrix, ref BoundingSphere boundingSphere)
        {
        }

        /// <summary>
        /// Apply all new properties on the material effect, implemented per material type.
        /// </summary>
        /// <param name="wasLastMaterial">If true, it means this material was the last material applied. Useful for internal optimizations.</param>
        abstract protected void MaterialSpecificApply(bool wasLastMaterial);

        /// <summary>
        /// Update material view matrix.
        /// </summary>
        /// <param name="view">New view to set.</param>
        abstract protected void UpdateView(ref Matrix view);

        /// <summary>
        /// Update material projection matrix.
        /// </summary>
        /// <param name="projection">New projection to set.</param>
        abstract protected void UpdateProjection(ref Matrix projection);

        /// <summary>
        /// Set bone transforms for an animated material.
        /// Useable only for materials that implement skinned animation in shader.
        /// </summary>
        /// <param name="bones">Bones to set.</param>
        virtual public void SetBoneTransforms(Matrix[] bones)
        {
            throw new Exceptions.InvalidActionException("Material does not support bone transformations in GPU.");
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
            cloned.DiffuseColor = DiffuseColor;
            cloned.SpecularColor = SpecularColor;
            cloned.SpecularPower = SpecularPower;
            cloned.AmbientLight = AmbientLight;
            cloned.EmissiveLight = EmissiveLight;
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
            DiffuseColor = Color.White;
            SpecularColor = Color.White;
            EmissiveLight = Color.Black;
            SpecularPower = 1f;
            AmbientLight = Color.White;
            SamplerState = DefaultSamplerState;
        }
    }
}

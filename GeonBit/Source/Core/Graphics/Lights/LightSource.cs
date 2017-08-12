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
// Basic light source entity.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;


namespace GeonBit.Core.Graphics.Lights
{
    /// <summary>
    /// Basic light source entity.
    /// </summary>
    public class LightSource
    {
        /// <summary>
        /// Is this light source currently visible?
        /// </summary>
        public bool Visible = true;

        // is light dirty / changed since last time we fetched it as an array?
        bool _isFloatBufferDirty = true;

        // cached light data as an array of floats.
        float[] _asFloatBuffer;

        /// <summary>
        /// Parent lights manager.
        /// </summary>
        public ILightsManager LightsManager = null;

        /// <summary>
        /// Light bounding sphere.
        /// </summary>
        public BoundingSphere BoundingSphere { get; protected set; }

        /// <summary>
        /// Return if this light source is infinite, eg has no range and reach anywhere (like a directional light).
        /// </summary>
        virtual public bool IsInfinite
        {
            get { return Direction != null; }
        }

        /// <summary>
        /// Light direction, if its a directional light.
        /// </summary>
        Vector3? Direction
        {
            get { return _direction; }
            set { _direction = value; RecalcBoundingSphere(); }
        }
        private Vector3? _direction = null;

        /// <summary>
        /// Light range.
        /// </summary>
        public float Range
        {
            get { return _range; }
            set { _range = value; RecalcBoundingSphere(); }
        }
        private float _range = 100f;

        /// <summary>
        /// Light color and strength (A field = light strength).
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _isFloatBufferDirty = true; _color = value; }
        }
        Color _color = Color.White;

        /// <summary>
        /// Light Intensity (equivilent to Color.A).
        /// </summary>
        public float Intensity
        {
            get { return _intensity; }
            set { _isFloatBufferDirty = true; _intensity = value; }
        }
        float _intensity = 1f;

        /// <summary>
        /// Specular factor.
        /// </summary>
        public float Specular
        {
            get { return _specular; }
            set { _isFloatBufferDirty = true; _specular = value; }
        }
        float _specular = 1f;

        /// <summary>
        /// Last light known transform.
        /// </summary>
        private Matrix _transform;

        /// <summary>
        /// Get the source light as a buffer of floats, in the following format:
        /// float3 Color, float3 Position, float Intensity, float Range, float specular.
        /// Note: if its a directional light, we'll get Direction instead of Position.
        /// </summary>
        virtual public float[] GetFloatBuffer()
        {
            // if not dirty, return from cache
            if (!_isFloatBufferDirty) { return _asFloatBuffer; }

            // if dirty we need to rebuild float buffer

            // get color as vector
            var color = Color.ToVector3();

            // get position / direction
            Vector3 pos = Direction ?? Position;

            // create data buffer
            _asFloatBuffer = new float[] {
                color.X, color.Y, color.Z,
                pos.X, pos.Y, pos.Z,
                Intensity, Range, Specular};

            // no longer dirty
            _isFloatBufferDirty = false;

            // return array
            return _asFloatBuffer;
        }

        /// <summary>
        /// Get light position in world space.
        /// </summary>
        Vector3 Position
        {
            get { return BoundingSphere.Center; }
        }

        /// <summary>
        /// Update light transformations.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public virtual void UpdateTransforms(ref Matrix worldTransformations)
        {
            // if didn't really change skip
            if (_transform == worldTransformations) { return; }

            // set transformations and recalc bounding sphere
            _transform = worldTransformations;
            RecalcBoundingSphere();
        }

        /// <summary>
        /// Recalculate light bounding sphere after transformations or radius change.
        /// </summary>
        protected virtual void RecalcBoundingSphere()
        {
            // buffer is dirty
            _isFloatBufferDirty = true;

            // calc light bounding sphere
            Vector3 scale; Vector3 position; Quaternion rotation;
            _transform.Decompose(out scale, out rotation, out position);
            BoundingSphere = new BoundingSphere(position, _range * scale.Length());

            // notify manager on update
            if (LightsManager != null) { LightsManager.UpdateLightTransform(this); }
        }
    }
}

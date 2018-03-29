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

        /// <summary>
        /// Parent lights manager.
        /// </summary>
        internal ILightsManager LightsManager = null;

        /// <summary>
        /// So we can cache lights and identify when they were changed.
        /// </summary>
        public uint ParamsVersion { get; private set; } = 1;

        /// <summary>
        /// Light bounding sphere.
        /// </summary>
        public BoundingSphere BoundingSphere { get; protected set; }

        /// <summary>
        /// Return if this light source is infinite, eg has no range and reach anywhere (like a directional light).
        /// </summary>
        virtual public bool IsInfinite
        {
            get { return Direction != null || _range == 0f; }
        }

        /// <summary>
        /// Return if this light is a directional light.
        /// </summary>
        virtual public bool IsDirectionalLight
        {
            get { return Direction != null; }
        }

        /// <summary>
        /// Light direction, if its a directional light.
        /// </summary>
        public Vector3? Direction
        {
            get { return _direction; }
            set { if (_direction == value) return; _direction = value; ParamsVersion++; RecalcBoundingSphere(); }
        }
        private Vector3? _direction = null;

        /// <summary>
        /// Light range.
        /// </summary>
        public float Range
        {
            get { return _range; }
            set { if (_range == value) return; _range = value; ParamsVersion++; RecalcBoundingSphere(); }
        }
        private float _range = 100f;

        /// <summary>
        /// Light position in world space.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set { if (_position == value) return; _position = value; ParamsVersion++; RecalcBoundingSphere(); }
        }
        Vector3 _position = Vector3.Zero;

        /// <summary>
        /// Light color and strength (A field = light strength).
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { if (_color == value) return; _color = value; ParamsVersion++; }
        }
        Color _color = Color.White;

        /// <summary>
        /// Light Intensity (equivilent to Color.A).
        /// </summary>
        public float Intensity
        {
            get { return _intensity; }
            set { if (_intensity == value) return; _intensity = value; ParamsVersion++; }
        }
        float _intensity = 1f;

        /// <summary>
        /// Specular factor.
        /// </summary>
        public float Specular
        {
            get { return _specular; }
            set { if (_specular == value) return; _specular = value; ParamsVersion++; }
        }
        float _specular = 1f;

        /// <summary>
        /// Last light known transform.
        /// </summary>
        private Matrix _transform;

        /// <summary>
        /// Remove self from parent lights manager.
        /// </summary>
        public void Remove()
        {
            if (LightsManager != null)
            {
                LightsManager.RemoveLight(this);
            }
        }

        /// <summary>
        /// Create the light source.
        /// </summary>
        public LightSource()
        {
            // count the object creation
            Utils.CountAndAlert.Count(Utils.CountAndAlert.PredefAlertTypes.AddedOrCreated);
        }

        /// <summary>
        /// Update light transformations.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public virtual void UpdateTransforms(ref Matrix worldTransformations)
        {
            // if didn't really change skip
            if (_transform == worldTransformations) { return; }

            // break transformation into components
            Vector3 scale; Vector3 position; Quaternion rotation;
            _transform.Decompose(out scale, out rotation, out position);

            // set world position. this will also recalc bounding sphere and update lights manager, if needed.
            Position = position;
        }

        /// <summary>
        /// Recalculate light bounding sphere after transformations or radius change.
        /// </summary>
        /// <param name="updateInLightsManager">If true, will also update light position in lights manager.</param>
        public virtual void RecalcBoundingSphere(bool updateInLightsManager = true)
        {
            // calc light bounding sphere
            BoundingSphere = new BoundingSphere(Position, _range);

            // notify manager on update
            if (updateInLightsManager && LightsManager != null)
            {
                LightsManager.UpdateLightTransform(this);
            }
        }
    }
}

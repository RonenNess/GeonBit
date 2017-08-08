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
        public ILightsManager LightsManager = null;

        /// <summary>
        /// Light bounding sphere.
        /// </summary>
        public BoundingSphere BoundingSphere { get; protected set; }

        // light radius value.
        private float _radius = 100f;

        /// <summary>
        /// Light radius.
        /// </summary>
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; RecalcBoundingSphere(); }
        }

        /// <summary>
        /// Light color and strength (A field = light strength).
        /// </summary>
        public Color Color = Color.White;

        /// <summary>
        /// Optional light direction, if its a directional light.
        /// </summary>
        public Vector3? Direction = null;

        /// <summary>
        /// Light Intensity (equivilent to Color.A).
        /// </summary>
        public byte Intensity
        {
            get { return Color.A; }
            set { Color.A = value; }
        }

        /// <summary>
        /// Last light transform.
        /// </summary>
        private Matrix _transform;

        /// <summary>
        /// Specular factor.
        /// </summary>
        public float Specular = 1f;

        /// <summary>
        /// Update light transformations.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public virtual void UpdateTransforms(ref Matrix worldTransformations)
        {
            _transform = worldTransformations;
            RecalcBoundingSphere();
        }

        /// <summary>
        /// Recalculate light bounding sphere after transformations or radius change.
        /// </summary>
        protected virtual void RecalcBoundingSphere()
        { 
            // calc light bounding sphere
            Vector3 scale; Vector3 position; Quaternion rotation;
            _transform.Decompose(out scale, out rotation, out position);
            BoundingSphere = new BoundingSphere(position, scale.Length());

            // notify manager on update
            if (LightsManager != null) { LightsManager.UpdateLightTransform(this); }
        }
    }
}

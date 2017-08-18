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
// A light source component.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.ECS.Components.Graphics
{
    /// <summary>
    /// This component implements a light source.
    /// </summary>
    public class Light : BaseComponent
    {
        // the core light source.
        Core.Graphics.Lights.LightSource _light;

        /// <summary>
        /// Light direction, if its a directional light.
        /// </summary>
        public Vector3? Direction
        {
            get { return _light.Direction; }
            set { _light.Direction = value; }
        }

        /// <summary>
        /// Return if this light source is infinite, eg has no range and reach anywhere (like a directional light).
        /// </summary>
        virtual public bool IsInfinite
        {
            get { return _light.IsInfinite; }
        }

        /// <summary>
        /// Return if this light is a directional light.
        /// </summary>
        public bool IsDirectionalLight
        {
            get { return _light.IsDirectionalLight; }
        }

        /// <summary>
        /// Light range.
        /// </summary>
        public float Range
        {
            get { return _light.Range; }
            set { _light.Range = value; }
        }

        /// <summary>
        /// Light color and strength (A field = light strength).
        /// </summary>
        public Color Color
        {
            get { return _light.Color; }
            set { _light.Color = value; }
        }

        /// <summary>
        /// Light Intensity (equivilent to Color.A).
        /// </summary>
        public float Intensity
        {
            get { return _light.Intensity; }
            set { _light.Intensity = value; }
        }

        /// <summary>
        /// Specular factor.
        /// </summary>
        public float Specular
        {
            get { return _light.Specular; }
            set { _light.Specular = value; }
        }

        /// <summary>
        /// Create the light component.
        /// </summary>
        public Light()
        {
            // create the light source
            _light = new Core.Graphics.Lights.LightSource();
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            Light ret = new Light();
            ret.Intensity = Intensity;
            ret.Specular = Specular;
            ret.Color = Color;
            ret.Direction = Direction;
            ret.Range = Range;
            CopyBasics(ret);
            return ret;
        }

        /// <summary>
        /// Called when GameObject turned disabled.
        /// </summary>
        protected override void OnDisabled()
        {
            _light.Visible = false;
        }

        /// <summary>
        /// Called when GameObject is enabled.
        /// </summary>
        protected override void OnEnabled()
        {
            _light.Visible = true;
        }

        /// <summary>
        /// Called every time scene node transformation updates.
        /// Note: this is called only if GameObject is enabled and have Update events enabled.
        /// </summary>
        protected override void OnTransformationUpdate()
        {
            if (!_light.IsInfinite) { _light.Position = _GameObject.SceneNode.WorldPosition; }
        }

        /// <summary>
        /// Called when this component is effectively removed from scene, eg when removed
        /// from a GameObject or when its GameObject is removed from scene.
        /// </summary>
        protected override void OnRemoveFromScene()
        {
            _light.Remove();
        }

        /// <summary>
        /// Called when this component is effectively added to scene, eg when added
        /// to a GameObject currently in scene or when its GameObject is added to scene.
        /// </summary>
        protected override void OnAddToScene()
        {
            _GameObject.ParentScene.Lights.AddLight(_light);
        }
    }
}

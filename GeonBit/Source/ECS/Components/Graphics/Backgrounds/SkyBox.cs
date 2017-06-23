#region LICENSE
/**
 * For the purpose of making video games only, GeonBit is distributed under the MIT license.
 * to use this source code or GeonBit as a whole for any other purpose, please seek written 
 * permission from the library author.
 * 
 * Copyright (c) 2017 Ronen Ness [ronenness@gmail.com].
 * You may not remove this license notice.
 */
#endregion
#region File Description
//-----------------------------------------------------------------------------
// A component that renders a 3D skybox.
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
    /// This component renders a 3d skybox.
    /// </summary>
    public class SkyBox : ModelRenderer
    {
        /// <summary>
        /// Skybox texture path.
        /// </summary>
        public string TexturePath { get; private set; }

        /// <summary>
        /// Default skybox texture.
        /// </summary>
        public static string DefaultTexture = "GeonBit.Core/Textures/Skybox";

        /// <summary>
        /// Create the skybox renderer component.
        /// </summary>
        /// <param name="texture">Skybox texture path (leave null for default texture).</param>
        public SkyBox(string texture = null) : base(ShapeRenderer.ShapeModelsRoot + "Skybox")
        {
            TexturePath = texture ?? DefaultTexture;
            _entity.RenderingQueue = Core.Graphics.RenderingQueue.BackgroundNoCull;
            _entity.SetMaterial(new Core.Graphics.Materials.SkyboxMaterial(TexturePath, true));
            _entity.OverrideWorldMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            SkyBox ret = CopyBasics(new SkyBox(TexturePath)) as SkyBox;
            return ret;
        }

        /// <summary>
        /// Called when this component spawns.
        /// </summary>
        protected override void OnSpawn()
        {
            _GameObject.SceneNode.DisableCulling = true;
        }
    }
}

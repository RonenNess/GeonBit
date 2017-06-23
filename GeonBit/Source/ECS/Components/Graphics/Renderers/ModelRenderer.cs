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
// A component that renders a 3D model.
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
    /// This component renders a 3D model.
    /// </summary>
    public class ModelRenderer : BaseRendererComponent
    {
        /// <summary>
        /// The entity from the core layer used to draw the model.
        /// </summary>
        protected Core.Graphics.ModelEntity _entity;

        /// <summary>
        /// Override material default settings for this specific model instance.
        /// </summary>
        public Core.Graphics.MaterialOverrides MaterialOverride
        {
            get { return _entity.MaterialOverride; }
            set { _entity.MaterialOverride = value; }
        }

        /// <summary>
        /// Entity blending state.
        /// </summary>
        public BlendState BlendingState
        {
            set { _entity.BlendingState = value; }
            get { return _entity.BlendingState; }
        }

        /// <summary>
        /// Set alternative material for a specific mesh id.
        /// </summary>
        /// <param name="material">Material to set.</param>
        /// <param name="meshId">Mesh name. If empty string is provided, this material will be used for all meshes.</param>
        public void SetMaterial(Core.Graphics.Materials.MaterialAPI material, string meshId = "")
        {
            _entity.SetMaterial(material, meshId);
        }

        /// <summary>
        /// Set alternative materials for a specific mesh id.
        /// </summary>
        /// <param name="material">Materials to set.</param>
        /// <param name="meshId">Mesh name. If empty string is provided, this material will be used for all meshes.</param>
        public void SetMaterials(Core.Graphics.Materials.MaterialAPI[] material, string meshId = "")
        {
            _entity.SetMaterials(material, meshId);
        }

        /// <summary>
        /// Get the main entity instance of this renderer.
        /// </summary>
        protected override Core.Graphics.BaseRenderableEntity Entity { get { return _entity; } }

        /// <summary>
        /// Protected constructor without params to use without creating entity, for inheriting classes.
        /// </summary>
        protected ModelRenderer()
        {
        }

        /// <summary>
        /// Create the model renderer component.
        /// </summary>
        /// <param name="model">Model to draw.</param>
        public ModelRenderer(Model model)
        {
            _entity = new Core.Graphics.ModelEntity(model);
        }

        /// <summary>
        /// Create the model renderer component.
        /// </summary>
        /// <param name="model">Path of the model asset to draw.</param>
        public ModelRenderer(string model) : this(Resources.GetModel(model))
        {
        }

        /// <summary>
        /// Copy basic properties to another component (helper function to help with Cloning).
        /// </summary>
        /// <param name="copyTo">Other component to copy values to.</param>
        /// <returns>The object we are copying properties to.</returns>
        protected override BaseComponent CopyBasics(BaseComponent copyTo)
        {
            ModelRenderer other = copyTo as ModelRenderer;
            other.MaterialOverride = MaterialOverride.Clone();
            other.BlendingState = BlendingState;
            other._entity.CopyMaterials(other._entity.OverrideMaterialsDictionary);
            return base.CopyBasics(other);
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            ModelRenderer ret = new ModelRenderer(_entity.Model);
            CopyBasics(ret);
            return ret;
        }
    }
}

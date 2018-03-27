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
    public class ModelRenderer : BaseRendererWithOverrideMaterial
    {
        /// <summary>
        /// The entity from the core layer used to draw the model.
        /// </summary>
        protected Core.Graphics.ModelEntity _entity;

        /// <summary>
        /// Get renderer model.
        /// </summary>
        public Model Model { get { return _entity.Model; } }

        /// <summary>
        /// Override material default settings for this specific model instance.
        /// </summary>
        public override Core.Graphics.MaterialOverrides MaterialOverride
        {
            get { return _entity.MaterialOverride; }
            set { _entity.MaterialOverride = value; }
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
        /// Get material for a given mesh id and part index.
        /// </summary>
        /// <param name="meshId">Mesh id to get material for.</param>
        /// <param name="meshPartIndex">MeshPart index to get material for.</param>
        public Core.Graphics.Materials.MaterialAPI GetMaterial(string meshId, int meshPartIndex = 0)
        {
            return _entity.GetMaterial(meshId, meshPartIndex);
        }

        /// <summary>
        /// Get the first material used in this renderer.
        /// </summary>
        public Core.Graphics.Materials.MaterialAPI GetFirstMaterial()
        {
            return _entity.GetFirstMaterial();
        }

        /// <summary>
        /// Return a list with all materials in model.
        /// Note: if alternative materials are set, will return them.
        /// Note2: prevent duplications, eg if even if more than one part uses the same material it will only return it once.
        /// </summary>
        /// <returns>List of materials.</returns>
        public System.Collections.Generic.List<Core.Graphics.Materials.MaterialAPI> GetMaterials()
        {
            return _entity.GetMaterials();
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
            other._entity.CopyMaterials(_entity.OverrideMaterialsDictionary);
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

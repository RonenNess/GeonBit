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
// A component that renders a mesh from a 3D model.
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
    /// This component renders a a specific mesh from a 3D model.
    /// </summary>
    public class ModelMeshRenderer : BaseRendererWithOverrideMaterial
    {
        /// <summary>
        /// The entity from the core layer used to draw the model mesh.
        /// </summary>
        protected Core.Graphics.MeshEntity _entity;

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
        public void SetMaterial(Core.Graphics.Materials.MaterialAPI material)
        {
            _entity.SetMaterial(material);
        }

        /// <summary>
        /// Set alternative materials for a specific mesh id.
        /// </summary>
        /// <param name="material">Materials to set.</param>
        public void SetMaterials(Core.Graphics.Materials.MaterialAPI[] material)
        {
            _entity.SetMaterials(material);
        }

        /// <summary>
        /// Get the main entity instance of this renderer.
        /// </summary>
        protected override Core.Graphics.BaseRenderableEntity Entity { get { return _entity; } }

        /// <summary>
        /// Protected constructor without params to use without creating entity, for inheriting classes.
        /// </summary>
        protected ModelMeshRenderer()
        {
        }

        /// <summary>
        /// Create the model renderer component.
        /// </summary>
        /// <param name="model">Model to draw.</param>
        /// <param name="mesh">Mesh to draw.</param>
        public ModelMeshRenderer(Model model, ModelMesh mesh)
        {
            _entity = new Core.Graphics.MeshEntity(model, mesh);
        }

        /// <summary>
        /// Create the mesh renderer component.
        /// </summary>
        /// <param name="model">Path of the model asset to draw.</param>
        /// <param name="meshName">Which mesh to draw from model.</param>
        public ModelMeshRenderer(string model, string meshName)
        {
            Model modelInstance = Resources.GetModel(model);
            ModelMesh mesh = modelInstance.Meshes[meshName];
            _entity = new Core.Graphics.MeshEntity(modelInstance, mesh);
        }

        /// <summary>
        /// Create the mesh renderer component.
        /// </summary>
        /// <param name="model">Path of the model asset to draw.</param>
        /// <param name="meshIndex">Which mesh to draw from model.</param>
        public ModelMeshRenderer(string model, int meshIndex)
        {
            Model modelInstance = Resources.GetModel(model);
            ModelMesh mesh = modelInstance.Meshes[meshIndex];
            _entity = new Core.Graphics.MeshEntity(modelInstance, mesh);
        }

        /// <summary>
        /// Copy basic properties to another component (helper function to help with Cloning).
        /// </summary>
        /// <param name="copyTo">Other component to copy values to.</param>
        /// <returns>The object we are copying properties to.</returns>
        protected override BaseComponent CopyBasics(BaseComponent copyTo)
        {
            ModelMeshRenderer other = copyTo as ModelMeshRenderer;
            other.MaterialOverride = MaterialOverride.Clone();
            other._entity.SetMaterials(_entity.OverrideMaterials);
            return base.CopyBasics(other);
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            ModelMeshRenderer ret = new ModelMeshRenderer(_entity.Model, _entity.Mesh);
            CopyBasics(ret);
            return ret;
        }
    }
}

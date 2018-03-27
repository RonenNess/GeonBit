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
// A component that renders a collection of meshes combined together into a static mesh.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeonBit.Core.Graphics;
using GeonBit.Core.Graphics.Materials;

namespace GeonBit.ECS.Components.Graphics
{
    /// <summary>
    /// This component combine together several meshes into a single static mesh.
    /// It reduceses draw calls and optimize renderings.
    /// Use this for stuff like building rooms, levels, etc. Anything made of multiple static meshes.
    /// </summary>
    public class CombinedMeshesRenderer<VertexType> : BaseRendererComponent where VertexType : struct, IVertexType
    {
        /// <summary>
        /// The entity from the core layer used to draw the model.
        /// </summary>
        protected CombinedMeshesEntity<VertexType> _entity;

        /// <summary>
        /// Get the main entity instance of this renderer.
        /// </summary>
        protected override BaseRenderableEntity Entity { get { return _entity; } }

        /// <summary>
        /// Build the combined meshes.
        /// </summary>
        public void Build()
        {
            _entity.Build();
            if (_GameObject != null) _GameObject.SceneNode.ForceFullUpdate(false);
        }

        /// <summary>
        /// Add a model from a model renderer component.
        /// Will use its parent game object world transformations and material.
        /// </summary>
        /// <param name="renderer">Renderer component to add.</param>
        /// <param name="removeAfterAdd">If true, will remove the renderer component from its parent once done.</param>
        public void AddModelRenderer(ModelRenderer renderer, bool removeAfterAdd)
        {
            // add model to combined mesh
            renderer._GameObject.SceneNode.ForceFullUpdate(true);
            AddModel(renderer.Model, renderer._GameObject.SceneNode.WorldTransformations, renderer.GetFirstMaterial());

            // if needed, remove form parent.
            if (removeAfterAdd)
            {
                renderer.RemoveFromParent();
            }
        }

        /// <summary>
        /// Add a model to the combined mesh.
        /// Note: will not take effect until 'Build()' is called.
        /// </summary>
        /// <param name="model">Model to add.</param>
        /// <param name="transform">World transformations.</param>
        /// <param name="material">Optional material to use instead of the model default materials.</param>
        public void AddModel(Model model, Matrix transform, MaterialAPI material = null)
        {
            _entity.AddModel(model, transform, material);
        }

        /// <summary>
        /// Add a model mesh to the combined mesh.
        /// Note: will not take effect until 'Build()' is called.
        /// </summary>
        /// <param name="mesh">Mesh to add.</param>
        /// <param name="transform">World transformations.</param>
        /// <param name="material">Optional material to use instead of the mesh default materials.</param>
        public void AddModelMesh(ModelMesh mesh, Matrix transform, MaterialAPI material = null)
        {
            _entity.AddModelMesh(mesh, transform, material);
        }

        /// <summary>
        /// Add array of vertices to the combined mesh.
        /// Note: will not take effect until 'Build()' is called.
        /// </summary>
        /// <param name="vertices">Vertices array to add.</param>
        /// <param name="indexes">Draw order / indexes array.</param>
        /// <param name="material">Material to use with the vertices.</param>
        public void AddVertices(VertexType[] vertices, ushort[] indexes, MaterialAPI material)
        {
            _entity.AddVertices(vertices, indexes, material);
        }

        /// <summary>
        /// Add array of vertices to the combined mesh.
        /// Note: will not take effect until 'Build()' is called.
        /// </summary>
        /// <param name="vertices">Vertices array to add.</param>
        /// <param name="indexes">Draw order / indexes array.</param>
        /// <param name="transform">World transformations.</param>
        /// <param name="material">Material to use with the vertices.</param>
        public void AddVertices(VertexType[] vertices, ushort[] indexes, Matrix transform, MaterialAPI material)
        {
            _entity.AddVertices(vertices, indexes, transform, material);
        }

        /// <summary>
        /// Clear everything from the combined meshes renderer.
        /// </summary>
        public void Clear()
        {
            _entity.Clear();
            if (_GameObject != null) _GameObject.SceneNode.ForceFullUpdate(false);
        }

        /// <summary>
        /// Create the Combined Meshes Renderer component.
        /// </summary>
        public CombinedMeshesRenderer()
        {
            _entity = new CombinedMeshesEntity<VertexType>();
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            CombinedMeshesRenderer<VertexType> ret = new CombinedMeshesRenderer<VertexType>();
            ret._entity = _entity.Clone();
            return ret;
        }
    }
}

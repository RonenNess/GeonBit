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
// A composite renderable model, made of meshes.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace GeonBit.Core.Graphics
{

    /// <summary>
    /// A renderable model, made of multiple mesh renderers.
    /// This type of model is slightly slower than the SimpleModelEntity and ModelEntity, but has the following advantages:
    /// 1. finer-grain control over parts of the model.
    /// 2. proper camera-distance sorting if the model contains both opaque and transparent parts.
    /// </summary>
    public class CompositeModelEntity : BaseRenderableEntity
    {
        /// <summary>
        /// Model to render.
        /// </summary>
        public Model Model
        {
            get; protected set;
        }

        /// <summary>
        /// Dictionary with all the mesh entities.
        /// </summary>
        protected OrderedDictionary _meshes = new OrderedDictionary();
        
        /// <summary>
        /// Create the model entity from model instance.
        /// </summary>
        /// <param name="model">Model to draw.</param>
        public CompositeModelEntity(Model model)
        {
            Model = model;
            foreach (var mesh in Model.Meshes)
            {
                _meshes[mesh.Name] = new MeshEntity(model, mesh);
            }
        }

        /// <summary>
        /// Return meshes count.
        /// </summary>
        public int MeshesCount
        {
            get { return _meshes.Count; }
        }

        /// <summary>
        /// Get mesh entity by index.
        /// </summary>
        /// <param name="index">Mesh index to get.</param>
        /// <returns>MeshEntity instance for this mesh.</returns>
        public MeshEntity GetMesh(int index)
        {
            return _meshes[index] as MeshEntity;
        }

        /// <summary>
        /// Get mesh entity by name.
        /// </summary>
        /// <param name="name">Mesh name to get.</param>
        /// <returns>MeshEntity instance for this mesh.</returns>
        public MeshEntity GetMesh(string name)
        {
            return _meshes[name] as MeshEntity;
        }

        /// <summary>
        /// Get all meshes in this composite model.
        /// </summary>
        /// <returns></returns>
        public List<MeshEntity> GetMeshes()
        {
            List<MeshEntity> ret = new List<MeshEntity>(_meshes.Values.Count);
            foreach (var mesh in _meshes.Values)
            {
                ret.Add(mesh as MeshEntity);
            }
            return ret;
        }

        /// <summary>
        /// Return a list with all materials in model.
        /// Note: if alternative materials are set, will return them.
        /// Note2: prevent duplications, eg if even if more than one part uses the same material it will only return it once.
        /// </summary>
        /// <returns>List of materials.</returns>
        public List<Materials.MaterialAPI> GetMaterials()
        {
            List<Materials.MaterialAPI> ret = new List<Materials.MaterialAPI>();
            foreach (DictionaryEntry entry in _meshes)
            {
                MeshEntity mesh = entry.Value as MeshEntity;
                for (int i = 0; i < mesh.Mesh.MeshParts.Count; ++i)
                {
                    Materials.MaterialAPI material = mesh.GetMaterial(i);
                    if (!ret.Contains(material))
                    {
                        ret.Add(material);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Draw this entity.
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void Draw(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            // not visible / no active camera? skip
            if (!Visible || GraphicsManager.ActiveCamera == null)
            {
                return;
            }

            // call draw callback
            OnDraw?.Invoke(this);

            // draw all meshes
            foreach (DictionaryEntry mesh in _meshes)
            {
                GraphicsManager.DrawEntity(mesh.Value as MeshEntity, worldTransformations);
            }
        }

        /// <summary>
        /// Create the model entity from asset path.
        /// </summary>
        /// <param name="path">Path of the model to load.</param>
        public CompositeModelEntity(string path) : this(ResourcesManager.Instance.GetModel(path))
        {
        }

        /// <summary>
        /// Draw this model.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void DoEntityDraw(ref Matrix worldTransformations)
        {
        }

        /// <summary>
        /// Get the bounding sphere of this entity.
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding box of the entity.</returns>
        protected override BoundingSphere CalcBoundingSphere(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            BoundingSphere modelBoundingSphere = ModelUtils.GetBoundingSphere(Model);
            Vector3 scale = Utils.ExtendedMath.GetScale(ref worldTransformations);
            modelBoundingSphere.Radius *= scale.Length();
            modelBoundingSphere.Center = worldTransformations.Translation;
            return modelBoundingSphere;
        }

        /// <summary>
        /// Get the bounding box of this entity.
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding box of the entity.</returns>
        protected override BoundingBox CalcBoundingBox(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            // get bounding box in local space
            BoundingBox modelBoundingBox = ModelUtils.GetBoundingBox(Model);

            // initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // iterate bounding box corners and transform them
            foreach (Vector3 corner in modelBoundingBox.GetCorners())
            {
                // get curr position and update min / max
                Vector3 currPosition = Vector3.Transform(corner, worldTransformations);
                min = Vector3.Min(min, currPosition);
                max = Vector3.Max(max, currPosition);
            }

            // create and return transformed bounding box
            return new BoundingBox(min, max);
        }
    }
}
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
// A basic renderable model.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GeonBit.Core.Graphics
{

    /// <summary>
    /// A basic renderable model.
    /// This type of model renderer renders the entire model as a single unit, and not as multiple meshes, but still
    /// provide some control over materials etc.
    /// </summary>
    public class ModelEntity : BaseRenderableEntity
    {
        /// <summary>
        /// Model to render.
        /// </summary>
        public Model Model
        {
            get; protected set;
        }

        /// <summary>
        /// Should we process mesh parts?
        /// This option is useful for inheriting types, it will iterate meshes before draw calls and call a virtual processing function.
        /// </summary>
        virtual protected bool ProcessMeshParts { get { return false; } }

        /// <summary>
        /// Add bias to distance from camera when sorting by distance from camera.
        /// </summary>
        override public float CameraDistanceBias { get { return _lastRadius * 100f; } }

        // store last rendering radius (based on bounding sphere)
        float _lastRadius = 0f;

        /// <summary>
        /// Dictionary with materials to use per meshes.
        /// Key is mesh name, value is material to use for this mesh.
        /// </summary>
        Dictionary<string, Materials.MaterialAPI[]> _materials = new Dictionary<string, Materials.MaterialAPI[]>();

        /// <summary>
        /// Get materials dictionary.
        /// </summary>
        internal Dictionary<string, Materials.MaterialAPI[]>  OverrideMaterialsDictionary { get { return _materials; } }

        /// <summary>
        /// Optional custom render settings for this specific instance.
        /// Note: this method is much less efficient than materials override.
        /// </summary>
        public MaterialOverrides MaterialOverride = new MaterialOverrides();

        /// <summary>
        /// Create the model entity from model instance.
        /// </summary>
        /// <param name="model">Model to draw.</param>
        public ModelEntity(Model model)
        {
            // store model
            Model = model;
        }

        /// <summary>
        /// Create the model entity from asset path.
        /// </summary>
        /// <param name="path">Path of the model to load.</param>
        public ModelEntity(string path) : this(ResourcesManager.Instance.GetModel(path))
        {
        }

        /// <summary>
        /// Copy materials from another dictionary of materials.
        /// </summary>
        /// <param name="materials">Source materials to copy.</param>
        public void CopyMaterials(Dictionary<string, Materials.MaterialAPI[]> materials)
        {
            foreach (var pair in materials)
            {
                _materials[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Set alternative material for a specific mesh id.
        /// </summary>
        /// <param name="material">Material to set.</param>
        /// <param name="meshId">Mesh name. If empty string is provided, this material will be used for all meshes.</param>
        public void SetMaterial(Materials.MaterialAPI material, string meshId = "")
        {
            _materials[meshId] = new Materials.MaterialAPI[] { material };
        }

        /// <summary>
        /// Set alternative materials for a specific mesh id.
        /// </summary>
        /// <param name="material">Materials to set (list where index is mesh-part index as value is material to use for this part).</param>
        /// <param name="meshId">Mesh name. If empty string is provided, this material will be used for all meshes.</param>
        public void SetMaterials(Materials.MaterialAPI[] material, string meshId = "")
        {
            _materials[meshId] = material;
        }

        /// <summary>
        /// Get material for a given mesh id.
        /// </summary>
        /// <param name="meshId">Mesh id to get material for.</param>
        /// <param name="meshPartIndex">MeshPart index to get material for.</param>
        public Materials.MaterialAPI GetMaterial(string meshId, int meshPartIndex = 0)
        {
            // material to return
            Materials.MaterialAPI[] ret = null;

            // try to get global material or material for this specific mesh
            if (_materials.TryGetValue(string.Empty, out ret) || _materials.TryGetValue(meshId, out ret))
            {
                // get material for effect index or null if overflow
                return meshPartIndex < ret.Length ? ret[meshPartIndex] : null;
            }

            // if not found, return the default material attached to the mesh effect
            return Model.Meshes[meshId].MeshParts[meshPartIndex].GetMaterial();
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
            foreach (var mesh in Model.Meshes)
            {
                for (int i = 0; i < mesh.MeshParts.Count; ++i)
                {
                    Materials.MaterialAPI material = GetMaterial(mesh.Name, i);
                    if (!ret.Contains(material))
                    {
                        ret.Add(material);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Get the first material used in this renderer.
        /// </summary>
        /// <returns>List of materials.</returns>
        public Materials.MaterialAPI GetFirstMaterial()
        {
            return GetMaterial(Model.Meshes[0].Name, 0);
        }

        /// <summary>
        /// Draw this model.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void DoEntityDraw(ref Matrix worldTransformations)
        {
            // call base draw entity
            base.DoEntityDraw(ref worldTransformations);

            // reset last radius
            _lastRadius = 0f;
            float scaleLen = Utils.ExtendedMath.GetScale(ref worldTransformations).Length();

            // iterate model meshes
            foreach (var mesh in Model.Meshes)
            {
                // check if in this mesh we have shared materials, eg same effects used for several mesh parts
                bool gotSharedEffects = mesh.Effects.Count != mesh.MeshParts.Count;

                // iterate over mesh parts
                int index = 0;
                foreach (var meshPart in mesh.MeshParts)
                {
                    // get material for this mesh and effect index
                    Materials.MaterialAPI material = GetMaterial(mesh.Name, index);

                    // no material found? skip.
                    // note: this can happen if user set alternative materials array with less materials than original mesh file
                    if (material == null) { break; }

                    // update per-entity override properties
                    material = MaterialOverride.Apply(material);

                    // if we don't have shared effects, eg every mesh part has its own effect, update material transformations
                    if (!gotSharedEffects) material.Apply(ref worldTransformations, ref _lastBoundingSphere);

                    // apply material effect on the mesh part. note: we first store original effect in mesh part's tag.
                    meshPart.Tag = meshPart.Effect;
                    meshPart.Effect = material.Effect;

                    // next index.
                    ++index;
                }

                // if we have shared effects, eg more than one mesh part with the same effect, we apply all materials here
                // this is to prevent applying the same material more than once
                if (gotSharedEffects)
                {
                    foreach (var effect in mesh.Effects)
                    {
                        effect.GetMaterial().Apply(ref worldTransformations, ref _lastBoundingSphere);
                    }
                }

                // update last radius
                _lastRadius = System.Math.Max(_lastRadius, mesh.BoundingSphere.Radius * scaleLen);

                // iterate mesh parts
                if (ProcessMeshParts)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        // call the before-drawing-mesh-part callback
                        BeforeDrawingMeshPart(part);
                    }
                }

                // draw the mesh itself
                mesh.Draw();

                // restore original effect to mesh parts
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = meshPart.Tag as Effect;
                    meshPart.Tag = null;
                }
            }
        }

        /// <summary>
        /// Called before drawing each mesh part.
        /// This is useful to extend this model with animations etc.
        /// </summary>
        /// <param name="part">Mesh part we are about to draw.</param>
        protected virtual void BeforeDrawingMeshPart(ModelMeshPart part)
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
            modelBoundingSphere.Radius *= Utils.ExtendedMath.GetScale(ref worldTransformations).Length();
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
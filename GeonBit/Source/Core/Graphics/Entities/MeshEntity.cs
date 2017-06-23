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
// A basic renderable mesh.
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
    /// A basic renderable mesh (part of a model).
    /// </summary>
    public class MeshEntity : BaseRenderableEntity
    {
        /// <summary>
        /// Mesh to render.
        /// </summary>
        public ModelMesh Mesh
        {
            get; protected set;
        }

        /// <summary>
        /// Mesh's parent model.
        /// </summary>
        public Model Model
        {
            get; protected set;
        }

        /// <summary>
        /// Add bias to distance from camera when sorting by distance from camera.
        /// </summary>
        override public float CameraDistanceBias { get { return _lastRadius * 100f; } }

        // store last rendering radius (based on bounding sphere)
        float _lastRadius = 0f;

        /// <summary>
        /// Optional custom render settings for this specific instance.
        /// Note: this method is much less efficient than materials override.
        /// </summary>
        public MaterialOverrides MaterialOverride = new MaterialOverrides();

        /// <summary>
        /// Blending state of this entity.
        /// </summary>
        public BlendState BlendingState = BlendState.AlphaBlend;

        /// <summary>
        /// Create the mesh entity from mesh instance.
        /// </summary>
        /// <param name="model">Model to draw.</param>
        /// <param name="mesh">Specific mesh in model to draw.</param>
        public MeshEntity(Model model, ModelMesh mesh)
        {
            Model = model;
            Mesh = mesh;
        }

        /// <summary>
        /// Optional array of materials to use instead of the mesh default materials.
        /// </summary>
        Materials.MaterialAPI[] _materials = null;

        /// <summary>
        /// Get materials dictionary.
        /// </summary>
        internal Materials.MaterialAPI[] OverrideMaterials { get { return _materials; } }

        /// <summary>
        /// Set first alternative material for this mesh (useful for meshes with one effect).
        /// </summary>
        /// <param name="material">Material to set.</param>
        public void SetMaterial(Materials.MaterialAPI material)
        {
            _materials = new Materials.MaterialAPI[] { material };
        }

        /// <summary>
        /// Set alternative array of materials for this mesh.
        /// Will replace mesh original materials.
        /// </summary>
        /// <param name="material">Materials array to set.</param>
        public void SetMaterials(Materials.MaterialAPI[] materials)
        {
            _materials = materials;
        }

        /// <summary>
        /// Get material for a given mesh id.
        /// </summary>
        /// <param name="effectIndex">Effect index to get material for.</param>
        public Materials.MaterialAPI GetMaterial(int effectIndex = 0)
        {
            // if we got alternative materials array defined, check if we got alternative material for this effect index
            if (_materials != null)
            {
                // get material for effect index or null if overflow
                return effectIndex < _materials.Length ? _materials[effectIndex] : null;
            }

            // if not found, return the default material attached to the mesh effect (via 'Tag')
            return (Materials.MaterialAPI)Mesh.Effects[effectIndex].Tag;
        }

        /// <summary>
        /// Draw this model.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void DoEntityDraw(Matrix worldTransformations)
        {
            // reset last radius
            _lastRadius = 0f;
            float scaleLen = worldTransformations.Scale.Length();

            // set blend state
            GraphicsManager.GraphicsDevice.BlendState = BlendingState;

            // iterate over mesh effects
            for (int index = 0; index < Mesh.Effects.Count; ++index)
            {
                // get material for this mesh and effect index
                Materials.MaterialAPI material = GetMaterial(index);

                // no material found? skip.
                // note: this can happen if user set alternative materials array with less materials than original mesh file
                if (material == null) { break; }

                // update per-entity override properties
                material = MaterialOverride.Apply(material);

                // apply material effect on the mesh part
                material.Apply(worldTransformations);
                Mesh.MeshParts[index].Effect = material.Effect;
            }

            // update last radius
            _lastRadius = System.Math.Max(_lastRadius, Mesh.BoundingSphere.Radius * scaleLen);

            // draw the mesh itself
            Mesh.Draw();
            
        }

        /// <summary>
        /// Get the bounding sphere of this mesh.
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding box of the entity.</returns>
        protected override BoundingSphere CalcBoundingSphere(Node parent, Matrix localTransformations, Matrix worldTransformations)
        {
            BoundingSphere modelBoundingSphere = Mesh.BoundingSphere;
            modelBoundingSphere.Radius *= worldTransformations.Scale.Length();
            modelBoundingSphere.Center = worldTransformations.Translation;
            return modelBoundingSphere;

        }

        /// <summary>
        /// Get the bounding box of this mesh.
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding box of the entity.</returns>
        protected override BoundingBox CalcBoundingBox(Node parent, Matrix localTransformations, Matrix worldTransformations)
        {
            return BoundingBox.CreateFromSphere(GetBoundingSphere(parent, localTransformations, worldTransformations));
        }
    }
}
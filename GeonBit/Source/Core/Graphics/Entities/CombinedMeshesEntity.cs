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
// A static mesh that is built from other models and meshes at runtime.
// This allows you to build parts of the level dynamically, and then reduce draw
// calls significantly by merging them into one static batch.
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
    /// A special renderer that takes several other renderers (meshes, models etc) and merge them into a single batch.
    /// This optimization allows you to build levels out of static models, and reduce draw calls by merging them into one
    /// static mesh at runtime. 
    /// 
    /// Note: While its a good way to reduce draw calls, don't create combined meshes too big or else you'll lose some culling optimizations.
    /// </summary>
    public class CombinedMeshesEntity : BaseRenderableEntity
    {
        /// <summary>
        /// Hold mesh + transform in combined mesh entity.
        /// </summary>
        struct MeshEntry
        {
            /// <summary>
            /// Mesh to add.
            /// </summary>
            public ModelMesh Mesh;

            /// <summary>
            /// Transformations to apply when adding the mesh.
            /// </summary>
            public Matrix Transform;

            /// <summary>
            /// Optional material to use instead of the mesh default materials.
            /// </summary>
            public Materials.MaterialAPI Material;

            /// <summary>
            /// Create the mesh entry.
            /// </summary>
            /// <param name="mesh">Mesh to use.</param>
            /// <param name="transform">World transformations.</param>
            /// <param name="material">Optional material to use instead of the default mesh materials.</param>
            public MeshEntry(ModelMesh mesh, Matrix transform, Materials.MaterialAPI material = null)
            {
                Mesh = mesh;
                Transform = transform;
                Material = material;
            }
        }

        /// <summary>
        /// Represent the vertices and indexes of a combined mesh part.
        /// This represent a chunk of couple of meshes combined together, all vertices are in world space after transformations applied.
        /// </summary>
        class CombinedMeshesPart
        {
            /// <summary>
            /// Vertices buffer.
            /// </summary>
            public List<VertexPositionNormalTexture> Vertices { get; internal set; } = new List<VertexPositionNormalTexture>();

            /// <summary>
            /// Vertices indexes.
            /// </summary>
            public List<short> Indexes { get; internal set; } = new List<short>();

            /// <summary>
            /// Primitives count.
            /// </summary>
            public int PrimitiveCount { get; internal set; } = 0;

            /// <summary>
            /// Count index offset while building the combined mesh.
            /// </summary>
            public int IndexOffset { get; internal set; } = 0;
        }

        // list of meshes to add to the combined mesh next time we build.
        List<MeshEntry> _pendingMeshes = new List<MeshEntry>();

        // dictionary to hold the combined parts and their materials.
        // key is material so we can draw them in chunks sharing the same material and texture.
        Dictionary<Materials.MaterialAPI, CombinedMeshesPart> _parts = new Dictionary<Materials.MaterialAPI, CombinedMeshesPart>();

        /// <summary>
        /// Combined mesh bounding box.
        /// </summary>
        BoundingBox _boundingBox;

        /// <summary>
        /// Combined mesh bounding sphere.
        /// </summary>
        BoundingSphere _boundingSphere;

        /// <summary>
        /// Add a model to the combined mesh.
        /// Note: you must call Build() for this to take effect.
        /// </summary>
        /// <param name="model">Model to add.</param>
        /// <param name="transform">World transformations.</param>
        /// <param name="material">Optional material to use instead of the model default materials.</param>
        public void AddModel(Model model, Matrix transform, Materials.MaterialAPI material = null)
        {
            foreach (var mesh in model.Meshes)
            {
                AddModelMesh(mesh, transform, material);
            }
        }

        /// <summary>
        /// Add a model mesh to the combined mesh.
        /// Note: you must call Build() for this to take effect.
        /// </summary>
        /// <param name="mesh">Mesh to add.</param>
        /// <param name="transform">World transformations.</param>
        /// <param name="material">Optional material to use instead of the mesh default materials.</param>
        public void AddModelMesh(ModelMesh mesh, Matrix transform, Materials.MaterialAPI material = null)
        {
            _pendingMeshes.Add(new MeshEntry(mesh, transform, material));
        }

        /// <summary>
        /// Build the entire combined meshes from the previously added models and meshes.
        /// You need to call this function after you're done adding all the parts using AddModel() and AddModelMesh().
        /// Note: once build, it will clear the list of meshes. So if you want to rebuild you need to re-add all model and meshes parts again.
        /// </summary>
        /// <param name="assertIfWriteOnly">If true and we get a mesh with write-only buffers (meaning we can't process it), will throw exception.
        /// If false, will just skip that mesh part quitely.</param>
        public void Build(bool assertIfWriteOnly = true)
        {
            // clear previous static parts
            _parts.Clear();

            // store all vertices in a temp buffer to create bounding box and sphere in the end
            List<Vector3> allVertices = new List<Vector3>();

            // iterate meshes
            foreach (var meshData in _pendingMeshes)
            {
                // get mesh and transform
                ModelMesh mesh = meshData.Mesh;
                Matrix transform = meshData.Transform;
                Materials.MaterialAPI overrideMaterial = meshData.Material;

                // iterate mesh parts
                foreach (var meshPart in mesh.MeshParts)
                {
                    // get material to use for this mesh part
                    var material = overrideMaterial ?? meshPart.GetMaterial();

                    // get the combined chunk to add this meshpart to
                    CombinedMeshesPart combinedPart;
                    if (!_parts.TryGetValue(material, out combinedPart))
                    {
                        combinedPart = new CombinedMeshesPart();
                        _parts[material] = combinedPart;
                    }

                    // make sure its not readonly
                    if (meshPart.VertexBuffer.BufferUsage == BufferUsage.WriteOnly ||
                        meshPart.IndexBuffer.BufferUsage == BufferUsage.WriteOnly)
                    {
                        if (assertIfWriteOnly) { throw new Exceptions.InvalidValueException("Cannot add mesh with write-only buffers to Combined Mesh!"); }
                        continue;
                    }

                    // make sure vertex buffer uses position-normal-texture
                    if (meshPart.VertexBuffer.VertexDeclaration.VertexStride < 8)
                    {
                        throw new Exceptions.InvalidValueException("Combined meshes can only use vertex buffers with position, normal and texture!");
                    }

                    // get vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // iterate through vertices and add them
                    int verticesInPart = 0;
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        // get curr position with transformations
                        Vector3 currPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), transform);

                        // get normals
                        Vector3 normal = Vector3.Transform(new Vector3(vertexData[i + 3], vertexData[i + 4], vertexData[i + 5]), transform);
                        normal.Normalize();

                        // get texture coords
                        Vector2 textcoords = new Vector2(vertexData[i + 6], vertexData[i + 7]);

                        // add to vertices buffer
                        combinedPart.Vertices.Add(new VertexPositionNormalTexture(currPosition, normal, textcoords));

                        // add to temp list of all points
                        allVertices.Add(currPosition);
                        verticesInPart++;
                    }

                    // set indexes
                    short[] drawOrder = new short[meshPart.IndexBuffer.IndexCount];
                    meshPart.IndexBuffer.GetData<short>(drawOrder);
                    foreach (short currIndex in drawOrder)
                    {
                        combinedPart.Indexes.Add((short)(currIndex + combinedPart.IndexOffset));
                    }
                    combinedPart.IndexOffset += verticesInPart;

                    // set primitives count
                    combinedPart.PrimitiveCount += meshPart.PrimitiveCount;
                }
            }

            // update bounding box and sphere
            _boundingSphere = BoundingSphere.CreateFromPoints(allVertices);
            _boundingBox = BoundingBox.CreateFromPoints(allVertices);

            // clear the list of static meshes to build
            _pendingMeshes.Clear();
        }

        /// <summary>
        /// Draw this model.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void DoEntityDraw(ref Matrix worldTransformations)
        {
            // iterate combined parts
            foreach (var combinedPart in _parts)
            {
                // get and setup material
                var material = combinedPart.Key;
                material.Apply(ref worldTransformations);

                // get vertices and indexes
                var buffers = combinedPart.Value;

                // draw with material
                material.IterateEffectPasses((EffectPass pass) =>
                {
                    GraphicsManager.GraphicsDevice.DrawUserIndexedPrimitives
                        <VertexPositionNormalTexture>(
                        PrimitiveType.TriangleList,
                        buffers.Vertices.ToArray(), 0, buffers.Vertices.Count,
                        buffers.Indexes.ToArray(), 0, buffers.PrimitiveCount);
                });
            }
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
            BoundingSphere modelBoundingSphere = _boundingSphere;
            modelBoundingSphere.Radius *= worldTransformations.Scale.Length();
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
            BoundingBox modelBoundingBox = _boundingBox;

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
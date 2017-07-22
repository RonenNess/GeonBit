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
            /// Create the mesh entry.
            /// </summary>
            /// <param name="mesh">Mesh to use.</param>
            /// <param name="transform">World transformations.</param>
            public MeshEntry(ModelMesh mesh, Matrix transform)
            {
                Mesh = mesh;
                Transform = transform;
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
            public List<ushort> Indexes { get; internal set; } = new List<ushort>();
        }

        // list of meshes to add to the combined mesh.
        List<MeshEntry> _meshes = new List<MeshEntry>();

        // dictionary to hold the combined parts and their materials.
        // key is material so we can draw them in chunks sharing the same material and texture.
        Dictionary<Materials.MaterialAPI, CombinedMeshesPart> _parts = new Dictionary<Materials.MaterialAPI, CombinedMeshesPart>();

        /// <summary>
        /// Add a model to the combined mesh.
        /// Note: you must call Build() for this to take effect.
        /// </summary>
        /// <param name="model">Model to add.</param>
        /// <param name="transform">World transformations.</param>
        public void AddModel(Model model, Matrix transform)
        {
            foreach (var mesh in model.Meshes)
            {
                AddModelMesh(mesh, transform);
            }
        }

        /// <summary>
        /// Add a model mesh to the combined mesh.
        /// Note: you must call Build() for this to take effect.
        /// </summary>
        /// <param name="mesh">Mesh to add.</param>
        /// <param name="transform">World transformations.</param>
        public void AddModelMesh(ModelMesh mesh, Matrix transform)
        {
            _meshes.Add(new MeshEntry(mesh, transform));
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

            // iterate meshes
            foreach (var meshData in _meshes)
            {
                // get mesh and transform
                ModelMesh mesh = meshData.Mesh;
                Matrix transform = meshData.Transform;

                // iterate mesh parts
                foreach (var meshPart in mesh.MeshParts)
                {
                    // get material from mesh part
                    var material = meshPart.GetMaterial();

                    // get the combined chunk to add this meshpart to
                    var combinedPart = _parts[material];

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
                    }

                    ushort[] drawOrder = new ushort[meshPart.IndexBuffer.IndexCount];
                    meshPart.IndexBuffer.GetData<ushort>(drawOrder);
                    combinedPart.Indexes.AddRange(drawOrder);
                }
            }

            // clear the list of static meshes to build
            _meshes.Clear();
        }

        /// <summary>
        /// Draw this model.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void DoEntityDraw(ref Matrix worldTransformations)
        {
            /*
            // setup material
            material.Apply(ref newWorld);

            // draw sprite
            // draw the cube vertices
            material.IterateEffectPasses((EffectPass pass) =>
            {
                GraphicsManager.GraphicsDevice.DrawUserIndexedPrimitives
                    <VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    _spritesheetStep.Vertices, 0, 4,
                    _spritesheetStep.Indexes, 0, 2);
            });
            */
        }
    }
}
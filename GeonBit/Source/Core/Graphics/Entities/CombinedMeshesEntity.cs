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
using GeonBit.Core.Utils;

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
        /// Represent the vertices and indexes of a combined mesh part.
        /// This represent a chunk of couple of meshes combined together, all vertices are in world space after transformations applied.
        /// </summary>
        class CombinedMeshesPart
        {
            /// <summary>
            /// Vertices buffer.
            /// </summary>
            public ResizableArray<VertexPositionNormalTexture> Vertices { get; internal set; } = new ResizableArray<VertexPositionNormalTexture>();

            /// <summary>
            /// Vertices indexes.
            /// </summary>
            public ResizableArray<short> Indexes { get; internal set; } = new ResizableArray<short>();

            /// <summary>
            /// Vertex buffer.
            /// </summary>
            public VertexBuffer _VertexBuffer;

            /// <summary>
            /// Index buffer.
            /// </summary>
            public IndexBuffer _IndexBuffer;

            /// <summary>
            /// Primitives count.
            /// </summary>
            public int PrimitiveCount { get; internal set; } = 0;

            /// <summary>
            /// Count index offset while building the combined mesh.
            /// </summary>
            public int IndexOffset { get; internal set; } = 0;

            /// <summary>
            /// Build vertex and indexes buffer and clear lists.
            /// </summary>
            public void Build()
            {
                // get device
                var device = Graphics.GraphicsManager.GraphicsDevice;

                // build vertex buffer
                Vertices.Trim();
                _VertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), Vertices.Count, BufferUsage.WriteOnly);
                _VertexBuffer.SetData<VertexPositionNormalTexture>(Vertices.InternalArray);
                Vertices.Clear();

                // build indexes buffer
                Indexes.Trim();
                _IndexBuffer = new IndexBuffer(device, typeof(short), Indexes.Count, BufferUsage.WriteOnly);
                _IndexBuffer.SetData(Indexes.InternalArray);
                Indexes.Clear();
            }
        }

        // dictionary to hold the combined parts and their materials.
        // key is material so we can draw them in chunks sharing the same material and texture.
        Dictionary<Materials.MaterialAPI, CombinedMeshesPart> _parts = new Dictionary<Materials.MaterialAPI, CombinedMeshesPart>();

        // store all vertices positions - needed to calculate bounding box / sphere. This list is cleared once built.
        List<Vector3> _allPoints = new List<Vector3>();

        /// <summary>
        /// Combined mesh bounding box, in local space.
        /// </summary>
        BoundingBox _localBoundingBox;

        /// <summary>
        /// Combined mesh bounding sphere, in local space.
        /// </summary>
        BoundingSphere _localBoundingSphere;

        /// <summary>
        /// Did we already build this combined mesh entity? happens on first draw, or when "build" is called.
        /// </summary>
        bool _wasBuilt = false;

        /// <summary>
        /// Clone this combined entity.
        /// </summary>
        /// <returns>Cloned copy.</returns>
        public CombinedMeshesEntity Clone()
        {
            CombinedMeshesEntity ret = new CombinedMeshesEntity();
            ret._localBoundingBox = _localBoundingBox;
            ret._localBoundingSphere = _localBoundingSphere;
            ret._allPoints = new List<Vector3>(_allPoints);
            ret._parts = new Dictionary<Materials.MaterialAPI, CombinedMeshesPart>(_parts);
            ret._wasBuilt = _wasBuilt;
            return ret;
        }

        /// <summary>
        /// Add a model to the combined mesh.
        /// </summary>
        /// <param name="model">Model to add.</param>
        /// <param name="transform">World transformations.</param>
        /// <param name="material">Optional material to use instead of the model default materials.</param>
        public void AddModel(Model model, Matrix transform, Materials.MaterialAPI material = null)
        {
            // sanity check - if build was called assert
            if (_wasBuilt) { throw new Exceptions.InvalidActionException("Cannot add model to Combined Mesh Entity after it was built!"); }

            // iterate model meshes and add them
            foreach (var mesh in model.Meshes)
            {
                AddModelMesh(mesh, transform, material);
            }
        }

        /// <summary>
        /// Add a model mesh to the combined mesh.
        /// </summary>
        /// <param name="mesh">Mesh to add.</param>
        /// <param name="transform">World transformations.</param>
        /// <param name="material">Optional material to use instead of the mesh default materials.</param>
        public void AddModelMesh(ModelMesh mesh, Matrix transform, Materials.MaterialAPI material = null)
        {
            // sanity check - if build was called assert
            if (_wasBuilt) { throw new Exceptions.InvalidActionException("Cannot add meshes to Combined Mesh Entity after it was built!"); }

            // did we get material override to set?
            bool externalMaterial = material != null;

            // iterate mesh parts
            foreach (var meshPart in mesh.MeshParts)
            {
                // if we didn't get external material to use, get material from mesh part.
                if (!externalMaterial)
                {
                    material = meshPart.GetMaterial();
                }

                // get the combined chunk to add this meshpart to
                CombinedMeshesPart combinedPart = GetCombinedPart(material);

                // make sure its not readonly
                if (meshPart.VertexBuffer.BufferUsage == BufferUsage.WriteOnly ||
                    meshPart.IndexBuffer.BufferUsage == BufferUsage.WriteOnly)
                {
                    throw new Exceptions.InvalidValueException("Cannot add mesh with write-only buffers to Combined Mesh!");
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
                    
                    // get normals and rotate it based on transformations
                    Vector3 normal = new Vector3(vertexData[i + 3], vertexData[i + 4], vertexData[i + 5]);
                    normal = Vector3.TransformNormal(normal, transform);

                    // get texture coords
                    Vector2 textcoords = new Vector2(vertexData[i + 6], vertexData[i + 7]);

                    // add to vertices buffer
                    combinedPart.Vertices.Add(new VertexPositionNormalTexture(currPosition, normal, textcoords));

                    // add to temp list of all points
                    _allPoints.Add(currPosition);
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

        /// <summary>
        /// Add array of vertices to the combined mesh.
        /// </summary>
        /// <param name="vertices">Vertices array to add.</param>
        /// <param name="indexes">Draw order / indexes array.</param>
        /// <param name="transform">World transformations.</param>
        /// <param name="material">Material to use with the vertices.</param>
        public void AddVertices(VertexPositionNormalTexture[] vertices, short[] indexes, Matrix transform, Materials.MaterialAPI material)
        {
            // sanity check - if build was called assert
            if (_wasBuilt) { throw new Exceptions.InvalidActionException("Cannot add vertices to Combined Mesh Entity after it was built!"); }

            // if transform is identity skip everything here
            if (transform == Matrix.Identity)
            {
                AddVertices(vertices, indexes, material);
                return;
            }

            // transform all vertices from array
            int i = 0;
            VertexPositionNormalTexture[] processed = new VertexPositionNormalTexture[vertices.Length];
            foreach (var vertex in vertices)
            {
                // get current vertex
                VertexPositionNormalTexture curr = vertex;

                // apply transformations
                curr.Position = Vector3.Transform(curr.Position, transform);
                curr.Normal = Vector3.TransformNormal(curr.Normal, transform);
                processed[i++] = curr;
            }

            // add processed vertices
            AddVertices(processed, indexes, material);
        }

        /// <summary>
        /// Add array of vertices to the combined mesh.
        /// </summary>
        /// <param name="vertices">Vertices array to add.</param>
        /// <param name="indexes">Draw order / indexes array.</param>
        /// <param name="material">Material to use with the vertices.</param>
        public void AddVertices(VertexPositionNormalTexture[] vertices, short[] indexes, Materials.MaterialAPI material)
        {
            // sanity check - if build was called assert
            if (_wasBuilt) { throw new Exceptions.InvalidActionException("Cannot add vertices to Combined Mesh Entity after it was built!"); }

            // get the combined chunk to add these vertices to
            CombinedMeshesPart combinedPart = GetCombinedPart(material);

            // add vertices to combined part
            combinedPart.Vertices.AddRange(vertices);
            foreach (var vertex in vertices)
            {
                _allPoints.Add(vertex.Position);
            }

            // add indexes (but first update them to be relative to whats already in combined part)
            for (int i = 0; i < indexes.Length; ++i)
            {
                combinedPart.Indexes.Add((short)(indexes[i] + combinedPart.IndexOffset));
            }

            // increase index offset in combined part
            combinedPart.IndexOffset += vertices.Length;

            // update primitives count
            combinedPart.PrimitiveCount += indexes.Length / 3;
        }

        /// <summary>
        /// Get combined mesh part from material.
        /// </summary>
        /// <param name="material">Material to get combined part for.</param>
        /// <returns>Combined mesh part.</returns>
        private CombinedMeshesPart GetCombinedPart(Materials.MaterialAPI material)
        {
            // try to get from existing parts and if not found create it
            CombinedMeshesPart combinedPart;
            if (!_parts.TryGetValue(material, out combinedPart))
            {
                combinedPart = new CombinedMeshesPart();
                _parts[material] = combinedPart;
            }

            // return combined part
            return combinedPart;
        }

        /// <summary>
        /// Clear everything from the combined meshed renderer.
        /// </summary>
        public void Clear()
        {
            _parts.Clear();
            _allPoints.Clear();
            RebuildBoundingBoxAndSphere();
            _wasBuilt = false;
        }

        /// <summary>
        /// Build the combined meshes renderer.
        /// </summary>
        public void Build()
        {
            // build parts
            foreach (var combinedPart in _parts)
            {
                combinedPart.Value.Build();
            }

            // build bounding box and sphere
            RebuildBoundingBoxAndSphere();

            // clear points
            _allPoints.Clear();

            // mark as built
            _wasBuilt = true;
        }

        /// <summary>
        /// Draw this model.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void DoEntityDraw(ref Matrix worldTransformations)
        {
            // get graphic device
            var device = Graphics.GraphicsManager.GraphicsDevice;

            // get bounding sphere
            var bs = GetLastBoundingSphere();

            // iterate combined parts
            foreach (var combinedPart in _parts)
            {
                // get and setup material
                var material = combinedPart.Key;
                material.Apply(ref worldTransformations, ref bs);

                // get vertices and indexes
                var buffers = combinedPart.Value;

                // set vertex and indices buffers
                device.SetVertexBuffer(buffers._VertexBuffer);
                device.Indices = buffers._IndexBuffer;

                // draw with material
                material.IterateEffectPasses((EffectPass pass) =>
                {
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, buffers.PrimitiveCount);
                });
            }
        }

        /// <summary>
        /// Rebuild bounding box and bounding sphere.
        /// </summary>
        private void RebuildBoundingBoxAndSphere()
        {
            if (_allPoints.Count != 0)
            {
                _localBoundingBox = BoundingBox.CreateFromPoints(_allPoints);
                _localBoundingSphere = BoundingSphere.CreateFromPoints(_allPoints);
            }
            else
            {
                _localBoundingBox = new BoundingBox();
                _localBoundingSphere = new BoundingSphere();
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
            // get bounding sphere in local space
            BoundingSphere modelBoundingSphere = _localBoundingSphere;

            // apply transformations on bounding sphere
            modelBoundingSphere.Radius *= System.Math.Max(worldTransformations.Scale.X, System.Math.Max(worldTransformations.Scale.Y, worldTransformations.Scale.Z));
            modelBoundingSphere.Center = Vector3.Transform(modelBoundingSphere.Center, worldTransformations);
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
            BoundingBox modelBoundingBox = _localBoundingBox;

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
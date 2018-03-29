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
// Helper / utilities to handle 3d models.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GeonBit.Core.Graphics
{
    /// <summary>
    /// Help utilities to handle 3d models.
    /// </summary>
    static class ModelUtils
    {
        // cache of bounding boxes we calculated for loaded models.
        static Dictionary<Model, BoundingBox> _calculatedBoundingBoxes = new Dictionary<Model, BoundingBox>();

        // cache of bounding spheres we calculated for loaded models.
        static Dictionary<Model, BoundingSphere> _calculatedBoundingSpheres = new Dictionary<Model, BoundingSphere>();

        /// <summary>
        /// Return bounding box for a model instance (calculate if needed, else return from cache).
        /// </summary>
        /// <param name="model">Model to get bounding box for.</param>
        /// <returns>BoundingBox instance, in local space.</returns>
        public static BoundingBox GetBoundingBox(Model model)
        {
            // try to get value from cache, and if got bounding box in cache return it.
            BoundingBox ret;
            if (_calculatedBoundingBoxes.TryGetValue(model, out ret))
            {
                return ret;
            }

            // count the bounding box calculation
            Utils.CountAndAlert.Count(Utils.CountAndAlert.PredefAlertTypes.VeryHeavyUpdate);

            // got here? it means we need to calculate bounding box.
            // initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // iterate over mesh parts
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // skip write-only buffers
                    if (meshPart.VertexBuffer.BufferUsage == BufferUsage.WriteOnly)
                    {
                        continue;
                    }

                    // vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // iterate through vertices (possibly) growing bounding box
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        // get curr position and update min / max
                        Vector3 currPosition = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
                        min = Vector3.Min(min, currPosition);
                        max = Vector3.Max(max, currPosition);
                    }
                }
            }

            // add to cache and return
            ret = new BoundingBox(min, max);
            _calculatedBoundingBoxes[model] = ret;
            return ret;
        }

        /// <summary>
        /// Return bounding sphere for a model instance (calculate if needed, else return from cache).
        /// </summary>
        /// <param name="model">Model to get bounding sphere for.</param>
        /// <returns>BoundingSphere instance, in local space.</returns>
        public static BoundingSphere GetBoundingSphere(Model model)
        {
            // try to get value from cache, and if got bounding box in cache return it.
            BoundingSphere ret;
            if (_calculatedBoundingSpheres.TryGetValue(model, out ret))
            {
                return ret;
            }

            // count the bounding sphere calculation
            Utils.CountAndAlert.Count(Utils.CountAndAlert.PredefAlertTypes.VeryHeavyUpdate);

            // got here? it means we need to calculate bounding sphere.
            ret = new BoundingSphere();
            foreach (var mesh in model.Meshes)
            {
                ret = BoundingSphere.CreateMerged(ret, mesh.BoundingSphere);
            }

            // add to cache and return
            _calculatedBoundingSpheres[model] = ret;
            return ret;
        }
    }
}

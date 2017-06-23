#region License
/// -------------------------------------------------------------------------------------
/// Notice: This file had been edited to integrate as core inside GeonBit.
/// Original license and attributes below. The license and copyright notice below affect
/// this file and this file only. https://github.com/tainicom/Aether.Extras
/// -------------------------------------------------------------------------------------
//   Copyright 2015-2016 Kastellanos Nikolaos
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeonBit.Extend.Graphics;

namespace GeonBit.Extend.Animation
{
    public class CpuAnimatedVertexBuffer: DynamicVertexBuffer
    {
        private VertexIndicesWeightsPositionNormal[] cpuVertices;
        private VertexPositionNormalTexture[] gpuVertices;
        
        public CpuAnimatedVertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage) :
            base(graphicsDevice, vertexDeclaration, vertexCount, bufferUsage)
        {
            return;
        }

        internal void SetGpuVertices(VertexPositionNormalTexture[] vertices)
        {
            this.gpuVertices = vertices;
        }

        internal void SetCpuVertices(VertexIndicesWeightsPositionNormal[] vertices)
        {
            this.cpuVertices = vertices;
        }

        internal void UpdateVertices(Matrix[] boneTransforms, int startIndex, int elementCount)
        {
            Matrix transformSum = Matrix.Identity;

            // skin all of the vertices
            for (int i = startIndex; i < (startIndex + elementCount); i++)
            {
                int b0 = cpuVertices[i].BlendIndex0;
                int b1 = cpuVertices[i].BlendIndex1;
                int b2 = cpuVertices[i].BlendIndex2;
                int b3 = cpuVertices[i].BlendIndex3;

                float w1 = cpuVertices[i].BlendWeights.X;
                float w2 = cpuVertices[i].BlendWeights.Y;
                float w3 = cpuVertices[i].BlendWeights.Z;
                float w4 = cpuVertices[i].BlendWeights.W;

            #if (WP7_1)
                // Moblunatic claims ~40% faster.
                // http://forums.create.msdn.com/forums/p/55123/335148.aspx
                // This is true on WP7 with SIMD enabled. 
                // On WP8/Monogame it is *TWO* times slower than the original code.                
                
                Matrix mm1, mm2, mm3, mm4;
                Matrix.Multiply(ref boneTransforms[b0], w1, out mm1);
                Matrix.Multiply(ref boneTransforms[b1], w2, out mm2);
                Matrix.Multiply(ref boneTransforms[b2], w3, out mm3);
                Matrix.Multiply(ref boneTransforms[b3], w4, out mm4);

                Matrix.Add(ref mm1, ref mm2, out transformSum);
                Matrix.Add(ref transformSum, ref mm3, out transformSum);
                Matrix.Add(ref transformSum, ref mm4, out transformSum);
                transformSum.M14 = 0.0f;
                transformSum.M24 = 0.0f;
                transformSum.M34 = 0.0f;
                transformSum.M44 = 1.0f;
            #else
                Matrix m1 = boneTransforms[b0];
                Matrix m2 = boneTransforms[b1];
                Matrix m3 = boneTransforms[b2];
                Matrix m4 = boneTransforms[b3];
                transformSum.M11 = (m1.M11 * w1) + (m2.M11 * w2) + (m3.M11 * w3) + (m4.M11 * w4);
                transformSum.M12 = (m1.M12 * w1) + (m2.M12 * w2) + (m3.M12 * w3) + (m4.M12 * w4);
                transformSum.M13 = (m1.M13 * w1) + (m2.M13 * w2) + (m3.M13 * w3) + (m4.M13 * w4);
                transformSum.M21 = (m1.M21 * w1) + (m2.M21 * w2) + (m3.M21 * w3) + (m4.M21 * w4);
                transformSum.M22 = (m1.M22 * w1) + (m2.M22 * w2) + (m3.M22 * w3) + (m4.M22 * w4);
                transformSum.M23 = (m1.M23 * w1) + (m2.M23 * w2) + (m3.M23 * w3) + (m4.M23 * w4);
                transformSum.M31 = (m1.M31 * w1) + (m2.M31 * w2) + (m3.M31 * w3) + (m4.M31 * w4);
                transformSum.M32 = (m1.M32 * w1) + (m2.M32 * w2) + (m3.M32 * w3) + (m4.M32 * w4);
                transformSum.M33 = (m1.M33 * w1) + (m2.M33 * w2) + (m3.M33 * w3) + (m4.M33 * w4);
                transformSum.M41 = (m1.M41 * w1) + (m2.M41 * w2) + (m3.M41 * w3) + (m4.M41 * w4);
                transformSum.M42 = (m1.M42 * w1) + (m2.M42 * w2) + (m3.M42 * w3) + (m4.M42 * w4);
                transformSum.M43 = (m1.M43 * w1) + (m2.M43 * w2) + (m3.M43 * w3) + (m4.M43 * w4);
            #endif

                // Support the 4 Bone Influences - Position then Normal
                Vector3.Transform(ref cpuVertices[i].Position, ref transformSum, out gpuVertices[i].Position);
                Vector3.TransformNormal(ref cpuVertices[i].Normal, ref transformSum, out gpuVertices[i].Normal);
            }

            // put the vertices into our vertex buffer
            SetData(gpuVertices, 0, VertexCount, SetDataOptions.NoOverwrite);
        }
    }
}
#region License
/// -------------------------------------------------------------------------------------
/// Notice: This file had been edited to integrate as core inside GeonBit.
/// Original license and attributes below. The license and copyright notice below affect
/// this file and this file only. https://github.com/tainicom/Aether.Extras
/// -------------------------------------------------------------------------------------
//   Copyright 2011-2016 Kastellanos Nikolaos
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
#if MAPPEDMEM
using System;
using System.Reflection;
using SharpDX.Direct3D11;
#endif


namespace GeonBit.Extend.Animation
{
    public class CpuAnimatedVertexBuffer: DynamicVertexBuffer
    {
        #if USE_NATIVE_ANIMATION
        Native.Animation.CpuAnimatedVertexBufferHelper _cpuVertexBufferHelper;
        #else
        private VertexIndicesWeightsPositionNormal[] _cpuVertices;
        #endif

        private VertexPositionNormalTexture[] _gpuVertices;
        
        #if MAPPEDMEM
        SharpDX.Direct3D11.Buffer _buffer;
        #endif

        public CpuAnimatedVertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage) :
            base(graphicsDevice, vertexDeclaration, vertexCount, bufferUsage)
        {            
            #if MAPPEDMEM
            #if W8_1
            FieldInfo _bufferInfo = typeof(VertexBuffer).GetTypeInfo().GetDeclaredField("_buffer");
            #else
            FieldInfo _bufferInfo = typeof(VertexBuffer).GetField("_buffer", BindingFlags.Instance | BindingFlags.NonPublic);
            #endif
            _buffer = _bufferInfo.GetValue(this) as SharpDX.Direct3D11.Buffer;
            #endif

            return;
        }

        internal void SetGpuVertices(VertexPositionNormalTexture[] vertices)
        {
            _gpuVertices = vertices;
        }

        internal void SetCpuVertices(VertexIndicesWeightsPositionNormal[] vertices)
        {
            #if USE_NATIVE_ANIMATION
            var nativeCpuVertices = new Native.Animation.VertexTypes.VertexIndicesWeightsPositionNormal[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                nativeCpuVertices[i] = vertices[i].ToNativeCpuVertex();
            _cpuVertexBufferHelper = new Native.Animation.CpuAnimatedVertexBufferHelper();
            _cpuVertexBufferHelper.SetCpuVertices(nativeCpuVertices);
            #else            
            _cpuVertices = vertices;
            #endif
        }

        internal unsafe void UpdateVertices(Matrix[] boneTransforms, int startIndex, int elementCount)
        {
            #if MAPPEDMEM
            var context = _buffer.Device.ImmediateContext;
            var dataBox = context.MapSubresource(_buffer, 0, MapMode.WriteNoOverwrite, MapFlags.None);
            var pgpuVertices = (VertexPositionNormalTexture*)dataBox.DataPointer.ToPointer();
            InnerUpdateVertices(pgpuVertices, boneTransforms, startIndex, elementCount);
            context.UnmapSubresource(_buffer, 0);
            #else
            fixed (VertexPositionNormalTexture* pgpuVertices = _gpuVertices)
                InnerUpdateVertices(pgpuVertices, boneTransforms, startIndex, elementCount);
            int vertexStride = VertexPositionNormalTexture.VertexDeclaration.VertexStride;
            int offsetInBytes = startIndex * vertexStride;
            SetData(offsetInBytes, _gpuVertices, startIndex, elementCount, vertexStride, SetDataOptions.NoOverwrite);
#endif
        }

        private unsafe void InnerUpdateVertices(VertexPositionNormalTexture* pgpuVertices, Matrix[] boneTransforms, int startIndex, int elementCount)
        {
            fixed (Matrix* pBoneTransforms = boneTransforms)
            {   
                #if USE_NATIVE_ANIMATION
                _cpuVertexBufferHelper.UpdateVertices((long)pBoneTransforms, (long)pgpuVertices, startIndex, elementCount);
#else
                fixed (VertexIndicesWeightsPositionNormal* pcpuVertices = _cpuVertices)
                {
                    Matrix transformSum = Matrix.Identity;
                    var pVertex = pcpuVertices + startIndex;
                    var pGpuVertex = pgpuVertices + startIndex;

                    // skin all of the vertices
                    for (int i = 0; i < elementCount; i++, pVertex++, pGpuVertex++)
                    {
                        int b0 = pVertex->BlendIndex0;
                        int b1 = pVertex->BlendIndex1;
                        int b2 = pVertex->BlendIndex2;
                        int b3 = pVertex->BlendIndex3;

                        float w1 = pVertex->BlendWeights.X;
                        float w2 = pVertex->BlendWeights.Y;
                        float w3 = pVertex->BlendWeights.Z;
                        float w4 = pVertex->BlendWeights.W;

                        Matrix* m1 = pBoneTransforms + b0;
                        Matrix* m2 = pBoneTransforms + b1;
                        Matrix* m3 = pBoneTransforms + b2;
                        Matrix* m4 = pBoneTransforms + b3;
                        transformSum.M11 = (m1->M11 * w1) + (m2->M11 * w2) + (m3->M11 * w3) + (m4->M11 * w4);
                        transformSum.M12 = (m1->M12 * w1) + (m2->M12 * w2) + (m3->M12 * w3) + (m4->M12 * w4);
                        transformSum.M13 = (m1->M13 * w1) + (m2->M13 * w2) + (m3->M13 * w3) + (m4->M13 * w4);
                        transformSum.M21 = (m1->M21 * w1) + (m2->M21 * w2) + (m3->M21 * w3) + (m4->M21 * w4);
                        transformSum.M22 = (m1->M22 * w1) + (m2->M22 * w2) + (m3->M22 * w3) + (m4->M22 * w4);
                        transformSum.M23 = (m1->M23 * w1) + (m2->M23 * w2) + (m3->M23 * w3) + (m4->M23 * w4);
                        transformSum.M31 = (m1->M31 * w1) + (m2->M31 * w2) + (m3->M31 * w3) + (m4->M31 * w4);
                        transformSum.M32 = (m1->M32 * w1) + (m2->M32 * w2) + (m3->M32 * w3) + (m4->M32 * w4);
                        transformSum.M33 = (m1->M33 * w1) + (m2->M33 * w2) + (m3->M33 * w3) + (m4->M33 * w4);
                        transformSum.M41 = (m1->M41 * w1) + (m2->M41 * w2) + (m3->M41 * w3) + (m4->M41 * w4);
                        transformSum.M42 = (m1->M42 * w1) + (m2->M42 * w2) + (m3->M42 * w3) + (m4->M42 * w4);
                        transformSum.M43 = (m1->M43 * w1) + (m2->M43 * w2) + (m3->M43 * w3) + (m4->M43 * w4);


                        pGpuVertex->Position.X = pVertex->Position.X * transformSum.M11 + pVertex->Position.Y * transformSum.M21 + pVertex->Position.Z * transformSum.M31 + transformSum.M41;
                        pGpuVertex->Position.Y = pVertex->Position.X * transformSum.M12 + pVertex->Position.Y * transformSum.M22 + pVertex->Position.Z * transformSum.M32 + transformSum.M42;
                        pGpuVertex->Position.Z = pVertex->Position.X * transformSum.M13 + pVertex->Position.Y * transformSum.M23 + pVertex->Position.Z * transformSum.M33 + transformSum.M43;

                        pGpuVertex->Normal.X = pVertex->Normal.X * transformSum.M11 + pVertex->Normal.Y * transformSum.M21 + pVertex->Normal.Z * transformSum.M31;
                        pGpuVertex->Normal.Y = pVertex->Normal.X * transformSum.M12 + pVertex->Normal.Y * transformSum.M22 + pVertex->Normal.Z * transformSum.M32;
                        pGpuVertex->Normal.Z = pVertex->Normal.X * transformSum.M13 + pVertex->Normal.Y * transformSum.M23 + pVertex->Normal.Z * transformSum.M33;
                    }
                }                
                #endif
            }
        }
    }
}
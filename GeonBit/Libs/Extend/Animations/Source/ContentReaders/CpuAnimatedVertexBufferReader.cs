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

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using GeonBit.Extend.Graphics;

namespace GeonBit.Extend.Animation.Content
{
    public class CpuAnimatedVertexBufferReader : ContentTypeReader<CpuAnimatedVertexBuffer>
    {
        protected override CpuAnimatedVertexBuffer Read(ContentReader input, CpuAnimatedVertexBuffer buffer)
        {
            IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            var device = graphicsDeviceService.GraphicsDevice;

            // read standard VertexBuffer
            var declaration = input.ReadRawObject<VertexDeclaration>();
            var vertexCount = (int)input.ReadUInt32();
            // int dataSize = vertexCount * declaration.VertexStride;
            //byte[] data = new byte[dataSize];
            //input.Read(data, 0, dataSize);

            //read data                      
            var channels = declaration.GetVertexElements();
            var cpuVertices = new VertexIndicesWeightsPositionNormal[vertexCount];
            var gpuVertices = new VertexPositionNormalTexture[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                foreach (var channel in channels)
                {
                    switch (channel.VertexElementUsage)
                    {
                        case VertexElementUsage.Position:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector3);
                            var pos = input.ReadVector3();
                            cpuVertices[i].Position = pos;
                            gpuVertices[i].Position = pos;
                            break;

                        case VertexElementUsage.Normal:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector3);
                            var nor = input.ReadVector3();
                            cpuVertices[i].Normal = nor;
                            gpuVertices[i].Normal = nor;
                            break;

                        case VertexElementUsage.TextureCoordinate:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector2);
                            var tex = input.ReadVector2();
                            gpuVertices[i].TextureCoordinate = tex;
                            break;

                        case VertexElementUsage.BlendWeight:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector4);
                            var wei = input.ReadVector4();
                            cpuVertices[i].BlendWeights = wei;
                            break;

                        case VertexElementUsage.BlendIndices:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Byte4);
                            var i0 = input.ReadByte();
                            var i1 = input.ReadByte();
                            var i2 = input.ReadByte();
                            var i3 = input.ReadByte();
                            cpuVertices[i].BlendIndex0 = i0;
                            cpuVertices[i].BlendIndex1 = i1;
                            cpuVertices[i].BlendIndex2 = i2;
                            cpuVertices[i].BlendIndex3 = i3;
                            break;

                        default:
                            throw new Exception();
                    }
                }
            }
            

            // read extras
            bool IsWriteOnly = input.ReadBoolean();

            if (buffer == null)
            {
                BufferUsage usage = (IsWriteOnly) ? BufferUsage.WriteOnly : BufferUsage.None;
                buffer = new CpuAnimatedVertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertexCount, usage);
            }

            buffer.SetData(gpuVertices, 0, vertexCount);
            buffer.SetGpuVertices(gpuVertices);
            buffer.SetCpuVertices(cpuVertices);

            return buffer;
        }
    }
}

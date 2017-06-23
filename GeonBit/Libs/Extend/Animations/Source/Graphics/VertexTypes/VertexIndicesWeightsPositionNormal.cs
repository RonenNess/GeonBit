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

using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.Extend.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct VertexIndicesWeightsPositionNormal : IVertexType
    {        
        [FieldOffset( 0)] public byte BlendIndex0;
        [FieldOffset( 1)] public byte BlendIndex1;        
        [FieldOffset( 2)] public byte BlendIndex2;                
        [FieldOffset( 3)] public byte BlendIndex3;
        [FieldOffset( 4)] public Vector4 BlendWeights;
        [FieldOffset(20)] public Vector3 Position;
        [FieldOffset(32)] public Vector3 Normal;

        
        #region IVertexType Members
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
                new VertexElement[] 
                {                  
                    new VertexElement( 0, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
                    new VertexElement( 4, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
                    new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                });

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
        #endregion
        

        public VertexIndicesWeightsPositionNormal(Vector3 position, Vector3 normal, Vector4 blendWeights, byte blendIndex0, byte blendIndex1, byte blendIndex2, byte blendIndex3)
        {
            this.BlendIndex0 = blendIndex0;
            this.BlendIndex1 = blendIndex1;
            this.BlendIndex2 = blendIndex2;
            this.BlendIndex3 = blendIndex3;
            this.BlendWeights = blendWeights;
            this.Position = position;
            this.Normal = normal;
        }

        public override string ToString()
        {
            return string.Format("{{Position:{0} Normal:{1} BlendWeights:{2} BlendIndices:{3},{4},{5},{6} }}",
                new object[] { Position, Normal, BlendWeights, BlendIndex0, BlendIndex1, BlendIndex2, BlendIndex3 });
        }
    }
}

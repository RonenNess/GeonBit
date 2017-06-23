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
using GeonBit.Extend.Graphics;

namespace GeonBit.Extend.Animation
{
    internal static class NativeExtensions
    {        
        #if USE_NATIVE_ANIMATION

        internal static Native.Animation.VertexTypes.VertexIndicesWeightsPositionNormal ToNativeCpuVertex(this VertexIndicesWeightsPositionNormal source)
        {
            Native.Animation.VertexTypes.VertexIndicesWeightsPositionNormal result;
            //result.BlendIndices = source.BlendIndices.ToInt4Data();
            result.BlendIndices = new Native.Animation.Data.Int4Data()
            {
                X = source.BlendIndex0,
                Y = source.BlendIndex1,
                Z = source.BlendIndex2,
                W = source.BlendIndex3,
            };
            result.BlendWeights = source.BlendWeights.ToNativeVector4Data();
            result.Position = source.Position.ToNativeVector3Data();
            result.Normal = source.Normal.ToNativeVector3Data();

            return result;
        }
        
        internal static Native.Animation.Data.Vector4Data ToNativeVector4Data(this Vector4 source)
        {
            Native.Animation.Data.Vector4Data result;
            result.X = source.X;
            result.Y = source.Y;
            result.Z = source.Z;
            result.W = source.W;
            return result;
        }

        internal static Native.Animation.Data.Vector3Data ToNativeVector3Data(this Vector3 source)
        {
            Native.Animation.Data.Vector3Data result;
            result.X = source.X;
            result.Y = source.Y;
            result.Z = source.Z;
            return result;
        }

        internal static Native.Animation.Data.Vector2Data ToNativeVector2Data(this Vector2 source)
        {
            Native.Animation.Data.Vector2Data result;
            result.X = source.X;
            result.Y = source.Y;
            return result;
        }

        #endif
    }
}


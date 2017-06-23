#region License
//   Copyright 2016 Kastellanos Nikolaos
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
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using GeonBit.Content.Pipeline.Graphics;

namespace GeonBit.Content.Pipeline.Serialization
{
    [ContentTypeWriter]
    public class DynamicVertexBufferWriter : ContentTypeWriter<DynamicVertexBufferContent>
    {    
        protected override void Write(ContentWriter output, DynamicVertexBufferContent buffer)
        {            
            WriteVertexBuffer(output, buffer);

            output.Write(buffer.IsWriteOnly);

            return;
        }

        private void WriteVertexBuffer(ContentWriter output, DynamicVertexBufferContent buffer)
        {
            var vertexCount = buffer.VertexData.Length / buffer.VertexDeclaration.VertexStride;
            output.WriteRawObject(buffer.VertexDeclaration);
            output.Write((UInt32)vertexCount);
            output.Write(buffer.VertexData);
        }
                
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "GeonBit.Graphics.Content.DynamicVertexBufferReader, Aether.Graphics";
        }
        
    }
}

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

using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace GeonBit.Content.Pipeline.Graphics
{
    public class DynamicVertexBufferContent: VertexBufferContent 
    {
        protected internal VertexBufferContent Source { get; protected set; }

        public bool IsWriteOnly = false;
                
        new public VertexDeclarationContent VertexDeclaration { get { return Source.VertexDeclaration; } }

        new public byte[] VertexData { get { return Source.VertexData; } }

        public DynamicVertexBufferContent(VertexBufferContent source):base()
        {
            Source = source;
        }

        public DynamicVertexBufferContent(VertexBufferContent source, int size):base(size)
        {
            Source = source;
        }


    }
}

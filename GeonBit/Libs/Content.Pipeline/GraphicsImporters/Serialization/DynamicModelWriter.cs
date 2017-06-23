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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using GeonBit.Content.Pipeline.Graphics;

namespace GeonBit.Content.Pipeline.Serialization
{
    [ContentTypeWriter]
    class DynamicModelWriter : ContentTypeWriter<DynamicModelContent>
    {
        /// <summary>
        /// Write a Model xnb, compatible with the XNB Container Format.
        /// </summary>
        protected override void Write(ContentWriter output, DynamicModelContent model)
        {   
            WriteBones(output, model.Bones);
            WriteMeshes(output, model, model.Meshes);
            WriteBoneReference(output, model.Bones.Count, model.Source.Root);
            output.WriteObject(model.Source.Tag);

            return;
        }


        private void WriteBones(ContentWriter output, ModelBoneContentCollection bones)
        {
            var bonesCount = bones.Count;
            output.Write((UInt32)bonesCount);

            foreach (var bone in bones)
            {
                output.WriteObject(bone.Name);
                output.Write(bone.Transform);
            }

            foreach (var bone in bones)
            {
                WriteBoneReference(output, bonesCount, bone.Parent);
                
                output.Write((uint)bone.Children.Count);
                foreach (var child in bone.Children)
                    WriteBoneReference(output, bonesCount, child);
            }

            return;
        }

        // The BoneReference type varies in size depending on the number of bones in the model. 
        // If bone count is less than 255 this value is serialized as a Byte, otherwise it is UInt32. 
        // If the reference value is zero the bone is null, otherwise (bone reference - 1) is an index into the model bone list.
        private void WriteBoneReference(ContentWriter output, int bonesCount, ModelBoneContent bone)
        {
            if (bone == null)
                output.Write((byte)0);
            else if (bonesCount < 255)
                output.Write((byte)(bone.Index + 1));
            else
                output.Write((UInt32)(bone.Index + 1));
        }

        private void WriteMeshes(ContentWriter output, DynamicModelContent model, List<DynamicModelMeshContent> meshes)
        {
            output.Write((UInt32)meshes.Count);

            var bonesCount = model.Bones.Count;
            foreach (var mesh in meshes)
            {
                output.WriteObject(mesh.Name); 
                WriteBoneReference(output, bonesCount, mesh.ParentBone);
                WriteBoundingSphere(output, mesh.BoundingSphere);
                output.WriteObject(mesh.Tag);
                
                WriteParts(output, model, mesh.MeshParts);
            }

            return;
        }

        private void WriteBoundingSphere(ContentWriter output, BoundingSphere value)
        {
            output.Write(value.Center);
            output.Write(value.Radius);
        }

        private void WriteParts(ContentWriter output, DynamicModelContent model, List<DynamicModelMeshPartContent> parts)
        {
            output.Write((UInt32)parts.Count);

            foreach (var part in parts)
            {
                output.Write((UInt32)part.VertexOffset);
                output.Write((UInt32)part.NumVertices);
                output.Write((UInt32)part.StartIndex);
                output.Write((UInt32)part.PrimitiveCount);
                output.WriteObject(part.Tag);

                output.WriteSharedResource(part.VertexBuffer);
                output.WriteSharedResource(part.IndexBuffer);
                output.WriteSharedResource(part.Material);
            }

            return;
        }
        
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "Microsoft.Xna.Framework.Graphics.Model";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Microsoft.Xna.Framework.Content.ModelReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553";
        }
    }
}

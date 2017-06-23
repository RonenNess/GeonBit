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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using GeonBit.Content.Pipeline.Animation;

namespace GeonBit.Content.Pipeline.Serialization
{   
    [ContentTypeWriter]
    class AnimationsDataWriter : ContentTypeWriter<AnimationsContent>
    {
        protected override void Write(ContentWriter output, AnimationsContent value)
        {
            WriteClips(output, value.Clips);
            WriteBindPose(output, value.BindPose);
            WriteInvBindPose(output, value.InvBindPose);
            WriteSkeletonHierarchy(output, value.SkeletonHierarchy);
            WriteBoneNames(output, value.BoneNames);
        }

        private void WriteClips(ContentWriter output, Dictionary<string, ClipContent> clips)
        {
            Int32 count = clips.Count;
            output.Write((Int32)count);

            foreach (var clip in clips)
            {
                output.Write(clip.Key);
                output.WriteObject<ClipContent>(clip.Value);
            }            

            return;
        }

        private void WriteBindPose(ContentWriter output, List<Microsoft.Xna.Framework.Matrix> bindPoses)
        {
            Int32 count = bindPoses.Count;
            output.Write((Int32)count);

            for (int i = 0; i < count; i++)
                output.Write(bindPoses[i]);

            return;
        }

        private void WriteInvBindPose(ContentWriter output, List<Microsoft.Xna.Framework.Matrix> invBindPoses)
        {
            Int32 count = invBindPoses.Count;
            output.Write((Int32)count);

            for (int i = 0; i < count; i++)
                output.Write(invBindPoses[i]);

            return;
        }

        private void WriteSkeletonHierarchy(ContentWriter output, List<int> skeletonHierarchy)
        {
            Int32 count = skeletonHierarchy.Count;
            output.Write((Int32)count);

            for (int i = 0; i < count; i++)
                output.Write((Int32)skeletonHierarchy[i]);

            return;
        }
    
        private void WriteBoneNames(ContentWriter output, List<string> boneNames)
        {
            Int32 count = boneNames.Count;
            output.Write((Int32)count);
            
            for (int boneIndex = 0; boneIndex < count; boneIndex++)
            {
                var boneName = boneNames[boneIndex];
                output.Write(boneName);
            }

            return;
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "GeonBit.Extend.Animation.Animations, " +
                typeof(GeonBit.Extend.Animation.Animations).Assembly.FullName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "GeonBit.Extend.Animation.Content.AnimationsReader, " +
                typeof(GeonBit.Extend.Animation.Content.AnimationsReader).Assembly.FullName;
        }
    }
        
}

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
    class ClipWriter : ContentTypeWriter<ClipContent>
    {
        protected override void Write(ContentWriter output, ClipContent value)
        {
            WriteDuration(output, value.Duration);
            WriteKeyframes(output, value.Keyframes);
        }

        private void WriteDuration(ContentWriter output, TimeSpan duration)
        {
            output.Write(duration.Ticks);
        }

        private void WriteKeyframes(ContentWriter output, IList<KeyframeContent> keyframes)
        {
            Int32 count = keyframes.Count;
            output.Write((Int32)count);

            for (int i = 0; i < count; i++)
            {
                KeyframeContent keyframe = keyframes[i];
                output.Write(keyframe.Bone);
                output.Write(keyframe.Time.Ticks);
                output.Write(keyframe.Transform.M11);
                output.Write(keyframe.Transform.M12);
                output.Write(keyframe.Transform.M13);
                output.Write(keyframe.Transform.M21);
                output.Write(keyframe.Transform.M22);
                output.Write(keyframe.Transform.M23);
                output.Write(keyframe.Transform.M31);
                output.Write(keyframe.Transform.M32);
                output.Write(keyframe.Transform.M33);
                output.Write(keyframe.Transform.M41);
                output.Write(keyframe.Transform.M42);
                output.Write(keyframe.Transform.M43);
            }

            return;
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "GeonBit.Extend.Animation.Clip, " +
                typeof(GeonBit.Extend.Animation.Clip).Assembly.FullName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "GeonBit.Extend.Animation.Content.ClipReader, " +
                typeof(GeonBit.Extend.Animation.Content.ClipReader).Assembly.FullName;
        }
    }
    
}

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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GeonBit.Extend.Animation.Content
{
    public class AnimationsReader : ContentTypeReader<Animations>
    {
        protected override Animations Read(ContentReader input, Animations existingInstance)
        {
            Animations animations = existingInstance;

            if (existingInstance == null)
            {
                Dictionary<string, Clip> clips = ReadAnimationClips(input, null);
                List<Matrix> bindPose = ReadBindPose(input, null);
                List<Matrix> invBindPose = ReadInvBindPose(input, null);
                List<int> skeletonHierarchy = ReadSkeletonHierarchy(input, null);
                Dictionary<string, int> boneMap = ReadBoneMap(input, null);
                animations = new Animations(bindPose, invBindPose, skeletonHierarchy, boneMap, clips);
            }
            else
            {
                ReadAnimationClips(input, animations.Clips);
                ReadBindPose(input, animations._bindPose);
                ReadInvBindPose(input, animations._invBindPose);
                ReadSkeletonHierarchy(input, animations._skeletonHierarchy);
                ReadBoneMap(input, animations._boneMap);
            }

            return animations;
        }

        private Dictionary<string, Clip> ReadAnimationClips(ContentReader input, Dictionary<string, Clip> existingInstance)
        {
            Dictionary<string, Clip> animationClips = existingInstance;

            int count = input.ReadInt32();
            if (animationClips == null)
                animationClips = new Dictionary<string, Clip>(count);

            for (int i = 0; i < count; i++)
            {
                string key = input.ReadString();
                Clip val = input.ReadObject<Clip>();
                if (existingInstance == null)
                    animationClips.Add(key, val);
                else
                    animationClips[key] = val;
            }

            return animationClips;
        }

        private List<Matrix> ReadBindPose(ContentReader input, List<Matrix> existingInstance)
        {
            List<Matrix> bindPose = existingInstance;

            int count = input.ReadInt32();
            if (bindPose == null)
                bindPose = new List<Matrix>(count);

            for (int i = 0; i < count; i++)
            {
                Matrix val = input.ReadMatrix();
                if (existingInstance == null)
                    bindPose.Add(val);
                else
                    bindPose[i] = val;
            }

            return bindPose;
        }

        private List<Matrix> ReadInvBindPose(ContentReader input, List<Matrix> existingInstance)
        {
            List<Matrix> invBindPose = existingInstance;

            int count = input.ReadInt32();
            if (invBindPose == null)
                invBindPose = new List<Matrix>(count);

            for (int i = 0; i < count; i++)
            {
                Matrix val = input.ReadMatrix();
                if (existingInstance == null)
                    invBindPose.Add(val);
                else
                    invBindPose[i] = val;
            }

            return invBindPose;
        }

        private List<int> ReadSkeletonHierarchy(ContentReader input, List<int> existingInstance)
        {
            List<int> skeletonHierarchy = existingInstance;

            int count = input.ReadInt32();
            if (skeletonHierarchy == null)
                skeletonHierarchy = new List<int>(count);

            for (int i = 0; i < count; i++)
            {
                Int32 val = input.ReadInt32();
                if (existingInstance == null)
                    skeletonHierarchy.Add(val);
                else
                    skeletonHierarchy[i] = val;
            }

            return skeletonHierarchy;
        }

        private Dictionary<string, int> ReadBoneMap(ContentReader input, Dictionary<string, int> existingInstance)
        {
            Dictionary<string, int> boneMap = existingInstance;

            int count = input.ReadInt32();
            if (boneMap == null)
                boneMap = new Dictionary<string, int>(count);

            for (int boneIndex = 0; boneIndex < count; boneIndex++)
            {
                string key = input.ReadString();
                if (existingInstance == null)
                    boneMap.Add(key, boneIndex);
                else
                    boneMap[key] = boneIndex;
            }

            return boneMap;
        }
        
    }
    
}

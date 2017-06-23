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

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GeonBit.Content.Pipeline.Animation
{
    public class AnimationsContent
    {
        public List<Matrix> BindPose { get; private set; }
        public List<Matrix> InvBindPose { get; private set; }
        public List<int> SkeletonHierarchy { get; private set; }
        public List<string> BoneNames { get; private set; }
        public Dictionary<string, ClipContent> Clips { get; private set; }


        internal AnimationsContent(List<Matrix> bindPose, List<Matrix> invBindPose, List<int> skeletonHierarchy, List<string> boneNames, Dictionary<string, ClipContent> clips)
        {
            BindPose = bindPose;
            InvBindPose = invBindPose;
            SkeletonHierarchy = skeletonHierarchy;
            BoneNames = boneNames;
            Clips = clips;
        }
    }
}

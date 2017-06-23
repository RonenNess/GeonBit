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

using System.ComponentModel;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.Content.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "GPU AnimatedModel - GeonBit")]
    public class GpuAnimatedModelProcessor : ModelProcessor
    {
        private int _maxBones = SkinnedEffect.MaxBones;
        private int _generateKeyframesFrequency = 0;
        private bool _fixRealBoneRoot = false;

#if !PORTABLE
        [DisplayName("MaxBones")]
#endif
        [DefaultValue(SkinnedEffect.MaxBones)]
        public virtual int MaxBones 
        {
            get { return _maxBones; }
            set { _maxBones = value; }
        }

#if !PORTABLE
        [DisplayName("Generate Keyframes Frequency")]
#endif
        [DefaultValue(0)] // (0=no, 30=30fps, 60=60fps)
        public virtual int GenerateKeyframesFrequency
        {
            get { return _generateKeyframesFrequency; }
            set { _generateKeyframesFrequency = value; }
        }

#if !PORTABLE
        [DisplayName("Fix BoneRoot from MG importer")]
#endif
        [DefaultValue(false)]
        public virtual bool FixRealBoneRoot
        {
            get { return _fixRealBoneRoot; }
            set { _fixRealBoneRoot = value; }
        }

        [DefaultValue(MaterialProcessorDefaultEffect.SkinnedEffect)]
        public override MaterialProcessorDefaultEffect DefaultEffect
        {
            get { return base.DefaultEffect; }
            set { base.DefaultEffect = value; }
        }
        
        public GpuAnimatedModelProcessor()
        {
            DefaultEffect = MaterialProcessorDefaultEffect.SkinnedEffect;
        }

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            var animationProcessor = new AnimationsProcessor();
            animationProcessor.MaxBones = this.MaxBones;   
            animationProcessor.GenerateKeyframesFrequency = this.GenerateKeyframesFrequency;
            animationProcessor.FixRealBoneRoot = this._fixRealBoneRoot;
            var animation = animationProcessor.Process(input, context);
            
            ModelContent model = base.Process(input, context);
            model.Tag = animation;
            return model;
        }
    }
}

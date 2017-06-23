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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeonBit.Extend.Graphics;

namespace GeonBit.Extend.Animation
{
    public static class Extensions
    {
        /// <summary>
        /// Get the animations of this model, if loaded.
        /// </summary>
        /// <param name="model">Model to get animations for.</param>
        /// <param name="cloned">If true, will return a clone of the animations.</param>
        /// <returns></returns>
        public static Animations GetAnimations(this Model model, bool cloned)
        {
            var animations = model.Tag as Animations;
            if (animations == null) { throw new System.Exception("No animations data found on model. Make sure the model processor is set to an animated model in the content manager."); }
            if (cloned) { animations = animations.Clone(); }
            return animations;
        }

        /// <summary>
        /// Update mesh vertices based on animation (animate in CPU).
        /// </summary>
        /// <param name="meshPart">Mesh part to animate.</param>
        /// <param name="boneTransforms">Bone transformations.</param>
        public static void UpdateVertices(this ModelMeshPart meshPart, Matrix[] boneTransforms)
        {
            var animatedVertexBuffer = meshPart.VertexBuffer as CpuAnimatedVertexBuffer;
            if (animatedVertexBuffer == null) { throw new System.Exception("Cannot find CPU-animated data. Make sure the model processor is set to CPU-animated model in the content manager."); }
            animatedVertexBuffer.UpdateVertices(boneTransforms, meshPart.VertexOffset, meshPart.NumVertices);
        }
        
        /// <summary>
        /// Get if a given model is CPU-animated.
        /// </summary>
        public static bool IsCpuAnimated(this Model model)
        {
            var animatedVertexBuffer = model.Meshes[0].MeshParts[0].VertexBuffer as CpuAnimatedVertexBuffer;
            return animatedVertexBuffer != null;
        }
    }
}

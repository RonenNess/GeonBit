#region LICENSE
//-----------------------------------------------------------------------------
// For the purpose of making video games, educational projects or gamification,
// GeonBit is distributed under the MIT license and is totally free to use.
// To use this source code or GeonBit as a whole for other purposes, please seek 
// permission from the library author, Ronen Ness.
// 
// Copyright (c) 2017 Ronen Ness [ronenness@gmail.com].
// Do not remove this license notice.
//-----------------------------------------------------------------------------
#endregion
#region File Description
//-----------------------------------------------------------------------------
// A basic renderable skinned model.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeonBit.Extend.Animation;
using System.Collections;
using System.Collections.Specialized;

namespace GeonBit.Core.Graphics
{
    /// <summary>
    /// Callback to execute when animation start / ends.
    /// </summary>
    public delegate void AnimationCallback();

    /// <summary>
    /// Different ways we can animated skinned mesh.
    /// </summary>
    public enum AnimatedMode
    {
        /// <summary>
        /// Animate in CPU.
        /// </summary>
        CPU,

        /// <summary>
        /// Animate in GPU (by shader).
        /// </summary>
        GPU
    }

    /// <summary>
    /// Skinned animation model entity (render model as a whole, with animation clips supported).
    /// </summary>
    public class SkinnedModelEntity : CompositeModelEntity
    {
        // model animations
        Animations _animations;

        // if during animation transition, this is the "previous" transformations we transform from
        Matrix[] _transitionFromTransforms;

        // if during animation transition, this is the current transformations state
        Matrix[] _transitionCurrTransforms;

        /// <summary>
        /// If > 0f, whenever we change clip we will transition into new animation clip, in an interpolation
        /// that will take this many seconds.
        /// </summary>
        public float TransitionTime = 0.25f;

        // time to finish current clip transition.
        float _timeToFinishTransition = 0f;

        /// <summary>
        /// If true, and trying to change animation while still transitioning from previous animation, will not replace animation
        /// and lock until animation transition is complete.
        /// </summary>
        public bool LockWhileTransitioning = true;

        // if set to lock while transitioning animations, this will be the next animation to play when finish current transition.
        string _nextAnimation = null;

        /// <summary>
        /// Are we animating in CPU or in GPU?
        /// </summary>
        AnimatedMode _animateMode;

        /// <summary>
        /// Optional callback to invoke every time an animation cycle ends.
        /// </summary>
        public AnimationCallback OnAnimationEnds = null;

        /// <summary>
        /// Optional callback to invoke every time an animation cycle begins.
        /// </summary>
        public AnimationCallback OnAnimationStart = null;

        /// <summary>
        /// Create the model entity from model instance.
        /// </summary>
        /// <param name="model">Model to draw.</param>
        public SkinnedModelEntity(Model model) : base(model)
        {
            // get animations from model
            _animations = model.GetAnimations(true);
            _animateMode = model.IsCpuAnimated() ? AnimatedMode.CPU : AnimatedMode.GPU;
        }

        /// <summary>
        /// Create the model entity from asset path.
        /// </summary>
        /// <param name="path">Path of the model to load.</param>
        public SkinnedModelEntity(string path) : this(ResourcesManager.Instance.GetModel(path))
        {
        }

        /// <summary>
        /// Set current animation clip.
        /// </summary>
        /// <param name="name">Animation clip to set.</param>
        public void SetClip(string name)
        {
            // if currently transitioning and animation is locked during transition, skip
            if (LockWhileTransitioning && _timeToFinishTransition > 0f)
            {
                _nextAnimation = name;
                return;
            }

            // if have transition time, store previous transformations
            if (TransitionTime != 0f && name != CurrentClipName)
            {
                _transitionFromTransforms = _animations.AnimationTransforms.Clone() as Matrix[];
                _timeToFinishTransition = TransitionTime;
            }
            else
            {
                _transitionFromTransforms = null;
            }

            // set clip
            _animations.SetClip(name);

            // invoke callback
            OnAnimationStart?.Invoke();
        }

        /// <summary>
        /// Get the currently playing animation clip (or null if none defined).
        /// </summary>
        public string CurrentClipName
        {
            get
            {
                return _animations.CurrentClipName;
            }
        }

        /// <summary>
        /// Update animation step.
        /// </summary>
        /// <param name="elapsedTime">Elapsed game time, in seconds, since last frame.</param>
        /// <param name="worldTransformations">Entity model root transform.</param>
        public void Update(float elapsedTime, ref Matrix worldTransformations)
        {
            // update animation 
            _animations.Update(elapsedTime, true, worldTransformations);

            // if ended, invoke callback
            if (_animations.HasEnded)
            {
                OnAnimationEnds?.Invoke();
            }

            // decrease time to finish current transition
            if (_timeToFinishTransition > 0f)
            {
                _timeToFinishTransition -= elapsedTime;
                if (_timeToFinishTransition <= 0f)
                {
                    _transitionFromTransforms = null;
                    _transitionCurrTransforms = null;
                    if (_nextAnimation != null)
                    {
                        SetClip(_nextAnimation);
                        _nextAnimation = null;
                    }
                }
            }
        }

        /// <summary>
        /// Draw this model.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void DoEntityDraw(ref Matrix worldTransformations)
        {
            // if during animation transition, prepare transformations for current frame
            if (_timeToFinishTransition > 0f)
            {
                // calc current slerp time
                float slerp = 1f - _timeToFinishTransition / TransitionTime;

                // get transformations
                _transitionCurrTransforms = _animations.AnimationTransforms;
                for (int i = 0; i < _transitionCurrTransforms.Length; ++i)
                {
                    SlerpMatrix(
                        ref _transitionFromTransforms[i],
                        ref _transitionCurrTransforms[i],
                        slerp,
                        out _transitionCurrTransforms[i]);
                }
            }

            // call base draw
            base.DoEntityDraw(ref worldTransformations);
        }

        /// <summary>
        /// Interpolate between matrixes, used for animations blend.
        /// </summary>
        /// <param name="start">Start matrix.</param>
        /// <param name="end">End matrix.</param>
        /// <param name="slerpAmount">Interpolation pos.</param>
        /// <param name="result">Output matrix.</param>
        public static void SlerpMatrix(ref Matrix start, ref Matrix end, float slerpAmount, out Matrix result)
        {
            Quaternion qStart, qEnd, qResult;
            Vector3 curTrans, nextTrans, lerpedTrans, curScale, nextScale, lerpedScale;

            start.Decompose(out curScale, out qStart, out curTrans);
            end.Decompose(out nextScale, out qEnd, out nextTrans);

            Quaternion.Lerp(ref qStart, ref qEnd, slerpAmount, out qResult);
            Vector3.Lerp(ref curTrans, ref nextTrans, slerpAmount, out lerpedTrans);
            Vector3.Lerp(ref curScale, ref nextScale, slerpAmount, out lerpedScale);

            result = Matrix.CreateScale(lerpedScale)
                   * Matrix.CreateFromQuaternion(qResult)
                   * Matrix.CreateTranslation(lerpedTrans);
        }

        /// <summary>
        /// Get current bone transformations.
        /// </summary>
        private Matrix[] CurrAnimationTransform
        {
            get
            {
                // if currently transitioning between animations
                if (_timeToFinishTransition > 0f)
                {
                    return _transitionCurrTransforms;
                }
                // if no transition, just return animations transforms
                else
                {
                    return _animations.AnimationTransforms;
                }
            }
        }

        /// <summary>
        /// Draw this entity.
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void Draw(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            // call base drawing
            base.Draw(parent, ref localTransformations, ref worldTransformations);

            // animate parts by updating bones transformations
            foreach (DictionaryEntry entry in _meshes)
            {
                // get mesh
                MeshEntity mesh = entry.Value as MeshEntity;

                // iterate parts and set bones
                foreach (var part in mesh.Mesh.MeshParts)
                {

                    // animate mesh parts by chosen method
                    switch (_animateMode)
                    {
                        // animate in gpu
                        case AnimatedMode.GPU:
                            part.Effect.GetMaterial().SetBoneTransforms(CurrAnimationTransform);
                            break;

                        // animate in cpu
                        case AnimatedMode.CPU:
                            part.UpdateVertices(CurrAnimationTransform);
                            break;
                    }
                }
            }
        }
    }
}
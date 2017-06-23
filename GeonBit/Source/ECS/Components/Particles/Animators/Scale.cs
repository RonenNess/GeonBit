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
// A scale animator that change the scaling of the scene node.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Particles.Animators
{
    /// <summary>
    /// Scale animator (change scene node scaling).
    /// </summary>
    public class ScaleAnimator : BaseAnimator
    {
        /// <summary>
        /// Starting scale.
        /// </summary>
        public Vector3 FromScale { get; private set; }

        /// <summary>
        /// Ending scale.
        /// </summary>
        public Vector3 ToScale { get; private set; }

        /// <summary>
        /// How long it takes to get from starting scale to ending scale.
        /// </summary>
        public float ScalingTime { get; private set; }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            // note: unlike in other clones that try to copy the entity perfectly, in this clone we create new with jitters
            // so we'll still have the random factor applied on the cloned entity.
            return CopyBasics(new ScaleAnimator(BaseProperties, FromScale, ToScale, ScalingTime, 
                _scaleTimeJitter, _startScaleJitter, _endScaleJitter));
        }

        /// <summary>
        /// Get if this animator is done, unrelated to time to live (for example, if transition is complete).
        /// </summary>
        override protected bool IsDone
        {
            get
            {
                return TimeAnimated >= ScalingTime;
            }
        }

        // store jitters, for the purpose of cloning
        float _scaleTimeJitter = 0f;
        float _startScaleJitter = 0f;
        float _endScaleJitter = 0f;

        /// <summary>
        /// Create the scale animator.
        /// </summary>
        /// <param name="properties">Basic animator properties.</param>
        /// <param name="fromScale">Starting scale.</param>
        /// <param name="toScale">Ending scale.</param>
        /// <param name="scaleTime">How long to transition from starting to ending scale.</param>
        /// <param name="scaleTimeJitter">If provided, will add random jitter to scaling time.</param>
        /// <param name="startScaleJitter">If provided, will add random jitter to scaling starting value.</param>
        /// /// <param name="endScaleJitter">If provided, will add random jitter to scaling ending value.</param>
        public ScaleAnimator(BaseAnimatorProperties properties, Vector3 fromScale, Vector3 toScale, float scaleTime, 
            float scaleTimeJitter = 0f, float startScaleJitter = 0f, float endScaleJitter = 0f) : base(properties)
        {
            // set basic properties
            FromScale = fromScale;
            ToScale = toScale;
            ScalingTime = scaleTime;
            _endScaleJitter = endScaleJitter;
            _scaleTimeJitter = scaleTimeJitter;
            _startScaleJitter = startScaleJitter;
        }

        /// <summary>
        /// Create the scale animator.
        /// </summary>
        /// <param name="properties">Basic animator properties.</param>
        /// <param name="fromScale">Starting scale.</param>
        /// <param name="toScale">Ending scale.</param>
        /// <param name="scaleTime">How long to transition from starting to ending scale.</param>
        /// <param name="scaleTimeJitter">If provided, will add random jitter to scaling time.</param>
        /// <param name="startScaleJitter">If provided, will add random jitter to scaling starting value.</param>
        /// /// <param name="endScaleJitter">If provided, will add random jitter to scaling ending value.</param>
        public ScaleAnimator(BaseAnimatorProperties properties, float fromScale, float toScale, float scaleTime,
            float scaleTimeJitter = 0f, float startScaleJitter = 0f, float endScaleJitter = 0f) : base(properties)
        {
            // set basic properties
            FromScale = Vector3.One * fromScale;
            ToScale = Vector3.One * toScale;
            ScalingTime = scaleTime;
            _endScaleJitter = endScaleJitter;
            _scaleTimeJitter = scaleTimeJitter;
            _startScaleJitter = startScaleJitter;
        }

        /// <summary>
        /// Called when GameObject spawns.
        /// </summary>
        protected override void OnSpawn()
        {
            // add scaling time jittering
            if (_scaleTimeJitter != 0f)
            {
                
                ScalingTime += (float)Random.NextDouble() * _scaleTimeJitter;
            }
            // add scaling start jittering
            if (_startScaleJitter != 0f)
            {
                FromScale += Vector3.One * ((float)Random.NextDouble() * _startScaleJitter);
            }
            // add scaling end jittering
            if (_endScaleJitter != 0f)
            {
                ToScale += Vector3.One * ((float)Random.NextDouble() * _endScaleJitter);
            }

            // update starting scale
            _GameObject.SceneNode.Scale = FromScale;
        }

        /// <summary>
        /// The animator implementation.
        /// </summary>
        override protected void DoAnimation(float speedFactor)
        {
            // get current scaling step, and if done, skip
            float position = AnimatorUtils.CalcTransitionPercent(TimeAnimated, ScalingTime);

            // calc current scale value
            _GameObject.SceneNode.Scale = (FromScale * (1f - position)) + (ToScale * position);
        }
    }
}

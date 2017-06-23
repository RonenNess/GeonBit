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
// A fade animator that change alpha values of graphic entities.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

namespace GeonBit.ECS.Components.Particles.Animators
{
    /// <summary>
    /// Fade animator (change alpha).
    /// </summary>
    public class FadeAnimator : BaseAnimator
    {
        /// <summary>
        /// Starting alpha.
        /// </summary>
        public float FromAlpha { get; private set; }

        /// <summary>
        /// Ending alpha.
        /// </summary>
        public float ToAlpha { get; private set; }

        /// <summary>
        /// How long it takes to get from starting alpha to ending alpha.
        /// </summary>
        public float FadingTime { get; private set; }

        // store fade time jitter for cloning.
        float _fadeTimeJitter = 0f;

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            // note: unlike in other clones that try to copy the entity perfectly, in this clone we create new with jitter
            // so we'll still have the random factor applied on the cloned entity.
            return CopyBasics(new FadeAnimator(BaseProperties, FromAlpha, ToAlpha, FadingTime, _fadeTimeJitter));
        }

        /// <summary>
        /// Get if this animator is done, unrelated to time to live (for example, if transition is complete).
        /// </summary>
        override protected bool IsDone
        {
            get
            {
                return TimeAnimated >= FadingTime;
            }
        }

        /// <summary>
        /// Create the fade animator.
        /// </summary>
        /// <param name="properties">Basic animator properties.</param>
        /// <param name="fromAlpha">Starting alpha.</param>
        /// <param name="toAlpha">Ending alpha.</param>
        /// <param name="fadeTime">How long to transition from starting to ending alpha.</param>
        /// <param name="fadeTimeJitter">If provided, will add random jitter to fading time.</param>
        public FadeAnimator(BaseAnimatorProperties properties, float fromAlpha, float toAlpha, float fadeTime, float fadeTimeJitter = 0f) : base(properties)
        {
            // set basic properties
            FromAlpha = fromAlpha;
            ToAlpha = toAlpha;
            FadingTime = fadeTime;
            _fadeTimeJitter = fadeTimeJitter;
        }

        /// <summary>
        /// Called when GameObject spawns.
        /// </summary>
        protected override void OnSpawn()
        {
            // add fading time jittering
            if (_fadeTimeJitter != 0f)
            {
                FadingTime += (float)Random.NextDouble() * _fadeTimeJitter;
            }

            // update renderables starting alpha
            foreach (var renderable in ModelRenderables)
            {
                renderable.MaterialOverride.Alpha = FromAlpha;
            }
        }

        /// <summary>
        /// The animator implementation.
        /// </summary>
        override protected void DoAnimation(float speedFactor)
        {
            // get current fade step, and if done, skip
            float position = AnimatorUtils.CalcTransitionPercent(TimeAnimated, FadingTime);

            // calc current alpha value
            float alpha = (FromAlpha * (1f - position)) + (ToAlpha * position);

            // update renderables
            foreach (var renderable in ModelRenderables)
            {
                renderable.MaterialOverride.Alpha = alpha;
            }
        }
    }
}

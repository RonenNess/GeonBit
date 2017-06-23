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
// A color animator that change color of graphic entities over time.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Particles.Animators
{
    /// <summary>
    /// Color animator (change color).
    /// </summary>
    public class ColorAnimator : BaseAnimator
    {
        /// <summary>
        /// Starting color.
        /// </summary>
        public Color FromColor { get; private set; }

        /// <summary>
        /// Ending alpha.
        /// </summary>
        public Color ToColor { get; private set; }

        /// <summary>
        /// How long it takes to get from starting alpha to ending alpha.
        /// </summary>
        public float ColoringTime { get; private set; }

        // store fade time jitter for cloning.
        float _coloringTimeJitter = 0f;

        /// <summary>
        /// Optional color jitter to change the starting color.
        /// </summary>
        public Color? _startColorJitter = null;

        /// <summary>
        /// Optional color jitter to change the ending color.
        /// </summary>
        public Color? _endColorJitter = null;

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            // note: unlike in other clones that try to copy the entity perfectly, in this clone we create new with jitter
            // so we'll still have the random factor applied on the cloned entity.
            return CopyBasics(new ColorAnimator(BaseProperties, FromColor, ToColor, ColoringTime, 
                _coloringTimeJitter, _startColorJitter, _endColorJitter));
        }

        /// <summary>
        /// Get if this animator is done, unrelated to time to live (for example, if transition is complete).
        /// </summary>
        override protected bool IsDone
        {
            get
            {
                return TimeAnimated >= ColoringTime;
            }
        }

        /// <summary>
        /// Create the color animator.
        /// </summary>
        /// <param name="properties">Basic animator properties.</param>
        /// <param name="fromColor">Starting color.</param>
        /// <param name="toColor">Ending color.</param>
        /// <param name="coloringTime">How long to transition from starting to ending color.</param>
        /// <param name="colorTimeJitter">If provided, will add random jitter to fading time.</param>
        /// <param name="startColorJitter">If provided, will add random shift to starting color. For example, if color (100, 0, 0) is set, will add random 0-100 to red component.</param>
        /// <param name="endColorJitter">If provided, will add random shift to ending color. For example, if color (100, 0, 0) is set, will add random 0-100 to red component.</param>
        public ColorAnimator(BaseAnimatorProperties properties, Color fromColor, Color toColor, float coloringTime,
            float colorTimeJitter = 0f, Color? startColorJitter = null, Color? endColorJitter = null) : base(properties)
        {
            // set basic properties
            FromColor = fromColor;
            ToColor = toColor;
            ColoringTime = coloringTime;
            _coloringTimeJitter = colorTimeJitter;
            _startColorJitter = startColorJitter;
            _endColorJitter = endColorJitter;
        }

        /// <summary>
        /// Create the color animator with random colors.
        /// </summary>
        /// <param name="properties">Basic animator properties.</param>
        /// <param name="coloringTime">How long to transition from starting to ending color.</param>
        public ColorAnimator(BaseAnimatorProperties properties, float coloringTime) : 
            this(properties, Color.Black, Color.Black, coloringTime, 0f, Color.White, Color.White)
        {
        }

        /// <summary>
        /// Called when GameObject spawns.
        /// </summary>
        protected override void OnSpawn()
        {
            // add fading time jittering
            if (_coloringTimeJitter != 0f)
            {
                ColoringTime += (float)Random.NextDouble() * _coloringTimeJitter;
            }

            // add start color jittering
            if (_startColorJitter != null)
            {
                FromColor = RandColor(FromColor, _startColorJitter.Value);
            }

            // add end color jittering
            if (_endColorJitter != null)
            {
                ToColor = RandColor(ToColor, _endColorJitter.Value);
            }

            // update renderables starting alpha
            foreach (var renderable in ModelRenderables)
            {
                renderable.MaterialOverride.DiffuseColor = FromColor;
            }
        }

        /// <summary>
        /// The animator implementation.
        /// </summary>
        override protected void DoAnimation(float speedFactor)
        {
            // get current fade step, and if done, skip
            float position = AnimatorUtils.CalcTransitionPercent(TimeAnimated, ColoringTime);

            // calc current alpha value
            Color colora = (FromColor * (1f - position));
            Color colorb = (ToColor * position);
            Color finalColor = new Color((colora.R + colorb.R), (colora.G + colorb.G), (colora.B + colorb.B));

            // update renderables
            foreach (var renderable in ModelRenderables)
            {
                renderable.MaterialOverride.DiffuseColor = finalColor;
            }
        }
    }
}

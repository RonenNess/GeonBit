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
// Create base class for all particle animators.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Particles.Animators
{
    /// <summary>
    /// Animator basic properties.
    /// </summary>
    public struct BaseAnimatorProperties
    {
        /// <summary>
        /// Default properties.
        /// </summary>
        public static BaseAnimatorProperties Defaults = new BaseAnimatorProperties(speedFactor: 1f);

        /// <summary>
        /// Optional delay, in seconds, before this animator kicks in.
        /// </summary>
        public float DelayToStart { get; private set; }

        /// <summary>
        /// Optional timer until this animator is destroyed.
        /// </summary>
        public float TimeToLive { get; private set; }

        /// <summary>
        /// If true, will also destroy parent GameObject when time to live runs out.
        /// </summary>
        public bool DestroyObjectOnFinish { get; private set; }

        /// <summary>
        /// Animation intervals (creates visible animation steps).
        /// </summary>
        public float Intervals { get; private set; }

        /// <summary>
        /// Animator speed factor.
        /// </summary>
        public float SpeedFactor { get; private set; }

        /// <summary>
        /// If this animator operates on components (in oppose to Scene Node), filter which components to operate on by name.
        /// </summary>
        public string FilterTargetsByName { get; private set; }

        /// <summary>
        /// Create base animation.
        /// </summary>
        /// <param name="delayToStart">Time to wait before animator starts.</param>
        /// <param name="timeToLive">Time until the animator terminates (0f for no limit).</param>
        /// <param name="destroyObjectOnFinish">If true, will also destroy GameObject when time to live runs out.</param>
        /// <param name="intervals">If true, will only animate in these intervals.</param>
        /// <param name="speedFactor">Animator speed factor.</param>
        /// <param name="filterTargetsByName">Filter target components by name.</param>
        public BaseAnimatorProperties(float delayToStart = 0f, float timeToLive = 0f, bool destroyObjectOnFinish = false, 
            float intervals = 0f, float speedFactor = 1f, string filterTargetsByName = null)
        {
            TimeToLive = timeToLive;
            DelayToStart = delayToStart;
            DestroyObjectOnFinish = destroyObjectOnFinish;
            Intervals = intervals;
            SpeedFactor = speedFactor;
            FilterTargetsByName = filterTargetsByName;
        }
    }

    /// <summary>
    /// Base class for all particle animators.
    /// </summary>
    abstract public class BaseAnimator : BaseComponent
    {
        /// <summary>
        /// For how long, in seconds, this animator was alive (including initial delay time).
        /// </summary>
        public float TimeAlive { get; private set; }

        /// <summary>
        /// Return how long, in seconds, the animation actually works.
        /// This is just TimeAlive - DelayToStart.
        /// </summary>
        public float TimeAnimated { get { return TimeAlive - BaseProperties.DelayToStart; } }

        /// <summary>
        /// Random object to use for all animators.
        /// </summary>
        private static System.Random _rand = new System.Random();

        /// <summary>
        /// Get the random object.
        /// </summary>
        protected System.Random Random
        {
            get { return _rand; }
        }

        /// <summary>
        /// Get if this animator is done, unrelated to time to live (for example, if transition is complete).
        /// </summary>
        abstract protected bool IsDone
        {
            get;
        }

        // key used to store internal data for animators
        private static string AnimatorRenderablesInternalKey = "animator-renderables";
        private static string AnimatorModelsInternalKey = "animator-model-renderers";

        /// <summary>
        /// Get all renderable entities in an efficient way.
        /// </summary>
        protected Graphics.BaseRendererComponent[] Renderables
        {
            get
            {
                // create list of renderables to operate on (only happens first time)
                object list = _GameObject.GetInternalData(ref AnimatorRenderablesInternalKey);
                if (list == null)
                {
                    list = _GameObject.GetComponents<Graphics.BaseRendererComponent>(BaseProperties.FilterTargetsByName).ToArray();
                    _GameObject.SetInternalData(ref AnimatorRenderablesInternalKey, list);
                }

                // return list of renderables
                return list as Graphics.BaseRendererComponent[];
            }
        }

        /// <summary>
        /// Get all model renderable entities in an efficient way.
        /// </summary>
        protected Graphics.BaseRendererWithOverrideMaterial[] ModelRenderables
        {
            get
            {
                // create list of renderables to operate on (only happens first time)
                object list = _GameObject.GetInternalData(ref AnimatorModelsInternalKey);
                if (list == null)
                {
                    list = _GameObject.GetComponents<Graphics.BaseRendererWithOverrideMaterial>(BaseProperties.FilterTargetsByName).ToArray();
                    _GameObject.SetInternalData(ref AnimatorModelsInternalKey, list);
                }

                // return list of renderables
                return list as Graphics.BaseRendererWithOverrideMaterial[];
            }
        }

        // time in current interval, if we have interval set.
        private float _timeInInterval = 0f;

        /// <summary>
        /// Animator basic properties.
        /// </summary>
        public BaseAnimatorProperties BaseProperties { get; private set; }

        /// <summary>
        /// Create base animation.
        /// </summary>
        /// <param name="properties">Basic animator properties.</param>
        protected BaseAnimator(BaseAnimatorProperties properties)
        {
            // store properties
            BaseProperties = properties;

            // reset counters
            TimeAlive = 0f;
        }

        /// <summary>
        /// Reset current animator time.
        /// </summary>
        public void ResetTime()
        {
            TimeAlive = 0f;
            _timeInInterval = 0f;
        }

        /// <summary>
        /// Random vector direction.
        /// </summary>
        protected Vector3 RandDirection(Vector3 baseVector, Vector3 randDir)
        {
            return AnimatorUtils.RandDirection(Random, baseVector, randDir);
        }

        /// <summary>
        /// Random color value from base and rand color.
        /// </summary>
        protected Color RandColor(Color baseColor, Color randColor)
        {
            return AnimatorUtils.RandColor(Random, baseColor, randColor);
        }

        /// <summary>
        /// Called every frame in the Update() loop.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected override void OnUpdate()
        {
            // increase time alive
            float timeIncreased = Managers.TimeManager.TimeFactor * (BaseProperties.SpeedFactor);
            TimeAlive += timeIncreased;

            // if there's initial delay that need to expire stop here
            if (TimeAlive < BaseProperties.DelayToStart)
            {
                return;
            }

            // get if done animating
            bool finished = IsDone;

            // check if should die due to time to live
            if ((BaseProperties.TimeToLive != 0f && TimeAnimated > BaseProperties.TimeToLive) ||
                (BaseProperties.TimeToLive == 0f && BaseProperties.DestroyObjectOnFinish && finished))
            {
                // destroy parent game object if needed
                if (BaseProperties.DestroyObjectOnFinish && !_GameObject.WasDestroyed)
                {
                    _GameObject.Destroy();
                }

                // destroy self
                Destroy();

                // stop here..
                return;
            }

            // check if there's intervals to wait
            if (_timeInInterval > 0f)
            {
                _timeInInterval -= timeIncreased;
                return;
            }
            _timeInInterval = BaseProperties.Intervals;

            // finally, if we got here we need to do animation
            DoAnimation(Managers.TimeManager.TimeFactor * BaseProperties.SpeedFactor);

            // if done, disable self
            if (finished)
            {
                // disable
                Enabled = false;

                // destroy parent game object if needed
                if (BaseProperties.DestroyObjectOnFinish && !_GameObject.WasDestroyed)
                {
                    _GameObject.Destroy();
                }
            }
        }

        /// <summary>
        /// The animator implementation.
        /// </summary>
        abstract protected void DoAnimation(float timeFactor);
    }
}

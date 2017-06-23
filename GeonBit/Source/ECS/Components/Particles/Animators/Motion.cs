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
// A motion animator that change position values of scene node.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Particles.Animators
{
    /// <summary>
    /// Motion animator (change position).
    /// </summary>
    public class MotionAnimator : BaseAnimator
    {
        /// <summary>
        /// Movement vector (represent direction and speed).
        /// </summary>
        public Vector3 Velocity { get; private set; }

        /// <summary>
        /// A constant force that affect velocity over time.
        /// </summary>
        public Vector3? Acceleration { get; private set; }

        // store jitters, for cloning
        float _velocityJitter = 0f;
        float _accelerationJitter = 0f;
        Vector3? _velocityDirectionJitter = null;
        Vector3? _accelerationDirectionJitter = null;

        /// <summary>
        /// Optional max velocity (only applies if there's acceleration).
        /// </summary>
        public float MaxVelocity { get; private set; }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            // note: unlike in other clones that try to copy the entity perfectly, in this clone we create new with jitter
            // so we'll still have the random factor applied on the cloned entity.
            return CopyBasics(new MotionAnimator(BaseProperties, Velocity, Acceleration, MaxVelocity, 
                _velocityJitter, _accelerationJitter, _velocityDirectionJitter, _accelerationDirectionJitter));
        }

        /// <summary>
        /// Get if this animator is done, unrelated to time to live (for example, if transition is complete).
        /// </summary>
        override protected bool IsDone
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Create the motion animator.
        /// </summary>
        /// <param name="properties">Base animator properties.</param>
        /// <param name="velocity">Starting velocity (movement vector).</param>
        /// <param name="acceleration">Acceleration force.</param>
        /// <param name="maxVelocity">If provided, will limit max velocity.</param>
        /// <param name="velocityJitter">Add random jittering to velocity size.</param>
        /// <param name="accelerationJitter">Add random jittering to acceleration size.</param>
        /// <param name="velocityDirectionJitter">Add random direction jitter to velocity.</param>
        /// <param name="accelerationDirectionJitter">Add random direction jitter to acceleration.</param>
        public MotionAnimator(BaseAnimatorProperties properties, Vector3 velocity, Vector3? acceleration = null,
            float maxVelocity = 0f, float velocityJitter = 0f, float accelerationJitter = 0f,
            Vector3? velocityDirectionJitter = null, Vector3? accelerationDirectionJitter = null) : base(properties)
        {
            // set basic properties
            Velocity = velocity;
            Acceleration = acceleration;
            MaxVelocity = maxVelocity;
            _velocityJitter = velocityJitter;
            _accelerationJitter = accelerationJitter;
            _velocityDirectionJitter = velocityDirectionJitter;
            _accelerationDirectionJitter = accelerationDirectionJitter;
        }

        /// <summary>
        /// Called when GameObject spawns.
        /// </summary>
        protected override void OnSpawn()
        {
            // add velocity jitter
            if (_velocityJitter != 0f)
            {
                Velocity *= (float)Random.NextDouble() * _velocityJitter;
            }

            // add acceleration jitter
            if (_accelerationJitter != 0f)
            {
                Acceleration *= (float)Random.NextDouble() * _accelerationJitter;
            }

            // add velocity direction jitter
            if (_velocityDirectionJitter != null)
            {
                Velocity = RandDirection(Velocity, _velocityDirectionJitter.Value);
            }
        }

        /// <summary>
        /// The animator implementation.
        /// </summary>
        override protected void DoAnimation(float speedFactor)
        {
            // move scene node
            _GameObject.SceneNode.Position += Velocity * speedFactor;

            // add acceleration
            if (Acceleration != null && (MaxVelocity == 0f || Velocity.Length() < MaxVelocity))
            {
                Velocity += (Vector3)Acceleration * Managers.TimeManager.TimeFactor;
            }
        }
    }
}

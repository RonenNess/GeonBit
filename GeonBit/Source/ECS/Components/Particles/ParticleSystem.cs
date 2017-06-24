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
// Particles system emmiter.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GeonBit.ECS.Components.Particles
{
    /// <summary>
    /// All the basic properties of a particle type the particle system may emit.
    /// </summary>
    public struct ParticleType
    {
        /// <summary>
        /// The particle GameObject (we emit clone of these objects).
        /// </summary>
        public GameObject ParticlePrototype { get; private set; }

        /// <summary>
        /// How often to spawn particles (value range should be 0f - 1f).
        /// In every spawn event if the Frequency >= Random(0f, 1f), new particles will emit.
        /// </summary>
        public float Frequency { get; private set; }

        /// <summary>
        /// Min particles amount to create every spawn.
        /// </summary>
        public uint MinParticlesPerSpawn { get; private set; }

        /// <summary>
        /// Max particles amount to create every spawn.
        /// </summary>
        public uint MaxParticlesPerSpawn { get; private set; }

        /// <summary>
        /// How much to change frequency over time.
        /// For example, if value is -0.5, will decrease Frequency by 0.5f over the span time of 1 second.
        /// </summary>
        public float FrequencyChange { get; private set; }

        /// <summary>
        /// Get frequency with FrequencyChange applied.
        /// </summary>
        /// <param name="timeAlive">For how long the particle system was alive.</param>
        /// <returns>Actual frequency for current time.</returns>
        public float GetFrequency(float timeAlive)
        {
            return Frequency + FrequencyChange * timeAlive;
        }

        /// <summary>
        /// Create the particle type.
        /// </summary>
        /// <param name="particle">Particle object prototype.</param>
        /// <param name="frequency">Spawn frequency.</param>
        /// <param name="minCountPerSpawn">How many min particles to spawn every time.</param>
        /// <param name="maxCountPerSpawn">How many max particles to spawn every time.</param>
        /// <param name="frequencyChange">Change frequency over time.</param>
        public ParticleType(GameObject particle, float frequency = 0.01f, uint minCountPerSpawn = 1, uint maxCountPerSpawn = 1, float frequencyChange = 0f)
        {
            ParticlePrototype = particle.Clone();
            Frequency = frequency;
            MinParticlesPerSpawn = minCountPerSpawn;
            MaxParticlesPerSpawn = maxCountPerSpawn;
            FrequencyChange = frequencyChange;
        }

        /// <summary>
        /// Clone particle type.
        /// </summary>
        /// <returns>Cloned particle type.</returns>
        public ParticleType Clone()
        {
            return new ParticleType(ParticlePrototype, Frequency, MinParticlesPerSpawn, MaxParticlesPerSpawn, FrequencyChange);
        }
    }

    /// <summary>
    /// Particle system component that emit predefined particles.
    /// </summary>
    public class ParticleSystem : BaseComponent
    {
        // list of particle types
        List<ParticleType> _particles;

        // for how long this particle system exists
        float _timeAlive = 0f;

        /// <summary>
        /// If true, will add all particles to root scene node.
        /// This is useful for when the particle system moves (and you want it to affect spawning position), but you
        /// don't want the movement to move existing particles, only change spawning point.
        /// </summary>
        public bool AddParticlesToRoot = false;

        /// <summary>
        /// Spawn events intervals. If set, will only spawn particles between these intervals.
        /// </summary>
        public float Interval = 0f;

        // time until next interval
        float _timeForNextInterval = 0f;

        /// <summary>
        /// If set, will destroy self once time to live expires.
        /// </summary>
        public float TimeToLive = 0f;

        /// <summary>
        /// Speed factor (affect the particle system spawn rates).
        /// </summary>
        public float SpawningSpeedFactor = 1f;

        /// <summary>
        /// If true and tile-to-live expires, will also destroy parent Game Object.
        /// </summary>
        public bool DestroyParentWhenExpired = false;

        // for random values
        System.Random _random;

        /// <summary>
        /// Create the new particles system.
        /// </summary>
        public ParticleSystem()
        {
            _particles = new List<ParticleType>();
            _random = new System.Random();
        }

        /// <summary>
        /// Create the new particles system with a base particle type.
        /// </summary>
        /// <param name="type">First particles type in this system.</param>
        public ParticleSystem(ParticleType type) : this()
        {
            AddParticleType(type);
        }

        /// <summary>
        /// Add particle type to this particles system.
        /// </summary>
        /// <param name="type">Particle type to add.</param>
        public void AddParticleType(ParticleType type)
        {
            _particles.Add(type);
        }

        /// <summary>
        /// Called every const X seconds.
        /// </summary>
        protected override void OnFixedUpdate()
        {
            // get time factor
            float timeFactor = Managers.TimeManager.FixedTimeFactor * SpawningSpeedFactor;

            // increase time alive
            _timeAlive += timeFactor;

            // check if expired
            if (TimeToLive != 0f && _timeAlive > TimeToLive)
            {
                // destroy parent if needed (note: if destroying parent we don't need to destroy self as well)
                if (DestroyParentWhenExpired && !_GameObject.WasDestroyed)
                {
                    _GameObject.Destroy();
                    return;
                }

                // destroy self
                Destroy();
                return;
            }

            // check if there's intervals to wait
            if (_timeForNextInterval > 0f)
            {
                _timeForNextInterval -= timeFactor;
                return;
            }
            _timeForNextInterval = Interval;

            // iterate over particle types and emit them
            foreach (var particleType in _particles)
            {
                // get current spawn frequency
                float frequency = particleType.GetFrequency(_timeAlive);

                // negative? skip
                if (frequency <= 0f) { continue; }

                // check if should spawn particles
                if (frequency >= (float)_random.NextDouble())
                {
                    // rand quantity to spawn
                    uint toSpawn = (uint)_random.Next((int)particleType.MinParticlesPerSpawn, (int)particleType.MaxParticlesPerSpawn);

                    // spawn particles
                    for (int i = 0; i < toSpawn; ++i)
                    {
                        // create new particle and add to self game object
                        GameObject newPart = particleType.ParticlePrototype.Clone();
                        newPart.Parent = _GameObject;

                        // if need to add particles to root
                        if (AddParticlesToRoot)
                        {
                            Vector3 position = newPart.SceneNode.WorldPosition;
                            newPart.Parent = Managers.ActiveScene.Root;
                            newPart.SceneNode.Position = position;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            ParticleSystem ret = CopyBasics(new ParticleSystem()) as ParticleSystem;
            ret.TimeToLive = TimeToLive;
            ret.DestroyParentWhenExpired = DestroyParentWhenExpired;
            ret.Interval = Interval;
            ret.SpawningSpeedFactor = SpawningSpeedFactor;
            ret.AddParticlesToRoot = AddParticlesToRoot;
            foreach (var particleType in _particles)
            {
                ret._particles.Add(particleType.Clone());
            }
            return ret;
        }
    }
}

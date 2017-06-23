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
// Implements a simple sound effect.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GeonBit.Core.Sound
{
    /// <summary>
    /// GeonBit.Core.Sound implement sound effects and music related stuff.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Play a sound effect.
    /// </summary>
    public class SoundPlayer
    {
        // the sound effect instance
        SoundEffectInstance _sound;

        // the sound effect type
        SoundEffect _soundEffect;

        // audio emitter and listener, for when 3d audio is enabled
        AudioEmitter _emitter = null;
        AudioListener _listener = null;

        /// <summary>
        /// Get if the sound currently have 3d effect applied on it.
        /// </summary>
        public bool Is3D
        {
            get { return _emitter != null; }
        }

        /// <summary>
        /// Get the audio emitter (when using 3d sound).
        /// </summary>
        public AudioEmitter Emitter
        {
            get { return _emitter; }
        }
        
        /// <summary>
        /// Get the audio listener (when using 3d sound).
        /// </summary>
        public AudioListener Listener
        {
            get { return _listener; }
        }

        /// <summary>
        /// Get the sound effect itself.
        /// </summary>
        public SoundEffect SoundEffect
        {
            get { return _soundEffect; }
        }

        /// <summary>
        /// Sound effect volume.
        /// </summary>
        public float Volume
        {
            get { return _sound.Volume; }
            set { _sound.Volume = value; }
        }

        /// <summary>
        /// Pitch effect.
        /// </summary>
        public float Pitch
        {
            get { return _sound.Pitch; }
            set { _sound.Pitch = value; }
        }

        /// <summary>
        /// Pan effect.
        /// </summary>
        public float Pan
        {
            get { return _sound.Pan; }
            set { _sound.Pan = value; }
        }

        /// <summary>
        /// Make the sound play in loop.
        /// </summary>
        public bool IsLooped
        {
            get { return _sound.IsLooped; }
            set { _sound.IsLooped = value; }
        }

        /// <summary>
        /// Is the sound effect currently playing?
        /// </summary>
        public bool IsPlaying
        {
            get { return _sound.State == SoundState.Playing; }
        }

        /// <summary>
        /// Is the sound effect currently paused?
        /// </summary>
        public bool IsPaused
        {
            get { return _sound.State == SoundState.Paused; }
        }

        /// <summary>
        /// Is the sound effect currently stopped?
        /// </summary>
        public bool IsStopped
        {
            get { return _sound.State == SoundState.Stopped; }
        }

        /// <summary>
        /// Get sound duration.
        /// </summary>
        public System.TimeSpan Duration
        {
            get { return _soundEffect.Duration; }
        }

        /// <summary>
        /// Create the sound player.
        /// </summary>
        /// <param name="sound">Sound effect to play.</param>
        public SoundPlayer(SoundEffect sound)
        {
            _soundEffect = sound;
            _sound = sound.CreateInstance();
        }

        /// <summary>
        /// Create the sound player from asset path.
        /// </summary>
        /// <param name="path">Path of the sound effect to load.</param>
        public SoundPlayer(string path) : this(ResourcesManager.Instance.GetSound(path))
        {
        }

        /// <summary>
        /// Destroy the sound player.
        /// </summary>
        ~SoundPlayer()
        {
            _sound.Dispose();
        }

        /// <summary>
        /// Enable 3d sound effect with emitter and listener.
        /// </summary>
        public void Apply3D()
        {
            _emitter = new AudioEmitter();
            _listener = new AudioListener();
            _sound.Apply3D(_listener, _emitter);
        }

        /// <summary>
        /// Play sound effect.
        /// </summary>
        /// <param name="volumeFactor">Volume factor to multiply with currently set volume.</param>
        public void Play(float volumeFactor = 1f)
        {
            _sound.Play();
        }

        /// <summary>
        /// Stop playing the sound.
        /// </summary>
        public void Stop()
        {
            _sound.Stop();
        }

        /// <summary>
        /// Pause the sound.
        /// </summary>
        public void Pause()
        {
            _sound.Pause();
        }

    }
}

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
// Implement basic sound component.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Sound
{
    /// <summary>
    /// A sound effect you can attach to a GameObject and constantly play or play on demand.
    /// </summary>
    public class SoundEffect : BaseComponent
    {
        /// <summary>
        /// The sound player to use from the core layer.
        /// </summary>
        private Core.Sound.SoundPlayer _soundPlayer;

        /// <summary>
        /// Sound effect volume.
        /// </summary>
        public float Volume
        {
            get { return _soundPlayer.Volume; }
            set { _soundPlayer.Volume = value; }
        }

        /// <summary>
        /// Pitch effect.
        /// </summary>
        public float Pitch
        {
            get { return _soundPlayer.Pitch; }
            set { _soundPlayer.Pitch = value; }
        }

        /// <summary>
        /// Pan effect.
        /// </summary>
        public float Pan
        {
            get { return _soundPlayer.Pan; }
            set { _soundPlayer.Pan = value; }
        }

        /// <summary>
        /// If true will play this sound in a loop.
        /// </summary>
        public bool IsLooped
        {
            get { return _soundPlayer.IsLooped; }
            set { _soundPlayer.IsLooped = value; }
        }

        /// <summary>
        /// Is the sound currently playing?
        /// </summary>
        public bool IsPlaying
        {
            get { return _soundPlayer.IsPlaying; }
        }

        /// <summary>
        /// Is the sound currently paused?
        /// </summary>
        public bool IsPaused
        {
            get { return _soundPlayer.IsPaused; }
        }

        /// <summary>
        /// If true, will play this sound effect immediately on spawn event.
        /// </summary>
        public bool PlayOnSpawn = false;

        /// <summary>
        /// Is the sound currently stopped?
        /// </summary>
        public bool IsStopped
        {
            get { return _soundPlayer.IsStopped; }
        }

        /// <summary>
        /// Get sound duration.
        /// </summary>
        public System.TimeSpan Duration
        {
            get { return _soundPlayer.Duration; }
        }

        /// <summary>
        /// Optional Game Object you can set as the "listener" source for 3d sounds.
        /// If set, will decrease volume as the distance grows between this sound effect and the object.
        /// </summary>
        public GameObject Listener = null;

        /// <summary>
        /// A default listener to use for 3D sound effects that have no listener and don't use the camera as default.
        /// </summary>
        public static GameObject DefaultListener = null;

        /// <summary>
        /// Default 'Up' vector to use for 3D sound effect listeners.
        /// </summary>
        public static Vector3 DefaultListenerUp = Vector3.Up;

        /// <summary>
        /// Default 'Forward' vector to use for 3D sound effect listeners.
        /// </summary>
        public static Vector3 DefaultListenerForward = Vector3.Forward;

        /// <summary>
        /// If a listener object is set, this factor will determine how much the volume will decrease per distance unit.
        /// </summary>
        public float DistanceFadeFactor = 0.1f;

        /// <summary>
        /// If true, 'Listener' object will always be the currently active camera.
        /// </summary>
        public bool UseCameraAsListener = true;

        /// <summary>
        /// Create the Sound Effect component.
        /// </summary>
        /// <param name="sound">Path of the sound effect to play.</param>
        public SoundEffect(string sound) : this(Resources.GetSound(sound))
        {
        }

        /// <summary>
        /// Create the Sound Effect component.
        /// </summary>
        /// <param name="sound">Sound effect to play.</param>
        public SoundEffect(Microsoft.Xna.Framework.Audio.SoundEffect sound)
        {
            // create the sound player
            _soundPlayer = new Core.Sound.SoundPlayer(sound);

            // by default sound effects are not looping
            IsLooped = false;
        }

        /// <summary>
        /// Called when GameObject turned disabled.
        /// </summary>
        protected override void OnDisabled()
        {
            _soundPlayer.Stop();
        }

        /// <summary>
        /// When this sound effect spawns.
        /// </summary>
        protected override void OnSpawn()
        {
            // if set to play on spawn, play sound effect
            if (PlayOnSpawn)
            {
                Play();
            }
        }

        /// <summary>
        /// Enable 3d sound effects with emitter and listeners.
        /// </summary>
        public void Enable3D()
        {
            _soundPlayer.Apply3D();
        }

        /// <summary>
        /// Called every frame in the Update() loop.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected override void OnUpdate()
        {
            // if 3d sound is enabled
            if (_soundPlayer.Is3D)
            {
                // update emitter source
                _soundPlayer.Emitter.Position = _GameObject.SceneNode.WorldPosition;

                // get current listener
                GameObject listener = Listener ?? DefaultListener;

                // if set to always listen to active camera
                if (UseCameraAsListener)
                {
                    // set listener instance to the camera object
                    listener = Managers.ActiveScene.ActiveCamera._GameObject;
                    
                    // set effect listener Up and Forward based on camera view matrix
                    Matrix view = Managers.ActiveScene.ActiveCamera.View;
                    _soundPlayer.Listener.Up = view.Up;
                    _soundPlayer.Listener.Forward = view.Forward;
                }
                // if not using camera as listener, reset Up and Forward vectors
                else
                {
                    _soundPlayer.Listener.Up = DefaultListenerUp;
                    _soundPlayer.Listener.Forward = DefaultListenerForward;
                }

                // if we got a listener object set its position
                if (listener != null)
                {
                    _soundPlayer.Listener.Position = Listener.SceneNode.WorldPosition;
                }
                // if not, set to zero by default
                else
                {
                    _soundPlayer.Listener.Position = Vector3.Zero;
                }
            }
        }

        /// <summary>
        /// Play the sound effect.
        /// </summary>
        public void Play()
        {
            _soundPlayer.Play();
        }

        /// <summary>
        /// Pause the sound effect.
        /// </summary>
        public void Pause()
        {
            _soundPlayer.Pause();
        }

        /// <summary>
        /// Stop the sound effect.
        /// </summary>
        public void Stop()
        {
            _soundPlayer.Stop();
        }

        /// <summary>
        /// Clone this sound effect.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            SoundEffect ret = new SoundEffect(_soundPlayer.SoundEffect);
            CopyBasics(ret);
            ret.Volume = Volume;
            ret.Pitch = Pitch;
            ret.Pan = Pan;
            ret.DistanceFadeFactor = DistanceFadeFactor;
            ret.UseCameraAsListener = UseCameraAsListener;
            ret.Listener = Listener;
            ret.IsLooped = IsLooped;
            ret.PlayOnSpawn = PlayOnSpawn;
            return ret;
        }
    }
}

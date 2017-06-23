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
// Implement a component that plays background music.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Sound
{
    /// <summary>
    /// A component that plays background music.
    /// Note: having multiple active background music components will just override each other.
    /// </summary>
    public class BackgroundMusic : BaseComponent
    {
        /// <summary>
        /// The background music component currently playing.
        /// </summary>
        private static BackgroundMusic _activeBackgroundMusic = null;

        /// <summary>
        /// The background music asset path.
        /// </summary>
        public string MusicAssetPath { get; private set; }

        // current volume
        float _volume;

        /// <summary>
        /// Background music volume.
        /// </summary>
        public float Volume
        {
            get { return _volume; }
            set { _volume = value; if (IsActiveMusic) { Managers.SoundManager.MusicVolume = _volume; } }
        }

        /// <summary>
        /// Return if this background music component is the currently active background music.
        /// </summary>
        public bool IsActiveMusic
        {
            get { return _activeBackgroundMusic == this; }
        }

        // is this music plays in repeat?
        bool _isRepeating = true;

        /// <summary>
        /// If true will play this music in a loop.
        /// </summary>
        public bool IsRepeating
        {
            get { return _isRepeating; }
            set { _isRepeating = value; if (IsActiveMusic) { Managers.SoundManager.MusicRepeats = _isRepeating; } }
        }

        // return if music currently plays (not paused etc)
        bool _isPlaying = false;

        /// <summary>
        /// Return if this background music object is the music currently playing and not paused.
        /// </summary>
        public bool IsPlaying
        {
            get { return _activeBackgroundMusic == this && _isPlaying; }
        }

        /// <summary>
        /// If true, will play this music immediately on spawn event.
        /// </summary>
        public bool PlayOnSpawn = false;

        /// <summary>
        /// Create the Background Music component.
        /// </summary>
        /// <param name="song">Path of the music asset to play.</param>
        public BackgroundMusic(string song)
        {
            MusicAssetPath = song;
        }
        
        /// <summary>
        /// Called when GameObject turned disabled.
        /// </summary>
        protected override void OnDisabled()
        {
            Pause();
        }

        /// <summary>
        /// Called when GameObject turned enabled.
        /// </summary>
        protected override void OnEnabled()
        {
            Resume();
        }

        /// <summary>
        /// When this music component spawns.
        /// </summary>
        protected override void OnSpawn()
        {
            if (PlayOnSpawn)
            {
                Play();
            }
        }

        /// <summary>
        /// Play the background music (will replace currently playing music).
        /// </summary>
        public void Play()
        {
            _isPlaying = true;
            Managers.SoundManager.PlayMusic(MusicAssetPath, IsRepeating, Volume);
            _activeBackgroundMusic = this;
        }

        /// <summary>
        /// Pause the background music (will affect only if currently playing this music).
        /// </summary>
        public void Pause()
        {
            if (_activeBackgroundMusic == this)
            {
                Managers.SoundManager.PauseMusic();
            }
            _isPlaying = false;
        }

        /// <summary>
        /// Resume the music (will affect only if currently playing this music).
        /// </summary>
        public void Resume()
        {
            if (_activeBackgroundMusic == this)
            {
                Managers.SoundManager.ResumeMusic();
                _isPlaying = true;
            }
        }

        /// <summary>
        /// Stop the background music.
        /// </summary>
        public void Stop()
        {
            if (_activeBackgroundMusic == this)
            {
                Managers.SoundManager.StopMusic();
                _activeBackgroundMusic = null;
            }
            _isPlaying = false;
        }

        /// <summary>
        /// Change component parent GameObject.
        /// </summary>
        /// <param name="prevParent">Previous parent.</param>
        /// <param name="newParent">New parent.</param>
        override protected void OnParentChange(GameObject prevParent, GameObject newParent)
        {
            // stop if removed
            if (prevParent != null && newParent == null)
            {
                Stop();
            }
        }

        /// <summary>
        /// Clone this background music component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            BackgroundMusic ret = new BackgroundMusic(MusicAssetPath);
            CopyBasics(ret);
            ret.Volume = Volume;
            ret.IsRepeating = IsRepeating;
            ret.PlayOnSpawn = PlayOnSpawn;
            ret._isPlaying = _isPlaying;
            return ret;
        }
    }
}

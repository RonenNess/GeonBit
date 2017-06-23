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
// Manage game sound and provide sound-related utils.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace GeonBit.Managers
{
    /// <summary>
    /// Manage game sound and provide sound-related utils
    /// </summary>
    public class SoundManager : IManager
    {
        // singleton instance
        static SoundManager _instance = null;

        /// <summary>
        /// Get instance.
        /// </summary>
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoundManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// To make it a singleton.
        /// </summary>
        private SoundManager() { }

        /// <summary>
        /// Update manager.
        /// </summary>
        /// <param name="time">GameTime, as provided by MonoGame.</param>
        public void Update(GameTime time)
        {
        }

        /// <summary>
        /// Play a background music.
        /// </summary>
        /// <param name="name">Music content file path (must be of type song).</param>
        /// <param name="inLoop">If true, will play music in loop.</param>
        /// <param name="volume">If provided, will also set music volume. If not, will use last-set music volume.</param>
        public void PlayMusic(string name, bool inLoop = true, float? volume = null)
        {
            Song song = Core.ResourcesManager.Instance.GetSong(name);
            MusicRepeats = inLoop;
            MediaPlayer.Play(song);
            if (volume != null) { MusicVolume = volume.Value; }
        }

        /// <summary>
        /// Get / Set music volume.
        /// </summary>
        public float MusicVolume
        {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = value; }
        }

        /// <summary>
        /// Get / Set if music repeats.
        /// </summary>
        public bool MusicRepeats
        {
            get { return MediaPlayer.IsRepeating; }
            set { MediaPlayer.IsRepeating = value; }
        }

        /// <summary>
        /// Stop playing currently playing background music.
        /// </summary>
        public void StopMusic()
        {
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Pause background music.
        /// </summary>
        public void PauseMusic()
        {
            MediaPlayer.Pause();
        }

        /// <summary>
        /// Resume background music.
        /// </summary>
        public void ResumeMusic()
        {
            MediaPlayer.Resume();
        }

        /// <summary>
        /// Called every frame during the Draw() process.
        /// </summary>
        public void Draw(GameTime time)
        {
        }

        /// <summary>
        /// Init Game Utils manager.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Called every constant X seconds during the Update() phase.
        /// </summary>
        /// <param name="interval">Time since last FixedUpdate().</param>
        public void FixedUpdate(float interval)
        {
        }
    }
}

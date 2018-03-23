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
// Provide time-related utils and manage in-game time.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Managers
{
    /// <summary>
    /// Provide time-related utilities.
    /// </summary>
    public class TimeManager : IManager
    {
        // singleton instance
        static TimeManager _instance = null;

        /// <summary>
        /// How many FPS we want to get for fixed update functions.
        /// </summary>
        public float DesiredFixedFramesPerSecond = 32f;

        /// <summary>
        /// The interval, in seconds, of the FixedUpdate event.
        /// </summary>
        public float FixedUpdateInterval { get { return DesiredFixedFramesPerSecond / 1000f; } }

        /// <summary>
        /// Get time utils instance.
        /// </summary>
        public static TimeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TimeManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// To make it a singleton.
        /// </summary>
        private TimeManager() { }

        /// <summary>
        /// Init time utils manager.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// The last gametime object we got from MonoGame.
        /// </summary>
        GameTime _lastGameTime = new GameTime();

        /// <summary>
        /// Global speed factor to affect game timing.
        /// </summary>
        public float TimeSpeed = 1f;

        /// <summary>
        /// Get time factor for current frame (represent time passed in seconds since last frame, multiplied by speed).
        /// </summary>
        public float TimeFactor { get; private set; }

        /// <summary>
        /// Get time factor for fixed-update.
        /// </summary>
        public float FixedTimeFactor { get; private set; }

        /// <summary>
        /// Get time, in seconds, since last update().
        /// </summary>
        public float TimeSinceLastUpdate { get; private set; }

        /// <summary>
        /// Get total time including all frames.
        /// Note: does not affected by TimeSpeed.
        /// </summary>
        public System.TimeSpan TotalTime { get; private set; }

        /// <summary>
        /// Get elapsed time since last frame.
        /// Note: does not affected by TimeSpeed.
        /// </summary>
        public System.TimeSpan ElapsedTime { get; private set; }

        /// <summary>
        /// Get last game-time instance.
        /// </summary>
        public GameTime GameTime { get { return _lastGameTime; } }

        /// <summary>
        /// Unique increamented frame id.
        /// </summary>
        public ulong FrameId { get; private set; } = 0;

        /// <summary>
        /// Update current time.
        /// </summary>
        /// <param name="time">GameTime, as provided by MonoGame.</param>
        public void Update(GameTime time)
        {
            // store current time object
            _lastGameTime = time;

            // calculate some time factors
            TotalTime = _lastGameTime.TotalGameTime;
            ElapsedTime = _lastGameTime.ElapsedGameTime;
            TimeSinceLastUpdate = (float)_lastGameTime.ElapsedGameTime.TotalSeconds;
            TimeFactor = TimeSinceLastUpdate * TimeSpeed;
            FixedTimeFactor = FixedUpdateInterval * TimeSpeed;
            FrameId++;
        }

        /// <summary>
        /// Reset frame id.
        /// </summary>
        /// <param name="val"></param>
        public void ResetFrameId(ulong val = 0)
        {
            FrameId = val;
        }

        /// <summary>
        /// Called every frame during the Draw() process.
        /// </summary>
        public void Draw(GameTime time)
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

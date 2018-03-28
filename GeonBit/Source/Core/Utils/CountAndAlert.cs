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
// An intrusive utility to debug memory usage and function calls.
//
// Author: Ronen Ness.
// Since: 2018.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;


namespace GeonBit.Core.Utils
{
    /// <summary>
    /// A utility to count and alert events in the system for debug purposes.
    /// For example, you can use this to count how many times you create new objects per frame or minute,
    /// how many times a specific function is called, etc. You can later set threshold and invoke callbacks
    /// when specific events happen too often.
    /// Note: this tool was designed for debug purposes. 
    /// </summary>
    static class CountAndAlert
    {
        /// <summary>
        /// Enable / disable this mechanism.
        /// Note: in release mode it is always disabled.
        /// </summary>
        public static bool Enabled = true;

        /// <summary>
        /// The type of callback we register for alerts.
        /// </summary>
        /// <param name="eventType">The event that triggered the alert.</param>
        /// <param name="settings">Alert settings.</param>
        /// <param name="counts">Current counter values.</param>
        public delegate void AlertCallback(EventType eventType, AlertSettings settings, EventCounters counts);

        /// <summary>
        /// Different event types that can trigger an alert.
        /// </summary>
        public enum EventType
        {
            /// <summary>
            /// We got more than X calls per frame.
            /// </summary>
            PerFrameAlert,

            /// <summary>
            /// We got more than X calls per second.
            /// </summary>
            PerSecondAlert,

            /// <summary>
            /// We got more than X calls per custom time period.
            /// </summary>
            PerCustomTimeAlert,
        }

        /// <summary>
        /// Current counters for a specific counter.
        /// </summary>
        public struct EventCounters
        {
            /// <summary>
            /// Events call per frame.
            /// </summary>
            public ulong PerFrame;

            /// <summary>
            /// Events call per second.
            /// </summary>
            public ulong PerSecond;

            /// <summary>
            /// Events call for custom time period.
            /// </summary>
            public ulong PerCustomTime;

            /// <summary>
            /// Custom time period we can count events to trigger alert for, in seconds.
            /// </summary>
            internal float CustomTimeCount;

            /// <summary>
            /// Count stack traces (only if enabled).
            /// </summary>
            public Dictionary<string, ulong> TraceCounts;
        }

        /// <summary>
        /// Counter settings, eg how to count something.
        /// </summary>
        public class AlertSettings
        {
            /// <summary>
            /// The primary key that will be used for this counter.
            /// </summary>
            public object CounterKey = null;

            /// <summary>
            /// If true, will also add a counter for stack trace, eg count the caller functions
            /// that invoked the event.
            /// </summary>
            public bool IncludeStackCounter = false;

            /// <summary>
            /// If IncludeStackCounter is true, this will determine how far back we'll go in stack.
            /// </summary>
            public int StackCounterDepth = 1;

            /// <summary>
            /// How many times this event need to be called per frame to trigger an event.
            /// </summary>
            public ulong PerFrameAlertThreshold = 100;

            /// <summary>
            /// How many times this event need to be called per second to trigger an event.
            /// </summary>
            public ulong PerSecondAlertThreshold = 1000;

            /// <summary>
            /// How many times this event need to be called per a custom time period to trigger an event.
            /// </summary>
            public ulong PerCustomTimeAlertThreshold = 0;

            /// <summary>
            /// Custom time period we can count events to trigger alert for, in seconds.
            /// </summary>
            public float CustomTimeAlert = 7f;

            /// <summary>
            /// Callback to trigger for per-frame events.
            /// </summary>
            public AlertCallback FrameAlertHandler;

            /// <summary>
            /// Callback to trigger for per-second events.
            /// </summary>
            public AlertCallback SecondAlertHandler;

            /// <summary>
            /// Callback to trigger for per custom time period events.
            /// </summary>
            public AlertCallback TimePeriodAlertHandler;

            // current counter values.
            internal EventCounters _counters = new EventCounters();
        }

        // default alert settings
        static AlertSettings _defaultAlertSettings;

        /// <summary>
        /// If defined, will be used for all unknown / undefined counter keys.
        /// </summary>
        public static AlertSettings DefaultAlertSettings
        {
            get { return _defaultAlertSettings; }
            set { value.CounterKey = null; SetAlert(value); _defaultAlertSettings = value; }
        }

#if DEBUG && GB_DEBUG_ALERTS

        /// <summary>
        /// Dictionary to store registered alerts and their current settings.
        /// </summary>
        static Dictionary<object, AlertSettings> _alerts = new Dictionary<object, AlertSettings>();

        // create a stop watch to count time.
        static Stopwatch _watch = new Stopwatch();

        // total seconds passed.
        static double _totalSeconds = 0;

        // to count seconds.
        static double _secondCount = 0;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static CountAndAlert()
        {
            _watch.Start();
        }

        /// <summary>
        /// Reset time and clear all alerts.
        /// </summary>
        public static void Clear()
        {
            _secondCount = _totalSeconds = 0;
            _watch.Reset();
            _alerts.Clear();
        }

        /// <summary>
        /// Set alert settings for a counter.
        /// </summary>
        /// <param name="settings">Alert settings.</param>
        public static void SetAlert(AlertSettings settings)
        {
            // if include stack counters, create the stack dictionary
            if (settings.IncludeStackCounter)
                settings._counters.TraceCounts = new Dictionary<string, ulong>();

            // store settings
            _alerts[settings.CounterKey] = settings;
        }

        /// <summary>
        /// Call this function every frame.
        /// </summary>
        public static void Update()
        {
            // if disabled skip
            if (!Enabled)
                return;

            // get delta time in seconds and reset timer
            double dt = _watch.Elapsed.TotalSeconds;
            _watch.Reset();
            _watch.Start();

            // increase seconds count
            _secondCount += dt;

            // increase total counter
            _totalSeconds += dt;

            // reset alerts
            foreach (var alert in _alerts)
            {
                // reset per-frame counters
                alert.Value._counters.PerFrame = 0;

                // reset seconds counter
                if (_secondCount >= 1f)
                {
                    alert.Value._counters.PerSecond = 0;
                }

                // reset custom time period counter
                if (alert.Value.CustomTimeAlert != 0)
                {
                    alert.Value._counters.CustomTimeCount += (float)dt;
                    if (alert.Value._counters.CustomTimeCount >= alert.Value.CustomTimeAlert)
                    {
                        alert.Value._counters.PerCustomTime = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Increase a counter.
        /// </summary>
        /// <param name="key">Counter key.</param>
        /// <param name="amount">By how much to increase the counter.</param>
        public static void Count(object key, ulong amount = 1)
        {
            // if disabled skip
            if (!Enabled)
                return;

            // get alert
            AlertSettings alert;
            
            // try to get alert and if failed use default
            if (!_alerts.TryGetValue(key, out alert))
            {
                alert = DefaultAlertSettings;
            }

            // no alert no default? exception
            if (alert == null)
            {
                throw new Exception(string.Format("No alert found for key %s, and there's no default alert defined.", key.ToString()));
            }

            // increase counters
            alert._counters.PerCustomTime += amount;
            alert._counters.PerSecond += amount;
            alert._counters.PerFrame += amount;

            // if need to add stacks count, add it
            if (alert.IncludeStackCounter)
            {
                // convert stack trace to function calls, seperated by dash
                StackTrace stackTrace = new StackTrace();
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < 1 + alert.StackCounterDepth; ++i)
                {
                    // out of frames? skip
                    if (i >= stackTrace.FrameCount)
                        break;

                    // get frame method name
                    var frame = stackTrace.GetFrame(i);
                    sb.Append(frame.GetType().Name);
                    sb.Append('.');
                    sb.Append(frame.GetMethod().Name);
                    sb.Append('-');
                }
                sb.Remove(sb.Length - 1, 1);
                var stackKey = sb.ToString();

                // add stack counter
                ulong currentCount = 0;
                alert._counters.TraceCounts.TryGetValue(stackKey, out currentCount);
                alert._counters.TraceCounts[stackKey] = currentCount + 1;
            }

            // trigger per-frame alert
            if (alert.PerFrameAlertThreshold > 0 && alert._counters.PerFrame >= alert.PerFrameAlertThreshold)
                alert.FrameAlertHandler?.Invoke(EventType.PerFrameAlert, alert, alert._counters);

            // trigger per-second alert
            if (alert.PerSecondAlertThreshold > 0 && alert._counters.PerSecond >= alert.PerSecondAlertThreshold)
                alert.FrameAlertHandler?.Invoke(EventType.PerSecondAlert, alert, alert._counters);

            // trigger per-custom-period alert
            if (alert.PerCustomTimeAlertThreshold > 0 && alert._counters.PerCustomTime >= alert.PerCustomTimeAlertThreshold)
                alert.FrameAlertHandler?.Invoke(EventType.PerCustomTimeAlert, alert, alert._counters);
        }


#else
        static CountAndAlert()
        {
        }

        public static void Clear()
        {
        }

        public static void SetAlert(AlertSettings settings)
        {
        }

        public static void Update()
        {
        }

        public static void Count(object key, ulong amount = 1)
        {
        }
#endif
    }
}

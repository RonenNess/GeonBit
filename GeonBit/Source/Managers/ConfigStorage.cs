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
// Provide a basic persistent storage API, designed to store configuration data.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Specialized;
using Microsoft.Xna.Framework;

namespace GeonBit.Managers
{
    /// <summary>
    /// Provide a basic persistent storage for configuration and user-preference data.
    /// Note: the reason this manager is said to be for config and not for general purpose
    /// is because it does not guarantee good performance and it should not be used with
    /// too many (= millions) records. In addition, it has a very basic API.
    /// </summary>
    public class ConfigStorage : IManager
    {
        // singleton instance
        static ConfigStorage _instance = null;

        /// <summary>
        /// Path to hold the config storage files, relative to the GameData folder.
        /// </summary>
        public string ConfigFolderPath = "config";

        /// <summary>
        /// Max records we allow to store in cache.
        /// </summary>
        public int MaxRecordsInCache = 10000;

        /// <summary>
        /// Config files format.
        /// </summary>
        public GameFiles.FileFormats FilesFormat = GameFiles.FileFormats.Xml;

        /// <summary>
        /// If true, will use keys hash as filenames.
        /// If false, filenames will just be the keys (watch out not to give keys that cannot
        /// be a valid filename!).
        /// </summary>
        public bool UseKeysHashAsFilename = false;

        /// <summary>
        /// Get Storage instance.
        /// </summary>
        public static ConfigStorage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigStorage();
                }
                return _instance;
            }
        }

        /// <summary>
        /// To make it a true singleton.
        /// </summary>
        private ConfigStorage() { }

        // cache of loaded values
        private OrderedDictionary _cache = new OrderedDictionary();

        /// <summary>
        /// Init Storage manager.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Set config value.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="value">Value.</param>
        /// <param name="forceFormat">If provided, will use this file format instead of the currently set 'FilesFormat'.</param>
        public void Set(string key, object value, GameFiles.FileFormats? forceFormat = null)
        {
            // add to cache
            AddToCache(key, value);

            // write to file
            WriteToFile(KeyToFilename(key), value, forceFormat);
        }

        /// <summary>
        /// Get config value.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="forceFormat">If provided, will use this file format instead of the currently set 'FilesFormat'.</param>
        /// <returns>Config value, or null if undefined.</returns>
        public T Get<T>(string key, GameFiles.FileFormats? forceFormat = null)
        {
            // try to get from cache
            if (_cache.Contains(key))
            {
                return (T)_cache[key];
            }

            // load from file
            T ret = ReadFromFile<T>(KeyToFilename(key), forceFormat);

            // add to cache
            AddToCache(key, ret);

            // return value
            return ret;
        }

        /// <summary>
        /// Add value to cache.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="val">Config value.</param>
        private void AddToCache(string key, object val)
        {
            // cache disabled? skip
            if (MaxRecordsInCache == 0)
            {
                return;
            }

            // add to cache
            _cache[key] = val;

            // too many items in cache? remove random value
            if (_cache.Count > MaxRecordsInCache)
            {
                _cache.RemoveAt(0);
            }
        }

        /// <summary>
        /// Write value to persistent file.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="value">Config value.</param>
        /// <param name="format">If provided, will use this file format instead of the currently set 'FilesFormat'.</param>
        protected virtual void WriteToFile<T>(string key, T value, GameFiles.FileFormats? format = null)
        {
            GameFiles.Instance.WriteToFile(format ?? FilesFormat, key, value);
        }

        /// <summary>
        /// Read value from persistent file.
        /// </summary>
        /// <param name="key">Config key.</param>
        /// <param name="format">If provided, will use this file format instead of the currently set 'FilesFormat'.</param>
        protected virtual T ReadFromFile<T>(string key, GameFiles.FileFormats? format = null)
        {
            return GameFiles.Instance.ReadFromFile<T>(format ?? FilesFormat, key);
        }

        /// <summary>
        /// Convert key to filename.
        /// </summary>
        /// <param name="key">Key to convert.</param>
        /// <returns>Filename for this specific key.</returns>
        protected virtual string KeyToFilename(string key)
        {
            // if using keys hash for filenames, convert to hash
            if (UseKeysHashAsFilename)
            {
                key = key.GetHashCode().ToString();
            }

            // return file name
            return System.IO.Path.Combine(ConfigFolderPath, key);
        }

        /// <summary>
        /// Called every frame during the Update() phase.
        /// </summary>
        /// <param name="time">GameTime, as provided by MonoGame.</param>
        public void Update(GameTime time)
        {
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

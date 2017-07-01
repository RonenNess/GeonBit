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
// Manage external plugins.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace GeonBit.Managers
{
    /// <summary>
    /// Manage external plugins (load them on startup etc).
    /// </summary>
    public class Plugins : IManager
    {
        // singleton instance
        static Plugins _instance = null;

        /// <summary>
        /// Get plugins manager instance.
        /// </summary>
        public static Plugins Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Plugins();
                }
                return _instance;
            }
        }

        /// <summary>
        /// To make it a true singleton.
        /// </summary>
        private Plugins() { }

        /// <summary>
        /// List of loaded plugins.
        /// </summary>
        private List<string> _loadedPlugins = new List<string>();

        /// <summary>
        /// Return an array with currently loaded plugins.
        /// </summary>
        /// <returns>Array of string with loaded plugin names.</returns>
        public string[] GetPluginNames()
        {
            return _loadedPlugins.ToArray();
        }

        /// <summary>
        /// Load all the plugins.
        /// </summary>
        public void LoadAll()
        {
            // iterate over all loaded assemblies in domain
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // iterate over all types in assembly
                foreach (Type type in assembly.GetTypes())
                {
                    // if type is a GeonBit plugin init class, its a plugin to load!
                    if (type.Name == "GeonBitPluginInitializer")
                    {
                        // get plugin name
                        string name = (string)InvokePluginInitFunc(assembly, type, "GetName");

                        // init plugin
                        InvokePluginInitFunc(assembly, type, "Initialize");

                        // add to list of loaded plugins
                        _loadedPlugins.Add(name);
                    }
                }
            }
        }

        /// <summary>
        /// Call and return a static method inside a plugin initializer class.
        /// </summary>
        /// <param name="assembly">Plugin assembly.</param>
        /// <param name="type">Plugin initializer class.</param>
        /// <param name="name">Function name to invoke.</param>
        /// <returns></returns>
        private object InvokePluginInitFunc(Assembly assembly, Type type, string name)
        {
            // get the function to invoke
            MethodInfo func;
            try
            {
                func = type.GetMethod("GetName", BindingFlags.Public | BindingFlags.Static);
            }
            catch (AmbiguousMatchException)
            {
                throw new Exceptions.InternalError("Problem loading plugin from '" + assembly.FullName + "': AmbiguousMatchException in function '" + name + "'");
            }
            catch (ArgumentNullException)
            {
                throw new Exceptions.InternalError("Problem loading plugin from '" + assembly.FullName + "': ArgumentNullException in function '" + name + "'");
            }

            // not found?
            if (func == null)
            {
                throw new Exceptions.InternalError("Problem loading plugin from '" + assembly.FullName + "': function '" + name + "' not found.");
            }

            // invoke function
            try
            {
                return func.Invoke(null, null);
            }
            catch (Exception e)
            {
                throw new Exceptions.InternalError("Problem loading plugin from '" + assembly.FullName + "': got '" + e.Message + "' from function '" + name + "'");
            }
        }

        /// <summary>
        /// Init plugins manager.
        /// </summary>
        public void Initialize()
        {
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

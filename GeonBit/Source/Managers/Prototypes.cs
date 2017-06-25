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
// Manage GameObject prototypes that we can create and use.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GeonBit.Managers
{
    /// <summary>
    /// A generator function to spawn a new GameObject.
    /// This function is used when registering a function as a prototype.
    /// </summary>
    /// <returns>New GameObject instance.</returns>
    public delegate ECS.GameObject GameObjectGenerator();

    /// <summary>
    /// Manage GameObject prototypes that we can spawn.
    /// </summary>
    public class Prototypes : IManager
    {
        // singleton instance
        static Prototypes _instance = null;

        /// <summary>
        /// Get Prototypes instance.
        /// </summary>
        public static Prototypes Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Prototypes();
                }
                return _instance;
            }
        }

        // dictionary of loaded prototypes
        Dictionary<string, object> _prototypes;

        /// <summary>
        /// To make it a true singleton.
        /// </summary>
        private Prototypes() { }

        /// <summary>
        /// Init Prototypes manager.
        /// </summary>
        public void Initialize()
        {
            // create dictionary of prototypes
            _prototypes = new Dictionary<string, object>();
        }

        /// <summary>
        /// Update current time.
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
        /// Register a prototype from a GameObject instance.
        /// </summary>
        /// <param name="gameObject">GameObject to register as a prototype (note: instance will not be affected, it will be cloned).</param>
        /// <param name="name">Name from prototype. If not defined, will use GameObject name.</param>
        public void Register(ECS.GameObject gameObject, string name = null)
        {
            _prototypes[name ?? gameObject.Name ?? "untitled"] = gameObject.Clone(name, false);
        }

        /// <summary>
        /// Register a prototype from a function to generate a GameObject.
        /// </summary>
        /// <param name="generator">A function to return a new GameObject instance.</param>
        /// <param name="name">Name from prototype.</param>
        public void Register(GameObjectGenerator generator, string name)
        {
            _prototypes[name] = generator;
        }

        /// <summary>
        /// Remove a prototype.
        /// </summary>
        /// <param name="name">Name of prototype to remove.</param>
        public void Remove(string name)
        {
            _prototypes.Remove(name);
        }

        /// <summary>
        /// Spawn a prototype instance. 
        /// Note: parent of new GameObject will be null.
        /// </summary>
        /// <param name="name">Prototype identifier.</param>
        /// <returns>New GameObject instance, built from prototype.</returns>
        public ECS.GameObject Spawn(string name)
        {
            // get prototype from dictionary
            object prototype = _prototypes[name];

            // try to use as a gameobject
            ECS.GameObject asGameObject = prototype as ECS.GameObject;
            if (asGameObject != null)
            {
                return asGameObject.Clone();
            }
            // if not a game object, use as a function
            else
            {
                GameObjectGenerator func = prototype as GameObjectGenerator;
                return func();
            }
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

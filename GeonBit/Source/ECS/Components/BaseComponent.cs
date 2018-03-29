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
// Implement basic core functionality of a component.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("GameObject")]


namespace GeonBit.ECS.Components
{
    /// <summary>
    /// GeonBit.ECS.Components contain the built-in components you can attach to Game Objects.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// A GameObject component API.
    /// A component is basically something you can attach to a GameObject, like physics, collision, sound effects, model, etc.
    /// </summary>
    public abstract class BaseComponent : IComponent
    {
        /// <summary>
        /// Parent game object.
        /// </summary>
        private GameObject _gameObject = null;

        /// <summary>
        /// If true, it means this component was added to its parent in debug mode.
        /// </summary>
        public bool AsDebug = false;

        /// <summary>
        /// Get the game object this component is attached to.
        /// </summary>
        public GameObject _GameObject { get { return _gameObject; } }

        /// <summary>
        /// Get GeonBit custom content class.
        /// </summary>
        protected static GeonBit.Core.ResourcesManager Resources
        {
            get
            {
                return GeonBit.Core.ResourcesManager.Instance;
            }
        }

        /// <summary>
        /// List of frame-per events we want to test if this component has as an optimization to not iterate this component if not needed.
        /// </summary>
        private static string[] FrameBasedEvents = { "OnUpdate", "OnFixedUpdate", "OnBeforeDraw", "OnHeartbeat" };

        // did we already got spawn event?
        private bool _wasSpawned = false;

        /// <summary>
        /// Is this component currently enabled.
        /// </summary>
        public bool Enabled {
            get { return _enabled; }
            set { _enabled = value; if (value) OnEnabled(); else OnDisabled(); }
        }
        private bool _enabled = true;

        /// <summary>
        /// Count how many Component instances currently exist.
        /// Note: 'Destroy' doesn't decrease the counter, only when the real class destrcutor is called (eg object is cleared from memory) counter is decreased.
        /// </summary>
        public static uint Count { get; private set; } = 0;

        // was this component already destroyed?
        private bool _wasDestroyed = false;

        /// <summary>
        /// Create the game component.
        /// </summary>
        public BaseComponent()
        {
            Count++;
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.AddedOrCreated);
        }

        /// <summary>
        /// When destructor is called destroy component.
        /// </summary>
        ~BaseComponent()
        {
            // if not yet destroyed, destroy it now.
            // note: no need to remove from parent, if we got to destructor it means we are already removed..
            if (!_wasDestroyed)
            {
                Destroy();
            }

            // decrease instances counter
            Count--;
        }

        /// <summary>
        /// Destroy this component.
        /// </summary>
        public void Destroy()
        {
            // already destroyed? exception
            if (_wasDestroyed)
            {
                throw new Exceptions.InvalidActionException("Component already destroyed!");
            }

            // remove from parent (if got one)
            if (_gameObject != null)
            {
                RemoveFromParent();
            }

            // mark that this component was destroyed.
            _wasDestroyed = true;

            // call the event
            OnDestroyed();
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        public virtual BaseComponent Clone()
        {
            throw new System.NotImplementedException("Component 'Clone' is not implemented!");
        }

        /// <summary>
        /// Copy basic properties to another component (helper function to help with Cloning).
        /// </summary>
        /// <param name="copyTo">Other component to copy values to.</param>
        /// <returns>The object we are copying properties to.</returns>
        protected virtual BaseComponent CopyBasics(BaseComponent copyTo)
        {
            copyTo.Enabled = Enabled;
            copyTo.Name = Name;
            copyTo.AsDebug = AsDebug;
            return copyTo;
        }

        /// <summary>
        /// Update this component (called every frame parent is active).
        /// </summary>
        public void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// Just like Update(), but called every constant amount of time regardless of FPS and Vsync.
        /// </summary>
        public void FixedUpdate()
        {
            OnFixedUpdate();
        }

        /// <summary>
        /// Called every frame before the scene renders.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        internal void BeforeDraw()
        {
            OnBeforeDraw();
        }

        /// <summary>
        /// An event that triggers every X miliseconds (defined per GameObject instance).
        /// </summary>
        public void Heartbeat()
        {
            OnHeartbeat();
        }

        /// <summary>
        /// Call the spawn event of this component.
        /// </summary>
        internal void Spawn()
        {
            // only if spawn wasn't already called, invoke the spawn event.
            // note: why should spawn called twice? if a GameObject Spawn() was already called, and we add a new component to it,
            // it will call the component Spawn() immediately. So if we move components around between Spawned game objects it means
            // the Spawn() of the component may be called multiple times.
            if (!_wasSpawned)
            {
                OnSpawn();
            }

            // mark that spawn was called
            _wasSpawned = true;
        }

        /// <summary>
        /// Called when this component is effectively removed from scene, eg when removed
        /// from a GameObject or when its GameObject is removed from scene.
        /// </summary>
        internal void RemoveFromScene()
        {
            OnRemoveFromScene();
        }

        /// <summary>
        /// Called when this component is effectively added to scene, eg when added
        /// to a GameObject currently in scene or when its GameObject is added to scene.
        /// </summary>
        internal void AddToScene()
        {
            OnAddToScene();
        }

        /// <summary>
        /// Called every time scene node transformation updates.
        /// Note: this is called only if GameObject is enabled and have Update events enabled.
        /// </summary>
        internal void TransformationUpdate()
        {
            OnTransformationUpdate();
        }

        /// <summary>
        /// Change component parent GameObject.
        /// </summary>
        /// <param name="gameObject">New parent or null if removed from parent.</param>
        internal void SetParent(GameObject gameObject)
        {
            // set new parent
            GameObject prev = _gameObject;

            // check previous and new "in scene" state
            bool wasInScene = prev != null && prev.IsInScene;
            bool isInScene = gameObject != null && gameObject.IsInScene;

            // call remove-from-scene event (before setting new parent)
            if (wasInScene && !isInScene)
            {
                RemoveFromScene();
            }

            // set the new parent game object
            _gameObject = gameObject;

            // call add-to-scene event (after setting new parent)
            if (!wasInScene && isInScene)
            {
                AddToScene();
            }

            // call the event
            OnParentChange(prev, gameObject);
        }

        /// <summary>
        /// Remove this component from parent.
        /// </summary>
        public void RemoveFromParent()
        {
            if (_gameObject == null) { throw new Exceptions.InvalidActionException("Component have no parent to remove from!"); }
            _gameObject.RemoveComponent(this);
        }

        /// <summary>
        /// Called when this physical body start colliding with another body.
        /// </summary>
        /// <param name="other">The other body we collide with.</param>
        /// <param name="data">Extra collision data.</param>
        public void CollisionStart(GameObject other, Core.Physics.CollisionData data)
        {
            OnCollisionStart(other, data);
        }

        /// <summary>
        /// Called when this physical body stop colliding with another body.
        /// </summary>
        /// <param name="other">The other body we collided with, but no longer.</param>
        public void CollisionEnd(GameObject other)
        {
            OnCollisionEnd(other);
        }

        /// <summary>
        /// Called while this physical body is colliding with another body.
        /// </summary>
        /// <param name="other">The other body we are colliding with.</param>
        public void CollisionProcess(GameObject other)
        {
            OnCollisionProcess(other);
        }

        /// <summary>
        /// Send message to this component.
        /// </summary>
        /// <param name="type">Message type.</param>
        public ReceivedMessageFlowControl SendMessage(string type)
        {
            return OnReceiveMessage(type);
        }

        /// <summary>
        /// Return if this component has a method implemented.
        /// </summary>
        /// <param name="methodName">Method name to check.</param>
        /// <returns>If method exists.</returns>
        public bool HasMethodImplemented(string methodName)
        {
            // flags to get method
            var flags = System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public;

            // get method
            var type = this.GetType();

            // return true only if not null and not implemented in base class
            var method = type.GetMethod(methodName, flags);
            return (method != null && method.DeclaringType.Name != typeof(BaseComponent).Name);
        }

        /// <summary>
        /// Get a list of frame-based events this component implements.
        /// </summary>
        /// <returns>Array with the names of the implemented events.</returns>
        public string[] GetImplementedFrameBasedEvents()
        {
            var ret = new List<string>();
            foreach (var eventName in FrameBasedEvents)
            {
                if (HasMethodImplemented(eventName)) { ret.Add(eventName); }
            }
            return ret.ToArray();
        }

        /// <summary>
        /// Called when GameObject spawns.
        /// </summary>
        protected override void OnSpawn()
        {
        }

        /// <summary>
        /// Called when GameObject is destroyed.
        /// </summary>
        protected override void OnDestroyed()
        {
        }

        /// <summary>
        /// Called when GameObject turned disabled.
        /// </summary>
        protected override void OnDisabled()
        {
        }

        /// <summary>
        /// Called when GameObject is enabled.
        /// </summary>
        protected override void OnEnabled()
        {
        }

        /// <summary>
        /// Called every frame in the Update() loop.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// Called every frame before the scene renders.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected override void OnBeforeDraw()
        {
        }

        /// <summary>
        /// Called when this component is effectively removed from scene, eg when removed
        /// from a GameObject or when its GameObject is removed from scene.
        /// </summary>
        protected override void OnRemoveFromScene()
        {
        }

        /// <summary>
        /// Called when this component is effectively added to scene, eg when added
        /// to a GameObject currently in scene or when its GameObject is added to scene.
        /// </summary>
        protected override void OnAddToScene()
        {
        }

        /// <summary>
        /// An event that triggers every X miliseconds (defined per GameObject instance).
        /// </summary>
        protected override void OnHeartbeat()
        {
        }

        /// <summary>
        /// Called every constant amount of seconds in the Update() loop.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected override void OnFixedUpdate()
        {
        }

        /// <summary>
        /// Called every time scene node transformation updates.
        /// Note: this is called only if GameObject is enabled and have Update events enabled.
        /// </summary>
        protected override void OnTransformationUpdate()
        {
        }

        /// <summary>
        /// Called when the parent Game Object start colliding with another object.
        /// </summary>
        /// <param name="other">The other object we collide with.</param>
        /// <param name="data">Extra collision data.</param>
        protected override void OnCollisionStart(GameObject other, Core.Physics.CollisionData data)
        {
        }

        /// <summary>
        /// Called when the parent Game Object stop colliding with another object.
        /// </summary>
        /// <param name="other">The other object we collided with, but no longer.</param>
        protected override void OnCollisionEnd(GameObject other)
        {
        }

        /// <summary>
        /// Called every frame while the parent Game Object is colliding with another object.
        /// </summary>
        /// <param name="other">The other object we are colliding with.</param>
        protected override void OnCollisionProcess(GameObject other)
        {
        }

        /// <summary>
        /// Called when parent GameObject changes (after the change).
        /// </summary>
        /// <param name="prevParent">Previous parent.</param>
        /// <param name="newParent">New parent.</param>
        protected override void OnParentChange(GameObject prevParent, GameObject newParent)
        {
        }

        /// <summary>
        /// Called when someone send message to the parent GameObject.
        /// Note: this function is used for scripting and only user scripts-related components should respond to this.
        /// </summary>
        /// <param name="type">Message type.</param>
        protected override ReceivedMessageFlowControl OnReceiveMessage(string type)
        {
            return ReceivedMessageFlowControl.Continue;
        }
    }
}

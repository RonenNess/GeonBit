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
// A basic GameObject.
// Similar to the Unity engine, a game object have the following:
// 1. Scene Node (eg a place in the scene).
// 2. Components, like collision, model, graphics, etc.
// 3. Scripts.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GeonBit.ECS
{
    /// <summary>
    /// GeonBit.ECS implements the Entity-Component-System part of the GeonBit engine.
    /// This is the namespace you will use most of the time; all the components and game objects are defined here.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SceneNodeType
    {
        /// <summary>
        /// A simple node without any culling (will always draw, unless parent is culled).
        /// </summary>
        Simple,

        /// <summary>
        /// Scene node that cull using bounding-box and camera frustom.
        /// </summary>
        BoundingBoxCulling,

        /// <summary>
        /// Scene node that cull using bounding-sphere and camera frustom. TBD
        /// </summary>
        BoundingSphereCulling,

        /// <summary>
        /// Scene node to use for particles. TBD
        /// </summary>
        ParticlesNode,

        /// <summary>
        /// Scene node with octree culling. TBD
        /// </summary>
        OctreeCulling,
    }

    /// <summary>
    /// A callback to call on Update function.
    /// </summary>
    /// <param name="go">Game object callback was invoked from.</param>
    public delegate void OnUpdateCallback(GameObject go);

    /// <summary>
    /// Basic Game Object.
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// Object name (don't have to be unique).
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; _sceneNode.Identifier = "_" + value; }
        }
        string _name;

        /// <summary>
        /// Default scene node to use when no node type is provided in GameObject constructor.
        /// </summary>
        public static SceneNodeType DefaultSceneNodeType = SceneNodeType.Simple;

        /// <summary>
        /// Optional list of callbacks to call on update loop.
        /// This provide a quick method to add per-object extra functionality without having to define a whole component type.
        /// </summary>
        List<OnUpdateCallback> _onUpdate = null;

        // did we already got a 'spawn' event?
        bool _wasSpawned = false;

        /// <summary>
        /// If true, will call this GameObject Update() and FixedUpdate() even when not visible (outside camera / culled).
        /// If false, will not call Update() and FixedUpdate() when outside camera boundaries.
        /// </summary>
        public bool UpdateWhenNotVisible = true;

        /// <summary>
        /// Are we currently in scene (eg a decandent of the scene root).
        /// </summary>
        protected bool _isInScene = false;

        /// <summary>
        /// How often, in seconds, to trigger "heartbeat" event.
        /// For example, 0.1f means heartbeat every 100 ms.
        /// </summary>
        public float HeartbeatInterval = 0.1f;

        /// <summary>
        /// Time, in seconds, until next heartbeat.
        /// </summary>
        private float _timeForHeartbeat = 0f;

        /// <summary>
        /// Scene node last transformation version.
        /// Used to spawn TransformUpdate events.
        /// </summary>
        private uint _lastNodeTransformVersion = 0;

        /// <summary>
        /// Get the currently active scene instance.
        /// </summary>
        public GameScene ActiveScene
        {
            get { return Managers.Application.Instance.ActiveScene; }
        }

        /// <summary>
        /// The scene currently containing this game object.
        /// </summary>
        protected GameScene _parentScene;

        /// <summary>
        /// Get the parent scene of this game object.
        /// </summary>
        public GameScene ParentScene
        {
            get { return _parentScene; }
        }

        /// <summary>
        /// Return if this object is inside the currently active scene.
        /// </summary>
        public bool IsInActiveScene
        {
            get { return _parentScene != null && Managers.Application.Instance.ActiveScene == _parentScene; }
        }

        /// <summary>
        /// The graphics scene node of this game object (3d transformations).
        /// </summary>
        protected Core.Graphics.Node _sceneNode;

        /// <summary>
        /// List of components attached to this object.
        /// </summary>
        protected List<Components.BaseComponent> _components = new List<Components.BaseComponent>();

        /// <summary>
        /// Child Game Objects.
        /// </summary>
        protected List<GameObject> _children = new List<GameObject>();

        /// <summary>
        /// If true, will disable update events (Update and FixedUpdate) for this game object and all its children and components.
        /// This optimization is useful for containers that hold a lot of graphic entities, but don't have any updating logic.
        /// </summary>
        public bool DisableUpdateEvents = false;

        /// <summary>
        /// Optional data you can attach to this game object.
        /// Note: this data will not be serialized / deserialized.
        /// </summary>
        public object UserData = null;

        /// <summary>
        /// Internal dictionary of attached data.
        /// </summary>
        private Dictionary<string, object> _internalData = null;

        /// <summary>
        /// An alias to get the first physical body added to this GameObject.
        /// This is useful for performance and ease of access, since Physical Body is something that you often need to access.
        /// </summary>
        public ECS.Components.Physics.BasePhysicsComponent PhysicalBody { get; private set; }

        /// <summary>
        /// Count how many GameObject instances currently exist.
        /// Note: 'Destroy' doesn't decrease the counter, only when the real class destrcutor is called (eg object is cleared from memory) counter is decreased.
        /// </summary>
        public static uint Count { get; private set; } = 0;

        /// <summary>
        /// Iterating children / components is trickey, because if a component or child is removed / added inside an update
        /// loop etc, we'll get an exception of list change during ForEach().
        /// To handle this, we keep a cache of the last known components and children arrays, and we iterate on those while adding /
        /// removing from the original lists.
        /// </summary>
        private Components.BaseComponent[] _componentsArray;
        private bool _needComponentsArrayUpdate = true;
        private GameObject[] _childrenArray;
        private bool _needChildrenArrayUpdate = true;  

        /// <summary>
        /// Get / Set the parent of this GameObject.
        /// </summary>
        public GameObject Parent
        {
            get { return _parent; }
            set { SetParent(value); }
        }
        private GameObject _parent = null;

        /// <summary>
        /// Get if this object was destroyed and should not be used anymore.
        /// </summary>
        public bool WasDestroyed
        {
            get { return _destroyed; }
        }
        private bool _destroyed = false;

        /// <summary>
        /// Get / set object visibility.
        /// Invisible objects won't render, but will still update components.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; OnVisibleChange(); }
        }
        private bool _visible = true;

        /// <summary>
        /// Enable / disable this object.
        /// Disabled objects will not update any components and will not render.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; OnEnabledChange(); }
        }
        private bool _enabled = true;

        /// <summary>
        /// Get if this Game Object currently belong to any scene.
        /// </summary>
        public bool IsInScene
        {
            get { return _isInScene; }
        }

        /// <summary>
        /// Bounding box to use for octree culling.
        /// If you use octree culling scene node, objects that exceed this bounding box won't be culled properly.
        /// </summary>
        public static BoundingBox OctreeSceneBoundaries = new BoundingBox(Vector3.One * -1000, Vector3.One * 1000);

        /// <summary>
        /// How many times we can divide octree nodes, until reaching the minimum bounding box size.
        /// </summary>
        public static uint OctreeMaxDivisions = 5;

        /// <summary>
        /// Create the game object.
        /// </summary>
        /// <param name="name">Object name.</param>
        /// <param name="nodeType">Optional scene node type (uses DefaultSceneNodeType if not provided).</param>
        public GameObject(string name="", SceneNodeType? nodeType = null)
        {
            // set default scene node type
            nodeType = nodeType ?? DefaultSceneNodeType;

            // create the scene node
            switch (nodeType.Value)
            {
                // a simple scene node without culling
                case SceneNodeType.Simple:
                    _sceneNode = new Core.Graphics.Node();
                    break;

                // scene node with bounding-box culling
                case SceneNodeType.BoundingBoxCulling:
                    _sceneNode = new Core.Graphics.BoundingBoxCullingNode();
                    break;

                // scene node with bounding-sphere culling
                case SceneNodeType.BoundingSphereCulling:
                    _sceneNode = new Core.Graphics.BoundingSphereCullingNode();
                    break;

                // scene node optimized for particles
                case SceneNodeType.ParticlesNode:
                    _sceneNode = new Core.Graphics.ParticleNode();
                    break;

                // scene node with octree-based culling
                case SceneNodeType.OctreeCulling:
                    _sceneNode = new Core.Graphics.OctreeCullingNode(OctreeSceneBoundaries, OctreeMaxDivisions);
                    break;

                // unknown type
                default:
                    throw new Exceptions.UnsupportedTypeException("Unknown or unsupported scene node type!");
            }

            // increase instances counter
            Count++;

            // count the event
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.AddedOrCreated);

            // set name (must come after creating scene node because it also set the node identifier)
            Name = name;
        }

        /// <summary>
        /// Create the game object from scene node and name.
        /// Used internally for cloning.
        /// </summary>
        /// <param name="name">GameObject name.</param>
        /// <param name="node">Scene node (will be cloned).</param>
        /// <param name="cloneNode">If true, will clone the scene node. If false, will take it as-is</param>
        internal GameObject(string name, Core.Graphics.Node node, bool cloneNode = true)
        {
            // clone scene node and set name
            _sceneNode = cloneNode ? node.Clone() : node;
            Name = name;

            // count the event
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.AddedOrCreated);

            // increase instances counter
            Count++;
        }

        /// <summary>
        /// Destroy this GameObject.
        /// </summary>
        ~GameObject()
        {
            // if still alive, destroy this GameObject.
            if (!_destroyed)
            {
                Destroy();
            }

            // decrease instances counter
            Count--;
        }

        /// <summary>
        /// Create an octree object with params.
        /// </summary>
        /// <param name="origin">Octree center.</param>
        /// <param name="size">Octree size.</param>
        /// <param name="maxDivisions">Max number of subdivisions.</param>
        /// <returns></returns>
        public static GameObject CreateOctree(Vector3 origin, Vector3 size, uint maxDivisions)
        {
            Vector3 halfSize = size * 0.5f;
            BoundingBox bb = new BoundingBox(origin - halfSize, origin + halfSize);
            Core.Graphics.Node sceneNode = new Core.Graphics.OctreeCullingNode(bb, maxDivisions);
            return new GameObject("octree", sceneNode, false);
        }

        /// <summary>
        /// Attach internal data to game object.
        /// </summary>
        /// <param name="key">Internal data key.</param>
        /// <param name="value">Internal data value.</param>
        internal void SetInternalData(ref string key, object value)
        {
            // create internal data dictionary if needed
            if (_internalData == null)
            {
                _internalData = new Dictionary<string, object>();
            }

            // set value
            _internalData[key] = value;
        }

        /// <summary>
        /// Get internal data from game object.
        /// </summary>
        /// <param name="key">Internal data key.</param>
        internal object GetInternalData(ref string key)
        {
            // if internal data does not exist, return null
            if (_internalData == null)
            {
                return null;
            }

            // get value
            return _internalData[key];
        }

        /// <summary>
        /// Clone this game object.
        /// </summary>
        /// <param name="newName">Name to give to the cloned game object.</param>
        /// <param name="copyParent">If true, will also copy parent.</param>
        /// <returns>Clonsed game object.</returns>
        public GameObject Clone(string newName = null, bool copyParent = false)
        {
            // cannot clone destroyed objects
            if (_destroyed)
            {
                throw new Exceptions.InvalidActionException("Cannot clone destroyed objects!");
            }

            // cannot clone root
            if (IsRoot)
            {
                throw new Exceptions.InvalidActionException("Cannot clone root GameObject!");
            }

            // create child object
            GameObject ret = new GameObject(newName ?? Name, SceneNode);

            // copy user data
            ret.UserData = UserData;

            // copy disabled update events flag
            ret.DisableUpdateEvents = DisableUpdateEvents;

            // prepare lists
            PrepareChildrenAndComponentsArray();

            // clone all components
            foreach (var component in _componentsArray)
            {
                if (component.AsDebug)
                {
                    ret.AddComponentDebug(component.Clone());
                }
                else
                {
                    ret.AddComponent(component.Clone());
                }
            }

            // clone all children
            foreach (var child in _childrenArray)
            {
                GameObject childCopy = child.Clone(null, false);
                childCopy.Parent = ret;
            }

            // set some properties
            ret.Enabled = Enabled;
            ret.Visible = Visible;

            // set parent
            if (copyParent)
            {
                ret.Parent = Parent;
            }

            // copy the custom on-update callbacks
            if (_onUpdate != null)
            {
                ret._onUpdate = new List<OnUpdateCallback>(_onUpdate);
            }

            // return the newly cloned game object
            return ret;
        }

        /// <summary>
        /// Get the scene node of this Game Object.
        /// </summary>
        public Core.Graphics.Node SceneNode
        {
            get { return _sceneNode; }
        }

        /// <summary>
        /// Get if this game object is the root of the currently loaded scene.
        /// </summary>
        public bool IsRoot
        {
            get { return _parentScene != null && _parentScene.Root == this; }
        }

        /// <summary>
        /// Get if should call update / fixed update for this game object in this frame.
        /// </summary>
        bool ShouldUpdateThisFrame
        {
            get
            {
                // if disabled, should not update.
                if (!_enabled) { return false; }

                // if disabled update events, should not update
                if (DisableUpdateEvents) { return false; }

                // if culled and set not to update when not visible, should not update
                if (!UpdateWhenNotVisible && !_sceneNode.WasDrawnThisFrame) { return false; }

                // should update
                return true;
            }
        }

        /// <summary>
        /// Called every frame to do GameObject events.
        /// </summary>
        public void Update()
        {
            // if should not update, skip
            if (!ShouldUpdateThisFrame)
            {
                return;
            }

            // check if we need to trigger transformations update event
            if (_lastNodeTransformVersion != SceneNode.TransformVersion)
            {
                _lastNodeTransformVersion = SceneNode.TransformVersion;
                OnTransformationUpdate();   
            }

            // prepare children and components array before iteration
            PrepareChildrenAndComponentsArray();

            // update all components
            // note: the new list is to prevent exception if a component is added / removed from inside the update.
            foreach (var component in _componentsArray)
            {
                if (!component.Enabled) { continue; }
                component.Update();
                if (_destroyed) return;
            }

            // prepare children and components array before iteration
            PrepareChildrenAndComponentsArray();

            // update all child Game Objects
            // note: the new list is to prevent exception if a child is added / removed from inside the update.
            foreach (var child in _childrenArray)
            {
                if (!child.Enabled) { continue; }
                child.Update();
                if (_destroyed) return;
            }

            // if we got heartbeat interval handle heartbeat events
            if (HeartbeatInterval > 0f)
            {
                // decrease time until next heartbeat
                _timeForHeartbeat -= Managers.TimeManager.Instance.TimeFactor;

                // invoke heartbeat events and reset timer
                while (_timeForHeartbeat <= 0f)
                {
                    _timeForHeartbeat += HeartbeatInterval;
                    Heartbeat();
                }
            }

            // call the custom callbacks
            if (_onUpdate != null)
            {
                foreach (var callback in _onUpdate)
                {
                    callback(this);
                }
            }
        }

        /// <summary>
        /// Triggers a Fixed Update event (update that happens every const amount of seconds).
        /// </summary>
        public void FixedUpdate()
        {
            // if should not update, skip
            if (!ShouldUpdateThisFrame)
            {
                return;
            }

            // prepare children and components array before iteration
            PrepareChildrenAndComponentsArray();

            // call FixedUpdate in all components.
            // note: the new list is to prevent exception if a component is added / removed from inside the update.
            foreach (var component in _componentsArray)
            {
                if (!component.Enabled) { continue; }
                component.FixedUpdate();
                if (_destroyed) return;
            }

            // prepare children and components array before iteration
            PrepareChildrenAndComponentsArray();

            // update all child Game Objects
            // note: the new list is to prevent exception if a child is added / removed from inside the update.
            foreach (var child in _childrenArray)
            {
                child.FixedUpdate();
                if (_destroyed) return;
            }
        }

        /// <summary>
        /// Called when the scene node transformations update.
        /// </summary>
        private void OnTransformationUpdate()
        {
            foreach (var component in _componentsArray)
            {
                if (!component.Enabled) { continue; }
                component.TransformationUpdate();
            }
        }

        /// <summary>
        /// Register a function to call on every update loop.
        /// This is a quick method to add per-object functionality without defining a component type.
        /// </summary>
        /// <param name="callback">Callback to call every Update() frame.</param>
        public void DoOnUpdate(OnUpdateCallback callback)
        {
            // if needed, create the on-update callbacks list
            if (_onUpdate == null)
            {
                _onUpdate = new List<OnUpdateCallback>();
            }

            // add callback
            _onUpdate.Add(callback);
        }

        /// <summary>
        /// Trigger a heartbeat event.
        /// </summary>
        public void Heartbeat()
        {
            // call heartbeat in all components.
            // note: the new list is to prevent exception if a component is added / removed from inside the update.
            foreach (var component in _componentsArray)
            {
                component.Heartbeat();
            }
        }

        /// <summary>
        /// Called every frame to do updates right before drawing scene.
        /// </summary>
        public void BeforeDraw()
        {
            // if disabled or not currently visible, skip
            if (!_sceneNode.WasDrawnThisFrame)
            {
                return;
            }

            // update all components
            foreach (var component in _components)
            {
                component.BeforeDraw();
            }

            // update all child Game Objects
            foreach (var child in _children)
            {
                child.BeforeDraw();
            }
        }

        /// <summary>
        /// Return true only if this GameObject and all his parents are visible.
        /// </summary>
        public bool IsActuallyVisible
        {
            get
            {
                // starting from this GameObject, iterate over parensts until hitting null
                GameObject curr = this;
                while (curr != null)
                {
                    // if current GameObject is not visible, return false.
                    if (!curr.Visible)
                    {
                        return false;
                    }

                    // get next parent
                    curr = curr._parent;
                }

                // if got here it means its visible
                return true;
            }
        }

        /// <summary>
        /// Return true only if this GameObject and all his parents are enabled.
        /// </summary>
        public bool IsActuallyEnabled
        {
            get
            {
                // starting from this GameObject, iterate over parensts until hitting null
                GameObject curr = this;
                while (curr != null)
                {
                    // if current GameObject is not enabled, return false.
                    if (!curr.Enabled)
                    {
                        return false;
                    }

                    // get next parent
                    curr = curr._parent;
                }

                // if got here it means its enabled
                return true;
            }
        }

        /// <summary>
        /// Set the parent of this GameObject.
        /// </summary>
        /// <param name="newParent">New parent GameObject, or null if turn orphan.</param>
        protected void SetParent(GameObject newParent)
        {
            // sanity check - make sure its not root
            if (IsRoot && _parent != newParent)
            {
                throw new Exceptions.InvalidActionException("Cannot set parent to root scene node!");
            }

            // sanity check - make sure not self
            if (newParent == this)
            {
                throw new Exceptions.InvalidActionException("GameObject cannot be its own parent!");
            }

            // if we had previous parent, remove self from its children list
            if (_parent != null)
            {
                _parent._needChildrenArrayUpdate = true;
                _parent._children.Remove(this);
                if (_sceneNode.Parent != null) { _sceneNode.RemoveFromParent(); }
                _parentScene = null;
            }

            // set new parent pointer
            _parent = newParent;

            // if we got a new parent, add self to its children list
            if (_parent != null)
            {
                _parent._needChildrenArrayUpdate = true;
                _parent._children.Add(this);
                _parent._sceneNode.AddChildNode(_sceneNode);
                _parentScene = _parent._parentScene;
            }

            // update node visibility
            UpdateSceneNodeVisibility();

            // if parent was spawned and we got to active scene, call spawn event
            if (!_wasSpawned && _parent != null && _parent._wasSpawned && IsInActiveScene)
            {
                CallSpawnEvent();
            }

            // recalculate if we are currently in scene
            bool isInSceneNow = _parent != null && _parent._isInScene;

            // if in-in-scene changed (eg was just added / removed from scene):
            if (isInSceneNow != _isInScene)
            {
                // update if in scene now
                _isInScene = isInSceneNow;

                // if was added to scene
                if (isInSceneNow)
                {
                    OnAddToScene();
                }
                // if was removed from scene
                else
                {
                    OnRemoveFromScene();
                }
            }
        }

        /// <summary>
        /// Called when this game object or one of its ancesstors are added to scene.
        /// </summary>
        internal void OnAddToScene()
        {
            // update is-in-scene
            _isInScene = true;

            // update all components
            foreach (var component in _components)
            {
                component.AddToScene();
            }

            // update children
            foreach (var child in _children)
            {
                child._parentScene = _parentScene;
                child.OnAddToScene();
            }
        }

        /// <summary>
        /// Called when this game object or one of its ancesstors are removed from scene.
        /// </summary>
        protected void OnRemoveFromScene()
        {
            // update is-in-scene
            _isInScene = false;

            // update all components
            foreach (var component in _components)
            {
                component.RemoveFromScene();
            }

            // update children
            foreach (var child in _children)
            {
                child.OnRemoveFromScene();
            }
        }

        /// <summary>
        /// Find a child GameObject.
        /// </summary>
        /// <param name="name">Child GameObject name to look for.</param>
        /// <param name="recursive">If true, will also search inside children recursively.</param>
        /// <returns>Child GameObject, or null if not found.</returns>
        public GameObject Find(string name, bool recursive = true)
        {
            // iterate children
            foreach (GameObject child in _children)
            {
                // search in direct children
                if (child.Name == name)
                {
                    return child;
                }

                // recursive search
                if (recursive)
                {
                    GameObject foundInChild = child.Find(name, recursive);
                    if (foundInChild != null)
                    {
                        return foundInChild;
                    }
                }
            }

            // if got here it means we didn't find any child GameObject with given identifier
            return null;
        }

        /// <summary>
        /// Get component by type / name.
        /// </summary>
        /// <param name="name">If provided, will get component by type and name.</param>
        /// <typeparam name="CompType">Type of component to get.</typeparam>
        /// <returns>First component found of this type, or null if couldn't find.</returns>
        public CompType GetComponent<CompType>(string name = null) where CompType : Components.BaseComponent
        {
            // iterate components
            foreach(var comp in _components)
            {
                // if filter by name
                if (name != null && comp.Name != name)
                {
                    continue;
                }

                // if type match, return it
                System.Type type = comp.GetType();
                if (type == typeof(CompType) || type.IsSubclassOf(typeof(CompType)))
                {
                    return (CompType)comp;
                }
            }

            // didn't find? return null
            return null;
        }

        /// <summary>
        /// Get all components by type / name.
        /// </summary>
        /// <param name="name">If provided, will get components by type and name.</param>
        /// <typeparam name="CompType">Type of components to get.</typeparam>
        /// <returns>List with components matching conditions.</returns>
        public List<CompType> GetComponents<CompType>(string name = null) where CompType : Components.BaseComponent
        {
            // create list to return
            List<CompType> ret = new List<CompType>();

            // iterate components
            foreach (var comp in _components)
            {
                // if filter by name
                if (name != null && comp.Name != name)
                {
                    continue;
                }

                // if type match, return it
                System.Type type = comp.GetType();
                if (type == typeof(CompType) || type.IsSubclassOf(typeof(CompType)))
                {
                    ret.Add((CompType)comp);
                }
            }

            // return all components
            return ret;
        }

        /// <summary>
        /// Add a component to this GameObject instance.
        /// </summary>
        /// <param name="component">Component to add.</param>
        /// <param name="name">If provided, will also set the component name while adding it.</param>
        /// <returns>The newly added component.</returns>
        public Components.BaseComponent AddComponent(Components.BaseComponent component, string name = null)
        {
            // sanity check - make sure component don't have a parent
            if (component._GameObject != null)
            {
                throw new Exceptions.InvalidActionException("Cannot add the component to multiple game objects!");
            }

            // set component parent
            component.SetParent(this);

            // set as not debug mode by default
            component.AsDebug = false;

            // set physical body alias
            if (PhysicalBody == null && component is Components.Physics.BasePhysicsComponent)
            {
                PhysicalBody = component as Components.Physics.BasePhysicsComponent;
            }

            // add to list of components
            _components.Add(component);

            // set name
            if (name != null)
            {
                component.Name = name;
            }

            // if we were already spawned, invoke component spawn immediately
            if (_wasSpawned)
            {
                component.Spawn();
            }

            // need components array update
            _needComponentsArrayUpdate = true;

            // return component
            return component;
        }

        /// <summary>
        /// Add a component to this GameObject instance, but only if in debug mode.
        /// If not in debug mode, will do nothing.
        /// </summary>
        /// <param name="component">Component to add.</param>
        public Components.BaseComponent AddComponentDebug(Components.BaseComponent component)
        {
            if (GeonBitMain.Instance.DebugMode)
            {
                AddComponent(component);
                component.AsDebug = true;
            }
            return component;
        }

        /// <summary>
        /// Remove a component from this GameObject instance.
        /// </summary>
        /// <param name="component"></param>
        public void RemoveComponent(Components.BaseComponent component)
        {
            // detach component
            component.SetParent(null);

            // check if need to clear an alias
            if (PhysicalBody == component) { PhysicalBody = null; }

            // need components array update
            _needComponentsArrayUpdate = true;

            // remove from list of components
            _components.Remove(component);
        }

        /// <summary>
        /// Destroy this Game Object.
        /// </summary>
        public void Destroy()
        {
            // already destroyed? do nothing
            if (_destroyed) { return; }

            // make disabled
            _enabled = false;

            // prepare children and component arrays
            PrepareChildrenAndComponentsArray();

            // destroy all components 
            // note: we set parents to null because we want to first trigger parent event and then destroy.
            foreach (var component in _componentsArray)
            {
                component.SetParent(null);
                component.Destroy();
            }
            _componentsArray = null;

            // destroy all children (set the parent to null first to avoid cycle)
            foreach (var child in _childrenArray)
            {
                child._parent = null;
                child.Destroy();
            }
            _childrenArray = null;

            // clear internal and user data
            _internalData = null;
            UserData = null;

            // remove from parent
            Parent = null;

            // destroy scene node
            if (_sceneNode.Parent != null) { _sceneNode.RemoveFromParent(); }
            _sceneNode = null;

            // mark this object as destroyed
            _destroyed = true;
        }

        /// <summary>
        /// Invoke the 'OnReceiveMessage' callback of all the components of this entity.
        /// Note: this is used mostly for communication between user scripts.
        /// </summary>
        /// <param name="type">Message type / identifier.</param>
        /// <param name="recursive">If true, will send message recursively to all children and their children.</param>
        /// <returns>True if message should continue to next GameObjects, false otherwise.</returns>
        public bool SendMessage(string type, bool recursive)
        {
            if (recursive)
            {
                return SendMessageRecursive(type);
            }

            // last recieved message flow control
            Components.ReceivedMessageFlowControl ret = Components.ReceivedMessageFlowControl.Continue;

            // iterate components
            foreach (var component in _components)
            {
                // send message to current component and get instructions what to do with message next
                ret = component.SendMessage(type);

                // if its not continue, stop here
                if (ret != Components.ReceivedMessageFlowControl.Continue)
                {
                    break;
                }
            }

            // return if should continue to next game object or not.
            return ret != Components.ReceivedMessageFlowControl.FullBreak;
        }

        /// <summary>
        /// Just like 'SendMessage()', but walk on children recursively and send them the message too.
        /// </summary>
        /// <param name="type">Message type / identifier.</param>
        /// <returns>True if message should continue to next GameObjects, false otherwise.</returns>
        private bool SendMessageRecursive(string type)
        {
            // send to self
            bool continueIterating = SendMessage(type, false);

            // if need to break, stop here
            if (!continueIterating)
            {
                return false;
            }

            // iterate children
            foreach (var child in _children)
            {
                // send message to child, but if should break stop here and return false
                if (!child.SendMessageRecursive(type))
                {
                    return false;
                }
            }

            // can continue to next object
            return true;
        }

        /// <summary>
        /// Iterate over all the components and call 'methodName()', if such method exists.
        /// </summary>
        /// <param name="methodName">Method name to invoke.</param>
        /// <param name="componentNameFilter">If provided, will only invoke callback in components sharing this name.</param>
        /// <param name="parameters">Optional parameters to pass to called function.</param>
        /// <param name="recursive">If true, will also trigger child game objects recursively.</param>
        public void Trigger(string methodName, string componentNameFilter = null, object[] parameters = null, bool recursive = false)
        {
            // iterate components to trigger events
            foreach (var component in _components)
            {
                // skip components by name filter
                if (componentNameFilter != null && component.Name != componentNameFilter)
                {
                    continue;
                }

                // get the function to call
                System.Reflection.MethodInfo method;
                System.Type thisType = component.GetType();
                try
                {
                    method = thisType.GetMethod(methodName);
                }
                catch (System.ArgumentNullException)
                {
                    continue;
                }

                // call function
                method.Invoke(component, parameters);
            }

            // if recursive, call children
            if (recursive)
            {
                foreach (var child in _children)
                {
                    child.Trigger(methodName, componentNameFilter, parameters, recursive);
                }
            }
        }

        /// <summary>
        /// Optimization, prepare array of components and children before updates etc.
        /// </summary>
        private void PrepareChildrenAndComponentsArray()
        {
            // update components array if needed
            if (_needComponentsArrayUpdate)
            {
                _componentsArray = _components.ToArray();
                _needComponentsArrayUpdate = false;
            }

            // update children array if needed
            if (_needChildrenArrayUpdate)
            {
                _childrenArray = _children.ToArray();
                _needChildrenArrayUpdate = false;
            }
        }

        /// <summary>
        /// Spawn event is something that should be called once after the game object is ready with all components.
        /// </summary>
        public void CallSpawnEvent()
        {
            // was already spawned? skip
            if (_wasSpawned)
            {
                return;
            }

            // prepare children and components array before iteration
            PrepareChildrenAndComponentsArray();

            // call all components spawned
            foreach (var component in _componentsArray)
            {
                component.Spawn();
            }

            // prepare children and components array before iteration
            PrepareChildrenAndComponentsArray();

            // iterate and spawn children
            foreach (var child in _childrenArray)
            {
                child.CallSpawnEvent();
            }

            // mark that we were already spawned
            _wasSpawned = true;

            // special handling physical body - if we got physical body upon spawn, call its update after completing all other spawn callbacks.
            // this is to avoid flickering if physical body transformations are set from one of the components spawn event
            if (PhysicalBody != null)
            {
                PhysicalBody.UpdateNodeTransforms();
            }

            // after spawn event also update scene node, so that if the spawn triggered changes to transform they will apply before first draw.
            SceneNode.UpdateTransformations(false);
        }

        /// <summary>
        /// Update the visibility of the scene node, based on visibility, enabled, etc.
        /// </summary>
        protected void UpdateSceneNodeVisibility()
        {
            if (_sceneNode != null) { _sceneNode.Visible = _parent != null && _visible && _enabled; }
        }

        /// <summary>
        /// Called when the visibility of this GameObject changes.
        /// </summary>
        protected void OnVisibleChange()
        {
            // update scene node visibility
            UpdateSceneNodeVisibility();
        }

        /// <summary>
        /// Called when this GameObject turns enabled/disabled.
        /// </summary>
        protected void OnEnabledChange()
        {
            // update scene node visibility
            UpdateSceneNodeVisibility();
        }


        /// <summary>
        /// Called when the Game Object start colliding with another object.
        /// </summary>
        /// <param name="other">The other object we collide with.</param>
        /// <param name="data">Extra collision data.</param>
        public void CallCollisionStart(GameObject other, Core.Physics.CollisionData data)
        {
            foreach (var component in new List<Components.BaseComponent>(_components))
            {
                component.CollisionStart(other, data);
            }
        }

        /// <summary>
        /// Called when the Game Object stop colliding with another object.
        /// </summary>
        /// <param name="other">The other object we collided with, but no longer.</param>
        public void CallCollisionEnd(GameObject other)
        {
            foreach (var component in new List<Components.BaseComponent>(_components))
            {
                component.CollisionEnd(other);
            }
        }

        /// <summary>
        /// Called every frame while the Game Object is colliding with another object.
        /// </summary>
        /// <param name="other">The other object we are colliding with.</param>
        public void CallCollisionProcess(GameObject other)
        {
            foreach (var component in new List<Components.BaseComponent>(_components))
            {
                component.CollisionProcess(other);
            }
        }

    }
}

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
// A scene is an object that manage all the GameObjects etc.
// You can think of it as Scene == Game level, but a scene can also be used
// for stuff like menus, movies, etc.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

namespace GeonBit.ECS
{
    /// <summary>
    /// Special game object that is used for the scene root.
    /// </summary>
    internal class RootGameObject : GameObject
    {
        /// <summary>
        /// Create the root game object.
        /// </summary>
        /// <param name="scene">Parent scene instance.</param>
        /// <param name="nodeType">Node type to use for this game object.</param>
        public RootGameObject(GameScene scene, SceneNodeType nodeType) : base("root", nodeType)
        {
            _parentScene = scene;
            _isInScene = true;
        }
    }

    /// <summary>
    /// A game scene object.
    /// </summary>
    public class GameScene
    {
        /// <summary>
        /// Shortcut to get the currently active camera.
        /// </summary>
        public Components.Graphics.Camera ActiveCamera { get; internal set; } = null;

        /// <summary>
        /// Root GameObject (most top level GameObject in the tree).
        /// </summary>
        public GameObject Root { get; set; }

        /// <summary>
        /// Lights manager of this scene.
        /// If you want to use a custom lights manager class you can override this object, just
        /// be sure to do this before loading the scene.
        /// </summary>
        public Core.Graphics.Lights.ILightsManager Lights = new Core.Graphics.Lights.LightsManager();

        /// <summary>
        /// User interface for this scene.
        /// </summary>
        public UI.UserInterface UserInterface;

        /// <summary>
        /// If true, will draw physics debug.
        /// </summary>
        public static bool DebugRenderPhysics = false;

        /// <summary>
        /// The physical world of this scene.
        /// </summary>
        protected Core.Physics.PhysicsWorld _physics;

        /// <summary>
        /// Get the physical world instance.
        /// </summary>
        public Core.Physics.PhysicsWorld Physics
        {
            get { return _physics; }
        }

        /// <summary>
        /// Init the scene.
        /// </summary>
        public GameScene()
        {
            // create physical world
            _physics = new Core.Physics.PhysicsWorld();

            // create user interface
            UserInterface = GeonBitMain.Instance.UiEnabled ? new UI.UserInterface() : null;

            // count the event
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.AddedOrCreated);

            // create scene root
            Root = new RootGameObject(this, SceneNodeType.Simple);
            Root.SceneNode.DisableCulling = true;
        }

        /// <summary>
        /// Destroy the scene.
        /// </summary>
        ~GameScene()
        {
            Destroy();
        }

        /// <summary>
        /// Make this scene the currently loaded scene.
        /// </summary>
        public void Load()
        {
            GeonBitMain.Instance.Application.LoadScene(this);
        }

        /// <summary>
        /// Activate the scene, this will update root node, camera, and invoke 'spawn' and 'load' events.
        /// You do not need to call this function yourself, it is used internally.
        /// </summary>
        public void Activate()
        {
            // force-update scene nodes + camera
            // this is to make sure if we relay on camera or node positions in one of the entities 'spawn' code,
            // we will have valid values and not NaN / nulls.
            Root.SceneNode.UpdateTransformations(false);
            if (ActiveCamera != null)
            {
                ActiveCamera.UpdateCameraView();
                ActiveCamera.Update();
            }

            // make the light manager of this scene the active lights manager
            Core.Graphics.GraphicsManager.ActiveLightsManager = Lights;

            // make the scene UI the currently active UI
            UI.UserInterface.Active = UserInterface;

            // call spawn and load events
            Root.CallSpawnEvent();
        }

        /// <summary>
        /// Deactivate the scene and invoke the 'unload' events.
        /// You do not need to call this function yourself, it is used internally.
        /// </summary>
        public void Deactivate()
        {
        }

        /// <summary>
        /// Destroy this scene.
        /// After calling this, it cannot be used again. Also, don't call this on the currently active scene.
        /// </summary>
        public void Destroy()
        {
            if (Root != null)
            {
                Root.Destroy();
                Root = null;
            }
        }

        /// <summary>
        /// Draw the scene.
        /// You do not need to call this function yourself, it is used internally.
        /// </summary>
        public void Draw()
        {
            // draw scene
            Root.SceneNode.Draw();

            // if in debug mode, draw physics
            if (DebugRenderPhysics)
            {
                _physics.DebugDraw();
            }
        }

        /// <summary>
        /// Called to update stuff right before drawing scene.
        /// You do not need to call this function yourself, it is used internally.
        /// </summary>
        public void BeforeDraw()
        {
            Root.BeforeDraw();
        }

        /// <summary>
        /// Called every frame to do GameObject events.
        /// You do not need to call this function yourself, it is used internally.
        /// </summary>
        public void Update()
        {
            _physics.Update(Managers.TimeManager.Instance.TimeFactor);
            Root.Update();
        }

        /// <summary>
        /// Triggers a Fixed Update event (update that happens every const amount of seconds).
        /// </summary>
        public void FixedUpdate()
        {
            Root.FixedUpdate();
        }
    }
}

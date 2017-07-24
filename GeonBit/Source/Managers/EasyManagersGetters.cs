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
// This class is used to get easier access to all public core managers in GeonBit. 
// Inherit from it to get a set of pre-defined public getters for main entities.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

namespace GeonBit.Managers
{
    /// <summary>
    /// Provide a shortcut to all the important public stuff and core managers.
    /// Once inherit from this, you can easily access managers with "Managers.Manager" style.
    /// </summary>
    public class EasyManagersGetters
    {
        /// <summary>Get the ActiveScene this entity is inside.</summary>
        public ECS.GameScene ActiveScene { get { return Application.Instance.ActiveScene; } }

        /// <summary>Get the TimeManager manager.</summary>
        public TimeManager TimeManager { get { return TimeManager.Instance; } }

        /// <summary>Get the Input manager.</summary>
        public GameInput GameInput { get { return GameInput.Instance; } }

        /// <summary>Get the Application manager.</summary>
        public Application Application { get { return Application.Instance; } }

        /// <summary>Get the Prototypes manager.</summary>
        public Prototypes Prototypes { get { return Prototypes.Instance; } }

        /// <summary>Get the Diagnostic manager.</summary>
        public Diagnostic Diagnostic { get { return Diagnostic.Instance; } }

        /// <summary>Get the Graphics manager.</summary>
        public GraphicsManager GraphicsManager { get { return GraphicsManager.Instance; } }

        /// <summary>Get the Sound manager.</summary>
        public SoundManager SoundManager { get { return SoundManager.Instance; } }

        /// <summary>Get the currently active physical world.</summary>
        public Core.Physics.PhysicsWorld ScenePhysics { get { return Application.Instance.ActiveScene.Physics; } }

        /// <summary>Get the plugins manager.</summary>
        public Plugins Plugins { get { return Plugins.Instance; } }

        /// <summary>Get the config storage manager.</summary>
        public ConfigStorage ConfigStorage { get { return ConfigStorage.Instance; } }

        /// <summary>Get the game files manager.</summary>
        public GameFiles GameFiles { get { return GameFiles.Instance; } }

        /// <summary>
        /// Return if we are currently in debug mode.
        /// </summary>
        public bool DebugMode { get { return GeonBitMain.Instance.DebugMode; } }

        /// <summary>Get resources manager.</summary>
        public Core.ResourcesManager Resources { get { return Core.ResourcesManager.Instance; } }
    }
}

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
// Define the component API.
// A component is something you can attach to a GameObject, like collision, sound
// 3d model to render, etc. They serve as immediators layer between the GameObjects
// and the 'Core' layer.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

namespace GeonBit.ECS.Components
{
    /// <summary>
    /// A return value that lets OnReceiveMessage() control what happens next with the message.
    /// </summary>
    public enum ReceivedMessageFlowControl
    {
        /// <summary>
        /// Continue as normal to the next component / GameObject (if recursive).
        /// </summary>
        Continue = 0,

        /// <summary>
        /// Don't continue to iterate for next components on this game object, but if the message is recursive, we will continue to next GameObjects.
        /// </summary>
        BreakForObject = 1,

        /// <summary>
        /// Will stop completely, both recursive and next components.
        /// </summary>
        FullBreak = 2,
    }

    /// <summary>
    /// The interface every component must implement.
    /// </summary>
    public abstract class IComponent
    {
        /// <summary>
        /// Provide an easy access to all GeonBit managers.
        /// </summary>
        protected readonly Managers.EasyManagersGetters Managers = new Managers.EasyManagersGetters();

        /// <summary>
        /// Get / set the name of this component.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get the game object this component is attached to.
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// Called when GameObject spawns.
        /// </summary>
        protected abstract void OnSpawn();

        /// <summary>
        /// Called when GameObject is destroyed.
        /// </summary>
        protected abstract void OnDestroyed();

        /// <summary>
        /// Called when GameObject turned disabled.
        /// </summary>
        protected abstract void OnDisabled();

        /// <summary>
        /// Called when GameObject is enabled.
        /// </summary>
        protected abstract void OnEnabled();

        /// <summary>
        /// Called when parent GameObject changes (after the change).
        /// </summary>
        /// <param name="prevParent">Previous parent.</param>
        /// <param name="newParent">New parent.</param>
        protected abstract void OnParentChange(GameObject prevParent, GameObject newParent);

        /// <summary>
        /// Called every frame in the Update() loop.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// Called every constant amount of seconds in the Update() loop.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected abstract void OnFixedUpdate();

        /// <summary>
        /// Called every frame before the scene renders.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected abstract void OnBeforeDraw();

        /// <summary>
        /// Called every time scene node transformation updates.
        /// Note: this is called only if GameObject is enabled and have Update events enabled.
        /// </summary>
        protected abstract void OnTransformationUpdate();

        /// <summary>
        /// Called when this component is effectively removed from scene, eg when removed
        /// from a GameObject or when its GameObject is removed from scene.
        /// </summary>
        protected abstract void OnRemoveFromScene();

        /// <summary>
        /// Called when this component is effectively added to scene, eg when added
        /// to a GameObject currently in scene or when its GameObject is added to scene.
        /// </summary>
        protected abstract void OnAddToScene();

        /// <summary>
        /// An event that triggers every X miliseconds (defined per GameObject instance).
        /// </summary>
        protected abstract void OnHeartbeat();

        /// <summary>
        /// Called when the parent Game Object start colliding with another object.
        /// </summary>
        /// <param name="other">The other object we collide with.</param>
        /// <param name="data">Extra collision data.</param>
        protected abstract void OnCollisionStart(GameObject other, Core.Physics.CollisionData data);

        /// <summary>
        /// Called when the parent Game Object stop colliding with another object.
        /// </summary>
        /// <param name="other">The other object we collided with, but no longer.</param>
        protected abstract void OnCollisionEnd(GameObject other);

        /// <summary>
        /// Called every frame while the parent Game Object is colliding with another object.
        /// </summary>
        /// <param name="other">The other object we are colliding with.</param>
        protected abstract void OnCollisionProcess(GameObject other);

        /// <summary>
        /// Called when someone send message to the parent GameObject.
        /// Note: this function is used for scripting and only user scripts-related components should respond to this.
        /// </summary>
        /// <param name="type">Message type.</param>
        /// <returns>True to continue to next component / child, false to break.</returns>
        protected abstract ReceivedMessageFlowControl OnReceiveMessage(string type);
    }
}

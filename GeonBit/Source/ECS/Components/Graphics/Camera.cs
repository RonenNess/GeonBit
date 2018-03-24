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
// A 3d camera component.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.ECS.Components.Graphics
{
    /// <summary>
    /// This component implements a 3d camera.
    /// </summary>
    public class Camera : BaseComponent
    {
        // the graphic camera object (core layer).
        Core.Graphics.CameraEntity _graphicCamera;

        /// <summary>
        /// Far clip plane distance.
        /// </summary>
        public float FarPlane
        {
            set { _graphicCamera.FarClipPlane = value; }
            get { return _graphicCamera.FarClipPlane; }
        }

        /// <summary>
        /// Near clip plane distance.
        /// </summary>
        public float NearPlane
        {
            set { _graphicCamera.NearClipPlane = value; }
            get { return _graphicCamera.NearClipPlane; }
        }

        /// <summary>
        /// Set / Get camera field of view.
        /// </summary>
        public float FieldOfView
        {
            get { return _graphicCamera.FieldOfView; }
            set { _graphicCamera.FieldOfView = value; }
        }

        /// <summary>
        /// If defined, this will be used as screen size (affect aspect ratio in perspective camera,
        /// and view size in Orthographic camera. If not set, default screen size will be used.
        /// </summary>
        public Point? ForceScreenSize
        {
            get { return _graphicCamera.ForceScreenSize; }
            set { _graphicCamera.ForceScreenSize = value; }
        }

        /// <summary>
        /// Set / get camera type.
        /// </summary>
        public Core.Graphics.CameraType CameraType
        {
            get { return _graphicCamera.CameraType; }
            set { _graphicCamera.CameraType = value; }
        }

        /// <summary>
        /// If set, camera will always look at this point, regardless of scene node rotation.
        /// </summary>
        public Vector3? LookAt = null;

        /// <summary>
        /// Set a target that the camera will always look at, regardless of scene node rotation.
        /// Note: this override the LookAt position, even if set.
        /// </summary>
        public GameObject LookAtTarget = null;

        /// <summary>
        /// If 'LookAtTarget' is used, this vector will be offset from target position.
        /// For example, if you want the camera to look at 5 units above target, set this to Vector3(0, 5, 0).
        /// </summary>
        public Vector3 LookAtTargetOffset = Vector3.Zero;

        /// <summary>
        /// Does this camera auto-update on update loop?
        /// Note: if you turn this false you must call Update() manually.
        /// </summary>
        public bool AutoUpdate = true;

        /// <summary>
        /// Get a vector representing camera current forward direction.
        /// </summary>
        public Vector3 Forward
        {
            get { return _graphicCamera.Forward; }
        }

        /// <summary>
        /// Get a vector representing camera current backward direction.
        /// </summary>
        public Vector3 Backward
        {
            get { return _graphicCamera.Backward; }
        }

        /// <summary>
        /// Get camera current position.
        /// </summary>
        public Vector3 Position
        {
            get { return _graphicCamera.Position; }
        }

        /// <summary>
        /// Create the camera component.
        /// </summary>
        public Camera()
        {
            // create new graphic camera object
            _graphicCamera = new Core.Graphics.CameraEntity();
        }

        /// <summary>
        /// Get if this camera is the active camera in its scene.
        /// Note: it doesn't mean that the scene this camera belongs to is currently active.
        /// </summary>
        public bool IsActiveCamera
        {
            get
            {
                return _GameObject.ParentScene.ActiveCamera == this;
            }
        }

        /// <summary>
        /// On spawn, set this camera as the active camera of the scene, if no other camera is set.
        /// </summary>
        override protected void OnSpawn()
        {
            // if there's no active camera, set self as the active camera
            if (_GameObject.ParentScene.ActiveCamera == null)
            {
                SetAsActive();
            }
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            Camera ret = new Camera();
            ret.LookAt = LookAt;
            ret.LookAtTarget = LookAtTarget;
            ret.LookAtTargetOffset = LookAtTargetOffset;
            ret.CameraType = CameraType;
            ret.ForceScreenSize = ForceScreenSize;
            ret.FarPlane = FarPlane;
            ret.NearPlane = NearPlane;
            ret.FieldOfView = FieldOfView;
            ret.AutoUpdate = AutoUpdate;
            CopyBasics(ret);
            return ret;
        }

        /// <summary>
        /// Get the 3d ray that starts from camera position and directed at current mouse position.
        /// </summary>
        /// <returns>Ray from camera to mouse position.</returns>
        public Ray GetMouseRay()
        {
            return _graphicCamera.RayFromMouse();
        }

        /// <summary>
        /// Get the 3d ray that starts from camera position and directed at a given 2d position.
        /// </summary>
        /// <param name="position">Position to get ray to.</param>
        /// <returns>Ray from camera to given position.</returns>
        public Ray GetRay(Vector2 position)
        {
            return _graphicCamera.RayFrom2dPoint(position);
        }

        /// <summary>
        /// Get the 3d ray that starts from camera position and directed at a given 3d position.
        /// </summary>
        /// <param name="position">Position to get ray to.</param>
        /// <returns>Ray from camera to given position.</returns>
        public Ray GetRay(Vector3 position)
        {
            return _graphicCamera.RayFrom3dPoint(position);
        }

        /// <summary>
        /// Get camera view frustum.
        /// </summary>
        public BoundingFrustum ViewFrustum
        {
            get { return _graphicCamera.ViewFrustum; }
        }

        /// <summary>
        /// Get camera view matrix.
        /// </summary>
        public Matrix View
        {
            get { return _graphicCamera.View; }
        }

        /// <summary>
        /// Get camera projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get { return _graphicCamera.Projection; }
        }

        /// <summary>
        /// Set this camera as the currently active camera.
        /// </summary>
        public void SetAsActive()
        {
            // if not in scene, throw exception
            if (_GameObject == null || _GameObject.ParentScene == null)
            {
                throw new Exceptions.InvalidActionException("Cannot make a camera active when its not under any scene!");
            }

            // set active camera pointer
            _GameObject.ParentScene.ActiveCamera = this;

            // update core graphics about new active camera
            Core.Graphics.GraphicsManager.ActiveCamera = _graphicCamera;
        }

        /// <summary>
        /// Called every frame in the Update() loop.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected override void OnUpdate()
        {
            // if we are the currently active camera, update view matrix
            if (IsActiveCamera && AutoUpdate)
            {
                // update camera view
                UpdateCameraView();
            }
        }

        /// <summary>
        /// Update camera view matrix.
        /// </summary>
        public void UpdateCameraView()
        {
            // if there's a lookat target, override current LookAt
            if (LookAtTarget != null)
            {
                LookAt = LookAtTarget.SceneNode.WorldPosition + LookAtTargetOffset;
            }

            // new view matrix
            Matrix view;

            // get current world position (of the camera)
            Vector3 worldPos = _GameObject.SceneNode.WorldPosition;

            // if we have lookat-target, create view from look-at matrix.
            if (LookAt != null)
            {
                view = Matrix.CreateLookAt(worldPos, (Vector3)LookAt, Vector3.Up);
            }
            // if we don't have a look-at target, create view matrix from scene node transformations
            else
            {
                Vector3 target = worldPos + Vector3.Transform(Vector3.Forward, _GameObject.SceneNode.WorldRotation);
                view = Matrix.CreateLookAt(worldPos, target, Vector3.Up);
            }

            // update the view matrix of the graphic camera component
            _graphicCamera.UpdateViewPosition(view, worldPos);
        }
    }
}

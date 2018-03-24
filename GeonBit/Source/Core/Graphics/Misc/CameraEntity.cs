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
// A graphic camera object.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GeonBit.Core.Graphics
{
    /// <summary>
    /// Camera types.
    /// </summary>
    public enum CameraType
    {
        /// <summary>
        /// Perspective camera.
        /// </summary>
        Perspective,

        /// <summary>
        /// Orthographic camera.
        /// </summary>
        Orthographic,
    };

    /// <summary>
    /// A 3d camera object.
    /// </summary>
    public class CameraEntity
    {
        /// <summary>
        /// Default field of view.
        /// </summary>
        public static readonly float DefaultFieldOfView = MathHelper.PiOver4;

        // projection params
        float _fieldOfView = MathHelper.PiOver4;
        float _nearClipPlane = 1.0f;
        float _farClipPlane = 950.0f;
        float _aspectRatio = 1.0f;

        // current camera type
        CameraType _cameraType = CameraType.Perspective;

        // camera screen size
        Point? _altScreenSize = null;

        /// <summary>
        /// If defined, this will be used as screen size (affect aspect ratio in perspective camera,
        /// and view size in Orthographic camera). If not set, the actual screen resolution will be used.
        /// </summary>
        public Point? ForceScreenSize
        {
            get { return _altScreenSize; }
            set { _altScreenSize = value; }
        }

        /// <summary>
        /// Set / get camera type.
        /// </summary>
        public CameraType CameraType
        {
            set { _cameraType = value; _needUpdateProjection = true; }
            get { return _cameraType; }
        }

        /// <summary>
        /// Set / Get camera field of view.
        /// </summary>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set { _fieldOfView = value; _needUpdateProjection = true; }
        }

        /// <summary>
        /// Set / Get camera near clip plane.
        /// </summary>
        public float NearClipPlane
        {
            get { return _nearClipPlane; }
            set { _nearClipPlane = value; _needUpdateProjection = true; }
        }

        /// <summary>
        /// Set / Get camera far clip plane.
        /// </summary>
        public float FarClipPlane
        {
            get { return _farClipPlane; }
            set { _farClipPlane = value; _needUpdateProjection = true; }
        }

        // true if we need to update projection matrix next time we try to get it
        private bool _needUpdateProjection = true;

        // current view matrix
        Matrix _view;

        // current projection matrix
        Matrix _projection;

        // current world position
        Vector3 _position;

        /// <summary>
        /// Get camera position.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Create a new camera instance
        /// </summary>
        public CameraEntity()
        {
        }

        /// <summary>
        /// Return the current camera projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get { UpdateProjectionIfNeeded(); return _projection; }
        }

        /// <summary>
        /// Get / Set the current camera view matrix.
        /// </summary>
        public Matrix View
        {
            get { return _view; }
        }

        /// <summary>
        /// Get camera forward vector.
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                Vector3 ret = Vector3.Transform(Vector3.Forward, Matrix.Invert(View));
                ret.Normalize();
                return -ret;
            }
        }

        /// <summary>
        /// Get camera backward vector.
        /// </summary>
        public Vector3 Backward
        {
            get
            {
                Vector3 ret = Vector3.Transform(Vector3.Forward, Matrix.Invert(View));
                ret.Normalize();
                return ret;
            }
        }

        /// <summary>
        /// Get camera bounding frustum.
        /// </summary>
        public BoundingFrustum ViewFrustum
        {
            get
            {
                UpdateProjectionIfNeeded();
                return new BoundingFrustum(_view * _projection);
            }
        }

        /// <summary>
        /// Store camera world position.
        /// </summary>
        /// <param name="view">Current view matrix</param>
        /// <param name="position">Camera world position.</param>
        public void UpdateViewPosition(Matrix view, Vector3 position)
        {
            _view = view;
            _position = position;
        }

        /// <summary>
        /// Update projection matrix after changes.
        /// </summary>
        private void UpdateProjectionIfNeeded()
        {
            // if don't need update, skip
            if (!_needUpdateProjection)
            {
                return;
            }

            // screen width and height
            float width; float height;

            // if we have alternative screen size defined, use it
            if (ForceScreenSize != null)
            {
                width = ForceScreenSize.Value.X;
                height = ForceScreenSize.Value.Y;
            }
            // if we don't have alternative screen size defined, get current backbuffer size
            else
            {
                GraphicsDeviceManager deviceManager = GraphicsManager.GraphicsDeviceManager;
                width = deviceManager.PreferredBackBufferWidth;
                height = deviceManager.PreferredBackBufferHeight;
            }

            // calc aspect ratio
            _aspectRatio = width / height;

            // create view and projection matrix
            switch (_cameraType)
            {
                case CameraType.Perspective:
                    _projection = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, _nearClipPlane, _farClipPlane);
                    break;

                case CameraType.Orthographic:
                    _projection = Matrix.CreateOrthographic(width, height, _nearClipPlane, _farClipPlane);
                    break;
            }

            // no longer need projection update
            _needUpdateProjection = false;
        }

        /// <summary>
        /// Return a ray starting from the camera and pointing directly at mouse position (translated to 3d space).
        /// This is a helper function that help to get ray collision based on camera and mouse.
        /// </summary>
        /// <returns>Ray from camera to mouse.</returns>
        public Ray RayFromMouse()
        {
            MouseState mouseState = Mouse.GetState();
            return RayFrom2dPoint(new Vector2(mouseState.X, mouseState.Y));
        }

        /// <summary>
        /// Return a ray starting from the camera and pointing directly at a 3d position.
        /// </summary>
        /// <param name="point">Point to send ray to.</param>
        /// <returns>Ray from camera to given position.</returns>
        public Ray RayFrom3dPoint(Vector3 point)
        {
            return new Ray(Position, point - Position);
        }

        /// <summary>
        /// Return a ray starting from the camera and pointing directly at a 2d position translated to 3d space.
        /// This is a helper function that help to get ray collision based on camera and position on screen.
        /// </summary>
        /// <param name="point">Point to send ray to.</param>
        /// <returns>Ray from camera to given position.</returns>
        public Ray RayFrom2dPoint(Vector2 point)
        {
            // get graphic device
            GraphicsDevice device = GraphicsManager.GraphicsDevice;

            // convert point to near and far points as 3d vectors
            Vector3 nearsource = new Vector3(point.X, point.Y, 0f);
            Vector3 farsource = new Vector3(point.X, point.Y, 1f);

            // create empty world matrix
            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            // convert near point to world space
            Vector3 nearPoint = device.Viewport.Unproject(nearsource,
                _projection, _view, world);

            // convert far point to world space
            Vector3 farPoint = device.Viewport.Unproject(farsource,
                _projection, _view, world);

            // get direction
            Vector3 dir = farPoint - nearPoint;
            dir.Normalize();

            // return ray
            return new Ray(nearPoint, dir);
        }
    }
}

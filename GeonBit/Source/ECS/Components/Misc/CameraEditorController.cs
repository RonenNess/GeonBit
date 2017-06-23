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
// A camera controller for an editor program.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GeonBit.ECS.Components.Misc
{
    /// <summary>
    /// A component to put on the GameObject containing the active camera, which provide an editor-like camera controls. 
    /// </summary>
    public class CameraEditorController : BaseComponent
    {
        /// <summary>
        /// Camera movement speed.
        /// </summary>
        public float MovementSpeedFactor = 125f;

        /// <summary>
        /// Camera rotation speed.
        /// </summary>
        public float RotationSpeedFactor = 1f;

        /// <summary>
        /// Game key used to rotate camera (hold down this key and move mouse to rotate camera).
        /// </summary>
        public Input.GameKeys RotationKey = Input.GameKeys.AlternativeFire;

        /// <summary>
        /// If true, will invert rotation on Y axis.
        /// </summary>
        public bool InvertY = true;

        /// <summary>
        /// If true, will invert rotation on X axis.
        /// </summary>
        public bool InvertX = true;

        /// <summary>
        /// Current camera direction.
        /// </summary>
        protected Vector3 Direction = Vector3.Zero;

        // rotation around X and Y axis
        Vector2 _currRotation = Vector2.Zero;

        /// <summary>
        /// If true, will prevent camera overflow on Y axis.
        /// </summary>
        public bool PreventOverflowY = true;

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            CameraEditorController ret = CopyBasics(new CameraEditorController()) as CameraEditorController;
            ret.MovementSpeedFactor = MovementSpeedFactor;
            ret.RotationSpeedFactor = RotationSpeedFactor;
            ret.RotationKey = RotationKey;
            ret.InvertY = InvertY;
            ret.InvertX = InvertX;
            ret.Direction = Direction;
            ret.PreventOverflowY = PreventOverflowY;
            ret._currRotation = _currRotation;
            return ret;
        }

        /// <summary>
        /// Called when this component spawns.
        /// </summary>
        protected override void OnSpawn()
        {
            // get camera and copy starting direction
            Graphics.Camera camera = _GameObject.GetComponent<Graphics.Camera>();
            camera.UpdateCameraView();
            Direction = camera.Forward;

            // cancel lookat
            camera.LookAt = null;
            camera.LookAtTarget = null;

            // set starting rotation
            float yaw; float pitch; float roll;
            Core.Utils.ExtendedMath.ExtractYawPitchRoll(camera.View, out yaw, out pitch, out roll);
            _currRotation.X = yaw;
            _currRotation.Y = pitch;
        }

        /// <summary>
        /// Called every frame in the Update() loop.
        /// Note: this is called only if GameObject is enabled.
        /// </summary>
        protected override void OnUpdate()
        {
            // get current speed factor
            float speed = Managers.TimeManager.TimeFactor * MovementSpeedFactor;

            // get camera component
            Graphics.Camera camera = _GameObject.GetComponent<Graphics.Camera>();

            // move camera forward
            if (Managers.GameInput.IsKeyDown(Input.GameKeys.Forward))
            {
                _GameObject.SceneNode.Position += Direction * speed;
            }
            // move camera backward
            if (Managers.GameInput.IsKeyDown(Input.GameKeys.Backward))
            {
                _GameObject.SceneNode.Position += -Direction * speed;
            }
            // move camera left
            if (Managers.GameInput.IsKeyDown(Input.GameKeys.Left))
            {
                _GameObject.SceneNode.Position += Core.Utils.ExtendedMath.GetLeftVector(Direction, true) * speed;
            }
            // move camera right
            if (Managers.GameInput.IsKeyDown(Input.GameKeys.Right))
            {
                _GameObject.SceneNode.Position += Core.Utils.ExtendedMath.GetRightVector(Direction, true) * speed;
            }

            // rotate camera
            if (Managers.GameInput.IsKeyDown(RotationKey))
            {
                // get rotation factor (note: rotation is based on mouse diff and angles, therefore we don't need to add time factor)
                Vector2 rotation = Managers.GameInput.MousePositionDiff * (RotationSpeedFactor / 250f);

                // if need to rotate..
                if (rotation.X != 0 || rotation.Y != 0)
                {
                    // invert if needed
                    if (InvertY) { rotation.Y *= -1; }
                    if (InvertX) { rotation.X *= -1; }

                    // update rotation
                    _currRotation += rotation;

                    // prevent overflow on Y axis
                    if (PreventOverflowY)
                    {
                        _currRotation.Y = MathHelper.Min(MathHelper.Pi / 2f - 0.001f, _currRotation.Y);
                        _currRotation.Y = MathHelper.Max(-MathHelper.Pi / 2f + 0.001f, _currRotation.Y);
                    }
                }
            }

            // set new direction
            Direction = Vector3.Transform(Vector3.Forward,
                Matrix.CreateFromYawPitchRoll(_currRotation.X, _currRotation.Y, 0f));

            // update camera look-at direction
            camera.LookAt = _GameObject.SceneNode.WorldPosition + Direction;
        }
    }
}

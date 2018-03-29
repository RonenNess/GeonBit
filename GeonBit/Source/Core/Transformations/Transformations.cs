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
// A class containing all the basic transformations an object can have.
// This include: Translation, Rotation, and Scale.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Core
{
    /// <summary>
    /// How to apply rotation (euler vs quaternion).
    /// </summary>
    public enum RotationType
    {
        /// <summary>
        /// Euler rotation.
        /// </summary>
        Euler,

        /// <summary>
        /// Quaternion rotation.
        /// </summary>
        Quaternion,
    }

    /// <summary>
    /// Different way to build matrix from transformations.
    /// </summary>
    public enum TransformOrder
    {
        /// <summary>
        /// Apply position, then rotation, then scale.
        /// </summary>
        PositionRotationScale,

        /// <summary>
        /// Apply position, then scale, then rotation.
        /// </summary>
        PositionScaleRotation,

        /// <summary>
        /// Apply scale, then position, then rotation.
        /// </summary>
        ScalePositionRotation,

        /// <summary>
        /// Apply scale, then rotation, then position.
        /// </summary>
        ScaleRotationPosition,

        /// <summary>
        /// Apply rotation, then scale, then position.
        /// </summary>
        RotationScalePosition,

        /// <summary>
        /// Apply rotation, then position, then scale.
        /// </summary>
        RotationPositionScale,
    }

    /// <summary>
    /// Different ways to apply rotation (order in which we rotate the different axis).
    /// </summary>
    public enum RotationOrder
    {
        /// <summary>
        /// Rotate by axis order X, Y, Z.
        /// </summary>
        RotateXYZ,

        /// <summary>
        /// Rotate by axis order X, Z, Y.
        /// </summary>
        RotateXZY,

        /// <summary>
        /// Rotate by axis order Y, X, Z.
        /// </summary>
        RotateYXZ,

        /// <summary>
        /// Rotate by axis order Y, Z, X.
        /// </summary>
        RotateYZX,

        /// <summary>
        /// Rotate by axis order Z, X, Y.
        /// </summary>
        RotateZXY,

        /// <summary>
        /// Rotate by axis order Z, Y, X.
        /// </summary>
        RotateZYX,
    }

    /// <summary>
    /// Contain all the possible node transformations.
    /// </summary>
    public class Transformations
    {
        /// <summary>
        /// Node position / translation.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Node rotation.
        /// </summary>
        public Vector3 Rotation;

        /// <summary>
        /// Node scale.
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// Order to apply different transformations to create the final matrix.
        /// </summary>
        public TransformOrder TransformOrder = TransformOrder.ScaleRotationPosition;

        /// <summary>
        /// Axis order to apply rotation.
        /// </summary>
        public RotationOrder RotationOrder = RotationOrder.RotateYXZ;

        /// <summary>
        /// What type of rotation to use.
        /// </summary>
        public RotationType RotationType = RotationType.Quaternion;

        /// <summary>
        /// Create new default transformations.
        /// </summary>
        public Transformations()
        {
            // set defaults
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;

            // count the object creation
            Utils.CountAndAlert.Count(Utils.CountAndAlert.PredefAlertTypes.AddedOrCreated);
        }

        /// <summary>
        /// Clone transformations.
        /// </summary>
        public Transformations(Transformations other)
        {
            // set values from other
            Position = other.Position;
            Rotation = other.Rotation;
            Scale = other.Scale;
            TransformOrder = other.TransformOrder;
            RotationOrder = other.RotationOrder;
            RotationType = other.RotationType;

            // count the object creation
            Utils.CountAndAlert.Count(Utils.CountAndAlert.PredefAlertTypes.AddedOrCreated);
        }

        /// <summary>
        /// Clone transformations.
        /// </summary>
        /// <returns>Copy of this transformations.</returns>
        public Transformations Clone()
        {
            return new Transformations(this);
        }

        /// <summary>
        /// Build and return just the rotation matrix for this treansformations.
        /// </summary>
        /// <returns>Rotation matrix.</returns>
        public Matrix BuildRotationMatrix()
        {
            // handle euler rotation
            if (RotationType == RotationType.Euler)
            {
                switch (RotationOrder)
                {
                    case RotationOrder.RotateXYZ:
                        return Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z);

                    case RotationOrder.RotateXZY:
                        return Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationY(Rotation.Y);

                    case RotationOrder.RotateYXZ:
                        return Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);

                    case RotationOrder.RotateYZX:
                        return Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationX(Rotation.X);

                    case RotationOrder.RotateZXY:
                        return Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);

                    case RotationOrder.RotateZYX:
                        return Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationX(Rotation.X);

                    default:
                        throw new Exceptions.UnsupportedTypeException("Unknown rotation order!");
                }
            }
            // handle quaternion rotation
            else if (RotationType == RotationType.Quaternion)
            {
                // quaternion to use
                Quaternion quat;

                // build quaternion based on rotation order
                switch (RotationOrder)
                {
                    case RotationOrder.RotateXYZ:
                        quat = Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z);
                        break;

                    case RotationOrder.RotateXZY:
                        quat = Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y);
                        break;

                    case RotationOrder.RotateYXZ:
                        quat = Quaternion.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
                        break;

                    case RotationOrder.RotateYZX:
                        quat = Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X);
                        break;

                    case RotationOrder.RotateZXY:
                        quat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y);
                        break;

                    case RotationOrder.RotateZYX:
                        quat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X);
                        break;

                    default:
                        throw new Exceptions.UnsupportedTypeException("Unknown rotation order!");
                }

                // convert to a matrix and return
                return Matrix.CreateFromQuaternion(quat);
            }
            // should never happen.
            else
            {
                throw new Exceptions.UnsupportedTypeException("Unknown rotation type!");
            }
        }

        /// <summary>
        /// Build and return a matrix from current transformations.
        /// </summary>
        /// <returns>Matrix with all transformations applied.</returns>
        public Matrix BuildMatrix()
        {
            // create the matrix parts
            Matrix pos = Matrix.CreateTranslation(Position);
            Matrix rot = BuildRotationMatrix();
            Matrix scale = Matrix.CreateScale(Scale);

            // build and return matrix based on order
            switch (TransformOrder)
            {
                case TransformOrder.PositionRotationScale:
                    return pos * rot * scale;

                case TransformOrder.PositionScaleRotation:
                    return pos * scale * rot;

                case TransformOrder.ScalePositionRotation:
                    return scale * pos * rot;

                case TransformOrder.ScaleRotationPosition:
                    return scale * rot * pos;

                case TransformOrder.RotationScalePosition:
                    return rot * scale * pos;

                case TransformOrder.RotationPositionScale:
                    return rot * pos * scale;

                default:
                    throw new Exceptions.UnsupportedTypeException("Unknown build matrix order!");
            }
        }
    }
}

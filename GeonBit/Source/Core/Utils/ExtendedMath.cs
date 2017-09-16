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
// Math and vector-related utils & functions.
//
// Author: Ronen Ness.
// Since: 2016.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Utils
{
    /// <summary>
    /// GeonBit.Core.Utils provide general helper classes for the core layer.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Contain different math utils and vector-related helper functions.
    /// </summary>
    public static class ExtendedMath
    {
        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        /// <param name="degrees">Degrees to convert to radians.</param>
        /// <returns>Converted degrees as radians.</returns>
        static public float DegreeToRadian(float degrees)
        {
            return (float)((Math.PI / 180) * degrees);
        }

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        /// <param name="radians">Radians to convert to degrees.</param>
        /// <returns>Converted radians as degrees.</returns>
        static public float RadianToDegree(float radians)
        {
            return (float)(radians * (180.0 / Math.PI));
        }

        /// <summary>
        /// Return a vector pointing to the 'left' side of a given vector.
        /// </summary>
        /// <param name="vector">Vector to get left vector from.</param>
        /// <param name="zeroY">If true, will zero Y component.</param>
        /// <returns>Vector pointing to the left of the given vector.</returns>
        static public Vector3 GetLeftVector(Vector3 vector, bool zeroY = false)
        {
            Vector3 ret = Vector3.Transform(vector, Matrix.CreateFromAxisAngle(Vector3.Up, ExtendedMath.DegreeToRadian(90)));
            if (zeroY) { ret.Y = 0.0f; ret.Normalize(); }
            return ret;
        }

        /// <summary>
        /// Return a vector pointing to the 'right' side of a given vector.
        /// </summary>
        /// <param name="vector">Vector to get right vector from.</param>
        /// <param name="zeroY">If true, will zero Y component.</param>
        /// <returns>Vector pointing to the right of the given vector.</returns>
        static public Vector3 GetRightVector(Vector3 vector, bool zeroY = false)
        {
            Vector3 ret = Vector3.Transform(vector, Matrix.CreateFromAxisAngle(Vector3.Up, ExtendedMath.DegreeToRadian(-90)));
            if (zeroY) { ret.Y = 0.0f; ret.Normalize(); }
            return ret;
        }

        /// <summary>
        /// Extract the correct scale from matrix.
        /// </summary>
        /// <param name="mat">Matrix to get scale from.</param>
        /// <returns>Matrix scale.</returns>
        static public Vector3 GetScale(ref Matrix mat)
        {
            Vector3 scale; Vector3 pos; Quaternion rot;
            mat.Decompose(out scale, out rot, out pos);
            return scale;
        }

        /// <summary>
        /// Extract the correct rotation from matrix.
        /// </summary>
        /// <param name="mat">Matrix to get rotation from.</param>
        /// <returns>Matrix rotation.</returns>
        static public Quaternion GetRotation(ref Matrix mat)
        {
            Vector3 scale; Vector3 pos; Quaternion rot;
            mat.Decompose(out scale, out rot, out pos);
            return rot;
        }

        /// <summary>
        /// Extract yaw, pitch and roll from existing matrix.
        /// </summary>
        /// <param name="matrix">Matrix to extract from.</param>
        /// <param name="yaw">Out yaw value.</param>
        /// <param name="pitch">Out pitch value.</param>
        /// <param name="roll">Out roll value.</param>
        public static void ExtractYawPitchRoll(Matrix matrix, out float yaw, out float pitch, out float roll)
        {
            yaw = (float)Math.Atan2(matrix.M13, matrix.M33);
            pitch = (float)Math.Asin(-matrix.M23);
            roll = (float)Math.Atan2(matrix.M21, matrix.M22);
        }

        /// <summary>
        /// Wrap an angle to be between 0 and 360.
        /// </summary>
        /// <param name="angle">Angle to wrap (degrees).</param>
        /// <returns>Wrapped angle.</returns>
        static public uint WrapAngle(int angle)
        {
            while (angle < 0) { angle += 360; }
            while (angle > 360) { angle -= 360; }
            return (uint)angle;
        }

        /// <summary>
        /// Wrap an radian to be between 0 and 2PI.
        /// </summary>
        /// <param name="radian">Radian to wrap.</param>
        /// <returns>Wrapped radian.</returns>
        static public float WrapRadian(float radian)
        {
            float max = DegreeToRadian(360);
            while (radian < 0) { radian += max; }
            while (radian > max) { radian -= max; }
            return radian;
        }

        /// <summary>
        /// Return distance between two angles, in degrees.
        /// For example: AnglesDistance(90, 45) will return 45, 
        /// AnglesDistance(1, 360) will return 1, etc..
        /// </summary>
        /// <param name="angle1">First angle to check distance from (degrees).</param>
        /// <param name="angle2">Second angle to check distance from (degrees).</param>
        /// <returns>Return minimal degree between two angles.</returns>
        static public uint AnglesDistance(uint angle1, uint angle2)
        {
            // calc distance from 1 to 2
            int a = (int)angle1 - (int)angle2;
            while (a < 0) a += 360;

            // if less than 180, this is the shortest distance between angles
            if (a <= 180)
            {
                return (uint)a;
            }
            // if more than 180, shortest distance is 360 - a
            else
            {
                return (uint)(360 - a);
            }
        }

        // used as default random object if not provided
        static Random _rand = new Random();

        /// <summary>
        /// pick a random index based of list of probabilities (array of floats representing chances).
        /// </summary>
        /// <param name="probabilities">Array of floats representing chance for every index.</param>
        /// <param name="rand">Optional random instance to provide (if null will create new one internally).</param>
        /// <returns>The index of the item picked randomly from the list of probabilities.</returns>
        static public uint PickBasedOnProbability(float[] probabilities, Random rand = null)
        {
            // if not provided, create default random object
            if (rand == null) { rand = _rand; }

            // get random double
            double diceRoll = rand.NextDouble();

            // multiply diceroll by total
            double fac = 0.0;
            for (int i = 0; i < probabilities.Length; ++i)
            {
                fac += probabilities[i];
            }
            diceRoll *= fac;

            // iterate over probabilities and pick the one that match
            double cumulative = 0.0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulative += probabilities[i];
                if (diceRoll < cumulative)
                {
                    return (uint)i;
                }
            }

            // should never happen!
            throw new Exceptions.InternalError("Internal error with PickBasedOnProbability!");
        }
    }
}

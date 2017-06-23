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
// Help functions and utilities for animators.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Particles.Animators
{
    /// <summary>
    /// Misc animator related utilities.
    /// </summary>
    static public class AnimatorUtils
    {
        /// <summary>
        /// Random vector direction.
        /// </summary>
        public static Vector3 RandDirection(Random random, Vector3 baseVector, Vector3 randDir)
        {
            float originalLen = baseVector.Length();
            if (originalLen == 0f) originalLen = 1f;
            Vector3 newVelocity = baseVector;
            newVelocity.X += (float)(random.NextDouble() * (randDir.X * 2) - randDir.X);
            newVelocity.Y += (float)(random.NextDouble() * (randDir.Y * 2) - randDir.Y);
            newVelocity.Z += (float)(random.NextDouble() * (randDir.Z * 2) - randDir.Z);
            newVelocity.Normalize();
            return newVelocity * originalLen;
        }

        /// <summary>
        /// Random a vector from min and max.
        /// </summary>
        public static Vector3 RandVector(Random random, Vector3 minVector, Vector3 maxVector)
        {
            return new Vector3(
                    minVector.X + ((float)random.NextDouble() * (maxVector.X - minVector.X)),
                    minVector.Y + ((float)random.NextDouble() * (maxVector.Y - minVector.Y)),
                    minVector.Z + ((float)random.NextDouble() * (maxVector.Z - minVector.Z)));
        }

        /// <summary>
        /// Random a vector from max vector only.
        /// </summary>
        public static Vector3 RandVector(Random random, Vector3 maxVector)
        {
            return new Vector3(
                    -maxVector.X + ((float)random.NextDouble() * (maxVector.X * 2f)),
                    -maxVector.Y + ((float)random.NextDouble() * (maxVector.Y * 2f)),
                    -maxVector.Z + ((float)random.NextDouble() * (maxVector.Z * 2f)));
        }

        /// <summary>
        /// Random color value from base and rand color.
        /// </summary>
        public static Color RandColor(Random random, Color baseColor, Color colorJitter)
        {
            return new Color(
                    (byte)Math.Min((int)255, baseColor.R + random.Next(colorJitter.R)),
                    (byte)Math.Min((int)255, baseColor.G + random.Next(colorJitter.G)),
                    (byte)Math.Min((int)255, baseColor.B + random.Next(colorJitter.B)));
        }

        /// <summary>
        /// Random color value from min and max color values.
        /// </summary>
        public static Color RandColor2(Random random, Color minColor, Color maxColor)
        {
            return new Color(
                    random.Next(minColor.R, maxColor.R),
                    random.Next(minColor.G, maxColor.G),
                    random.Next(minColor.B, maxColor.B));
        }

        /// <summary>
        /// Calculate transition percent from current time and max time (return values from 0f to 1f).
        /// </summary>
        public static float CalcTransitionPercent(float timeAnimated, float maxTime)
        {
            return Math.Min(timeAnimated / maxTime, 1f);
        }
    }
}

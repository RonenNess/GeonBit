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
// Default materials to use when loading models.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.Core.Graphics.Materials
{
    /// <summary>
    /// Different material types.
    /// </summary>
    public enum MaterialTypes
    {
        /// <summary>
        /// Basic materials.
        /// </summary>
        Basic = 0,

        /// <summary>
        /// Skinned animated materials.
        /// </summary>
        Skinned = 1,

        /// <summary>
        /// Material with alpha test (usually used for sprites and billboards).
        /// </summary>
        AlphaTest = 2,
    }

    /// <summary>
    /// A callback to generate a default materials for a model type.
    /// </summary>
    /// <param name="mgEffect">MonoGame effect loaded by the mesh loader. You can use it to extract data.</param>
    /// <returns>Material instance.</returns>
    public delegate MaterialAPI MaterialGenerator(Effect mgEffect);

    /// <summary>
    /// Class to hold the callback to generate default materials.
    /// </summary>
    public static class DefaultMaterialsFactory
    {
        // all material generators
        static MaterialGenerator[] generators = new MaterialGenerator[] {
            
            // Basic
            (Effect mgEffect) => {return new BasicMaterial((BasicEffect)mgEffect, true);},

            // Skinned
            (Effect mgEffect) => {return new SkinnedMaterial((SkinnedEffect)mgEffect, true);},

            // Alpha test
            (Effect mgEffect) => {return new AlphaTestMaterial((AlphaTestEffect)mgEffect, true);},
        };

        /// <summary>
        /// Create and return a default material for a basic MonoGame effect.
        /// </summary>
        /// <param name="effect">Effect to create default material for.</param>
        static public MaterialAPI GetDefaultMaterial(Effect effect)
        {
            // create basic effects
            if (effect.GetType() == typeof(BasicEffect))
            {
                return Base(effect);
            }
            // create skinned effects
            else if (effect.GetType() == typeof(SkinnedEffect))
            {
                return Skinned(effect);
            }
            // create skinned effects
            else if (effect.GetType() == typeof(AlphaTestEffect))
            {
                return AlphaTest(effect);
            }
            // unknown type!
            else
            {
                throw new Exceptions.UnsupportedTypeException("Model had unsuporrted effect type!");
            }
        }

        /// <summary>
        /// Set the default material generator for a material type.
        /// </summary>
        /// <param name="type">Material type to set.</param>
        /// <param name="generator">Generator function to use on this material.</param>
        static public void SetDefaultMaterialGenerator(MaterialTypes type, MaterialGenerator generator)
        {
            generators[(int)type] = generator;
        }

        /// <summary>
        /// Function to generate default materials to newly-loaded models.
        /// </summary>
        static public MaterialGenerator Base
        {
            get { return generators[(int)MaterialTypes.Basic]; }
            set { generators[(int)MaterialTypes.Basic] = value; }
        }

        /// <summary>
        /// Function to generate default materials to newly-loaded skinned models.
        /// </summary>
        static public MaterialGenerator Skinned
        {
            get { return generators[(int)MaterialTypes.Skinned]; }
            set { generators[(int)MaterialTypes.Skinned] = value; }
        }

        /// <summary>
        /// Function to generate default materials to newly-loaded models with alpha-test effect.
        /// </summary>
        static public MaterialGenerator AlphaTest
        {
            get { return generators[(int)MaterialTypes.AlphaTest]; }
            set { generators[(int)MaterialTypes.AlphaTest] = value; }
        }
    }
}

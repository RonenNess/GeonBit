#region LICENSE
/**
 * For the purpose of making video games only, GeonBit is distributed under the MIT license.
 * to use this source code or GeonBit as a whole for any other purpose, please seek written 
 * permission from the library author.
 * 
 * Copyright (c) 2017 Ronen Ness [ronenness@gmail.com].
 * You may not remove this license notice.
 */
#endregion
#region File Description
//-----------------------------------------------------------------------------
// The API for a global manager class.
// A manager is a singleton in GeonBit that provide utilities to a specific
// aspect of the game, like physics, time utils, input, etc.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Managers
{
    /// <summary>
    /// API for a public manager class.
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// Initialize the manager.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called every frame during the Update() phase.
        /// </summary>
        void Update(GameTime time);

        /// <summary>
        /// Called every frame during the Draw() phase.
        /// </summary>
        void Draw(GameTime time);

        /// <summary>
        /// Called every constant X seconds during the Update() phase.
        /// </summary>
        /// <param name="interval">Time since last FixedUpdate().</param>
        void FixedUpdate(float interval);
    }
}

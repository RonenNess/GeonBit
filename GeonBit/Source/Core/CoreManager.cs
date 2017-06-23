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
// Core components global manager.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GeonBit.Core
{
    /// <summary>
    /// GeonBit.Core contains implementation of core components and integration with external libraries.
    /// This layer implements most of the engine stuff.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// General 'Core' manager.
    /// </summary>
    public static class CoreManager
    {
        /// <summary>
        /// Init all core components.
        /// </summary>
        /// <param name="graphics">Graphic device manager.</param>
        /// <param name="content">Content manager.</param>
        public static void Initialize(GraphicsDeviceManager graphics, ContentManager content)
        {
            // init resources manager
            ResourcesManager.Instance.Initialize(content);

            // init physics callbacks
            Physics.PhysicsWorld.Initialize();

            // init graphics
            Graphics.GraphicsManager.Initialize(graphics);
        }
    }
}

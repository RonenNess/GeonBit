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
// This file is just a copy of the GeonBit.UI MouseButtons, to create a proxy of 
// UI's MouseButtons but inside GeonBit namespace.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

namespace GeonBit.Input
{
    /// <summary>
    /// Supported mouse buttons.
    /// Note: this is a proxy for the GeonBit.UI MouseButton enum, just to create encapsulation.
    /// </summary>
    public enum MouseButton
    {
        ///<summary>Left mouse button.</summary>
        Left = UI.MouseButton.Left,

        ///<summary>Right mouse button.</summary>
        Right = UI.MouseButton.Right,

        ///<summary>Middle mouse button (eg scrollwheel when clicked).</summary>
        Middle = UI.MouseButton.Middle
    };
}

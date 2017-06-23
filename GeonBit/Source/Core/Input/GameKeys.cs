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
// A layer of virtual keys with game meaning.
// These are the keys that do in-game actions, and we assign keyboard, mouse and
// other input methods to them.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

namespace GeonBit.Input
{
    /// <summary>
    /// GeonBit.Input implement input methods and utils.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Different game keys, eg keyboard keys that active things in game.
    /// You can map different input method to game keys.
    /// </summary>
    public enum GameKeys
    {
        /// <summary>
        /// Main fire / action key (for example, Left mouse button / Ctrl..).
        /// </summary>
        Fire,

        /// <summary>
        /// Alternative fire / action key (for example, Right mouse button / Alt..).
        /// </summary>
        AlternativeFire,

        /// <summary>
        /// Special attack / ability (for example, Middle mouse button, Q..).
        /// </summary>
        Special,

        /// <summary>
        /// Custom ability (for example, 1..)
        /// </summary>
        Ability1,

        /// <summary>
        /// Custom ability (for example, 2..)
        /// </summary>
        Ability2,

        /// <summary>
        /// Custom ability (for example, 3..)
        /// </summary>
        Ability3,

        /// <summary>
        /// Custom ability (for example, 4..)
        /// </summary>
        Ability4,

        /// <summary>
        /// Custom ability (for example, 5..)
        /// </summary>
        Ability5,

        /// <summary>
        /// Custom ability (for example, 6..)
        /// </summary>
        Ability6,

        /// <summary>
        /// Custom ability (for example, 7..)
        /// </summary>
        Ability7,

        /// <summary>
        /// Custom ability (for example, 8..)
        /// </summary>
        Ability8,

        /// <summary>
        /// Custom ability (for example, 9..)
        /// </summary>
        Ability9,

        /// <summary>
        /// Custom ability (for example, 0..)
        /// </summary>
        Ability0,

        /// <summary>
        /// Interact with stuff (for example, E, Enter..)
        /// </summary>
        Interact,

        /// <summary>
        /// Escape / back key (for example, escape..)
        /// </summary>
        Escape,

        /// <summary>
        /// Accept / continue key (for example, E, Enter..)
        /// </summary>
        Accept,

        /// <summary>
        /// Jump key (for example, space..)
        /// </summary>
        Jump,

        /// <summary>
        /// Shift mode (for example, run/walk, stand still, etc..)
        /// </summary>
        ShiftMode,

        /// <summary>
        /// Key to toggle mode (for example, capslock..)
        /// </summary>
        Toggle,

        /// <summary>
        /// Move forward (for example, Up / W..)
        /// </summary>
        Forward,

        /// <summary>
        /// Move backwards (for example, Down / S..)
        /// </summary>
        Backward,

        /// <summary>
        /// Move Left (for example, Left / D..)
        /// </summary>
        Left,

        /// <summary>
        /// Move Right (for example, Right / A..)
        /// </summary>
        Right,

        /// <summary>
        /// Possible alternative to "Forward" for top-down games.
        /// </summary>
        Up,

        /// <summary>
        /// Possible alternative to "Backwards" for top-down games.
        /// </summary>
        Down,

        /// <summary>
        /// Open menu button (for example, Tab..)
        /// </summary>
        OpenMenu,

        /// <summary>
        /// Open console button (for example, ~..)
        /// </summary>
        OpenConsole,

        /// <summary>
        /// Open Chat (for example, T..)
        /// </summary>
        OpenChat,

        /// <summary>
        /// Open Map (for example, M..)
        /// </summary>
        OpenMap,

        /// <summary>
        /// Open Inventory (for example, I..)
        /// </summary>
        OpenInventory,

        /// <summary>
        /// An extra key for whatever.
        /// </summary>
        Extra1,

        /// <summary>
        /// An extra key for whatever.
        /// </summary>
        Extra2,

        /// <summary>
        /// An extra key for whatever.
        /// </summary>
        Extra3,

        /// <summary>
        /// An extra key for whatever.
        /// </summary>
        Extra4,

        /// <summary>
        /// An extra key for quickload.
        /// </summary>
        QuickLoad,

        /// <summary>
        /// An extra key for quicksave.
        /// </summary>
        QuickSave,
    }
}

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
// Provide Input-related utilities.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using GeonBit.Input;

namespace GeonBit.Managers
{
    /// <summary>
    /// Provide high-level game input API.
    /// </summary>
    public class GameInput : IManager
    {
        // singleton instance
        static GameInput _instance = null;

        /// <summary>
        /// Get time utils instance.
        /// </summary>
        public static GameInput Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameInput();
                }
                return _instance;
            }
        }

        /// <summary>
        /// To make it a true singleton.
        /// </summary>
        private GameInput()
        {
        }

        /// <summary>
        /// Initialize game input.
        /// </summary>
        public void Initialize()
        { 
            // set default fps keys
            SetDefaultFirstPersonControls();

            // init game key states
            foreach (GameKeys key in System.Enum.GetValues(typeof(GameKeys)))
            {
                _gameKeysDown[key] = false;
                _gamePreviousKeysDown[key] = false;
            }
        }

        /// <summary>
        /// input wrapper from the UI layer.
        /// </summary>
        private UI.InputHelper _input = new UI.InputHelper();

        // Keyboard mapping
        private Dictionary<KeyboardKeys, GameKeys> _keyboardMap = new Dictionary<KeyboardKeys, GameKeys>();

        // Mouse keys mapping
        private Dictionary<MouseButton, GameKeys> _mouseKeysMap = new Dictionary<MouseButton, GameKeys>();

        // Dictionary of last state of game keys (eg from previous frame).
        private Dictionary<GameKeys, bool> _gamePreviousKeysDown = new Dictionary<GameKeys, bool>();

        // Dictionary of game keys which are currently down.
        private Dictionary<GameKeys, bool> _gameKeysDown = new Dictionary<GameKeys, bool>();

        // movement vector based on game keys
        private Vector3 _movementVector = Vector3.Zero;

        /// <summary>
        /// Set default common keys which are relevant to all layouts.
        /// </summary>
        private void SetDefaultCommonKeys()
        {
            // fire
            _keyboardMap[KeyboardKeys.LeftControl] = GameKeys.Fire;
            _mouseKeysMap[MouseButton.Left] = GameKeys.Fire;

            // alt fire
            _keyboardMap[KeyboardKeys.LeftAlt] = GameKeys.AlternativeFire;
            _mouseKeysMap[MouseButton.Right] = GameKeys.AlternativeFire;

            // alt fire
            _keyboardMap[KeyboardKeys.Q] = GameKeys.Special;
            _mouseKeysMap[MouseButton.Middle] = GameKeys.Special;

            // Abilities
            _keyboardMap[KeyboardKeys.D0] = GameKeys.Ability0;
            _keyboardMap[KeyboardKeys.D1] = GameKeys.Ability1;
            _keyboardMap[KeyboardKeys.D2] = GameKeys.Ability2;
            _keyboardMap[KeyboardKeys.D3] = GameKeys.Ability3;
            _keyboardMap[KeyboardKeys.D4] = GameKeys.Ability4;
            _keyboardMap[KeyboardKeys.D5] = GameKeys.Ability5;
            _keyboardMap[KeyboardKeys.D6] = GameKeys.Ability6;
            _keyboardMap[KeyboardKeys.D7] = GameKeys.Ability7;
            _keyboardMap[KeyboardKeys.D8] = GameKeys.Ability8;
            _keyboardMap[KeyboardKeys.D9] = GameKeys.Ability9;

            // Interact
            _keyboardMap[KeyboardKeys.E] = GameKeys.Interact;
            _keyboardMap[KeyboardKeys.Enter] = GameKeys.Interact;

            // Accept
            _keyboardMap[KeyboardKeys.E] = GameKeys.Accept;
            _keyboardMap[KeyboardKeys.Enter] = GameKeys.Accept;

            // Escape
            _keyboardMap[KeyboardKeys.Escape] = GameKeys.Escape;

            // Jump
            _keyboardMap[KeyboardKeys.Space] = GameKeys.Jump;

            // Shift mode
            _keyboardMap[KeyboardKeys.LeftShift] = GameKeys.ShiftMode;

            // Toggle mode
            _keyboardMap[KeyboardKeys.CapsLock] = GameKeys.Toggle;

            // Open menu
            _keyboardMap[KeyboardKeys.Tab] = GameKeys.OpenMenu;

            // Open console
            _keyboardMap[KeyboardKeys.OemTilde] = GameKeys.OpenConsole;

            // Open console
            _keyboardMap[KeyboardKeys.OemTilde] = GameKeys.OpenConsole;
            _keyboardMap[KeyboardKeys.F1] = GameKeys.OpenConsole;

            // Open chat
            _keyboardMap[KeyboardKeys.T] = GameKeys.OpenChat;

            // Open map
            _keyboardMap[KeyboardKeys.M] = GameKeys.OpenMap;

            // Open inventory
            _keyboardMap[KeyboardKeys.I] = GameKeys.OpenInventory;

            // extra keys
            _keyboardMap[KeyboardKeys.Z] = GameKeys.Extra1;
            _keyboardMap[KeyboardKeys.X] = GameKeys.Extra2;
            _keyboardMap[KeyboardKeys.C] = GameKeys.Extra3;
            _keyboardMap[KeyboardKeys.V] = GameKeys.Extra4;

            // Quick save / load
            _keyboardMap[KeyboardKeys.F5] = GameKeys.QuickSave;
            _keyboardMap[KeyboardKeys.F9] = GameKeys.QuickLoad;

            // Left
            _keyboardMap[KeyboardKeys.Left] = GameKeys.Left;
            _keyboardMap[KeyboardKeys.A] = GameKeys.Left;

            // Right
            _keyboardMap[KeyboardKeys.Right] = GameKeys.Right;
            _keyboardMap[KeyboardKeys.D] = GameKeys.Right;
        }

        /// <summary>
        /// Set default keys for first-person controls.
        /// </summary>
        public void SetDefaultFirstPersonControls()
        {
            // set basic keys
            SetDefaultCommonKeys();

            // forward
            _keyboardMap[KeyboardKeys.Up] = GameKeys.Forward;
            _keyboardMap[KeyboardKeys.W] = GameKeys.Forward;

            // Backwards
            _keyboardMap[KeyboardKeys.Down] = GameKeys.Backward;
            _keyboardMap[KeyboardKeys.S] = GameKeys.Backward;

            // up
            _keyboardMap[KeyboardKeys.PageUp] = GameKeys.Up;

            // down
            _keyboardMap[KeyboardKeys.PageDown] = GameKeys.Down;
        }

        /// <summary>
        /// Set default keys for side-scroller controls.
        /// </summary>
        public void SetDefaultSideScrollerControls()
        {
            // set basic keys
            SetDefaultCommonKeys();

            // up
            _keyboardMap[KeyboardKeys.Up] = GameKeys.Up;
            _keyboardMap[KeyboardKeys.W] = GameKeys.Up;

            // down
            _keyboardMap[KeyboardKeys.Down] = GameKeys.Down;
            _keyboardMap[KeyboardKeys.S] = GameKeys.Down;
        }

        /// <summary>
        /// Set default keys for side-scroller controls.
        /// </summary>
        public void SetDefaultTopDownControls()
        {
            // set basic keys
            SetDefaultCommonKeys();

            // set space as fire
            _keyboardMap[KeyboardKeys.Space] = GameKeys.Fire;

            // up
            _keyboardMap[KeyboardKeys.Up] = GameKeys.Forward;
            _keyboardMap[KeyboardKeys.W] = GameKeys.Forward;

            // down
            _keyboardMap[KeyboardKeys.Down] = GameKeys.Backward;
            _keyboardMap[KeyboardKeys.S] = GameKeys.Backward;
        }

        /// <summary>
        /// Set default keys for side-scroller controls.
        /// </summary>
        public void SetDefaultIsometricControls()
        {
            // set basic keys
            SetDefaultCommonKeys();

            // up
            _keyboardMap[KeyboardKeys.Up] = GameKeys.Up;
            _keyboardMap[KeyboardKeys.W] = GameKeys.Up;

            // down
            _keyboardMap[KeyboardKeys.Down] = GameKeys.Down;
            _keyboardMap[KeyboardKeys.S] = GameKeys.Down;
        }

        /// <summary>
        /// Write current controls layout to a config file.
        /// </summary>
        public void SaveKeysLayout()
        {
            ConfigStorage.Instance.Set("controls.keys.conf", _keyboardMap, GameFiles.FileFormats.Binary);
            ConfigStorage.Instance.Set("controls.mouse.conf", _mouseKeysMap, GameFiles.FileFormats.Binary);
        }

        /// <summary>
        /// Load current controls layout from config file.
        /// </summary>
        public void LoadKeysLayout()
        {
            _keyboardMap = ConfigStorage.Instance.Get<Dictionary<KeyboardKeys, GameKeys>>("controls.keys.conf", GameFiles.FileFormats.Binary);
            _mouseKeysMap = ConfigStorage.Instance.Get<Dictionary<MouseButton, GameKeys>>("controls.mouse.conf", GameFiles.FileFormats.Binary);
        }

        /// <summary>
        /// Called every frame during the Draw() process.
        /// </summary>
        public void Draw(GameTime time)
        {
        }

        /// <summary>
        /// Update input.
        /// </summary>
        /// <param name="time">GameTime, as provided by MonoGame.</param>
        public void Update(GameTime time)
        {
            // update input helper
            _input.Update(time);

            // update game keys
            UpdateGameKeys();

            // update axis movement (move on X / Y axis based on mouse / keyboard).
            UpdateAxisMovement();
        }

        /// <summary>
        /// Called every constant X seconds during the Update() phase.
        /// </summary>
        /// <param name="interval">Time since last FixedUpdate().</param>
        public void FixedUpdate(float interval)
        {
        }

        /// <summary>
        /// Update game keys state.
        /// </summary>
        private void UpdateGameKeys()
        {
            // disable all keys and set previous state
            foreach (GameKeys key in new List<GameKeys>(_gameKeysDown.Keys))
            {
                _gamePreviousKeysDown[key] = _gameKeysDown[key];
                _gameKeysDown[key] = false;
            }

            // first by mouse buttons mapping
            foreach (KeyValuePair<MouseButton, GameKeys> entry in _mouseKeysMap)
            {
                _gameKeysDown[entry.Value] = _input.MouseButtonDown((UI.MouseButton)entry.Key);
            }

            // now by keyboard buttons mapping
            foreach (KeyValuePair<KeyboardKeys, GameKeys> entry in _keyboardMap)
            {
                if (_input.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)entry.Key))
                {
                    _gameKeysDown[entry.Value] = true;
                }
            }
        }

        /// <summary>
        /// Update X and Y movements.
        /// </summary>
        private void UpdateAxisMovement()
        {
            // zero axis movement
            _movementVector = Vector3.Zero;

            // first check by keys
            if (_gameKeysDown[GameKeys.Up])
            {
                _movementVector += Vector3.Up;
            }
            if (_gameKeysDown[GameKeys.Down])
            {
                _movementVector += Vector3.Down;
            }
            if (_gameKeysDown[GameKeys.Left])
            {
                _movementVector += Vector3.Left;
            }
            if (_gameKeysDown[GameKeys.Right])
            {
                _movementVector += Vector3.Right;
            }
            if (_gameKeysDown[GameKeys.Forward])
            {
                _movementVector += Vector3.Forward;
            }
            if (_gameKeysDown[GameKeys.Backward])
            {
                _movementVector += Vector3.Backward;
            }
        }

        /// <summary>
        /// Get movement vector based on game keys currently pressed.
        /// </summary>
        public Vector3 MovementVector
        {
            get { return _movementVector; }
        }

        /// <summary>
        /// Assign a keyboard key to a Game key.
        /// </summary>
        /// <param name="keyboardKey">Keyboard key.</param>
        /// <param name="gameKey">Game key.</param>
        public void AssignKeyboardKeyToGameKey(KeyboardKeys keyboardKey, GameKeys gameKey)
        {
            _keyboardMap[keyboardKey] = gameKey;
        }

        /// <summary>
        /// Assign a mouse button to a Game key.
        /// </summary>
        /// <param name="mouseButton">Mouse button.</param>
        /// <param name="gameKey">Game key.</param>
        public void AssignMouseButtonToGameKey(MouseButton mouseButton, GameKeys gameKey)
        {
            _mouseKeysMap[mouseButton] = gameKey;
        }

        /// <summary>
        /// Get if a key is currently down.
        /// </summary>
        /// <param name="key">Keyboard key to test.</param>
        /// <returns>If keyboard key is currently down.</returns>
        public bool IsKeyboardKeyDown(KeyboardKeys key)
        {
            return _input.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)key);
        }

        /// <summary>
        /// Get if a key was released in this very frame.
        /// </summary>
        /// <param name="key">Keyboard key to test.</param>
        /// <returns>If keyboard key was released in this frame.</returns>
        public bool IsKeyboardKeyReleased(KeyboardKeys key)
        {
            return _input.IsKeyReleased((Microsoft.Xna.Framework.Input.Keys)key);
        }

        /// <summary>
        /// Get if a game key is currently down.
        /// </summary>
        /// <param name="key">Game key to test.</param>
        /// <returns>If Game key is currently down.</returns>
        public bool IsKeyDown(GameKeys key)
        {
            return _gameKeysDown[key];
        }

        /// <summary>
        /// Get if a game key was pressed down in this very frame.
        /// </summary>
        /// <param name="key">Game key to test.</param>
        /// <returns>If Game key was pressed in this frame.</returns>
        public bool IsKeyPressed(GameKeys key)
        {
            return _gameKeysDown[key] && !_gamePreviousKeysDown[key];
        }

        /// <summary>
        /// Get if a game key was released in this very frame.
        /// </summary>
        /// <param name="key">Game key to test.</param>
        /// <returns>If Game key was released in this frame.</returns>
        public bool IsKeyReleased(GameKeys key)
        {
            return !_gameKeysDown[key] && _gamePreviousKeysDown[key];
        }

        /// <summary>
        /// Get mouse current position.
        /// </summary>
        public Vector2 MousePosition
        {
            get
            {
                return _input.MousePosition;
            }
        }

        /// <summary>
        /// Get mouse position diff since last frame.
        /// </summary>
        public Vector2 MousePositionDiff
        {
            get
            {
                return _input.MousePositionDiff;
            }
        }

        /// <summary>
        /// Set mouse current position.
        /// </summary>
        /// <param name="newPos">New mouse position to set.</param>
        public void SetMousePosition(Vector2 newPos)
        {
            _input.UpdateCursorPosition(newPos);
        }

        /// <summary>
        /// Get mouse wheel current value.
        /// </summary>
        public int MouseWheel
        {
            get { return _input.MouseWheel; }
        }

        /// <summary>
        /// Get mouse wheel diff from last frame.
        /// </summary>
        public int MouseWheelDiff
        {
            get { return _input.MouseWheelChange; }
        }

        /// <summary>
        /// Get if mouse button is currently down.
        /// </summary>
        /// <param name="button">Mouse button to test.</param>
        /// <returns>If mouse button is down.</returns>
        public bool IsMouseButtonDown(MouseButton button = MouseButton.Left)
        {
            return _input.MouseButtonDown((UI.MouseButton)button);
        }

        /// <summary>
        /// Get if mouse button is was released on this frame.
        /// </summary>
        /// <param name="button">Mouse button to test.</param>
        /// <returns>If mouse button was released this frame.</returns>
        public bool MouseButtonReleased(MouseButton button = MouseButton.Left)
        {
            return _input.MouseButtonReleased((UI.MouseButton)button);
        }

        /// <summary>
        /// Get if mouse button was pressed on this frame.
        /// </summary>
        /// <param name="button">Mouse button to test.</param>
        /// <returns>If mouse button was pressed this frame.</returns>
        public bool MousePressed(MouseButton button = MouseButton.Left)
        {
            return _input.MouseButtonPressed((UI.MouseButton)button);
        }

        /// <summary>
        /// Get if any mouse button is currently down.
        /// </summary>
        public bool AnyMouseButtonDown
        {
            get
            {
                return _input.AnyMouseButtonDown();
            }
        }

        /// <summary>
        /// Read keyboard input into a string. Get one character at a time.
        /// </summary>
        /// <example>
        /// string userInput = "";
        /// int pos = 0;
        /// while (!GameInput.Instance.IsKeyDown(GameKeys.Escape)) 
        /// {
        ///     userInput = userInput + GameInput.Instance.GetKeyboardInput(currText, ref pos);
        /// }
        /// </example>
        /// <param name="currText">Current text to push input into.</param>
        /// <param name="pos">Position to push new characters to (will increase with every typed character).</param>
        public string GetKeyboardInput(string currText, ref int pos)
        {
            return _input.GetTextInput(currText, ref pos);
        }
    }
}

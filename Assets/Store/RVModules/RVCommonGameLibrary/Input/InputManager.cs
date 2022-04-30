// Created by Ronis Vision. All rights reserved
// 07.08.2020.

using System;
using System.Collections.Generic;
using RVModules.RVUtilities;

namespace RVModules.RVCommonGameLibrary.Input
{
    /// <summary>
    /// For now it allows to globally apply lock for keyboard and specific mouse buttons
    /// usage:
    /// InputManager.Instance.AddKeyboardLock(() => ... );
    /// if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && !InputManager.Instance.KeyboardLocked())
    /// {
    ///     ...
    /// }
    /// </summary>
    public class InputManager : MonoSingleton<InputManager>
    {
        #region Fields

        private List<Func<bool>> keyboardLocks = new List<Func<bool>>();
        private List<Func<int, bool>> mouseButtonLocks = new List<Func<int, bool>>();

        #endregion

        #region Public methods

        public void AddKeyboardLock(Func<bool> func) => keyboardLocks.Add(func);

        public void RemoveKeyboardLock(Func<bool> func) => keyboardLocks.Remove(func);

        public void AddMouseButtonLock(Func<int, bool> func) => mouseButtonLocks.Add(func);

        public void RemoveMouseButtonLock(Func<int, bool> func) => mouseButtonLocks.Remove(func);

        /// <summary>
        /// Is keyboard locked right now?
        /// </summary>
        public bool KeyboardLocked()
        {
            foreach (var keyboardLock in keyboardLocks)
                if (keyboardLock())
                    return false;

            return true;
        }

        /// <summary>
        /// Is mouse button locked right now?
        /// </summary>
        public bool MouseButtonLocked(int button)
        {
            foreach (var mouseButtonLock in mouseButtonLocks)
                if (mouseButtonLock(button))
                    return true;

            return false;
        }

        #endregion
    }
}
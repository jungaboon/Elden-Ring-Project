// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;

namespace RVModules.RVUtilities
{
    /// <summary>
    /// Works like bool, but calls some method on value change, property-simulator-like-thingy, so fields can be used as props. Can be used just like normal bool.
    /// (Unity can't display props in inspector and is often not worth time to create custom editor for the)
    /// </summary>
    public class ActionBool
    {
        #region Fields

        private bool boolValue;
        private Action<bool> onValueChange;

        #endregion

        #region Properties

        public bool BoolValue
        {
            get => boolValue;

            set
            {
                if (value != boolValue && onValueChange != null)
                    onValueChange.Invoke(value);
                boolValue = value;
            }
        }

        #endregion

        public ActionBool(Action<bool> onValueChange) => this.onValueChange = onValueChange;

        #region Public methods

        public static implicit operator bool(ActionBool _actionBool) => _actionBool.boolValue;

        #endregion

        //public static implicit operator ActionBool(bool _value)
        //{
        //    ActionBool ab = new ActionBool(null);
        //    ab.BoolValue = _value;
        //    return ab;
        //}
    }
}
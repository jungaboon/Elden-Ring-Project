// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System.Collections.Generic;

namespace RVModules.RVUtilities.Extensions
{
    public static class ListExtensions
    {
        #region Public methods

        public static void Move<T>(this IList<T> list, int iIndexToMove, MoveDirection direction)
        {
            if (direction == MoveDirection.Up)
            {
                if (iIndexToMove <= 0) return;

                var old = list[iIndexToMove - 1];
                list[iIndexToMove - 1] = list[iIndexToMove];
                list[iIndexToMove] = old;
            }
            else
            {
                if (iIndexToMove >= list.Count - 1) return;
                var old = list[iIndexToMove + 1];
                list[iIndexToMove + 1] = list[iIndexToMove];
                list[iIndexToMove] = old;
            }
        }

        #endregion
    }

    public enum MoveDirection
    {
        Up,
        Down
    }
}
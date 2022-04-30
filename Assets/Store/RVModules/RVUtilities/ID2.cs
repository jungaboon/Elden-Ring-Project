// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;

#pragma warning disable 660,661

namespace RVModules.RVUtilities
{
    /// <summary>
    /// 2d index
    /// </summary>
    [Serializable] public struct ID2
    {
        public ID2(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public int x;
        public int y;

        public static ID2 operator +(ID2 _id2, ID2 _id22) => new ID2 {x = _id2.x + _id22.x, y = _id2.y + _id22.y};

        public override string ToString() => x + ", " + y;

        public static bool operator ==(ID2 _id2, ID2 _id22) => _id2.x == _id22.x && _id2.y == _id22.y;

        public static bool operator !=(ID2 _id2, ID2 _id22) => _id2.x != _id22.x || _id2.y != _id22.y;
    }
}
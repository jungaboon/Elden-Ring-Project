// Created by Ronis Vision. All rights reserved
// 13.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities
{
    public static class RandomChance
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_percentChance">Percentage, value from 0 to 100</param>
        /// <returns></returns>
        public static bool Get(float _percentChance) => Random.Range(0, 100) < _percentChance;
    }
}
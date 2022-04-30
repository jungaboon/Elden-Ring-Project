// Created by Ronis Vision. All rights reserved
// 07.02.2021.

using System;
using RVModules.RVCommonGameLibrary.Gameplay;
using UnityEngine;

namespace RVHonorAI.Examples
{
    public class AddSpawnedCharsToCalcAvgPos : MonoBehaviour
    {
        [SerializeField]
        private CharacterSpawner[] spawners;

        [SerializeField]
        private SetPositionToAverage positionToAverage;

        private void Awake()
        {
            foreach (var spawner in spawners)
            {
                spawner.onCharacterSpawn += (_o, _vector3, _arg3) => positionToAverage.Transforms.Add(_o.transform);
            }
        }
    }
}
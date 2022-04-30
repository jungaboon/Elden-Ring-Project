// Created by Ronis Vision. All rights reserved
// 07.02.2021.

using System;
using UnityEngine;

namespace RVHonorAI.Examples
{
    public class LookAt : MonoBehaviour
    {
        [SerializeField]
        private Transform transformToLookAt;
        
        private void Update()
        {
            transform.LookAt(transformToLookAt.position);
        }
    }
}
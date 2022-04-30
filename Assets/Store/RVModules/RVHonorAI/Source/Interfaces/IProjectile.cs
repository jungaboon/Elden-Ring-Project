// Created by Ronis Vision. All rights reserved
// 27.05.2021.

using System;
using RVHonorAI.Combat;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVHonorAI
{
    public interface IProjectile : IPoolable
    {
        void Shoot(Component _shooter, float _damage, Vector3 pos, Vector3 dir, ITarget _targetbool, bool _dmgEnemyOnly);

        /// <summary>
        /// Collision can be null !!
        /// </summary>
        Action<Transform, Collision> OnHit { get; set; }
    }
}
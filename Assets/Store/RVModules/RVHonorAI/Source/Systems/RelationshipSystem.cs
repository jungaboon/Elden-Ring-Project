// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System.Linq;
using RVModules.RVSmartAI;
using UnityEngine;

namespace RVHonorAI.Systems
{
    public class RelationshipSystem : ScriptableObject
    {
        #region Public methods

        public virtual bool IsEnemy(IRelationship _our, IRelationship _other, bool _contraCheck)
        {
            // check if this is self-check, we have to compare game object, because we don't know which component may implement IRelationship
            if (_other.GameObject() == _our.GameObject()) return false;

            // check for both sides-relationship
            if (!_contraCheck && _other.IsEnemy(_our, true)) return true;

            if (_our.TreatNeutralCharactersAsEnemies) return !_our.IsAlly(_other);

            if (_our.AiGroup == null || _other.AiGroup == null || _our.AiGroup == _other.AiGroup) return false;

            return _other.AiGroup.enemyToAll || _our.AiGroup.enemies.Contains(_other.AiGroup) || _other.AiGroup.enemies.Contains(_our.AiGroup);
        }

        public virtual bool IsAlly(IRelationship _our, IRelationship _other)
        {
            if (_other.GameObject() == _our.GameObject()) return true;
            if (_our.AiGroup == null || _other.AiGroup == null) return false;
            return _other.AiGroup.allyToAll || _our.AiGroup == _other.AiGroup || _our.AiGroup.allies.Contains(_our.AiGroup);
        }

        #endregion
    }
}
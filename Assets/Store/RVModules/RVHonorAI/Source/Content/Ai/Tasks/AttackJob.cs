// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.Content.AI.Tasks;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVHonorAI.Content.AI.Tasks
{
    /// <summary>
    /// Required context: IAttackProvider, IAttackSoundPlayer(optional), ICharacterAnimation(optional), IAttacker(if animationEventBasedAttack is false)
    /// </summary>
    public class AttackJob : AiJob
    {
        #region Fields

        // moved to attacks system :D
//        [Header("If set to true, DealDamage should be called by unity event added to attack animation")]
//        [SerializeField]
//        private BoolProvider animationEventBasedAttack;

//        [Header("Time in seconds after which attack will deal damage.")]
//        [SerializeField]
//        private FloatProvider damageTime;

        private bool attacked;
        private bool hasAnimation;
        private float attackTime;
        private float attackDuration;
        private ICharacterAnimation characterAnimation;
        private IAttackSoundPlayer attackSoundPlayer;
        private IAttacker attacker;
        private IAttack currentAttack;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Plays attack animation, plays attack sound and deal damage to current target";

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            characterAnimation = GetComponentFromContext<ICharacterAnimationProvider>()?.CharacterAnimation;
            attackSoundPlayer = GetComponentFromContext<IAttackSoundPlayer>();
            attacker = GetComponentFromContext<IAttacker>();
            hasAnimation = characterAnimation != null;
        }

        protected override void OnJobStart()
        {
            attacked = false;
            attackDuration = 2;
            currentAttack = attacker.CurrentAttack;
            if (currentAttack == null)
            {
                FinishJob();
                return;
            }

            attackDuration = currentAttack.AttackDuration;
            if (hasAnimation) characterAnimation.PlayAttackAnimation(attacker.CurrentAttack.AnimationClip);
            attackSoundPlayer?.PlayAttackSound();
        }

        protected override void OnJobUpdate(float _dt)
        {
            attackTime += UnityTime.DeltaTime;

//            if (!attacked && animationEventBasedAttack.GetData() == false && attackTime >= damageTime)
            if (!attacked && attackTime >= attacker.CurrentAttack.DamageTime)
            {
                attacker.Attack();
                attacked = true;
            }

            if (attackTime > attackDuration + UnityTime.DeltaTime) FinishJob();
        }

        protected override void OnJobFinish()
        {
            currentAttack = null;
            attackTime = 0;
        }

        #endregion
    }
}
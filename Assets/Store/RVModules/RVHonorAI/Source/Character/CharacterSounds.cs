// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using RVModules.RVCommonGameLibrary.Audio;

namespace RVHonorAI
{
    [Serializable] public struct CharacterSounds
    {
//        public SoundConfig noWeaponAttackSound;
//        public SoundConfig noWeaponHitSound;
        public SoundConfig footstepSounds;
        public SoundConfig gotHitSound;
        public float chanceToPlayGotHitSound;
        public SoundConfig dieSound;
        public SoundConfig fightSound;
        public float chanceToPlayFightSound;
        public SoundConfig attackSound;
        public float chanceToPlayAttackSound;
    }
}
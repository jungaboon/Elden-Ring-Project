// Created by Ronis Vision. All rights reserved
// 01.02.2021.

using UnityEditor;
using UnityEngine;

namespace RVHonorAI.Editor
{
    public static class HonorAiMenus
    {
        #region Not public methods

        [MenuItem("RVHonorAI/Create ai zone")]
        private static void CreateAiZone()
        {
            var go = new GameObject("AiZone");
            var aiZone = CreateAiZone(go);
            Selection.objects = new Object[] {go};
        }

        private static AiZone CreateAiZone(GameObject go)
        {
            var aiZone = go.AddComponent<AiZone>();
            var sphereColllider = go.AddComponent<SphereCollider>();
            sphereColllider.radius = 25;
            sphereColllider.isTrigger = true;
            return aiZone;
        }

        [MenuItem("RVHonorAI/Create HonorAi manager")]
        private static void CreateRvHonorAiManager()
        {
            var man = Object.FindObjectOfType<HonorAIManager>();
            if (man != null)
            {
                Debug.Log("Theres already HonorAiManager on scene!");
                Selection.objects = new Object[] {man.gameObject};
                return;
            }

            var honMan = Object.Instantiate(Resources.Load("HonorAiManager")) as GameObject;
            honMan.name = "HonorAiManager";
            Selection.objects = new Object[] {honMan};

//            GameObject go = new GameObject("HonorAiManager");
//            var manager = go.AddComponent<HonorAIManager>();
//            var ds = go.AddComponent<BasicDamageSystem>();
//            manager.damageSystemObject = ds;
//            Selection.objects = new Object[] {go};
        }

        [MenuItem("RVHonorAI/Create character spawner")]
        private static void CreateCharacterSpawner()
        {
            var go = new GameObject("CharacterSpawner");
            go.AddComponent<CharacterSpawner>();
            Selection.objects = new Object[] {go};
        }

        [MenuItem("RVHonorAI/Create character spawner with ai zone")]
        private static void CreateCharacterSpawnerAiZone()
        {
            var go = new GameObject("CharacterSpawnerWithZone");
            var cs = go.AddComponent<CharacterSpawner>();
            cs.aiZone = CreateAiZone(go);

            Selection.objects = new Object[] {go};
        }

        #endregion
    }
}
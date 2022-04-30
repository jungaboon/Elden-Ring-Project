// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using System.Collections;
using System.Collections.Generic;
using RVModules.RVUtilities;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace RVHonorAI
{
    /// <summary>
    /// Spawns Character prefab using settings in inspector. Can spawn all in one frame, or spread spawning across multiple frames
    /// todo decouple from character class to be more universal ?
    /// todo arrayu of character prefabs to spawn instead of one type ?
    /// todo inspector hints
    /// </summary>
    public class CharacterSpawner : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Character prefab to spawn 
        /// </summary>
        public Character characterPrefab;

        /// <summary>
        /// How many characters should be spawned by single Spawn() call
        /// </summary>
        public int spawnCount = 10;

        [ConditionalHide(nameof(useBoxSizeSpawn), hideInInspector = true, inverse = true)]
        /// <summary>
        /// Spawn radius circle in meters
        /// </summary>
        public float radius = 10;

        [ConditionalHide(nameof(useBoxSizeSpawn), hideInInspector = true)]
        /// <summary>
        /// Spawn zone size
        /// </summary>
        public Vector2 size;

        /// <summary>
        /// Spawn zone will be box instead of circle
        /// </summary>
        public bool useBoxSizeSpawn;

        /// <summary>
        /// Waypoints that will be added for spawned characters
        /// </summary>
        [HideInInspector]
        public List<Waypoint> waypoints;

        /// <summary>
        /// Connected AiZone
        /// </summary>
        public AiZone aiZone;

        /// <summary>
        /// Should spawn be called at the beginning
        /// </summary>
        public bool spawnOnStart = true;

        /// <summary>
        /// Should spawn be initiated by AiZone? Automatically set to false after spawn initiated by zone   
        /// </summary>
        public bool spawnByAiZone;

        /// <summary>
        /// Event called upon spawn of every single Character
        /// Last two argumentss are spawn position and rotation
        /// </summary>
        /// <returns></returns>
        public Action<GameObject, Vector3, Quaternion> onCharacterSpawn;

        /// <summary>
        /// Called when spawning sequence starts
        /// Argument specifies spawns per frame
        /// </summary>
        public Action<int> onSpawningStart;

        /// <summary>
        /// Called when spawning sequence ends
        /// Argument specifies spawns per frame
        /// </summary>
        public Action<int> onSpawningEnd;

        /// <summary>
        /// Called after spawning is done
        /// </summary>
        public UnityEvent onSpawn;

        /// <summary>
        /// 0 if you want all spawns occur in single frame
        /// </summary>
        [SerializeField]
        protected int spawnsPerFrame;

        /// <summary>
        /// LayerMask used to detect ground height by raycasts
        /// </summary>
        [Tooltip("LayerMask used to detect ground height by raycasts")]
        [SerializeField]
        protected LayerMask layerMask = 1 << 11;

        /// <summary>
        /// LayerMask used to detect obstacles where you do not want to spawn
        /// </summary>
        [SerializeField]
        protected LayerMask obstaclesLayerMask = 1 << 9;

        /// <summary>
        /// Character radius. Used for detecting obstacles
        /// </summary>
        [SerializeField]
        protected float characterRadius = .5f;

        #endregion

        #region Public methods

        /// <summary>
        /// Starts spawning characters using settings from inspector
        /// </summary>
        public void Spawn() => StartCoroutine(SpawnCoroutine(spawnsPerFrame));


        /// <summary>
        /// Starts spawning characters using settings from inspector, with overriden spawns per frame
        /// </summary>
        public void Spawn(int _spawnsPerFrame) => StartCoroutine(SpawnCoroutine(_spawnsPerFrame));

        #endregion

        #region Not public methods

        private void Start()
        {
            if (spawnOnStart) Spawn(spawnsPerFrame);
            if (aiZone != null) aiZone.onActivation.AddListener(SpawnByAiZone);
        }

        private void SpawnByAiZone()
        {
            if (!spawnByAiZone) return;
            Spawn();
            spawnByAiZone = false;
            //aiZone.onActivation.RemoveListener(SpawnByAiZone);
        }

        /// <summary>
        /// Override if you want custom spawning logic
        /// </summary>
        protected virtual IEnumerator SpawnCoroutine(int _spawnsPerFrame = 0)
        {
            if (characterPrefab == null)
            {
                Debug.LogError("Assign character prefab first");
                yield break;
            }

            onSpawningStart?.Invoke(_spawnsPerFrame);

            var waypointsTransformed = new List<Waypoint>();
            var chars = new List<Character>();
            var zone = aiZone != null;
            var myPos = transform.position;

            foreach (var waypoint in waypoints)
                waypointsTransformed.Add(new Waypoint {position = waypoint.position + transform.position, radius = waypoint.radius});

            var spawnedPerFrame = 0;
            var failedTries = 0;

            for (var i = 0; i < spawnCount; i++)
            {
                Vector3 pos;
                if (useBoxSizeSpawn)
                    pos = new Vector3(Random.Range(-size.x * .5f, size.x * .5f), 0, Random.Range(-size.y * .5f, size.y * .5f));
                else
                    pos = Random.insideUnitSphere * radius;

                pos.y = 500;
                pos += myPos;

                if (Physics.SphereCast(pos, characterRadius, Vector3.down, out var hit, 1000, obstaclesLayerMask))
                {
                    failedTries++;
                    if (failedTries < 500)
                    {
                        i--;
                        continue;
                    }
                }

                failedTries = 0;

                pos.y = Physics.Raycast(pos, Vector3.down, out var hitInfo, 1000, layerMask) ? hitInfo.point.y : myPos.y;

                var rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                var newCharGo = Instantiate(characterPrefab.gameObject, pos, rotation);
                newCharGo.name = characterPrefab.gameObject.name;
                var newCharacter = newCharGo.GetComponent<Character>();
                if (zone) chars.Add(newCharacter);

                foreach (var waypoint in waypointsTransformed) newCharacter.CharacterAi.Waypoints.Add(new Waypoint(waypoint));

                spawnedPerFrame++;

                onCharacterSpawn?.Invoke(newCharacter.gameObject, pos, rotation);

                if (_spawnsPerFrame > 0 && spawnedPerFrame > _spawnsPerFrame)
                {
                    spawnedPerFrame = 0;
                    yield return null;
                }
            }

            yield return null;

            if (zone) aiZone.AddCharacters(chars);

            onSpawningEnd?.Invoke(_spawnsPerFrame);
            onSpawn?.Invoke();
        }

        #endregion
    }
}
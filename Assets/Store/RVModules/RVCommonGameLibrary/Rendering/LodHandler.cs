// Created by Ronis Vision. All rights reserved
// 07.03.2021.

using System;
using System.Collections.Generic;
using System.Linq;
using RVModules.RVLoadBalancer;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Rendering
{
    /// <summary>
    /// Handles lod switching of any type (mesh switching for mesh and skinned mesh renderers and game objects disabling)
    /// Should be added at object root, MeshSwitcher will be automatically added to every child with renderer
    /// </summary>
    public class LodHandler : MonoBehaviour
    {
        #region Fields

        [Tooltip("Children of this transform will be treated as lod levels, child with index of current lod will be enabled")]
        public Transform gosToSwitch;

        [SerializeField]
        [Tooltip("Should lods still be changed when disabled(component or gameObject not active)")]
        private bool updateWhenDisabled;

        /// <summary>
        /// First arg is old lod, second is new
        /// </summary>
        public Action<int, int> onLodChange;

        public Action onShow;
        public Action onHide;

        public bool automaticallySetRangeBasedOnSize = true;

        [ConditionalHide(nameof(automaticallySetRangeBasedOnSize), hideInInspector = true)]
        [Tooltip("Multiplier for automatic setting range based on size")]
        public float autoSetRangeMul = 1.0f;

        /// <summary>
        /// Distance at which we should set max LOD (higher lod - less detail mesh)
        /// </summary>
        [SerializeField]
        [ConditionalHide(nameof(automaticallySetRangeBasedOnSize), hideInInspector = true, inverse = true)]
        private float range = 100f;

        [SerializeField]
        private bool distanceCulling = true;

        [SerializeField]
        [ConditionalHide(nameof(distanceCulling), hideInInspector = true)]
        private float distanceCullingMul = 1.0f;

        private List<MeshSwitcher> meshSwitchers;

        private Transform[] transformToSwitchChildren;


//        [SerializeField]
        private int lodCount;

        private bool initialized;

//        [SerializeField]
        private float camDist;

        [SerializeField]
        private int updateRate = 20;

        [SerializeField]
        private int currentLod;

        [SerializeField]
        protected bool culled;

        #endregion

        #region Properties

        public bool IsInCullingRange => CamDist * LodSettings.Instance.LodBias < CullDistance;

        public int LodCount => lodCount;

        public bool Hidden { get; private set; }

        public int CurrentLod
        {
            get => currentLod;
            private set => currentLod = value;
        }

        /// <summary>
        /// Last calculated distance from camera to this LodHandler
        /// </summary>
        public float CamDist => camDist;

        public bool DistanceCulling
        {
            get => distanceCulling;
            set => distanceCulling = value;
        }

        /// <summary>
        /// Multiplier for distance-based culling, distance culling is CullDistanceMul * Range
        /// Can't be lower than 1.0
        /// </summary>
        public float CullDistanceMul
        {
            get => distanceCullingMul;
            set => distanceCullingMul = Mathf.Clamp(value, 1, float.MaxValue);
        }

        /// <summary>
        /// Distance after object will be automatically hidden
        /// </summary>
        public float CullDistance => distanceCullingMul * range;

        /// <summary>
        /// Distance at which we should set max LOD (higher lod - less detail mesh)
        /// </summary>
        public float Range
        {
            get => range;
            set => range = value;
        }

        /// <summary>
        /// Distance based culling
        /// </summary>
        public bool Culled => culled;

        public List<MeshSwitcher> MeshSwitchers
        {
            get => meshSwitchers;
            set => meshSwitchers = value;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public Renderer[] GetRenderersFromAllLods()
        {
            var rens = new List<Renderer>();
            foreach (var LodSwitcher in meshSwitchers)
            {
                if (LodSwitcher.SkinnedMeshRen != null) rens.Add(LodSwitcher.SkinnedMeshRen);
                if (LodSwitcher.MeshRenderer != null) rens.Add(LodSwitcher.MeshRenderer);
            }

            if (gosToSwitch == null) return rens.ToArray();
            for (var i = 0; i < gosToSwitch.childCount; i++)
            {
                var r = gosToSwitch.GetChild(i).GetComponent<Renderer>();
                if (r == null) continue;
                rens.Add(r);
            }

            return rens.ToArray();
        }

        public virtual void Show()
        {
            Hidden = false;

            if (DistanceCulling && Culled) return;

            if (transformToSwitchChildren != null)
                EnableDisableGoLods(CurrentLod);

            foreach (var switcher in meshSwitchers)
            {
                if (switcher == null) continue;
                switcher.Show();
            }

            onShow?.Invoke();
        }

        public virtual void Hide()
        {
            HideInternal();
            Hidden = true;
        }

//    public void CalculateRangeBasedOnSize(float _magnitudeCoeff, float _cullDistMul)
//    {
//        CalculateRangeBasedOnSize(_magnitudeCoeff);
//        cullDistanceMul = _cullDistMul;
//        useCullDistance = true;
//    }

        /// <summary>
        /// Calculates bounding box from all renderers using GetComponentInChildren,
        /// Range calculated as: size magnitude of bounds * 10 *  _magnitudeCoeff
        /// </summary>
        public void CalculateRangeBasedOnSize(float _magnitudeCoeff = 1)
        {
            var bounds = GetWorldBounds(gameObject);
            Range = bounds.size.magnitude * 10 * _magnitudeCoeff;
        }


        /// <summary>
        /// If if hidden, will show automatically
        /// Main thread only
        /// </summary>
        public void SetLod(int lod, bool editorSet = false)
        {
            if (lod == CurrentLod) return;

            var oldLod = lod;

            if (transformToSwitchChildren != null && !Hidden) EnableDisableGoLods(lod);

            foreach (var switcher in meshSwitchers)
            {
                if (switcher == null) continue;
                switcher.SetLod(lod, editorSet);
            }

            CurrentLod = lod;
            onLodChange?.Invoke(oldLod, lod);
        }

        #endregion

        #region Not public methods

        private void Loop(float _deltaTime)
        {
            if (!initialized) return;
            if (Hidden) return;

            var lodSettings = LodSettings.Instance;
            camDist = Vector3.Distance(transform.position, lodSettings.CameraTransform.position);

            if (DistanceCulling)
            {
                if (IsInCullingRange)
                {
                    if (!Hidden && Culled)
                    {
                        culled = false;
                        Show();
                        //Overhead.Instance.ScheduleMainThread(Show);
                    }
                }
                else
                {
                    if (!Culled)
                    {
                        HideInternal();
                        //Overhead.Instance.ScheduleMainThread(HideInternal);
                        culled = true;
                    }

                    return;
                }
            }
            else
            {
                if (!Hidden && Culled)
                {
                    culled = false;
                    Show();
                    //Overhead.Instance.ScheduleMainThread(Show);
                }
            }

            var lodToSet = Mathf.Clamp((int) (camDist * lodSettings.LodBias * lodCount), 0, lodCount - 1);

            //void SetLoda() => SetLod(lodToSet);
            //Overhead.Instance.ScheduleMainThread(SetLoda);
            SetLod(lodToSet);
        }

        // if out of visibility range
        protected virtual void HideInternal()
        {
            if (transformToSwitchChildren != null)
                EnableDisableGoLods(-1);

            foreach (var switcher in meshSwitchers)
            {
                if (switcher == null) continue;
                switcher.Hide();
            }

            onHide?.Invoke();
        }

        private Bounds GetWorldBounds(GameObject go)
        {
            if (go.transform == null) return new Bounds();
            var goBounds = new Bounds(go.transform.position, Vector3.zero);
            var renderers = go.GetComponentsInChildren<Renderer>(true);
            if (renderers != null && renderers.Length > 0) goBounds = renderers[0].bounds;
            foreach (var r in renderers)
            {
                var bounds = r.bounds;
                goBounds.Encapsulate(bounds);
            }

            return goBounds;
        }

        private void Awake()
        {
            // this is needed only to avoid stupid error log in editor when putting object with LodHandler to scene
//            if (Camera.main != null) camera = Camera.main.transform;

            //if (Settings.instance != null)
            //    Range = Range / Settings.instance.globalLODCoef;

            GetReferences();

//        cameraTransform = camera.transform.Handler();
//        cameraTransform.ReadPosition = true;
//        ttransform = transform.Handler(new LoadBalancerConfig("transform handlers for lodHandler", LoadBalancerType.EveryXFrames, 10));
//        ttransform.ReadPosition = true;
//        LoadBalancerSingletonThread.Instance.Register(this, Tick, new LoadBalancerConfig("lodHandlers updates", LoadBalancerType.EveryXFrames, 5),
//            ThreadsIds.logic);

            currentLod = -1;

            if (meshSwitchers.Any(l => l != null))
            {
                var a = meshSwitchers.OrderBy(_switcher => _switcher.lodMeshes.Length).LastOrDefault();
                if (a != null && a.lodMeshes != null) lodCount = a.lodMeshes.Length;
            }

            if (gosToSwitch != null) lodCount = gosToSwitch.childCount;

            initialized = true;

            if (automaticallySetRangeBasedOnSize) CalculateRangeBasedOnSize(autoSetRangeMul);
        }

        private void OnEnable() => LB.Register(this, Loop, new LoadBalancerConfig("lodHandlers", LoadBalancerType.XtimesPerSecond, updateRate));

        private void OnDisable()
        {
            if (updateWhenDisabled) return;
            LB.Unregister(this);
        }

        private void OnDestroy() => LB.Unregister(this);

        private void EditorSetLod(int _newLod)
        {
            GetReferences();
            SetLod(_newLod, true);
        }

        protected void GetReferences()
        {
            MeshSwitchers = GetComponentsInChildren<MeshSwitcher>().ToList();
            // to support nested LodHandlers in hierarchy we have to remove mesh switchers under other lod handler in our hirarchy
            foreach (var lh in GetComponentsInChildren<LodHandler>())
            {
                if (lh == this) continue;
                lh.GetReferences();
                for (var i = 0; i < meshSwitchers.Count; i++)
                {
                    var meshSwitcher = meshSwitchers[i];
                    if (lh.MeshSwitchers.Contains(meshSwitcher))
                    {
                        meshSwitchers.Remove(meshSwitcher);
                        i--;
                    }
                }
            }

            if (gosToSwitch)
            {
                transformToSwitchChildren = new Transform[gosToSwitch.transform.childCount];
                for (var i = 0; i < gosToSwitch.transform.childCount; i++)
                    transformToSwitchChildren[i] = gosToSwitch.GetChild(i);
            }
        }

        private void EnableDisableGoLods(int _newLod)
        {
            _newLod = Mathf.Clamp(_newLod, -1, transformToSwitchChildren.Length - 1);
            for (var i = 0; i < transformToSwitchChildren.Length; i++)
            {
                if (transformToSwitchChildren[i] == null) continue;
                transformToSwitchChildren[i].gameObject.SetActive(i == _newLod);
            }

            CurrentLod = _newLod;
        }

        private void Reset() => AddLodSwitchers(true);

        [InspectorButton]
        private void AddLodSwitchers() => AddLodSwitchers(true);

        private void AddLodSwitchers(bool log)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            foreach (var ren in renderers)
            {
                if (ren is SkinnedMeshRenderer == false && ren is MeshRenderer == false) continue;
                var switcher = ren.GetComponent<MeshSwitcher>();
                if (switcher == null)
                {
                    var renGameObject = ren.gameObject;
                    if (log) Debug.Log($"Added mesh switcher to {renGameObject.name}", renGameObject);
                    renGameObject.AddComponent<MeshSwitcher>();
                }
            }
        }

        [InspectorButton]
        protected virtual void RemoveLodSwitchers()
        {
            MeshSwitchers = GetComponentsInChildren<MeshSwitcher>().ToList();
            foreach (var lh in GetComponentsInChildren<LodHandler>())
            {
                lh.GetReferences();
                for (var i = 0; i < meshSwitchers.Count; i++)
                {
                    var meshSwitcher = meshSwitchers[i];
                    if (meshSwitcher == null) continue;
                    if (lh != this && lh.MeshSwitchers.Contains(meshSwitcher)) continue;

                    Debug.Log($"Removed mesh switcher on {meshSwitcher.gameObject.name}", meshSwitcher.gameObject);
                    if (Application.isPlaying) Destroy(meshSwitcher);
                    else DestroyImmediate(meshSwitcher);
                }
            }
        }

        #endregion
    }
}
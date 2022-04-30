// Created by Ronis Vision. All rights reserved
// 07.03.2021.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Rendering
{
    /// <summary>
    /// Switches meshes in renderer
    /// </summary>
    public class MeshSwitcher : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        internal Mesh[] lodMeshes = new Mesh[0];

//        [SerializeField]
        private int lodLevel = -1;

        private MeshFilter meshFilter;
        private SkinnedMeshRenderer skinnedMeshRen;
        private MeshRenderer meshRenderer;

        private bool hidden;

        #endregion

        #region Properties

        public MeshFilter MeshFilter => meshFilter;
        public SkinnedMeshRenderer SkinnedMeshRen => skinnedMeshRen;
        public int LodLevel => lodLevel;
        public MeshRenderer MeshRenderer => meshRenderer;

        public bool Hidden => hidden;

        #endregion

        #region Public methods

        public void SetLod(int _newLodLevel, bool _editorSet = false)
        {
            if (_editorSet) Awake();

            if (_newLodLevel == lodLevel || lodMeshes.Length == 0) return;

            _newLodLevel = Mathf.Clamp(_newLodLevel, 0, lodMeshes.Length - 1);

            if (skinnedMeshRen)
            {
                skinnedMeshRen.sharedMesh = lodMeshes[_newLodLevel];
                if (lodMeshes[_newLodLevel] == null)
                    skinnedMeshRen.enabled = false;
                else
                    skinnedMeshRen.enabled = !Hidden;
            }

            if (meshRenderer)
            {
                meshFilter.sharedMesh = lodMeshes[_newLodLevel];
                if (lodMeshes[_newLodLevel] == null)
                    meshRenderer.enabled = false;
                else
                    meshRenderer.enabled = !Hidden;
            }

            lodLevel = _newLodLevel;
        }

        public void Hide()
        {
            if (meshRenderer) meshRenderer.enabled = false;
            if (skinnedMeshRen) skinnedMeshRen.enabled = false;
            hidden = true;
        }

        public void Show()
        {
            if (meshRenderer) meshRenderer.enabled = true;
            if (skinnedMeshRen) skinnedMeshRen.enabled = true;
            hidden = false;
        }

        #endregion

        #region Not public methods

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            skinnedMeshRen = GetComponent<SkinnedMeshRenderer>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Reset()
        {
            meshFilter = GetComponent<MeshFilter>();
            skinnedMeshRen = GetComponent<SkinnedMeshRenderer>();
            meshRenderer = GetComponent<MeshRenderer>();

            if (meshRenderer == null && skinnedMeshRen == null) return;
            
            // try to add first mesh (lod 0) automatically
            if (lodMeshes.Length == 0 || lodMeshes[0] == null)
            {
                Mesh firstMesh = null;
                if (meshRenderer)
                {
                    firstMesh = meshFilter.sharedMesh;
                }else if (skinnedMeshRen)
                {
                    firstMesh = skinnedMeshRen.sharedMesh;
                }
                
                var tempList = lodMeshes.ToList();
                if (lodMeshes.Length == 0) tempList.Add(firstMesh);
                else
                {
                    tempList[0] = firstMesh;
                }

                lodMeshes = tempList.ToArray();
            }
        }

        #endregion
    }
}
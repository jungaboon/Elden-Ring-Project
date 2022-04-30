// Created by Ronis Vision. All rights reserved
// 06.08.2020.

#if UNITY_EDITOR 
using System.Collections.Generic;
using UnityEngine;

namespace RVModules.RVUtilities
{
    public class TerrainTools : MonoBehaviour
    {
        #region Fields

        public float lowestTreesDist = 3.0f;
        public float terrainUnevenness = 0.5f;
        private TerrainData tdata;
        private Terrain terrain;
        private float[,] beforeChangesHeightmap;

        #endregion

        #region Not public methods

        private bool GetSelectedTerrain()
        {
            if (FindObjectOfType<Terrain>() == null)
            {
                Debug.LogError("There is no terrain object on scene!");
                return false;
            }

            terrain = FindObjectOfType<Terrain>();
            tdata = terrain.terrainData;
            return true;
        }

        #endregion

        #region Removing overlapping trees

        public void RemoveOverlappingTrees()
        {
            if (!GetSelectedTerrain())
                return;

            var positions = new List<Vector3>();
            var treesNoOverlapping = new List<TreeInstance>();

            Debug.Log("Before tree instances: " + tdata.treeInstanceCount);

            foreach (var tree in tdata.treeInstances)
            {
                if (!CheckForOverlapping(positions, tree.position, tdata.size)) treesNoOverlapping.Add(tree);
                positions.Add(tree.position);
            }

            tdata.treeInstances = treesNoOverlapping.ToArray();
            terrain.Flush();
            Debug.Log("After removing overlapping tree instances: " + tdata.treeInstanceCount);
        }

        private bool CheckForOverlapping(List<Vector3> positions, Vector3 testingPos, Vector3 tdataSize)
        {
            foreach (var pos in positions)
                if (Vector3.Distance(pos, testingPos) < lowestTreesDist / tdataSize.x)
                    return true;

            return false;
        }

        #endregion

        #region TerrainUnevenness

        public void CreateTerrainUnevenness()
        {
            if (!GetSelectedTerrain())
                return;

            beforeChangesHeightmap = tdata.GetHeights(0, 0, tdata.heightmapResolution, tdata.heightmapResolution);
            var newHeightmap = new float[tdata.heightmapResolution, tdata.heightmapResolution];

            for (var x = 0; x < tdata.heightmapResolution; x++)
            for (var y = 0; y < tdata.heightmapResolution; y++)
            {
                var heightChangeValue = terrainUnevenness / tdata.size.y;
                newHeightmap[x, y] = beforeChangesHeightmap[x, y] +
                                     Random.Range(-heightChangeValue / 2, heightChangeValue / 2);
            }

            tdata.SetHeights(0, 0, newHeightmap);
            terrain.Flush();
        }

        public void RevertTerrainHeightmap()
        {
            if (beforeChangesHeightmap == null)
            {
                Debug.LogError("There was no changes made!");
                return;
            }

            tdata.SetHeights(0, 0, beforeChangesHeightmap);
            terrain.Flush();
        }

        #endregion
    }
}
#endif
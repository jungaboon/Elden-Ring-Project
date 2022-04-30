// Created by Ronis Vision. All rights reserved
// 20.06.2020.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RVHonorAI.Editor
{
    public static class HonorAiEditorHelpers
    {
        #region Public methods

        public static void WaypointsHandles(List<Waypoint> _waypoints, Vector3 _refPoint, Object _objectToUndo)
        {
            if (_waypoints == null) return;

            // are we pressing l shift?
            var shiftPressed = Event.current.shift;

            for (var i = 0; i < _waypoints.Count; i++)
            {
                var position = _waypoints[i].position + _refPoint;
                var beforeChangePos = _waypoints[i].position;

                //Handles.color = new Color(1, 1, 1, .2f);
                //Handles.DrawSolidDisc(position, Vector3.up, .1f);
                Handles.color = Color.white;
                Handles.DrawWireDisc(position, Vector3.up, _waypoints[i].radius);

                EditorGUI.BeginChangeCheck();
                var pos = Handles.PositionHandle(position, Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_objectToUndo, "Waypoint move");
                    _waypoints[i].position = pos - _refPoint;

                    if (shiftPressed)
                    {
                        var posDelta = _waypoints[i].position - beforeChangePos;

                        for (var index = 0; index < _waypoints.Count; index++)
                        {
                            if (index == i) continue;
                            var waypoint = _waypoints[index];
                            waypoint.position += posDelta;
                        }
                    }
                    PrefabUtility.RecordPrefabInstancePropertyModifications(_objectToUndo);
                }

                Handles.Label(position, $"Waypoint {i + 1}");
                if (i < _waypoints.Count - 1)
                    Handles.DrawLine(position, _waypoints[i + 1].position + _refPoint);
            }
        }

        #endregion
    }
}
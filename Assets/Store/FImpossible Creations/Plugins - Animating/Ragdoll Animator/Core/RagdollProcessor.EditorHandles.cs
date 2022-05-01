#if UNITY_EDITOR
using FIMSpace.FEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    public partial class RagdollProcessor
    {
        public static Transform _editor_symmetryRef = null;
        public static void DrawColliderHandles(CapsuleCollider coll, bool positionOrScale = false, bool drawName = false, CapsuleCollider symmetryColl = null)
        {
            if (coll == null) return;

            float scaleDiv = coll.transform.lossyScale.x;
            if (scaleDiv < 0.1f) scaleDiv = 0.1f;
            if (scaleDiv > 2f) scaleDiv = 2f;
                 
            if (drawName)
            {
                Handles.Label(coll.transform.TransformPoint(coll.center), coll.transform.name);
                FGUI_Handles.DrawBoneHandle(coll.transform.TransformPoint(coll.center), coll.transform.position, 0.5f);
            }

            if (positionOrScale == false)
            {
                Vector3 worldPos = coll.transform.TransformPoint(coll.center);
                Vector3 newWorldPos = FEditor_TransformHandles.PositionHandle(worldPos, coll.transform.rotation, 0.75f, true);
                Vector3 newInLoc = coll.transform.InverseTransformPoint(newWorldPos);

                if (Vector3.Distance(worldPos, newWorldPos) > 0.01f / scaleDiv * coll.bounds.size.magnitude)
                {
                    if (_editor_symmetryRef != null)
                    {
                        if (symmetryColl != null)
                        {
                            Undo.RecordObject(symmetryColl, "RagdCollChangeSym");
                            Vector3 diff = newInLoc - coll.center;
                            diff.z = -diff.z;
                            symmetryColl.center += diff;
                        }
                    }

                    Undo.RecordObject(coll, "RagdCollChange");
                    coll.center = newInLoc;
                }
            }
            else
            {
                Vector3 capsScale;
                if (coll.direction == 0) capsScale = new Vector3(coll.height, coll.radius, coll.radius);
                else if (coll.direction == 1) capsScale = new Vector3(coll.radius, coll.height, coll.radius);
                else capsScale = new Vector3(coll.radius, coll.radius, coll.height);

                Vector3 scaleDiff = FEditor_TransformHandles.ScaleHandle(capsScale, coll.transform.TransformPoint(coll.center), coll.transform.rotation, 0.75f, false, true);

                if (Vector3.Distance(capsScale, scaleDiff) > 0.01f / scaleDiv * coll.bounds.size.magnitude)
                {
                    if (_editor_symmetryRef != null)
                    {
                        if (symmetryColl != null)
                        {
                            Undo.RecordObject(symmetryColl, "RagdCollChangeSym");
                            if (symmetryColl.direction == 0) { symmetryColl.height = scaleDiff.x; symmetryColl.radius = scaleDiff.y != capsScale.y ? scaleDiff.y : scaleDiff.z; }
                            else if (symmetryColl.direction == 1) { symmetryColl.height = scaleDiff.y; symmetryColl.radius = scaleDiff.x != capsScale.x ? scaleDiff.x : scaleDiff.z; }
                            else { symmetryColl.height = scaleDiff.z; coll.radius = scaleDiff.x != capsScale.x ? scaleDiff.x : scaleDiff.y; }
                        }
                    }

                    Undo.RecordObject(coll, "RagdCollChange");
                    if (coll.direction == 0) { coll.height = scaleDiff.x; coll.radius = scaleDiff.y != capsScale.y ? scaleDiff.y : scaleDiff.z; }
                    else if (coll.direction == 1) { coll.height = scaleDiff.y; coll.radius = scaleDiff.x != capsScale.x ? scaleDiff.x : scaleDiff.z; }
                    else { coll.height = scaleDiff.z; coll.radius = scaleDiff.x != capsScale.x ? scaleDiff.x : scaleDiff.y; }
                }
            }

        }


        public static void DrawColliderHandles(BoxCollider coll, bool positionOrScale = false, bool drawName = false, BoxCollider symmetryColl = null)
        {
            if (coll == null) return;

            if (drawName)
            {
                Handles.Label(coll.transform.TransformPoint(coll.center), coll.transform.name);
                FGUI_Handles.DrawBoneHandle(coll.transform.TransformPoint(coll.center), coll.transform.position, 0.5f);
            }

            float scaleDiv = coll.transform.lossyScale.x;
            if (scaleDiv < 0.1f) scaleDiv = 0.1f;
            if (scaleDiv > 2f) scaleDiv = 2f;

            if (positionOrScale == false)
            {
                Vector3 worldPos = coll.transform.TransformPoint(coll.center);
                Vector3 newWorldPos = FEditor_TransformHandles.PositionHandle(worldPos, coll.transform.rotation, 0.75f, true);
                Vector3 newInLoc = coll.transform.InverseTransformPoint(newWorldPos);

                if (Vector3.Distance(worldPos, newWorldPos) > 0.01f * scaleDiv * coll.bounds.size.magnitude)
                {
                    Undo.RecordObject(coll, "RagdCollChange");

                    if (_editor_symmetryRef != null)
                    {
                        if (symmetryColl != null)
                        {
                            Undo.RecordObject(symmetryColl, "RagdCollChangeSym");
                            Vector3 diff = newInLoc - coll.center;
                            diff.z = -diff.z;
                            symmetryColl.center += diff;
                        }
                    }

                    coll.center = newInLoc;
                }
            }
            else
            {
                Vector3 scaleDiff = FEditor_TransformHandles.ScaleHandle(coll.size, coll.transform.TransformPoint(coll.center), coll.transform.rotation, 0.75f, false, true);

                if (Vector3.Distance(coll.size, scaleDiff) > 0.01f * scaleDiv * coll.bounds.size.magnitude)
                {
                    if (_editor_symmetryRef != null)
                    {
                        if (symmetryColl != null)
                        {
                            Undo.RecordObject(symmetryColl, "RagdCollChangeSym");
                            Vector3 locDiff = scaleDiff - coll.size;
                            locDiff.z = -locDiff.z;
                            symmetryColl.size += _editor_symmetryRef.TransformVector(locDiff);
                        }
                    }

                    Undo.RecordObject(coll, "RagdCollChange");
                    coll.size = scaleDiff;
                }
            }

        }

    }
}

#endif
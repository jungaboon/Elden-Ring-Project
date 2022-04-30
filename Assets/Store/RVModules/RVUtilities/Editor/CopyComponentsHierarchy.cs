// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using RVModules.RVUtilities.Extensions;
using RVModules.RVUtilities.Prototyping;
using UnityEditor;
using UnityEngine;

namespace RVModules.RVUtilities.Editor
{
    public class CopyComponentsHierarchy : MonoBehaviour
    {
        #region Fields

        public GameObject source;
        public GameObject target;
        public Component[] componentFilters;
        public Component[] excludeComponentFilters;

        public bool copySerializedManagedFieldsOnly;
        public bool copyComponents;
        public bool copyTransformLocals;

        [Tooltip("If true, will try to get component to apply it's values from source.")]
        public bool tryGetComponentOnTargetFirst;

        public bool filterComponentTypes;
        public bool filterExcludeComponentTypes;

        #endregion

        #region Public methods

        public void Work()
        {
            var sourceComponents = source.transform.GetTransformsRecursive();
            var targetComponents = target.transform.GetTransformsRecursive();


            if (copyComponents)
                for (var index = 0; index < targetComponents.Count; index++)
                {
                    var sourceTransform = sourceComponents[index];
                    var targetTransform = targetComponents[index];

                    foreach (var component in sourceTransform.GetComponents<Component>()) CopyComponent(component, targetTransform);
                }

            if (copyTransformLocals) TransformValuesCopy.Work(source.transform, target.transform);
        }

        #endregion

        #region Not public methods

        private void CopyComponent(Component component, Transform targetTransform)
        {
            if (component is Transform) return;

            var includeThisComponent = true;

            if (filterComponentTypes)
            {
                includeThisComponent = false;

                foreach (var filterComponentType in componentFilters)
                    if (filterComponentType.GetType() == component.GetType())
                    {
                        includeThisComponent = true;
                        break;
                    }
            }

            if (filterExcludeComponentTypes)
                foreach (var excludeComponentFilter in excludeComponentFilters)
                    if (excludeComponentFilter.GetType() == component.GetType())
                    {
                        includeThisComponent = false;
                        break;
                    }

            if (includeThisComponent)
            {
                Component newTargetComp;
                if (tryGetComponentOnTargetFirst)
                {
                    newTargetComp = targetTransform.GetComponent(component.GetType());
                    if (newTargetComp != null)
                    {
                        EditorUtility.CopySerialized(component, newTargetComp);
                        return;
                    }
                }

                newTargetComp = targetTransform.gameObject.AddComponent(component.GetType());
                if (copySerializedManagedFieldsOnly)
                    EditorUtility.CopySerializedManagedFieldsOnly(component, newTargetComp);
                else
                    EditorUtility.CopySerialized(component, newTargetComp);
            }
        }

        #endregion
    }
}
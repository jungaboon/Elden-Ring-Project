// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Linq;
using RVModules.RVSmartAI.GraphElements;

namespace RVModules.RVSmartAI.Editor.SelectWindows
{
    public class SelectTaskWindow : SelectWindowBase
    {
        #region Properties

        public override string Title => "Select AiTask";

        protected override Type GetTypes() => typeof(AiTask);

        #endregion

        #region Public methods

        //protected override Type GetWindowType() => GetType();

        //public Action<GameObject> onSelectedGameObject;
        //public GameObject passedGameObject;

        public void HideTaskParams() => types = types.Where(t => !t.BaseType.IsGenericType).ToArray();

        #endregion

        #region Not public methods

        protected override string NameToDisplay(Type type)
        {
            var nameToDisplay = type.Name;
            if (type.BaseType.IsGenericType)
                nameToDisplay = $" ({type.BaseType.GetGenericArguments()[0].Name}) {nameToDisplay}";
            return nameToDisplay;
        }

        #endregion

//        protected override void OnGUI()
//        {
//            Repaint();
//            //passedGameObject = null;
//            foreach (var type in types)
//            {
//                var nameToDisplay = type.Name;
//                if (type.BaseType.IsGenericType)
//                    nameToDisplay += $" ({type.BaseType.GetGenericArguments()[0].Name})";
//
//                if (!GUILayout.Button(nameToDisplay, GUIHelpers.GuiStyle(4))) continue;
//                onSelectedItem?.Invoke(type);
//                Close();
//            }
//
////            EditorGUILayout.Separator();
////            passedGameObject = EditorGUILayout.ObjectField(null, typeof(GameObject), true) as GameObject;
////            if (passedGameObject == null) return;
////            onSelectedGameObject?.Invoke(passedGameObject);
////            Close();
//        }
    }
}
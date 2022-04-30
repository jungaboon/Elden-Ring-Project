// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;
using UnityEngine.UI;

namespace RVModules.RVUtilities
{
    [ExecuteInEditMode] public class RecursiveChangeFont : MonoBehaviour
    {
        #region Fields

        public Text label;
        public bool work;
        public bool changeFont;
        public bool changeFontSize;
        public bool changeFontStyle;
        public bool changeFontColor;
        public bool changeOverflowType;
        public Transform[] excludedChildren;

        #endregion

        #region Not public methods

        private void Update()
        {
            if (work)
            {
                Work(transform);
                work = false;
            }
        }

        private void Work(Transform _transform)
        {
            foreach (Transform item in _transform)
            {
                if (IsExcluded(_transform)) continue;

                var labelToChange = item.GetComponent<Text>();
                if (labelToChange != null) ChangeLabel(labelToChange);
                Work(item);
            }
        }

        private void ChangeLabel(Text _label)
        {
            if (changeFont)
                _label.font = label.font;
            if (changeFontStyle)
                _label.fontStyle = label.fontStyle;
            if (changeFontSize)
                _label.fontSize = label.fontSize;
            if (changeFontColor)
                _label.color = label.color;
            if (changeOverflowType)
            {
                _label.horizontalOverflow = label.horizontalOverflow;
                _label.verticalOverflow = label.verticalOverflow;
            }
        }

        private bool IsExcluded(Transform item)
        {
            var excluded = false;
            foreach (var trans in excludedChildren)
                if (item == trans)
                {
                    excluded = true;
                    break;
                }

            return excluded;
        }

        #endregion
    }
}
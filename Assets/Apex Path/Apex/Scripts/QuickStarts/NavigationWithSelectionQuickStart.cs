/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.QuickStarts
{
    using Apex.Input;
    using Apex.Units;
    using UnityEngine;

    /// <summary>
    /// Extended version of <see cref="NavigationQuickStart"/> that makes the unit selectable.
    /// </summary>
    [AddComponentMenu("Apex/QuickStarts/Navigating Unit with Selection")]
    public class NavigationWithSelectionQuickStart : NavigationQuickStart
    {
        private const string SelectionVisualName = "SelectionVisual";

        /// <summary>
        /// Extends this quick start with additional components.
        /// </summary>
        /// <param name="gameWorld">The game world.</param>
        protected override void Extend(GameObject gameWorld)
        {
            var go = this.gameObject;

            SelectableUnitComponent selectableBehavior;
            AddIfMissing<SelectableUnitComponent>(go, false, out selectableBehavior);

            //Add the selection visual
            GameObject selectVisual;
            var selectVisualTransform = go.transform.Find(SelectionVisualName);
            if (selectVisualTransform == null)
            {
                var mold = Resources.Load<GameObject>("Prefabs/UnitSelectedCustom");
                if (mold == null)
                {
                    mold = Resources.Load<GameObject>("Prefabs/UnitSelected");
                }

                selectVisual = GameObject.Instantiate(mold) as GameObject;
                selectVisual.name = SelectionVisualName;

                selectVisualTransform = selectVisual.transform;
                selectVisualTransform.parent = go.transform;
                selectVisualTransform.localPosition = Vector3.zero;
                selectVisualTransform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
            }
            else
            {
                selectVisual = selectVisualTransform.gameObject;
            }

            selectVisual.SetActive(false);
            selectableBehavior.selectionVisual = selectVisual;

            AddIfMissing<InputReceiverBasic, InputReceiverAttribute>(gameWorld, true);
            if (gameWorld.GetComponentInChildren<SelectionRectangleComponent>() == null)
            {
                var mold = Resources.Load<GameObject>("Prefabs/SelectionRectCustom");
                if (mold == null)
                {
                    mold = Resources.Load<GameObject>("Prefabs/SelectionRect");
                }

                var selectionRect = GameObject.Instantiate(mold) as GameObject;
                selectionRect.transform.parent = gameWorld.transform;
            }
        }
    }
}

// --------------------------------------------------------------
// VLECanvas.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 6-16-2016
// --------------------------------------------------------------

using UnityEngine.EventSystems;

namespace VLabEnvironment
{
    public class VLECanvas : UIBehaviour
    {
        public VLEUIController uicontroller;

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            uicontroller.OnRectTransformDimensionsChange();
        }
    }
}
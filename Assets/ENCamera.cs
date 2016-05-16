﻿// --------------------------------------------------------------
// ENCamera.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 5-16-2016
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace VLab
{
    [NetworkSettings(channel = 0, sendInterval = 0)]
    public class ENCamera : NetworkBehaviour
    {
        [SyncVar(hook = "onbgcolor")]
        public Color bgcolor = new Color();
        [SyncVar(hook = "onscreenhalfheight")]
        public float screenhalfheight = 15;
        [SyncVar(hook = "onscreentoeye")]
        public float screentoeye = 57;

        public new Camera camera;

        void Awake()
        {
            OnAwake();
        }
        public virtual void OnAwake()
        {
            camera = gameObject.GetComponent<Camera>();
        }

        private void onbgcolor(Color c)
        {
            OnBgColor(c);
        }
        public virtual void OnBgColor(Color c)
        {
            if (camera != null)
            {
                camera.backgroundColor = c;
            }
            bgcolor = c;
        }

        private void onscreenhalfheight(float shh)
        {
            OnScreenHalfHeight(shh);
        }
        public virtual void OnScreenHalfHeight(float shh)
        {
            if (camera != null)
            {
                camera.orthographicSize = Mathf.Rad2Deg * Mathf.Atan2(shh, screentoeye);
            }
            screenhalfheight = shh;
        }

        private void onscreentoeye(float ste)
        {
            OnScreenToEye(ste);
        }
        public virtual void OnScreenToEye(float ste)
        {
            if (camera != null)
            {
                camera.orthographicSize = Mathf.Rad2Deg * Mathf.Atan2(screenhalfheight, ste);
            }
            screentoeye = ste;
        }

    }
}
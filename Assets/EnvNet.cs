﻿// --------------------------------------------------------------
// EnvNet.cs is part of the VLAB project.
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
    public class EnvNet : NetworkBehaviour
    {
        [SyncVar(hook = "onvisible")]
        public bool visible = true;
        [SyncVar(hook = "onposition")]
        public Vector3 position = new Vector3();

        public new Renderer renderer;

        void Awake()
        {
            OnAwake();
        }
        public virtual void OnAwake()
        {
            renderer = gameObject.GetComponent<Renderer>();
        }

        void onvisible(bool v)
        {
            OnVisible(v);
        }
        public virtual void OnVisible(bool v)
        {
            if (renderer != null)
            {
                renderer.enabled = v;
            }
            visible = v;
        }

        void onposition(Vector3 p)
        {
            OnPosition(p);
        }
        public virtual void OnPosition(Vector3 p)
        {
            transform.position = p;
            position = p;
        }

    }
}
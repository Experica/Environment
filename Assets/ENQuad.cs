// --------------------------------------------------------------
// ENQuad.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 5-21-2016
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace VLab
{
    public class ENQuad : EnvNet
    {
        [SyncVar(hook = "onori")]
        public float ori = 0;
        [SyncVar(hook = "onlength")]
        public float length = 1;
        [SyncVar(hook = "onwidth")]
        public float width = 1;
        [SyncVar(hook = "onheight")]
        public float height = 1;
        [SyncVar(hook = "oncolor")]
        public Color color = new Color();
        [SyncVar(hook = "onmasktype")]
        public int masktype = 0;

        public VLTimer t = new VLTimer();

        protected override void OnAwake()
        {
            base.OnAwake();
            t.Start();
        }

        void onori(float o)
        {
            OnOri(o);
        }
        protected virtual void OnOri(float o)
        {
            transform.eulerAngles = new Vector3(0, 0, o);
            ori = o;
        }

        void onlength(float l)
        {
            OnLength(l);
        }
        protected virtual void OnLength(float l)
        {
            transform.localScale = new Vector3(l, width, height);
            renderer.material.SetFloat("length", l);
            length = l;
        }

        void onwidth(float w)
        {
            OnWidth(w);
        }
        protected virtual void OnWidth(float w)
        {
            transform.localScale = new Vector3(length, w, height);
            renderer.material.SetFloat("width", w);
            width = w;
        }

        void onheight(float h)
        {
            OnHeight(h);
        }
        protected virtual void OnHeight(float h)
        {
            transform.localScale = new Vector3(length, width, h);
            height = h;
        }

        void oncolor(Color c)
        {
            OnColor(c);
        }
        protected virtual void OnColor(Color c)
        {
            renderer.material.color = c;
            color = c;
        }

        void onmasktype(int t)
        {
            OnMaskType(t);
        }
        protected virtual void OnMaskType(int t)
        {
            renderer.material.SetInt("masktype", t);
            masktype = t;
        }

    }
}
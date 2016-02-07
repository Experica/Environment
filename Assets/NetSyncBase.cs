﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[NetworkSettings(channel =0,sendInterval =0)]
public class NetSyncBase : NetworkBehaviour {
    [SyncVar(hook ="OnOri")]
    public float ori=0;
    [SyncVar(hook ="OnWidth")]
    public float width=1;
    [SyncVar(hook ="OnLength")]
    public float length=1;
    [SyncVar(hook ="OnHeight")]
    public float height = 1;
    [SyncVar(hook ="OnPosition")]
    public Vector3 position=new Vector3();

    public virtual void OnOri(float o)
    {
        ori = o;
        transform.Rotate(0, 0, ori);
    }

    public virtual void OnWidth(float w)
    {
        width = w;
        transform.localScale = new Vector3(length, width, height);
    }

    public virtual void OnLength(float l)
    {
        length = l;
        transform.localScale = new Vector3(length, width, height);
    }

    public virtual void OnHeight(float h)
    {
        height = h;
        transform.localScale = new Vector3(length, width, height);
    }

    public virtual void OnPosition(Vector3 p)
    {
        position = p;
        transform.position = position;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

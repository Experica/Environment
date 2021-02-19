/*
SyncFrameManager.cs is part of the Experica.
Copyright (c) 2016 Li Alex Zhang and Contributors

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using Fasterflect;
using System.Reflection;

namespace Experica.Environment
{
    public class SyncFrameManager : MonoBehaviour
    {
        public bool beginsyncframe;
        public bool endingsyncframe;
        public NetManager netmanager;
        public UIController uicontroler;
        public double SyncFrameOnTime;
        MethodInvoker UNetStaticUpdate;

        void Start()
        {
            UNetStaticUpdate = typeof(NetworkIdentity).DelegateForCallMethod("UNetStaticUpdate", BindingFlags.Static | BindingFlags.NonPublic);
        }

        // set this script execution order later than UNET
        void LateUpdate()
        {
            // update UNET until the whole SyncFrame Msg structure has been recived
            while (beginsyncframe)
            {
                if (endingsyncframe)
                {
                    endingsyncframe = false;
                    netmanager.OnFinishSyncFrame();
                    beginsyncframe = false;
                    break;
                }
                if (Time.realtimeSinceStartupAsDouble - SyncFrameOnTime >= uicontroler.config.SyncFrameTimeOut)
                {
                    Debug.Log($"SyncFrame Timeout({uicontroler.config.SyncFrameTimeOut}s), Stop Waiting.");
                    beginsyncframe = false;
                    break;
                }
                UNetStaticUpdate.Invoke(null, null);
            }
        }
    }
}
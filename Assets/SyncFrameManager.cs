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
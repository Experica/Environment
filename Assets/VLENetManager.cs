// --------------------------------------------------------------
// VLENetManager.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 6-16-2016
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using VLab;

namespace VLabEnvironment
{
    public class VLENetManager : NetworkManager
    {
        public VLEUIController uicontroller;

        public override void OnClientConnect(NetworkConnection conn)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("Send PeerType Message.");
            }
            client.Send(VLMsgType.PeerType, new IntegerMessage((int)VLPeerType.VLabEnvironment));
            if (LogFilter.logDebug)
            {
                Debug.Log("Send AspectRatio Message.");
            }
            client.Send(VLMsgType.AspectRatio, new FloatMessage(uicontroller.GetAspectRatio()));

            uicontroller.OnClientConnect();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            uicontroller.OnClientDisconnect();
        }

        /// <summary>
        /// when new scene has loaded, and old scene unloaded, we should unload any
        /// assets that is loaded by old scene but not used by new scene, keep a 
        /// smaller memeory footprint.
        /// </summary>
        /// <param name="conn"></param>
        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            Resources.UnloadUnusedAssets();
            base.OnClientSceneChanged(conn);
        }

    }
}
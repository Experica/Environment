// --------------------------------------------------------------
// VLENetManager.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 5-16-2016
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Diagnostics;
using System.Threading;
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
                UnityEngine.Debug.Log("Send PeerInfo Message.");
            }
            client.Send(VLMsgType.PeerInfo, new IntegerMessage((int)VLPeerType.VLabEnvironment));

            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            QualitySettings.antiAliasing = 4;
            QualitySettings.vSyncCount = 0;
            QualitySettings.maxQueuedFrames = 0;

            Time.fixedDeltaTime = 0.0001f;
            Time.maximumDeltaTime = 0.33f;
            Process.GetCurrentProcess().PriorityBoostEnabled = true;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            uicontroller.OnClientDisconnect();

            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.antiAliasing = 0;
            QualitySettings.vSyncCount = 1;
            QualitySettings.maxQueuedFrames = 2;

            Time.fixedDeltaTime = 0.02f;
            Time.maximumDeltaTime = 0.33f;
            Process.GetCurrentProcess().PriorityBoostEnabled = false;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            Resources.UnloadUnusedAssets();
            base.OnClientSceneChanged(conn);
        }

    }
}
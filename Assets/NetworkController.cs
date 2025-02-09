﻿/*
NetworkController.cs is part of the Experica.
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
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using Experica.NetEnv;
using System;

namespace Experica.Environment
{
    public class NetworkController : MonoBehaviour
    {
        public AppManager appmgr;

        /// <summary>
        /// NetworkManager and NetworkController have same lifetime(as the same GameObject components),
        /// so we register events only once here when NetworkManager has initialized.
        /// </summary>
        void Start()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            }
        }

        public bool StartClient()
        {
            var isstart = false;
            var nm = NetworkManager.Singleton;
            if (nm != null && !nm.IsListening)
            {
                isstart = nm.StartClient();
            }
            return isstart;
        }

        void OnClientDisconnectCallback(ulong clientid)
        {
            // when caused by self disconnect throught UI, NetworkManager has shutdown in UI OnValueChanged, then disconnect-event lead to this callback, and now set UI will not raise OnValueChanged again,
            // when triggered by ConnectingFailed/Server/Network disconnect-event, set UI will raise OnValueChanged in which NetworkManager will shutdown, but no extra disconnect-event to trigger this callback again.
            appmgr.ui.client.value = false;
            appmgr.UnBoost();
        }

        void OnClientConnectedCallback(ulong clientid)
        {
            appmgr.ui.client.label += $" {clientid}";
            appmgr.Boost();
        }

        public void Shutdown(bool cleanscene = true)
        {
            var nm = NetworkManager.Singleton;
            if (nm != null && nm.IsListening)
            {
                nm.Shutdown();
                if (cleanscene)
                {
                    // Network SceneManager will not exist when NetworkManager shutdown, so here we use UnityEngine's SceneManager to clean any loaded scene by loading an Empty scene
                    SceneManager.LoadScene(Base.EmptyScene, LoadSceneMode.Single);
                }
            }
        }


    }
}
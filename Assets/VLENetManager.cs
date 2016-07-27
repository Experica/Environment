﻿// -----------------------------------------------------------------------------
// VLENetManager.cs is part of the VLAB project.
// Copyright (c) 2016  Li Alex Zhang  fff008@gmail.com
//
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// -----------------------------------------------------------------------------

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
        /// assets that was loaded by old scene but not used by the new scene, keeping a 
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
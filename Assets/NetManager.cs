/*
NetManager.cs is part of the Experica.
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

namespace Experica.Environment
{
    public class NetManager : NetworkManager
    {
        public UIController uicontroller;

        public override void OnStartClient(NetworkClient client)
        {
            base.OnStartClient(client);
            client.RegisterHandler(MsgType.BeginSyncFrame, new NetworkMessageDelegate(BeginSyncFrameHandler));
            client.RegisterHandler(MsgType.EndSyncFrame, new NetworkMessageDelegate(EndSyncFrameHandler));
            client.RegisterHandler(MsgType.CLUT, new NetworkMessageDelegate(CLUTHandler));
        }

        void CLUTHandler(NetworkMessage netMsg)
        {
            uicontroller.SetCLUT(netMsg.ReadMessage<CLUTMessage>());
        }

        void BeginSyncFrameHandler(NetworkMessage netMsg)
        {
            uicontroller.syncmanager.beginsyncframe = true;
            uicontroller.syncmanager.SyncFrameOnTime = Time.realtimeSinceStartupAsDouble;
        }

        void EndSyncFrameHandler(NetworkMessage netMsg)
        {
            uicontroller.syncmanager.endingsyncframe = true;
        }

        public void OnFinishSyncFrame()
        {
            client.Send(MsgType.EndSyncFrame, new EmptyMessage());
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            if (LogFilter.logDebug)
            {
                Debug.Log("Send PeerType Message.");
            }
            client.Send(MsgType.PeerType, new IntegerMessage((int)PeerType.Environment));
            if (LogFilter.logDebug)
            {
                Debug.Log("Send AspectRatio Message.");
            }
            client.Send(MsgType.AspectRatio, new FloatMessage() { value = uicontroller.GetAspectRatio() });
            //client.Send(MsgType.AspectRatio, new StringMessage(uicontroller.GetAspectRatio().ToString()));

            uicontroller.OnClientConnect();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            uicontroller.OnClientDisconnect();
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            GC.Collect();
        }

        public override void OnStopClient()
        {
            NetworkClient.ShutdownAll();
            client = null;
        }

    }
}
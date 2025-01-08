/*
AppManager.cs is part of the Experica.
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
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Unity.Netcode;
using UnityEngine.UI;
using System.Diagnostics;
using System.Threading;
using System.Runtime;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.IO;
using UnityEngine.InputSystem;
using System.Collections;
using Experica.NetEnv;


namespace Experica.Environment
{
    public class AppManager : MonoBehaviour
    {
        public ConfigManager<EnvironmentConfig> cfgmgr = ConfigManager<EnvironmentConfig>.Load(Base.EnvironmentConfigManagerPath);
        public UI ui;

        public NetworkController networkcontroller;
        public SyncFrameManager syncmanager;
        public Volume postprocessing;


        void Awake()
        {
            Application.wantsToQuit += Application_wantsToQuit;
        }

        void Start()
        {
            TryAutoConnect();
        }

        Coroutine autoconnect;
        public void TryAutoConnect()
        {
            if (cfgmgr.config.AutoConnect) { autoconnect = StartCoroutine(AutoConnect(cfgmgr.config.AutoConnectTimeOut)); }
        }

        IEnumerator AutoConnect(float waittime_s)
        {
            var waituntiltime = Time.realtimeSinceStartup + waittime_s;
            while (Time.realtimeSinceStartup < waituntiltime)
            {
                ui.client.label = $"Connect in {Mathf.CeilToInt(waituntiltime - Time.realtimeSinceStartup)}s";
                yield return null;
            }
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsListening)
            {
                ui.client.value = true;
            }
        }

        bool Application_wantsToQuit()
        {
            cfgmgr.Save(Base.EnvironmentConfigManagerPath);
            return true;
        }

        public void OnScreenSizeChanged()
        {
            var nm = NetworkManager.Singleton;
            if (nm != null && nm.IsListening)
            {
                var mcgo = GameObject.FindWithTag("MainCamera");
                if (mcgo != null)
                {
                    var mc = mcgo.GetComponent<INetEnvCamera>();
                    mc.ReportRpc("ScreenAspect", Base.ScreenAspect);
                }
            }
        }

        //public void SetCLUT(CLUTMessage msg)
        //{
        //    if (postprocessing.profile.TryGet(out Tonemapping tonemapping))
        //    {
        //        var tex = new Texture3D(msg.size, msg.size, msg.size, TextureFormat.RGB24, false);
        //        tex.SetPixelData(msg.clut.Decompress(), 0);
        //        tex.Apply();
        //        tonemapping.lutTexture.value = tex;
        //    }
        //}


        #region Environment Action Callback
        public void OnToggleClientAction(InputAction.CallbackContext context)
        {
            if (context.performed) { ui.client.value = !ui.client.value; }
        }

        public void OnToggleFullViewportAction(InputAction.CallbackContext context)
        {
            if (context.performed) { FullViewport = !FullViewport; }
        }

        public bool FullViewport
        {
            get => !ui.uidoc.rootVisualElement.visible;
            set => ui.uidoc.rootVisualElement.visible = !value;
        }

        public void OnToggleFullScreenAction(InputAction.CallbackContext context)
        {
            if (context.performed) { FullScreen = !FullScreen; }
        }

        int lastwindowwidth = 800, lastwindowheight = 600;
        public bool FullScreen
        {
            get { return Screen.fullScreen; }
            set
            {
                if (Screen.fullScreen == value) { return; }
                if (value)
                {
                    lastwindowwidth = Math.Max(400, Screen.width);
                    lastwindowheight = Math.Max(300, Screen.height);
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, cfgmgr.config.FullScreenMode);
                }
                else
                {
                    Screen.SetResolution(lastwindowwidth, lastwindowheight, false);
                }
            }
        }

        public void OnToggleCursor(InputAction.CallbackContext context)
        {
            if (context.performed) { Cursor.visible = !Cursor.visible; }
        }

        public void OnQuitAction(InputAction.CallbackContext context)
        {
            if (context.performed) { Application.Quit(); }
        }
        #endregion


        bool isboosted;
        public void Boost()
        {
            if (isboosted) { return; }
            // We may want to hide UI and Cursor when connected to Server.
            FullViewport = cfgmgr.config.HideUIWhenConnected;
            Cursor.visible = !cfgmgr.config.HideCursorWhenConnected;
            // We need to present fastest and best quality virtual reality environment when connected to Server.
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            QualitySettings.vSyncCount = cfgmgr.config.VSyncCount;
            QualitySettings.maxQueuedFrames = cfgmgr.config.MaxQueuedFrames;
            Time.fixedDeltaTime = cfgmgr.config.FixedDeltaTime;

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            GC.Collect();
            isboosted = true;
        }

        public void UnBoost()
        {
            if (!isboosted) { return; }
            // We should turn on UI and Cursor when disconnected to Server.
            FullViewport = false;
            Cursor.visible = true;
            // Return to normal quality state when disconnected to Server.
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.vSyncCount = 1;
            QualitySettings.maxQueuedFrames = 2;
            Time.fixedDeltaTime = 0.016666f;

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
            Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Normal;
            GCSettings.LatencyMode = GCLatencyMode.Interactive;
            GC.Collect();
            isboosted = false;
        }

        public void OnClientChanged(bool newValue)
        {
            if (newValue)
            {
                // stop auto connect before connect, coroutine may still in waiting, or this call is triggered at the end of coroutine(just stop the already stopped)
                StopCoroutine(autoconnect);
                networkcontroller.StartClient();
            }
            else
            {
                networkcontroller.Shutdown();
                // try auto connect when caused directly by UI disconnect from connecting/connected state, or indirectly by server/network disconnect
                TryAutoConnect();
            }
            ui.client.label = newValue ? "Shutdown" : "Connect";
        }
    }
}
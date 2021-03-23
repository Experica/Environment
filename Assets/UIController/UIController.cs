/*
UIController.cs is part of the Experica.
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
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using System.Diagnostics;
using System.Threading;
using System.Runtime;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.IO;
using UnityEngine.InputSystem;

namespace Experica.Environment
{
    public class UIController : MonoBehaviour
    {
        public InputField serveraddress;
        public Toggle clientconnect, autoconn;
        public Text autoconntext, version;
        public NetManager netmanager;
        public SyncFrameManager syncmanager;
        public GameObject canvas;
        public EnvironmentConfig config;
        readonly string configpath = "EnvironmentConfig.yaml";

        bool isautoconn, isconnect;
        int autoconncountdown;
        float lastautoconntime;
        int lastwindowwidth = 800, lastwindowheight = 600;

        /// <summary>
        /// Because the unorderly manner unity Awake monobehaviors, we need to set ApplicationManager
        /// as the first to Awake in unity project setting(Script Order), so that application wide 
        /// configuration will be ready for all other objects to use.
        /// </summary>
        void Awake()
        {
            Application.runInBackground = true;
            if (File.Exists(configpath))
            {
                config = configpath.ReadYamlFile<EnvironmentConfig>();
            }
            if (config == null)
            {
                config = new EnvironmentConfig();
            }
        }

        public float GetAspectRatio()
        {
            return Screen.width.Convert<float>() / Screen.height.Convert<float>();
        }

        public void OnWindowChange()
        {
            if (isconnect)
            {
                //netmanager.client.Send(MsgType.AspectRatio, new FloatMessage() { value = GetAspectRatio() });
                netmanager.client.Send(MsgType.AspectRatio, new StringMessage(GetAspectRatio().ToString()));
            }
        }

        public void OnToggleClientConnect(bool isconn)
        {
            if (isconn)
            {
                netmanager.networkAddress = serveraddress.text;
                netmanager.StartClient();
            }
            else
            {
                netmanager.StopClient();
                OnClientDisconnect();
            }
        }

        public void OnServerAddressEndEdit(string v)
        {
            config.ServerAddress = v;
        }

        public void OnToggleAutoConnect(bool ison)
        {
            config.AutoConnect = ison;
            ResetAutoConnect();
        }

        public void ResetAutoConnect()
        {
            autoconncountdown = config.AutoConnectTimeOut;
            isautoconn = config.AutoConnect;
            if (!isautoconn)
            {
                autoconntext.text = "Auto Connect OFF";
            }
            autoconn.isOn = isautoconn;
        }

        void Start()
        {
            version.text = $"Version {Application.version}\nUnity {Application.unityVersion}";
            serveraddress.text = config.ServerAddress;
            ResetAutoConnect();
        }

        void Update()
        {
            if (!isconnect)
            {
                if (isautoconn)
                {
                    if (Time.unscaledTime - lastautoconntime >= 1)
                    {
                        autoconncountdown--;
                        if (autoconncountdown > 0)
                        {
                            lastautoconntime = Time.unscaledTime;
                            autoconntext.text = "Auto Connect " + autoconncountdown + "s";
                        }
                        else
                        {
                            clientconnect.isOn = true;
                            clientconnect.onValueChanged.Invoke(true);
                            autoconntext.text = "Connecting ...";
                            isautoconn = false;
                        }
                    }
                }
            }
        }

        public void OnQuitAction(InputAction.CallbackContext context)
        {
            if (!isconnect && context.performed) { Application.Quit(); }
        }

        public void OnToggleFullScreenAction(InputAction.CallbackContext context)
        {
            if (!isconnect && context.performed)
            {
                if (Screen.fullScreen)
                {
                    Screen.SetResolution(lastwindowwidth, lastwindowheight, false);
                }
                else
                {
                    lastwindowwidth = Math.Max(800, Screen.width);
                    lastwindowheight = Math.Max(600, Screen.height);
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, config.FullScreenMode);
                }
            }
        }

        public void OnClientConnect()
        {
            isconnect = true;
            autoconntext.text = "Connected";
            // Environment is to present final stimuli, so we may want to hide UI and Cursor when connected to Command.
            canvas.SetActive(!config.HideUIWhenConnected);
            Cursor.visible = !config.HideCursorWhenConnected;
            // When connected to Command, we need to make sure Environment present fastest and best quality stimuli.
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            QualitySettings.vSyncCount = config.VSyncCount;
            QualitySettings.maxQueuedFrames = config.MaxQueuedFrames;
            Time.fixedDeltaTime = config.FixedDeltaTime;

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            GC.Collect();
        }

        public void OnClientDisconnect()
        {
            isconnect = false;
            ResetAutoConnect();

            var callback = clientconnect.onValueChanged;
            clientconnect.onValueChanged = new Toggle.ToggleEvent();
            clientconnect.isOn = false;
            clientconnect.onValueChanged = callback;

            // When disconnected, we should turn on UI and Cursor.
            canvas.SetActive(true);
            Cursor.visible = true;
            // Return normal when disconnected
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.vSyncCount = 1;
            QualitySettings.maxQueuedFrames = 2;
            Time.fixedDeltaTime = 0.016666f;

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
            Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Normal;
            GCSettings.LatencyMode = GCLatencyMode.Interactive;
            GC.Collect();
        }

        void OnApplicationQuit()
        {
            if (netmanager.IsClientConnected())
            {
                netmanager.StopClient();
            }
            configpath.WriteYamlFile(config);
        }

    }
}
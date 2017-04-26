/*
VLEUIController.cs is part of the VLAB project.
Copyright (c) 2017 Li Alex Zhang and Contributors

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
using UnityEngine.UI;
using System.Diagnostics;
using VLab;
using System.Threading;
using System.Runtime;
using System;

namespace VLabEnvironment
{
    public class VLEUIController : MonoBehaviour
    {
        public InputField serveraddress;
        public Toggle clientconnect, autoconn;
        public Text autoconntext;
        public VLENetManager netmanager;
        public Canvas canvas;
        public VLEApplicationManager appmanager;

        bool isautoconn, isconnect;
        int autoconncountdown;
        float lastautoconntime;

        public float GetAspectRatio()
        {
            var rootrt = canvas.gameObject.transform as RectTransform;
            return rootrt.rect.width / rootrt.rect.height;
        }

        public void OnRectTransformDimensionsChange()
        {
            if (isconnect)
            {
                netmanager.client.Send(VLMsgType.AspectRatio, new FloatMessage(GetAspectRatio()));
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
            appmanager.config[VLECFG.ServerAddress] = v;
        }

        public void OnToggleAutoConnect(bool ison)
        {
            appmanager.config[VLECFG.AutoConnect] = ison;
            ResetAutoConnect();
        }

        public void ResetAutoConnect()
        {
            autoconncountdown = (int)appmanager.config[VLECFG.AutoConnectTimeOut];
            isautoconn = (bool)appmanager.config[VLECFG.AutoConnect];
            if (!isautoconn)
            {
                autoconntext.text = "Auto Connect OFF";
            }
            autoconn.isOn = isautoconn;
        }

        void Start()
        {
            serveraddress.text = (string)appmanager.config[VLECFG.ServerAddress];
            ResetAutoConnect();
        }

        void Update()
        {
            if (!isconnect)
            {
                if (Input.GetButton("Quit"))
                {
                    Application.Quit();
                    return;
                }
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

        public void OnClientConnect()
        {
            isconnect = true;
            autoconntext.text = "Connected";
            // since VLabEnvironment is to provide virtual reality environment, we may want to
            // hide cursor and ui when connected to VLab.
            canvas.enabled = !(bool)appmanager.config[VLECFG.HideUIWhenConnected];
            Cursor.visible = !(bool)appmanager.config[VLECFG.HideCursorWhenConnected];
            // when connected to VLab, we need to make sure that all system resourses
            // VLabEnvironment needed is ready to start experiment.
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            QualitySettings.vSyncCount = (int)appmanager.config[VLECFG.VSyncCount];
            QualitySettings.maxQueuedFrames = (int)appmanager.config[VLECFG.MaxQueuedFrames];
            Time.fixedDeltaTime = (float)appmanager.config[VLECFG.FixedDeltaTime];

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
            GC.Collect();
            GCSettings.LatencyMode = GCLatencyMode.LowLatency;
        }

        public void OnClientDisconnect()
        {
            isconnect = false;
            // when disconnected, we should go back to ui and turn on cursor.
            ResetAutoConnect();

            var callback = clientconnect.onValueChanged;
            clientconnect.onValueChanged = new Toggle.ToggleEvent();
            clientconnect.isOn = false;
            clientconnect.onValueChanged = callback;

            canvas.enabled = true;
            Cursor.visible = true;
            // when disconnect, we can relax and release some system resourses for other process
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.vSyncCount = 1;
            QualitySettings.maxQueuedFrames = 1;
            Time.fixedDeltaTime = 0.02f;

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
            Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Normal;
            GCSettings.LatencyMode = GCLatencyMode.Interactive;
            GC.Collect();
        }

    }
}
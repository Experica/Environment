/*
VLEApplicationManager.cs is part of the VLAB project.
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
using System.Collections.Generic;
using System.IO;
using VLab;

namespace VLabEnvironment
{
    public enum VLECFG
    {
        AutoConnect,
        AutoConnectTimeOut,
        ServerAddress,
        HideUIWhenConnected,
        HideCursorWhenConnected,
        VSyncCount,
        MaxQueuedFrames,
        FixedDeltaTime
    }

    public class VLEApplicationManager : MonoBehaviour
    {
        public VLEUIController uicontroller;
        public Dictionary<VLECFG, object> config;
        public readonly string configpath = "VLabEnvironmentConfig.yaml";

        /// <summary>
        /// Because the unorderly manner unity Awake monobehaviors, we need to set ApplicationManager
        /// as the first to Awake in unity project setting(Script Order), so that application wide 
        /// configuration is ready for all other objects to use.
        /// </summary>
        void Awake()
        {
            if (File.Exists(configpath))
            {
                config = Yaml.ReadYaml<Dictionary<VLECFG, object>>(configpath);
            }
            if (config == null)
            {
                config = new Dictionary<VLECFG, object>();
            }
            ValidateConfig();
        }

        void ValidateConfig()
        {
            if (!config.ContainsKey(VLECFG.AutoConnect))
            {
                config[VLECFG.AutoConnect] = true;
            }
            else
            {
                config[VLECFG.AutoConnect] = config[VLECFG.AutoConnect].Convert<bool>();
            }
            if (!config.ContainsKey(VLECFG.AutoConnectTimeOut))
            {
                config[VLECFG.AutoConnectTimeOut] = 10;
            }
            else
            {
                config[VLECFG.AutoConnectTimeOut] = config[VLECFG.AutoConnectTimeOut].Convert<int>();
            }
            if (!config.ContainsKey(VLECFG.ServerAddress))
            {
                config[VLECFG.ServerAddress] = "localhost";
            }
            if (!config.ContainsKey(VLECFG.HideUIWhenConnected))
            {
                config[VLECFG.HideUIWhenConnected] = true;
            }
            else
            {
                config[VLECFG.HideUIWhenConnected] = config[VLECFG.HideUIWhenConnected].Convert<bool>();
            }
            if (!config.ContainsKey(VLECFG.HideCursorWhenConnected))
            {
                config[VLECFG.HideCursorWhenConnected] = true;
            }
            else
            {
                config[VLECFG.HideCursorWhenConnected] = config[VLECFG.HideCursorWhenConnected].Convert<bool>();
            }
            if (!config.ContainsKey(VLECFG.VSyncCount))
            {
                config[VLECFG.VSyncCount] = 0;
            }
            else
            {
                config[VLECFG.VSyncCount] = config[VLECFG.VSyncCount].Convert<int>();
            }
            if (!config.ContainsKey(VLECFG.MaxQueuedFrames))
            {
                config[VLECFG.MaxQueuedFrames] = 0;
            }
            else
            {
                config[VLECFG.MaxQueuedFrames] = config[VLECFG.MaxQueuedFrames].Convert<int>();
            }
            if (!config.ContainsKey(VLECFG.FixedDeltaTime))
            {
                config[VLECFG.FixedDeltaTime] = 0.02f;
            }
            else
            {
                config[VLECFG.FixedDeltaTime] = config[VLECFG.FixedDeltaTime].Convert<float>();
            }
        }

        void OnApplicationQuit()
        {
            if (uicontroller.netmanager.IsClientConnected())
            {
                uicontroller.netmanager.StopClient();
            }
            Yaml.WriteYaml(configpath, config);
        }

    }
}
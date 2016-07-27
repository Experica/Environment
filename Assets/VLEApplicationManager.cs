// --------------------------------------------------------------
// VLEApplicationManager.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 6-16-2016
// --------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using VLab;

namespace VLabEnvironment
{
    public enum VLECFG
    {
        AutoConnection,
        AutoConnectionTimeOut,
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
            if (!config.ContainsKey(VLECFG.AutoConnection))
            {
                config[VLECFG.AutoConnection] = true;
            }
            else
            {
                config[VLECFG.AutoConnection] = config[VLECFG.AutoConnection].Convert<bool>();
            }
            if (!config.ContainsKey(VLECFG.AutoConnectionTimeOut))
            {
                config[VLECFG.AutoConnectionTimeOut] = 10;
            }
            else
            {
                config[VLECFG.AutoConnectionTimeOut] = config[VLECFG.AutoConnectionTimeOut].Convert<int>();
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

        void Update()
        {
            if (Input.GetButton("Quit"))
            {
                Application.Quit();
            }
        }

    }
}
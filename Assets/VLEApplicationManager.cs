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
    public class VLEApplicationManager : MonoBehaviour
    {
        public VLEUIController uicontroller;
        public Dictionary<string, object> config;
        public static readonly string configpath = "VLabEnvironmentConfig.yaml";

        /// <summary>
        /// because the unorderly manner unity Awake monobehaviors, we need to set ApplicationManager
        /// as the first to Awake in unity project setting(Script Order), so that application wide 
        /// configuration is ready for all other objects to use.
        /// </summary>
        void Awake()
        {
            if (File.Exists(configpath))
            {
                config = Yaml.ReadYaml<Dictionary<string, object>>(configpath);
            }
            if (config == null)
            {
                config = new Dictionary<string, object>();
            }
            ValidateConfig();
        }

        void ValidateConfig()
        {
            if (!config.ContainsKey("isautoconn"))
            {
                config["isautoconn"] = true;
            }
            if (!config.ContainsKey("autoconntimeout"))
            {
                config["autoconntimeout"] = 10;
            }
            if (!config.ContainsKey("serveraddress"))
            {
                config["serveraddress"] = "localhost";
            }
            if (!config.ContainsKey("ishideuiwhenconnect"))
            {
                config["ishideuiwhenconnect"] = true;
            }
            if (!config.ContainsKey("ishidecursorwhenconnect"))
            {
                config["ishidecursorwhenconnect"] = true;
            }
            if (!config.ContainsKey("vsynccount"))
            {
                config["vsynccount"] = 0;
            }
            if (!config.ContainsKey("maxqueuedframes"))
            {
                config["maxqueuedframes"] = 0;
            }
            if (!config.ContainsKey("fixeddeltatime"))
            {
                config["fixeddeltatime"] = 0.02f;
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
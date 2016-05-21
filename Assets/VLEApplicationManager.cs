// --------------------------------------------------------------
// VLEApplicationManager.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 5-21-2016
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
        public readonly string configpath = "VLabEnvironmentConfig.yaml";

        void Awake()
        {
            if (File.Exists(configpath))
            {
                config = Yaml.ReadYaml<Dictionary<string, object>>(configpath);
            }
            else
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
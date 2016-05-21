// --------------------------------------------------------------
// VLEUIController.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 5-21-2016
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using VLab;

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

        private bool isautoconn;
        private int autoconncountdown;
        private float lastautoconntime;

        public float GetAspectRatio()
        {
            var rootrt = canvas.gameObject.transform as RectTransform;
            return rootrt.rect.width / rootrt.rect.height;
        }

        public void OnRectTransformDimensionsChange()
        {
            if (netmanager.IsClientConnected())
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

                if ((bool)VLConvert.Convert(appmanager.config["ishideuiwhenconnect"], typeof(bool)))
                {
                    canvas.enabled = false;
                }
                if ((bool)VLConvert.Convert(appmanager.config["ishidecursorwhenconnect"], typeof(bool)))
                {
                    Cursor.visible = false;
                }
            }
            else
            {
                netmanager.StopClient();
            }
        }

        public void OnServerAddressEndEdit(string v)
        {
            appmanager.config["serveraddress"] = v;
        }

        public void OnToggleAutoConnect(bool ison)
        {
            appmanager.config["isautoconn"] = ison;
            ResetAutoConnect();
        }

        public void ResetAutoConnect()
        {
            autoconncountdown = (int)VLConvert.Convert(appmanager.config["autoconntimeout"], typeof(int));
            isautoconn = (bool)VLConvert.Convert(appmanager.config["isautoconn"], typeof(bool));
            if (!isautoconn)
            {
                autoconntext.text = "Auto Connect OFF";
            }
            autoconn.isOn = isautoconn;
        }

        void Start()
        {
            ResetAutoConnect();
            serveraddress.text = (string)appmanager.config["serveraddress"];
        }

        void Update()
        {
            if (isautoconn && !netmanager.IsClientConnected())
            {
                if (Time.unscaledTime - lastautoconntime >= 1)
                {
                    autoconncountdown--;
                    if (autoconncountdown > 0)
                    {
                        autoconntext.text = "Auto Connect " + autoconncountdown + "s";
                        lastautoconntime = Time.unscaledTime;
                    }
                    else
                    {
                        clientconnect.onValueChanged.Invoke(true);
                        isautoconn = false;
                    }
                }
            }
        }

        public void OnClientDisconnect()
        {
            ResetAutoConnect();
            clientconnect.isOn = false;
            canvas.enabled = true;
            Cursor.visible = true;
        }

    }
}
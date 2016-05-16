// --------------------------------------------------------------
// VLNetwork.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 5-16-2016
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace VLab
{
    public class VLMsgType
    {
        public const short PeerInfo = MsgType.Highest + 1;

        public const short Highest = PeerInfo;

        internal static string[] msgLabels = new string[]
        {
            "PeerInfo"
        };

        public static string MsgTypeToString(short value)
        {
            if (value < PeerInfo || value > Highest)
            {
                return string.Empty;
            }
            string text = msgLabels[value - MsgType.Highest - 1];
            if (string.IsNullOrEmpty(text))
            {
                text = "[" + value + "]";
            }
            return text;
        }
    }

    public enum VLPeerType
    {
        VLab,
        VLabEnvironment,
        VLabAnalysis
    }
}

/*
EnvironmentConfig.cs is part of the Experica.
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experica.Environment
{
    public class EnvironmentConfig
    {
        public bool AutoConnect { get; set; } = true;
        public int AutoConnectTimeOut { get; set; } = 10;
        public string ServerAddress { get; set; } = "LocalHost";
        public bool HideUIWhenConnected { get; set; } = true;
        public bool HideCursorWhenConnected { get; set; } = true;
        public int VSyncCount { get; set; } = 1;
        public int MaxQueuedFrames { get; set; } = 0;
        public float FixedDeltaTime { get; set; } = 1000000f;
        public float SyncFrameTimeOut { get; set; } = 2;
        public FullScreenMode FullScreenMode { get; set; } = FullScreenMode.FullScreenWindow;
    }
}
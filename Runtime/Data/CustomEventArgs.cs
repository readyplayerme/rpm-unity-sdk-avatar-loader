﻿using System;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    public class FailureEventArgs : EventArgs
    {
        public string Url { get; set; }
        public string Message { get; set; }
        public FailureType Type { get; set; }
    }

    public class ProgressChangeEventArgs : EventArgs
    {
        public string Url { get; set; }
        public float Progress { get; set; }
        public string Operation { get; set; }
    }

    public class CompletionEventArgs : EventArgs
    {
        public string Url { get; set; }

        public GameObject Avatar { get; set; }
    }
}

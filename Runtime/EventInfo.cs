using System;
using UnityEngine;

namespace MbsCore.InsightTrack.Runtime
{
    [Serializable]
    internal struct EventInfo
    {
        [SerializeField] private string _eventName;

        public string EventName => _eventName;

        public EventInfo(string eventName)
        {
            _eventName = eventName;
        }
    }
}
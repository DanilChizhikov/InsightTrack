using System;
using System.Collections.Generic;
using MbsCore.InsightTrack.Infrastructure;
using UnityEngine;

namespace MbsCore.InsightTrack.Runtime
{
    public abstract class AnalyticsConfig : ScriptableObject, IAnalyticsConfig
    {
        [SerializeField] private EventInfo[] _infos = Array.Empty<EventInfo>();

        public IReadOnlyList<string> Events
        {
            get
            {
                var events = new List<string>(_infos.Length);
                for (int i = _infos.Length - 1; i >= 0; i--)
                {
                    EventInfo eventInfo = _infos[i];
                    if (!events.Contains(eventInfo.EventName))
                    {
                        events.Add(eventInfo.EventName);
                    }
                }

                return events;
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace MbsCore.InsightTrack.Infrastructure
{
    public interface IAnalyticsService : IDisposable
    {
        void Initialize();
        void SetUserProperty(string propName, string value);
        void SendEvent(string eventName);
        void SendEvent(string eventName, string eventValue);
        void SendEventParams(string eventName, string paramName, object paramValue);
        void SendEventParams(string eventName, string eventValue, IDictionary<string, object> parameters);
        void SendEventParams(string eventName, IDictionary<string, object> parameters);
    }
}

using System.Collections.Generic;

namespace MbsCore.InsightTrack.Infrastructure
{
    public interface IAnalyticsAdapter
    {
        bool IsInitialized { get; }
        
        void Initialize();
        void SetUserProperty(string propName, string value);
        void SendEvent(string eventName, string value);
        void SendEventParam(string eventName, string value, IDictionary<string, object> eventParameters);
        void DeInitialize();
    }
}
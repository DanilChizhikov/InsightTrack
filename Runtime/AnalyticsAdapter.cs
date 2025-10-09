using System.Collections.Generic;
using MbsCore.InsightTrack.Infrastructure;

namespace MbsCore.InsightTrack.Runtime
{
    public abstract class AnalyticsAdapter : IAnalyticsAdapter
    {
        public bool IsInitialized { get; private set; }
        
        protected EventConcatBuilder EventConcatBuilder { get; }

        public AnalyticsAdapter()
        {
            IsInitialized = false;
            EventConcatBuilder = new EventConcatBuilder();
        }

        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            
            InitializeProcessing();
            IsInitialized = true;
        }

        public virtual void SetUserProperty(string propName, string value) { }

        public abstract void SendEvent(string eventName, string value);

        public abstract void SendEventParam(string eventName, string value, IDictionary<string, object> eventParameters);

        public void DeInitialize()
        {
            if (!IsInitialized)
            {
                return;
            }
            
            IsInitialized = false;
            DeInitializeProcessing();
        }

        protected virtual void Send(string eventName, string value, IDictionary<string, object> eventParameters) {}
		
        protected abstract void Send(string eventName, string value);
        
        protected virtual void InitializeProcessing() { }
        protected virtual void DeInitializeProcessing() { }
    }

    public abstract class AnalyticsAdapter<TConfig> : AnalyticsAdapter where TConfig : IAnalyticsConfig
    {
        private readonly HashSet<string> _events;
        
        protected TConfig Config { get; }

        public AnalyticsAdapter(TConfig config)
        {
            _events = new HashSet<string>(config.Events);
            Config = config;
        }

        public sealed override void SendEvent(string eventName, string value)
        {
            if (!IsInitialized || !_events.Contains(eventName))
            {
                return;
            }
            
            Send(eventName, value);
        }

        public sealed override void SendEventParam(string eventName, string value, IDictionary<string, object> eventParameters)
        {
            if (!IsInitialized || !_events.Contains(eventName))
            {
                return;
            }
            
            Send(eventName, value, eventParameters);
        }
    }
}
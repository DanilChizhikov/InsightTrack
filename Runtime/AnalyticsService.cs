using System;
using System.Collections.Generic;
using MbsCore.InsightTrack.Infrastructure;

namespace MbsCore.InsightTrack.Runtime
{
    public sealed class AnalyticsService : IAnalyticsService
    {
        private readonly HashSet<IAnalyticsAdapter> _adapters;
        private readonly Dictionary<string, object> _tempParams;

        public AnalyticsService(IEnumerable<IAnalyticsAdapter> adapters)
        {
            _adapters = new HashSet<IAnalyticsAdapter>(adapters);
            _tempParams = new Dictionary<string, object>();
        }

        public AnalyticsService(params IAnalyticsAdapter[] adapters)
        {
            _adapters = new HashSet<IAnalyticsAdapter>(adapters);
            _tempParams = new Dictionary<string, object>();
        }
        
        public void Initialize()
        {
            foreach (var adapter in _adapters)
            {
                adapter.Initialize();
            }
        }

        public void SetUserProperty(string propName, string value)
        {
            if (string.IsNullOrEmpty(propName) || string.IsNullOrEmpty(value))
            {
                return;
            }
			
            AdaptersBypass(AdapterSetUserProperty);
            
            void AdapterSetUserProperty(IAnalyticsAdapter adapter)
            {
                adapter.SetUserProperty(propName, value);
            }
        }

        public void SendEvent(string eventName)
        {
            SendEvent(eventName, string.Empty);
        }

        public void SendEvent(string eventName, string eventValue)
        {
            Send(eventName, eventValue);
        }

        public void SendEventParams(string eventName, string paramName, object paramValue)
        {
            SendParams(eventName, paramName, paramValue);
        }

        public void SendEventParams(string eventName, string eventValue, IDictionary<string, object> parameters)
        {
            SendParams(eventName, eventValue, parameters);
        }

        public void SendEventParams(string eventName, IDictionary<string, object> parameters)
        {
            SendParams(eventName, string.Empty, parameters);
        }

        public void Dispose()
        {
            _tempParams.Clear();
            foreach (var adapter in _adapters)
            {
                adapter.DeInitialize();
            }
            
            _adapters.Clear();
        }
        
        private void Send(string id, string value)
        {
            AdaptersBypass(AdapterSendEventComplex);

            void AdapterSendEventComplex(IAnalyticsAdapter adapter)
            {
                adapter.SendEvent(id, value);
            }
        }
		
        private void SendParams(string eventId, string eventValue, IDictionary<string, object> parameters)
        {
            AdaptersBypass(AdapterSendEventParam);

            void AdapterSendEventParam(IAnalyticsAdapter adapter)
            {
                adapter.SendEventParam(eventId, eventValue, parameters);
            }
        }
		
        private void SendParams(string eventId, string paramName, object paramValue)
        {
            _tempParams.Clear();
            _tempParams.Add(paramName, paramValue);
            SendEventParams(eventId, string.Empty, _tempParams);
        }
        
        private void AdaptersBypass(Action<IAnalyticsAdapter> action)
        {
            foreach (IAnalyticsAdapter adapter in _adapters)
            {
                if (adapter.IsInitialized)
                {
                    action(adapter);   
                }
            }
        }
    }
}
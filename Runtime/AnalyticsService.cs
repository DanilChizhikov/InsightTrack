using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace DTech.InsightTrack
{
    public sealed class AnalyticsService : IAnalyticsService, IDisposable
    {
        public event Action OnInitialized;
        public event Action<ExceptionDispatchInfo> OnSendException;

        private readonly List<IAnalyticsAdapter> _adapters;
        private readonly Queue<IAnalyticEvent> _eventBuffer;
        
        public bool IsInitialized { get; private set; }
        public bool IsSendingActive { get; private set; }

        private CancellationTokenSource _bufferTokenSource;

        public AnalyticsService(IEnumerable<IAnalyticsAdapter> adapters)
        {
            _adapters = new List<IAnalyticsAdapter>(adapters);
            _eventBuffer = new Queue<IAnalyticEvent>();
            IsSendingActive = false;
        }
        
        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (IsInitialized)
            {
                return;
            }

            List<IAnalyticsAdapter> adapters = ListPool<IAnalyticsAdapter>.Get();
            adapters.Clear();
            adapters.AddRange(_adapters);
            adapters.Sort((x, y) => x.InitializeOrder.CompareTo(y.InitializeOrder));
            for (int i = 0; i < adapters.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    for (int j = 0; j < i; j++)
                    {
                        IAnalyticsAdapter analyticsAdapter = adapters[j];
                        analyticsAdapter.Dispose();
                    }
                    
                    break;
                }
                
                IAnalyticsAdapter adapter = adapters[i];
                await adapter.InitializeAsync(cancellationToken);
            }
            
            adapters.Clear();
            ListPool<IAnalyticsAdapter>.Release(adapters);
            IsInitialized = !cancellationToken.IsCancellationRequested;
            if (IsInitialized)
            {
                OnInitialized?.Invoke();
            }
        }

        public void SetSendingActive(bool isActive)
        {
            IsSendingActive = isActive;
            if (IsSendingActive)
            {
                _bufferTokenSource?.Cancel();
                _bufferTokenSource?.Dispose();
                _bufferTokenSource = new CancellationTokenSource();
                Task.Run(() => SendBufferAsync(_bufferTokenSource.Token));
            }
        }

        public void SendEvent(IAnalyticEvent analyticEvent)
        {
            if (!IsSendingActive)
            {
                _eventBuffer.Enqueue(analyticEvent);
                return;
            }

            Task.Run(() => SendEventAsync(analyticEvent));
        }

        public void Dispose()
        {
            _bufferTokenSource?.Cancel();
            _bufferTokenSource?.Dispose();
            _bufferTokenSource = null;
            
            for (int i = 0; i < _adapters.Count; i++)
            {
                IAnalyticsAdapter adapter = _adapters[i];
                adapter.Dispose();
            }
            
            _adapters.Clear();
        }
        
        private async Task SendBufferAsync(CancellationToken cancellationToken)
        {
            while (_eventBuffer.TryDequeue(out IAnalyticEvent analyticEvent))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                
                await SendEventAsync(analyticEvent);
            }
            
            _eventBuffer.Clear();
        }
        
        private async Task SendEventAsync(IAnalyticEvent analyticEvent)
        {
            for (int i = 0; i < _adapters.Count; i++)
            {
                IAnalyticsAdapter adapter = _adapters[i];
                try
                {
                    adapter.SendEvent(analyticEvent);
                }
                catch (Exception exception)
                {
                    OnSendException?.Invoke(ExceptionDispatchInfo.Capture(exception));
                    Debug.LogException(exception);
                }
                
                await Task.Yield();
            }
        }
    }
}
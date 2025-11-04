using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DTech.InsightTrack
{
    public abstract class AnalyticsAdapter : IAnalyticsAdapter
    {
        public bool IsInitialized { get; private set; } = false;
        public virtual int InitializeOrder => 0;

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (IsInitialized)
            {
                return;
            }
            
            await InitializeProcessingAsync(cancellationToken);
            IsInitialized = !cancellationToken.IsCancellationRequested;
            if (IsInitialized)
            {
                SendBufferEvents();
            }
        }

        public abstract void SendEvent(IAnalyticEvent value);
        
        public virtual void Dispose() { }

        protected virtual Task InitializeProcessingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        protected abstract void SendBufferEvents();
    }
    
    public abstract class AnalyticsAdapter<TEvent> : AnalyticsAdapter
        where TEvent : struct, IAnalyticEvent
    {
        private readonly Queue<TEvent> _eventBuffer = new ();
        
        public sealed override void SendEvent(IAnalyticEvent value)
        {
            if (value is not TEvent generic)
            {
                return;
            }

            if (IsInitialized)
            {
                SendEvent(generic);
            }
            else
            {
                _eventBuffer.Enqueue(generic);
            }
        }

        protected sealed override void SendBufferEvents()
        {
            while (_eventBuffer.TryDequeue(out TEvent analyticsEvent))
            {
                SendEvent(analyticsEvent);
            }
        }

        protected abstract void SendEvent(TEvent value);
    }
}
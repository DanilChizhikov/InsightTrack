using System;
using System.Threading;
using System.Threading.Tasks;

namespace DTech.InsightTrack
{
    public interface IAnalyticsAdapter : IDisposable
    {
        bool IsInitialized { get; }
        int InitializeOrder { get; }
        
        Task InitializeAsync(CancellationToken cancellationToken);
        void SendEvent(IAnalyticEvent value);
    }
}
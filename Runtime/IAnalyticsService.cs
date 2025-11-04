using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace DTech.InsightTrack
{
    public interface IAnalyticsService
    {
        event Action OnInitialized;
        event Action<ExceptionDispatchInfo> OnSendException;
        
        bool IsInitialized { get; }
        bool IsSendingActive { get; }
        
        Task InitializeAsync(CancellationToken cancellationToken);
        void SetSendingActive(bool isActive);
        void SendEvent(IAnalyticEvent analyticEvent);
    }
}
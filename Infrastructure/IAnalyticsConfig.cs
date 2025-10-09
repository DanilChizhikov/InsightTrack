using System.Collections.Generic;

namespace MbsCore.InsightTrack.Infrastructure
{
    public interface IAnalyticsConfig
    {
        IReadOnlyList<string> Events { get; }
    }
}
using System;
using UnityEngine;

namespace MbsCore.InsightTrack.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AnalyticsEventAttribute : PropertyAttribute { }
}
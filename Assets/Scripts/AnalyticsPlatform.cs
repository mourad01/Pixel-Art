using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class AnalyticsPlatform : MonoBehaviour
{
    protected List<AnalyticsEvents> PlatformEvents { get; set; }

    public virtual void Init()
    {
        PlatformEvents = Enum.GetValues(typeof(AnalyticsEvents)).OfType<AnalyticsEvents>().ToList();
    }

    public void SendEvent(AnalyticsEvents eventType)
    {
        if (PlatformEvents != null)
        {
            if (PlatformEvents.Contains(eventType))
            {
                SendEventInternal(eventType);
            }
        }
    }

    public void SendEvent(AnalyticsEvents eventType, Dictionary<string, object> parameters)
    {
        if (PlatformEvents != null && PlatformEvents.Contains(eventType))
        {
            SendEventInternal(eventType, parameters);
        }
    }

    protected void Log(AnalyticsEvents eventType, Dictionary<string, object> parameters)
    {
        if (AnalyticsHelper.IsTestDevice())
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[Analytics] eventName : ").Append(eventType.ToString());
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    stringBuilder.Append("\nparam[").Append(parameter.Key).Append("] = ")
                        .Append(parameter.Value);
                }
            }
            Debug.Log(stringBuilder.ToString());
            CustomLogger.Instance.Log(stringBuilder.ToString());
        }
    }

    protected abstract void SendEventInternal(AnalyticsEvents eventType);

    protected abstract void SendEventInternal(AnalyticsEvents eventType, Dictionary<string, object> parameters);
}



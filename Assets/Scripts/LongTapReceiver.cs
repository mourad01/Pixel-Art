using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongTapReceiver : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
    DateTime m_downTime = DateTime.MaxValue;

    [SerializeField] float m_duration = 1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_downTime = DateTime.Now;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if ((DateTime.Now - m_downTime).TotalSeconds > (double)m_duration)
        {
            DebugController.Instance.ClickDebugTexts();
        }
    }
}



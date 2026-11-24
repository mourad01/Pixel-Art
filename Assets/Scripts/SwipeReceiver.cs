using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeReceiver : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
    public Action OnSwipeLeft;
    public Action OnSwipeRight;
    public Action OnSkipIt;

    Vector2 m_pos = Vector2.zero;
    Vector2 m_lastPos = Vector2.zero;
    DateTime m_lastTime = DateTime.MinValue;
    bool m_skip;

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_lastTime = DateTime.Now;
        m_pos = eventData.position;
        m_skip = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_lastTime = DateTime.Now;
        m_lastPos = eventData.position;
        //vertical moves over 50 px are passed to the parent scroll rect
        float dy = m_lastPos.y - m_pos.y;
        if (!(dy > 50f) && !(dy < -50f) && !m_skip)
        {
            return;
        }
        if (!m_skip)
        {
            m_skip = true;
            base.GetComponentInParent<ScrollRect>().SendMessage("OnBeginDrag", eventData);
        }
        else
        {
            base.GetComponentInParent<ScrollRect>().SendMessage("OnDrag", eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_skip)
        {
            base.GetComponentInParent<ScrollRect>().SendMessage("OnEndDrag", eventData);
        }
        // swipe needs 100 px sideways and no pause longer than 0.3s
        else if (!((DateTime.Now - m_lastTime).TotalSeconds > 0.3))
        {
            Vector2 dir = eventData.position - m_pos;
            float x = dir.x;
            Vector2 totalSwipeOffset = eventData.position - m_lastPos;
            if (x * totalSwipeOffset.x >= 0f)
            {
                if (dir.x > 100f)
                {
                    OnSwipeRight.SafeInvoke();
                }
                else if (dir.x < -100f)
                {
                    OnSwipeLeft.SafeInvoke();
                }
            }
        }
    }
}



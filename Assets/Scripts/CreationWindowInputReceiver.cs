using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreationWindowInputReceiver : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler, IEventSystemHandler
{
    public Action<float> OnScrolled;
    public Action<float> OnZoomed;
    public Action<Vector2> OnDragged;

    Vector2 m_pos = Vector2.zero;
    InputReceiver.TapState m_currentTapState;
    float m_touchesDelta = -3.40282347E+38f;

    void Update()
    {
        if (m_currentTapState != InputReceiver.TapState.Multitouch)
        {
            CheckMultiTouch();
        }
    }

    bool CheckMultiTouch()
    {
        if (Input.touches.Length > 1)
        {
            var touches = Input.touches;
            Vector2 dir = touches[0].position - touches[1].position;
            m_touchesDelta = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
            m_pos = Input.mousePosition;
            m_currentTapState = InputReceiver.TapState.Multitouch;
            return true;
        }
        return false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (m_currentTapState)
        {
            case InputReceiver.TapState.Drag:
                OnDragged.SafeInvoke(eventData.position - m_pos);
                m_pos = eventData.position;
                break;
            case InputReceiver.TapState.Multitouch:
                {
                    var touches = Input.touches;
                    if (touches.Length >= 2)
                    {
                        Vector2 dir = touches[0].position - touches[1].position;
                        float touchDistance = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
                        float par = touchDistance / m_touchesDelta;
                        OnZoomed.SafeInvoke(par);
                        m_touchesDelta = touchDistance;
                    }
                    break;
                }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_pos = eventData.position;
        m_currentTapState = InputReceiver.TapState.Drag;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_currentTapState = InputReceiver.TapState.None;
    }

    public void OnScroll(PointerEventData data)
    {
        OnScrolled.SafeInvoke(data.scrollDelta.y);
    }
}



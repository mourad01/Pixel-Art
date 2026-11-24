using UnityObject = global::UnityEngine.Object;

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputReceiver3D : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler, IEventSystemHandler
{
    public enum TapState
    {
        None,
        Down,
        Drag,
        Multitouch,
        BrushMode
    }

    [SerializeField] Canvas m_canvas;

    DateTime m_downTime = DateTime.MinValue;
    float m_longTapTime = 0.15f;
    Vector3 m_tapPosition = Vector3.zero;
    Vector2 m_mousePrev = Vector2.zero;
    float m_touchesDelta = -3.40282347E+38f;

    public TapState CurrentTapState { get; private set; }

    void Update()
    {
        switch (CurrentTapState)
        {
            case TapState.Down:
                if (!CheckMultiTouch())
                {
                    CheckLongTap();
                }
                break;
            case TapState.Drag:
                CheckMultiTouch();
                break;
            case TapState.Multitouch:
                if (Input.touches.Length == 0)
                {
                    CurrentTapState = TapState.None;
                }
                break;
            case TapState.BrushMode:
                if (CheckMultiTouch())
                {
                    break;
                }
                break;
            case TapState.None:
                CheckMultiTouch();
                break;
        }
    }

    bool CheckMultiTouch()
    {
        if (Input.touches.Length > 1)
        {
            var touches = Input.touches;
            Vector2 dir = touches[0].position - touches[1].position;
            m_touchesDelta = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
            m_mousePrev = Input.mousePosition;
            if (CurrentTapState == TapState.BrushMode)
            {
                WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType = ColorizationModeModel.BrushType.Singular;
            }
            CurrentTapState = TapState.Multitouch;
            return true;
        }
        return false;
    }

    void CheckLongTap()
    {
        if (m_downTime > DateTime.MinValue && (DateTime.Now - m_downTime).TotalSeconds > (double)m_longTapTime)
        {
            WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType = ColorizationModeModel.BrushType.Plural;
            CurrentTapState = TapState.BrushMode;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!CheckMultiTouch())
        {
            if (CurrentTapState == TapState.BrushMode)
            {
                WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType = ColorizationModeModel.BrushType.Singular;
            }
            CurrentTapState = TapState.Down;
            m_downTime = DateTime.Now;
            m_tapPosition = Input.mousePosition;
            m_mousePrev = Input.mousePosition;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        switch (CurrentTapState)
        {
            case TapState.Down:
                UnityObject.FindObjectOfType<TouchManager>().CheckCube(data.position, true);
                break;
        }
        CurrentTapState = TapState.None;
    }

    public void OnDrag(PointerEventData data)
    {
        switch (CurrentTapState)
        {
            case TapState.None:
                break;
            case TapState.Down:
                CurrentTapState = TapState.Drag;
                break;
            case TapState.Drag:
                {
                    Vector2 dragOffset = (Vector2)Input.mousePosition - m_mousePrev;
                    float value2 = dragOffset.x / (float)Screen.width * 180f;
                    float value3 = dragOffset.y / (float)Screen.width * 180f;
                    Quaternion rotation = Camera.main.transform.rotation;
                    Vector3 eulerAngles = rotation.eulerAngles;
                    float value4 = eulerAngles.x - value3;
                    Vector3 eulerAngles2 = rotation.eulerAngles;
                    float y = eulerAngles2.y + value2;
                    if (value4 > 180f)
                    {
                        value4 -= 360f;
                    }
                    else if (value4 < -180f)
                    {
                        value4 += 360f;
                    }
                    value4 = Mathf.Clamp(value4, -80f, 80f);
                    rotation.eulerAngles = new Vector3(value4, y, 0f);
                    Camera.main.transform.rotation = rotation;
                    m_mousePrev = Input.mousePosition;
                    Vector3 centerOffset = Vector3.zero - Camera.main.transform.position;
                    if (centerOffset.magnitude > 0.2f)
                    {
                        centerOffset = centerOffset / centerOffset.magnitude * 0.2f;
                    }
                    var transform = Camera.main.transform;
                    transform.position += centerOffset;
                    break;
                }
            case TapState.Multitouch:
                {
                    var touches = Input.touches;
                    if (touches.Length >= 2)
                    {
                        Vector2 pinchOffset = touches[0].position - touches[1].position;
                        float value5 = Mathf.Sqrt(pinchOffset.x * pinchOffset.x + pinchOffset.y * pinchOffset.y);
                        float value6 = value5 / m_touchesDelta;
                        float value = Camera.main.orthographicSize / value6;
                        value = Mathf.Clamp(value, 3f, 25f);
                        Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(data.position);
                        Vector3 a3 = Camera.main.ScreenToWorldPoint(m_mousePrev);
                        Camera.main.orthographicSize = value;
                        m_touchesDelta = value5;
                        Vector3 b = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector3 dir = a3 - b;
                        var transform2 = Camera.main.transform;
                        transform2.position += dir;
                        float z = Mathf.InverseLerp(15f, 5f, value);
                        UnitySingleton<GameController>.Instance.ChangeCamDepthPosition(z);
                        m_mousePrev = Input.mousePosition;
                    }
                    break;
                }
            case TapState.BrushMode:
                {
                    Vector2 prevPos = data.position - data.delta;
                    var deltas = new float[3] {
                    1f,
                    0f,
                    0f
                };
                    Vector2 delta = data.delta;
                    deltas[1] = Mathf.Abs(delta.x);
                    Vector2 delta2 = data.delta;
                    deltas[2] = Mathf.Abs(delta2.y);
                    float steps = Mathf.Max(deltas);
                    Vector2 a = data.delta / steps;
                    Vector2 a2 = prevPos;
                    var touchManager = UnityObject.FindObjectOfType<TouchManager>();
                    for (int i = 1; (float)i < steps; i++)
                    {
                        touchManager.CheckCube(a2 + (float)i * a, false);
                    }
                    touchManager.CheckCube(data.position, false);
                    break;
                }
        }
    }

    public void OnScroll(PointerEventData data)
    {
        float orthographicSize = Camera.main.orthographicSize;
        Vector2 scrollDelta = data.scrollDelta;
        float value = orthographicSize * Mathf.Pow(1.1f, 0f - scrollDelta.y);
        value = Mathf.Clamp(value, 3f, 25f);
        Vector2 position = data.position;
        Vector3 a = Camera.main.ScreenToWorldPoint(position);
        Camera.main.orthographicSize = value;
        Vector3 b = Camera.main.ScreenToWorldPoint(position);
        Vector3 translation = a - b;
        Camera.main.transform.Translate(translation);
        float z = Mathf.InverseLerp(15f, 5f, value);
        UnitySingleton<GameController>.Instance.ChangeCamDepthPosition(z);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (CurrentTapState == TapState.BrushMode)
            {
                CurrentTapState = TapState.None;
                WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType = ColorizationModeModel.BrushType.Singular;
            }
            m_downTime = DateTime.MinValue;
        }
    }
}



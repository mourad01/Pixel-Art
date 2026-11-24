using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputReceiver : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler, IEventSystemHandler
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
    bool m_currentZoomLogged;
    bool m_loupeEnabled;
    public TapState CurrentTapState { get; private set; }

    void Start()
    {
        m_loupeEnabled = AppData.LoupeEnabled;
    }

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
                if (!CheckMultiTouch())
                {
                    Vector3 mousePosition = Input.mousePosition;
                    int height = Screen.height;
                    int width = Screen.width;
                    Rect rect = ((RectTransform)m_canvas.transform).rect;
                    float scale = rect.width / (float)width;
                    Vector2 pos = new Vector2(mousePosition.x / (float)width * rect.width, mousePosition.y / (float)height * rect.height);
                    var workspaceRect = NewWorkbookManager.Instance.CameraManager.WorkspaceRect;
                    float x = pos.x;
                    Vector2 size = NewWorkbookManager.Instance.CameraManager.Loupe.Size;
                    float value2 = x + size.x / 4f;
                    Vector2 offsetMax = workspaceRect.offsetMax;
                    if (value2 > offsetMax.x)
                    {
                        Vector2 offsetMax2 = workspaceRect.offsetMax;
                        float value3 = offsetMax2.x - pos.x;
                        Vector2 size2 = NewWorkbookManager.Instance.CameraManager.Loupe.Size;
                        float a = value3 - size2.x / 4f;
                        a = Mathf.Max(a, (0f - workspaceRect.rect.width) / 10f);
                        //NewWorkbookManager.Instance.CameraManager.Move(new Vector2(a / num * 0.05f, 0f));
                        NewWorkbookManager.Instance.CameraManager.UpdateLoupe(Input.mousePosition);
                        NewWorkbookManager.Instance.NumberColoring.TryClickPixel(Input.mousePosition);
                    }
                    else
                    {
                        float x2 = pos.x;
                        Vector2 size3 = NewWorkbookManager.Instance.CameraManager.Loupe.Size;
                        float value4 = x2 - size3.x / 4f;
                        Vector2 offsetMin = workspaceRect.offsetMin;
                        if (value4 < offsetMin.x)
                        {
                            Vector2 offsetMin2 = workspaceRect.offsetMin;
                            float value5 = offsetMin2.x - pos.x;
                            Vector2 size4 = NewWorkbookManager.Instance.CameraManager.Loupe.Size;
                            float a2 = value5 + size4.x / 4f;
                            a2 = Mathf.Min(a2, workspaceRect.rect.width / 10f);

                            //NewWorkbookManager.Instance.CameraManager.Move(new Vector2(a2 / num * 0.05f, 0f));
                            NewWorkbookManager.Instance.CameraManager.UpdateLoupe(Input.mousePosition);
                            Debug.Log(NewWorkbookManager.Instance.NumberColoring);
                                NewWorkbookManager.Instance.NumberColoring.TryClickPixel(Input.mousePosition);



                            
                        }
                    }
                    float y = pos.y;
                    Vector2 size5 = NewWorkbookManager.Instance.CameraManager.Loupe.Size;
                    float value6 = y + size5.y * 0.75f;
                    float height2 = rect.height;
                    Vector2 offsetMax3 = workspaceRect.offsetMax;
                    if (value6 > height2 + offsetMax3.y)
                    {
                        float height3 = rect.height;
                        Vector2 offsetMax4 = workspaceRect.offsetMax;
                        float value7 = height3 + offsetMax4.y - pos.y;
                        Vector2 size6 = NewWorkbookManager.Instance.CameraManager.Loupe.Size;
                        float a3 = value7 - size6.y * 0.75f;
                        a3 = Mathf.Max(a3, (0f - workspaceRect.rect.height) / 10f);
                        //NewWorkbookManager.Instance.CameraManager.Move(new Vector2(0f, a3 / num * 0.05f));
                        NewWorkbookManager.Instance.CameraManager.UpdateLoupe(Input.mousePosition);
                        NewWorkbookManager.Instance.NumberColoring.TryClickPixel(Input.mousePosition);
                    }
                    else if (!m_loupeEnabled)
                    {
                        float y2 = pos.y;
                        float height4 = rect.height;
                        Vector2 offsetMin3 = workspaceRect.offsetMin;
                        if (y2 < height4 + offsetMin3.y + workspaceRect.rect.height / 10f)
                        {
                            float height5 = rect.height;
                            Vector2 offsetMin4 = workspaceRect.offsetMin;
                            float a4 = height5 + offsetMin4.y + workspaceRect.rect.height / 10f - pos.y;
                            a4 = Mathf.Min(a4, workspaceRect.rect.height / 10f);
                            //NewWorkbookManager.Instance.CameraManager.Move(new Vector2(0f, a4 / num * 0.05f));
                            NewWorkbookManager.Instance.CameraManager.UpdateLoupe(Input.mousePosition);
                            NewWorkbookManager.Instance.NumberColoring.TryClickPixel(Input.mousePosition);
                        }
                    }
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
                NewWorkbookManager.Instance.CameraManager.SwitchLoupeOnOff(false);
                WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType = ColorizationModeModel.BrushType.Singular;
            }
            CurrentTapState = TapState.Multitouch;
            m_currentZoomLogged = false;
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
            NewWorkbookManager.Instance.CameraManager.CheckMinZoom(Input.mousePosition);
            NewWorkbookManager.Instance.CameraManager.SwitchLoupeOnOff(true);
            NewWorkbookManager.Instance.CameraManager.UpdateLoupe(Input.mousePosition);
            AnalyticsManager.Instance.LongTapActivated();
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!CheckMultiTouch())
        {
            if (CurrentTapState == TapState.BrushMode)
            {
                NewWorkbookManager.Instance.CameraManager.SwitchLoupeOnOff(false);
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
                if (!NewWorkbookManager.Instance.CameraManager.CheckMinZoom(Input.mousePosition))
                {
                    NewWorkbookManager.Instance.NumberColoring.TryClickPixel(Input.mousePosition);
                }
                break;
            case TapState.Multitouch:
                NewWorkbookManager.Instance.CameraManager.EndZoom();
                break;
            case TapState.BrushMode:
                NewWorkbookManager.Instance.NumberColoring.TryClickPixel(Input.mousePosition);
                WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType = ColorizationModeModel.BrushType.Singular;
                NewWorkbookManager.Instance.CameraManager.SwitchLoupeOnOff(false);
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
                NewWorkbookManager.Instance.CameraManager.Move((Vector2)Input.mousePosition - m_mousePrev);
                m_mousePrev = Input.mousePosition;
                break;
            case TapState.Multitouch:
                {
                    var touches = Input.touches;
                    if (touches.Length >= 2)
                    {
                        Vector2 pinchOffset = touches[0].position - touches[1].position;
                        float value2 = Mathf.Sqrt(pinchOffset.x * pinchOffset.x + pinchOffset.y * pinchOffset.y);
                        float koef = value2 / m_touchesDelta;
                        NewWorkbookManager.Instance.CameraManager.ChangeZoom(koef);
                        if (!m_currentZoomLogged)
                        {
                            m_currentZoomLogged = true;
                            AnalyticsManager.Instance.ZoomDone();
                        }
                        m_touchesDelta = value2;
                        NewWorkbookManager.Instance.CameraManager.Move((Vector2)Input.mousePosition - m_mousePrev);
                        m_mousePrev = Input.mousePosition;
                    }
                    break;
                }
            case TapState.BrushMode:
                {
                    Vector2 dir = data.position - data.delta;
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
                    Vector2 a2 = dir;
                    for (int i = 1; (float)i < steps; i++)
                    {
                        NewWorkbookManager.Instance.NumberColoring.TryClickPixel(a2 + (float)i * a);
                    }
                    if (!NewWorkbookManager.Instance.CameraManager.CheckMinZoom(Input.mousePosition))
                    {
                        NewWorkbookManager.Instance.NumberColoring.TryClickPixel(data.position);
                    }
                    NewWorkbookManager.Instance.CameraManager.UpdateLoupe(data.position);
                    break;
                }
        }
    }

    public void OnScroll(PointerEventData data)
    {
        var cameraManager = NewWorkbookManager.Instance.CameraManager;
        Vector2 scrollDelta = data.scrollDelta;
        cameraManager.Scroll(scrollDelta.y);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (CurrentTapState == TapState.BrushMode)
            {
                CurrentTapState = TapState.None;
                NewWorkbookManager.Instance.CameraManager.SwitchLoupeOnOff(false);
                WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType = ColorizationModeModel.BrushType.Singular;
            }
            m_downTime = DateTime.MinValue;
        }
    }
}



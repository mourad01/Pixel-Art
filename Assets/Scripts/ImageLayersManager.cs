using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageLayersManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
    public enum TapState
    {
        None,
        Down,
        Drag,
        LongTap,
        Multitouch,
        BrushMode
    }

    [SerializeField] Canvas m_canvas;
    [SerializeField] Loupe m_loupe;

    float m_longTapTime = 0.7f;
    Vector3 m_tapPosition = Vector3.zero;
    Color m_longTapPipetColor = Color.white;
    DateTime m_downTime = default(DateTime);
    Camera m_mainCamera;
    List<Vector2> m_clickedPixels;
    Guid m_clickId;
    bool m_brushDrag;

    public static ImageLayersManager Instance { get; private set; }

    public TapState CurrentTapState { get; private set; }

    public bool Hovered { get; private set; }

    public bool Drag
    {
        get
        {
            return CurrentTapState == TapState.Drag || CurrentTapState == TapState.Multitouch;
        }
    }

    void Start()
    {
        Instance = this;
        m_mainCamera = Camera.main;
        m_clickedPixels = new List<Vector2>();
    }

    void Update()
    {
        switch (CurrentTapState)
        {
            case TapState.LongTap:
                break;
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
                if (!CheckMultiTouch() && !m_brushDrag)
                {
                    CheckLongTap();
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
            CurrentTapState = TapState.Multitouch;
            return true;
        }
        return false;
    }

    void CheckLongTap()
    {
        bool tapped = false;
        if ((DateTime.Now - m_downTime).TotalSeconds > (double)m_longTapTime)
        {
            if (WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType != 0 && m_brushDrag)
            {
                return;
            }
            WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType = ColorizationModeModel.BrushType.Plural;
            tapped = true;
            CurrentTapState = TapState.BrushMode;
            m_loupe.SwitchOnOff(true);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!CheckMultiTouch())
        {
            m_clickedPixels.Clear();
            m_clickId = Guid.NewGuid();
            CurrentTapState = TapState.Down;
            m_downTime = DateTime.Now;
            m_tapPosition = Input.mousePosition;
            if (WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType == ColorizationModeModel.BrushType.Plural)
            {
                CurrentTapState = TapState.BrushMode;
                m_brushDrag = false;
            }
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        switch (CurrentTapState)
        {
            case TapState.Down:
                ClickLayer();
                break;
            case TapState.LongTap:
                WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType = ColorizationModeModel.BrushType.Singular;
                break;
            case TapState.BrushMode:
                ClickLayer();
                WorkbookModel.Instance.ColorizationModeModel.CurrentSpaceType = ColorizationModeModel.BrushType.Singular;
                m_loupe.SwitchOnOff(false);
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
            case TapState.Drag:
                break;
            case TapState.LongTap:
                break;
            case TapState.Multitouch:
                break;
            case TapState.Down:
                CurrentTapState = TapState.Drag;
                break;
            case TapState.BrushMode:
                {
                    m_brushDrag = true;
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
                        ClickLayer(a2 + (float)i * a);
                    }
                    ClickLayer(data.position);
                    break;
                }
        }
    }

    void ClickLayer()
    {
        ClickLayer(Input.mousePosition);
    }

    void ClickLayer(Vector2 point)
    {
        var ray = m_mainCamera.ScreenPointToRay(point);
        Vector2 zero = Vector2.zero;
        if (CheckLayers(ray, out zero))
        {
            WorkbookModel.Instance.TutorialModel.ImageClick();
            m_clickedPixels.Add(zero);
        }
    }

    public bool CheckLayers(Ray ray, out Vector2 pixelPos)
    {
        var raycastHits = Physics.RaycastAll(ray);
        if (raycastHits != null && raycastHits.Length > 0)
        {
            var mainTexture = raycastHits[0].collider.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            Vector3 point = raycastHits[0].point;
            float x = point.x;
            Vector3 lossyScale = raycastHits[0].collider.transform.lossyScale;
            point.x = x / lossyScale.x;
            float y = point.y;
            Vector3 lossyScale2 = raycastHits[0].collider.transform.lossyScale;
            point.y = y / lossyScale2.y;
            point.x = (point.x + 1f) / 2f * (float)mainTexture.width;
            point.y = (point.y + 1f) / 2f * (float)mainTexture.height;
            pixelPos = point;
            return true;
        }
        pixelPos = Vector2.zero;
        return false;
    }

    bool GetWorldPos(Ray ray, out Vector2 worldPos)
    {
        var raycastHits = Physics.RaycastAll(ray);
        if (raycastHits != null && raycastHits.Length > 0)
        {
            var mainTexture = raycastHits[0].collider.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            worldPos = raycastHits[0].point;
            return true;
        }
        worldPos = Vector2.zero;
        return false;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        Hovered = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        Hovered = false;
    }
}



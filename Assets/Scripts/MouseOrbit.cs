


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOrbit : MonoBehaviour
{
    enum Directional
    {
        up,
        down,
        right,
        left
    }

    enum SwipeType
    {
        undefined,
        zoom,
        move
    }

    public Transform target;
    public float distance = 20f;
    public float xSpeedRotate = 40f;
    public float ySpeedRotate = 40f;
    public float xSpeedMove = 1f;
    public float ySpeedMove = 1f;
    public float ySpeedZoom = 50f;
    public int speedToZeroMove = 2;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    public float distanceMinOrto = 15f;
    public float distanceMaxOrto = 30f;
    public float distanceMinPersp = 110f;
    public float distanceMaxPersp = 150f;
    public float scrollSpeedOrto = 0.5f;
    public float scrollSpeedPerspect = 2f;
    public float intensTouchForMoveAndZoom = 5f;

    bool isRotate;
    bool isZoom;
    bool isMove;

    public Camera selectedCamera;
    public float MINSCALE = 2f;
    public float MAXSCALE = 5f;
    public float minPinchSpeed = 5f;
    public float varianceInDistances = 5f;

    float touchDelta;
    Vector2 prevDist = new Vector2(0f, 0f);
    Vector2 curDist = new Vector2(0f, 0f);
    float speedTouch0;
    float speedTouch1;
    Vector2 startPos1 = Vector2.zero;
    Vector2 startPos2 = Vector2.zero;
    Vector2 movePos1 = Vector2.zero;
    Vector2 movePos2 = Vector2.zero;
    string text2 = string.Empty;
    Rigidbody rigidbody;

    public Vector3 position;

    Vector3 offset;
    float x;
    float y;
    Vector2 direction1;
    Vector2 direction2;
    List<Directional> touchDirectional;
    float startClickTime;
    float endClickTime;
    bool isClick;

    void Start()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        touchDirectional = new List<Directional>();
        rigidbody = base.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
        distance = distanceMaxOrto;
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        Vector3 point = new Vector3(0f, 0f, 0f - distance);
        Vector3 pos = rotation * point + target.position;
    }

    public void UpdateAngles()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        x = eulerAngles.y;
        y = eulerAngles.x;
        if (y > yMaxLimit)
        {
            y -= 360f;
        }
        else if (x < yMinLimit)
        {
            y += 360f;
        }
    }

    void Update()
    {
        if (EventSystem.current != null)
        {
            if (EventSystem.current.IsPointerOverGameObject(0))
            {
                return;
            }
            if (EventSystem.current.IsPointerOverGameObject(1))
            {
                return;
            }
        }
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startClickTime = Time.time;
                isClick = true;
                text2 = "Touch start ";
            }
            if (isClick && Time.time - startClickTime > 1f)
            {
                UnitySingleton<GameController>.Instance.IsLongClick = true;
                text2 = "isLongClick ";
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved && !UnitySingleton<GameController>.Instance.IsLongClick)
            {
                isRotate = true;
                isClick = false;
                UnitySingleton<GameController>.Instance.IsLongClick = false;
                text2 = "Touch move ";
            }
        }
        else
        {
            isRotate = false;
            isClick = false;
            UnitySingleton<GameController>.Instance.IsLongClick = false;
            text2 = "Touch stop ";
        }
        if (!UnitySingleton<GameController>.Instance.IsLongClick)
        {
            if (target && isRotate && !isZoom && !isMove)
            {
                x += Input.GetAxis("Mouse X") * xSpeedRotate * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeedRotate * distance * 0.02f;
                if (Math.Abs(Input.GetAxis("Mouse X")) > 20f && Math.Abs(Input.GetAxis("Mouse Y")) > 20f)
                {
                    isClick = false;
                    UnitySingleton<GameController>.Instance.IsLongClick = false;
                    text2 = "Touch move " + x + y;
                }
                y = ClampAngle(y, yMinLimit, yMaxLimit);
                Quaternion rotation = Quaternion.Euler(y, x, 0f);
                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5f, distanceMinOrto, distanceMaxOrto);
                Vector3 camOffset = new Vector3(0f, 0f, 0f - distance);
                transform.rotation = rotation;
                transform.position = Vector3.Lerp(transform.position, Vector3.zero, Time.deltaTime * (float)speedToZeroMove);
            }
            if (Input.touchCount == 2)
            {
                curDist = Input.GetTouch(0).position - Input.GetTouch(1).position;
                prevDist = Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);
                direction1 = Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition;
                direction2 = Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition;
                touchDelta = curDist.magnitude - prevDist.magnitude;
                speedTouch0 = Input.GetTouch(0).deltaPosition.magnitude / Input.GetTouch(0).deltaTime;
                speedTouch1 = Input.GetTouch(1).deltaPosition.magnitude / Input.GetTouch(1).deltaTime;
                float koef = touchDelta / ySpeedZoom;
                if (!isZoom && !isMove)
                {
                    var touches = Input.touches;
                    if (touches[0].phase == TouchPhase.Began && touches[1].phase == TouchPhase.Began)
                    {
                        startPos1 = touches[0].position;
                        startPos2 = touches[1].position;
                    }
                    if (touches[0].phase == TouchPhase.Moved && touches[1].phase == TouchPhase.Moved && (startPos1 - touches[0].position).magnitude > 10f && (startPos2 - touches[1].position).magnitude > 10f)
                    {
                        movePos1 = touches[0].position;
                        movePos2 = touches[1].position;
                        if (movePos1.y > startPos1.y)
                        {
                            touchDirectional.Add(Directional.right);
                        }
                        else if (movePos1.y < startPos1.y)
                        {
                            touchDirectional.Add(Directional.left);
                        }
                        if (movePos1.x > startPos1.x)
                        {
                            touchDirectional.Add(Directional.up);
                        }
                        else if (movePos1.x < startPos1.x)
                        {
                            touchDirectional.Add(Directional.down);
                        }
                        if (movePos2.y > startPos2.y)
                        {
                            touchDirectional.Add(Directional.right);
                        }
                        else if (movePos2.y < startPos2.y)
                        {
                            touchDirectional.Add(Directional.left);
                        }
                        if (movePos2.x > startPos2.x)
                        {
                            touchDirectional.Add(Directional.up);
                        }
                        else if (movePos2.x < startPos2.x)
                        {
                            touchDirectional.Add(Directional.down);
                        }
                        ZommOrMove(koef);
                    }
                }
                else
                {
                    ZommOrMove(koef);
                }
            }
            else if (Input.touchCount == 0)
            {
                isZoom = false;
                isMove = false;
                isMove = false;
                touchDirectional = new List<Directional>();
            }
            if (Input.GetKeyUp(KeyCode.B))
            {
                if (selectedCamera.orthographic)
                {
                    selectedCamera.orthographic = false;
                }
                else
                {
                    selectedCamera.orthographic = true;
                }
            }
        }
    }

    void OnGUI()
    {
        if (VoxConstants.isTest)
        {
            int width = Screen.width;
            int height = Screen.height;
            var gUIStyle = new GUIStyle();
            Rect rect = new Rect(0f, 0f, (float)width, (float)(height * 2 / 100));
            Rect labelRect = new Rect(0f, 200f, (float)width, (float)(height * 2 / 100));
            gUIStyle.alignment = TextAnchor.UpperLeft;
            gUIStyle.fontSize = height * 2 / 100;
            gUIStyle.normal.textColor = new Color(0f, 0f, 0.5f, 1f);
            string text = touchDelta.ToString();
            if (touchDirectional != null)
            {
                foreach (Directional directional in touchDirectional)
                {
                    text2 = text2 + directional.ToString() + " ";
                }
            }
            GUI.Label(rect, text, gUIStyle);
            GUI.Label(labelRect, text2, gUIStyle);
        }
    }

    public static float ZoomLimit(float dist, float min, float max)
    {
        if (dist < min)
        {
            dist = min;
        }
        if (dist > max)
        {
            dist = max;
        }
        return dist;
    }

    public static float ZoomCheck(float dist)
    {
        if (dist < 0f)
        {
            dist = 0f;
        }
        if (dist >= 1f)
        {
            dist = 1f;
        }
        return dist;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
        {
            angle += 360f;
        }
        if (angle > 360f)
        {
            angle -= 360f;
        }
        return Mathf.Clamp(angle, min, max);
    }

    void ZommOrMove(float koef)
    {
        if (CheckDirectional() == SwipeType.zoom)
        {
            isZoom = true;
            if (touchDelta + varianceInDistances <= 1f && speedTouch0 > minPinchSpeed && speedTouch1 > minPinchSpeed)
            {
                ZoomOut(koef);
            }
            if (touchDelta + varianceInDistances > 1f && speedTouch0 > minPinchSpeed && speedTouch1 > minPinchSpeed)
            {
                ZoomIn(koef);
            }
        }
        if (CheckDirectional() == SwipeType.move)
        {
            isMove = true;
            Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
            Vector2 secondDelta = Input.GetTouch(1).deltaPosition;
            transform.Translate((0f - deltaPosition.x) * xSpeedMove * distance * 0.02f, (0f - deltaPosition.y) * xSpeedMove * distance * 0.02f, 0f);
        }
    }

    void ZoomOut(float koef = 1f)
    {
        float zoom = 1f;
        if (selectedCamera.fieldOfView <= distanceMaxPersp)
        {
            selectedCamera.fieldOfView += scrollSpeedPerspect;
        }
        if (selectedCamera.orthographicSize <= distanceMaxOrto)
        {
            selectedCamera.orthographicSize -= scrollSpeedOrto * koef;
        }
        zoom = ((!selectedCamera.orthographic) ? (1f - (selectedCamera.fieldOfView - distanceMinPersp) / (distanceMaxPersp - distanceMinPersp)) : (1f - (selectedCamera.orthographicSize - distanceMinOrto) / (distanceMaxOrto - distanceMinOrto)));
        transform.position = Vector3.Lerp(transform.position, Vector3.zero, Time.deltaTime * (float)speedToZeroMove * 5f);
        UnitySingleton<GameController>.Instance.ChangeCamDepthPosition(ZoomCheck(zoom));
    }

    void ZoomIn(float koef = 1f)
    {
        float zoom = 1f;
        if (selectedCamera.fieldOfView > distanceMinPersp)
        {
            selectedCamera.fieldOfView -= scrollSpeedPerspect;
        }
        if (selectedCamera.orthographicSize >= distanceMinOrto)
        {
            selectedCamera.orthographicSize -= scrollSpeedOrto * koef;
        }
        zoom = ((!selectedCamera.orthographic) ? (1f - (selectedCamera.fieldOfView - distanceMinPersp) / (distanceMaxPersp - distanceMinPersp)) : (1f - (selectedCamera.orthographicSize - distanceMinOrto) / (distanceMaxOrto - distanceMinOrto)));
        UnitySingleton<GameController>.Instance.ChangeCamDepthPosition(ZoomCheck(zoom));
    }

    SwipeType CheckDirectional()
    {
        if (touchDirectional.Count == 4)
        {
            if (touchDirectional[0] != touchDirectional[2] && touchDirectional[1] != touchDirectional[3])
            {
                return SwipeType.zoom;
            }
            return SwipeType.move;
        }
        return SwipeType.undefined;
    }
}



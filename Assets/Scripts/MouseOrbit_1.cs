


using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOrbit_1 : MonoBehaviour
{
    public Transform target;
    public float distance = 20f;
    public float xSpeed = 120f;
    public float ySpeed = 120f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    public float distanceMinOrto = 15f;
    public float distanceMaxOrto = 30f;
    public float distanceMinPersp = 110f;
    public float distanceMaxPersp = 150f;
    public float scrollSpeedOrto = 0.5f;
    public float scrollSpeedPerspect = 2f;

    bool isCanRotate;
    bool isCanZoom;

    public int speed = 4;
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
    Rigidbody rigidbody;

    public Vector3 position;

    float x;
    float y;

    void Start()
    {
        rigidbody = base.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
        distance = distanceMaxOrto;
        Vector3 point = new Vector3(0f, 0f, 0f - distance);
        Vector3 pos = transform.rotation * point + target.position;
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
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(0))
        {
            return;
        }
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            isCanRotate = true;
        }
        else
        {
            isCanRotate = false;
        }
        if (target && isCanRotate && !isCanZoom)
        {
            float axis = Input.GetAxis("Mouse X");
            float axisY = Input.GetAxis("Mouse Y");
            x += axis * xSpeed * distance * 0.02f;
            y -= axisY * ySpeed * distance * 0.02f;
            y = ClampAngle(y, yMinLimit, yMaxLimit);
            Quaternion rotation = Quaternion.Euler(y, x, 0f);
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5f, distanceMinOrto, distanceMaxOrto);
            Vector3 point = new Vector3(0f, 0f, 0f - distance);
            Vector3 pos = rotation * point + target.position;
            transform.rotation = rotation;
            transform.position = pos;
        }
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            isCanZoom = true;
            curDist = Input.GetTouch(0).position - Input.GetTouch(1).position;
            prevDist = Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);
            touchDelta = curDist.magnitude - prevDist.magnitude;
            speedTouch0 = Input.GetTouch(0).deltaPosition.magnitude / Input.GetTouch(0).deltaTime;
            speedTouch1 = Input.GetTouch(1).deltaPosition.magnitude / Input.GetTouch(1).deltaTime;
            float koef = touchDelta / 10f;
            if (touchDelta + varianceInDistances <= 1f && speedTouch0 > minPinchSpeed && speedTouch1 > minPinchSpeed)
            {
                ZoomIn(koef);
            }
            if (touchDelta + varianceInDistances > 1f && speedTouch0 > minPinchSpeed && speedTouch1 > minPinchSpeed)
            {
                ZoomOut(koef);
            }
        }
        else if (Input.touchCount == 0)
        {
            isCanZoom = false;
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

    void ZoomIn(float koef = 1f)
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
        UnitySingleton<GameController>.Instance.ChangeCamDepthPosition(ZoomCheck(zoom));
    }

    void ZoomOut(float koef = 1f)
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
}



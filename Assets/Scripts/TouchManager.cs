


using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    RaycastHit hit;
    Camera cam;
    Vector3 startPosition;
    bool isClick;
    string nameHint;

    void Awake()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update1()
    {
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && !isClick)
        {
            isClick = true;
            startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        bool hasMouseUp = true;
        if (Input.GetMouseButtonUp(0))
        {
            hasMouseUp = false;
            isClick = false;
            Vector3 b = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!(Math.Abs(Vector3.Distance(startPosition, b)) > 5f))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Clicked on the UI");
                }
                else if (hit.collider.tag == VoxConstants.Tag)
                {
                    var voxCubeItem = ((Component)hit.transform).GetComponent<VoxCubeItem>();
                    if (voxCubeItem != null)
                    {
                        voxCubeItem.TochCubeItem();
                    }
                }
                hasMouseUp = true;
            }
            return;
        }
        if (hasMouseUp)
        {
            if (UnitySingleton<GameController>.Instance.IsLongClick)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Clicked on the UI");
                }
                else if (hit.collider.tag == VoxConstants.Tag && !hit.collider.name.Equals(nameHint))
                {
                    nameHint = hit.collider.name;
                    var touchedCube = ((Component)hit.transform).GetComponent<VoxCubeItem>();
                    if (touchedCube != null)
                    {
                        touchedCube.TochCubeItem();
                    }
                }
            }
        }
    }

    public void CheckCube(Vector2 touchPos, bool isVibro = true)
    {
        if (Physics.Raycast(cam.ScreenPointToRay(touchPos), out hit) && hit.collider.tag == VoxConstants.Tag)
        {
            var voxCubeItem = ((Component)hit.transform).GetComponent<VoxCubeItem>();
            if (voxCubeItem != null)
            {
                int result = voxCubeItem.TochCubeItem();
                if (result >= 0)
                {
                    if (result > 0)
                    {
                        UnitySingleton<GameController>.Instance.OnCubeColored(voxCubeItem.ColorIndex);
                        UnitySingleton<GameController>.Instance.SetCubeItemProgress(voxCubeItem);
                    }
                    if (isVibro)
                    {
                        if (result > 0)
                        {
                            VibroWrapper.PlayVibroRight();
                        }
                        else
                        {
                            VibroWrapper.PlayVibroWrong();
                            DailyGame.IncresetError();
                        }
                    }
                }
            }
        }
    }
}



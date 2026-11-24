


using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    private RaycastHit hit;

    private Camera cam;

    private Vector3 startPosition;

    private bool isClick;

    private string nameHint;

    private void Awake()
    {
        this.cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void Update1()
    {
        if (!Physics.Raycast(this.cam.ScreenPointToRay(Input.mousePosition), out this.hit))
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && !this.isClick)
        {
            this.isClick = true;
            this.startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        bool hasMouseUp = true;
        if (Input.GetMouseButtonUp(0))
        {
            hasMouseUp = false;
            this.isClick = false;
            Vector3 b = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!(Math.Abs(Vector3.Distance(this.startPosition, b)) > 5f))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Clicked on the UI");
                }
                else if (this.hit.collider.tag == VoxConstants.Tag)
                {
                    VoxCubeItem component = ((Component)this.hit.transform).GetComponent<VoxCubeItem>();
                    if (component != null)
                    {
                        component.TochCubeItem();
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
                Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Clicked on the UI");
                }
                else if (this.hit.collider.tag == VoxConstants.Tag && !this.hit.collider.name.Equals(this.nameHint))
                {
                    this.nameHint = this.hit.collider.name;
                    VoxCubeItem touchedCube = ((Component)this.hit.transform).GetComponent<VoxCubeItem>();
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
        if (Physics.Raycast(this.cam.ScreenPointToRay(touchPos), out this.hit) && this.hit.collider.tag == VoxConstants.Tag)
        {
            VoxCubeItem component = ((Component)this.hit.transform).GetComponent<VoxCubeItem>();
            if (component != null)
            {
                int num = component.TochCubeItem();
                if (num >= 0)
                {
                    if (num > 0)
                    {
                        UnitySingleton<GameController>.Instance.OnCubeColored(component.ColorIndex);
                        UnitySingleton<GameController>.Instance.SetCubeItemProgress(component);
                    }
                    if (isVibro)
                    {
                        if (num > 0)
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



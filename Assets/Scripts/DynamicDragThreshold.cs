using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem))]
internal class DynamicDragThreshold : MonoBehaviour
{
    void Awake()
    {
        float density = ScreenToolWrapper.Density;
        var eventSystem = base.GetComponent<EventSystem>();
        eventSystem.pixelDragThreshold = (int)((float)eventSystem.pixelDragThreshold * density);
    }
}



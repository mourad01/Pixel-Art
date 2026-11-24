using System.Collections.Generic;
using UnityEngine;

internal class MultiTouchModule : MonoBehaviour
{
    void Update()
    {
        var list = new List<Touch>();
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            list.Add(touch);
        }
        var list2 = new List<Touch>();
        foreach (Touch touch in list)
        {
            if (touch.fingerId < 2)
            {
                list2.Add(touch);
            }
        }
        list2.Sort((Touch a, Touch b) => a.fingerId.CompareTo(b.fingerId));
        if (list2.Count != 0 && list2.Count > 1)
        {
            Vector2 dir = list2[0].position - list2[0].deltaPosition - (list2[1].position - list2[1].deltaPosition);
            float distance = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
            Vector2 currentTouchOffset = list2[0].position - list2[1].position;
            float value2 = Mathf.Sqrt(currentTouchOffset.x * currentTouchOffset.x + currentTouchOffset.y * currentTouchOffset.y);
        }
    }
}



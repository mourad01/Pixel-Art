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
        var touches = new List<Touch>();
        foreach (Touch touch in list)
        {
            if (touch.fingerId < 2)
            {
                touches.Add(touch);
            }
        }
        touches.Sort((Touch a, Touch b) => a.fingerId.CompareTo(b.fingerId));
        if (touches.Count != 0 && touches.Count > 1)
        {
            Vector2 dir = touches[0].position - touches[0].deltaPosition - (touches[1].position - touches[1].deltaPosition);
            float distance = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);
            Vector2 currentTouchOffset = touches[0].position - touches[1].position;
            float touchDistance = Mathf.Sqrt(currentTouchOffset.x * currentTouchOffset.x + currentTouchOffset.y * currentTouchOffset.y);
        }
    }
}



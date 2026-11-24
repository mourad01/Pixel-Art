using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter))]
public class ScreenFormatAspectRatioFitter : MonoBehaviour
{
    [SerializeField] float m_aspectRation4_3 = 100f;

    void Awake()
    {
        bool found = false;
        if (Screen.width > Screen.height)
        {
            if ((float)Screen.width / (float)Screen.height < 1.45f)
            {
                found = true;
            }
        }
        else if ((float)Screen.height / (float)Screen.width < 1.45f)
        {
            found = true;
        }
        if (found)
        {
            base.GetComponent<AspectRatioFitter>().aspectRatio = m_aspectRation4_3;
        }
    }
}



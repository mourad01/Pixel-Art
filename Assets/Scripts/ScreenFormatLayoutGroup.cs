using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
internal class ScreenFormatLayoutGroup : MonoBehaviour
{
    [SerializeField] float m_spacing4_3;

    void Awake()
    {
        bool found = false;
        if (Screen.width > Screen.height)
        {
            if ((float)Screen.width / (float)Screen.height < 1.55f)
            {
                found = true;
            }
        }
        else if ((float)Screen.height / (float)Screen.width < 1.55f)
        {
            found = true;
        }
        if (found)
        {
            base.GetComponent<HorizontalOrVerticalLayoutGroup>().spacing = m_spacing4_3;
        }
    }
}



using UnityEngine;
using UnityEngine.UI;

public class SetPrefferedHeightByChild : MonoBehaviour
{
    float m_size;

    [SerializeField] RectTransform m_childRectTransform;

    void Update()
    {
        if (m_childRectTransform.rect.height != m_size)
        {
            m_size = m_childRectTransform.rect.height;
            base.GetComponent<LayoutElement>().preferredHeight = m_size;
        }
    }
}



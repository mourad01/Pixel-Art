using UnityEngine;

public class CustomVerticalLayoutGroupElement : MonoBehaviour
{
    [SerializeField] int m_siblingIndex;

    public int SiblingIndex
    {
        get
        {
            return m_siblingIndex;
        }
    }

    void Awake()
    {
        transform.SetAsLastSibling();
    }
}



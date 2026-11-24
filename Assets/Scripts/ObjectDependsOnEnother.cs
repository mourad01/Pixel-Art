using UnityEngine;

public class ObjectDependsOnEnother : MonoBehaviour
{
    bool prevValue = true;

    [SerializeField] GameObject m_content;
    [SerializeField] MonoBehaviour m_targetObject;

    void Update()
    {
        bool isActiveAndEnabled = m_targetObject.isActiveAndEnabled;
        if (prevValue != isActiveAndEnabled)
        {
            prevValue = isActiveAndEnabled;
            m_content.SetActive(isActiveAndEnabled);
        }
    }
}



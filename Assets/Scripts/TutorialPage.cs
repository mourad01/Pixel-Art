using UnityEngine;

public class TutorialPage : MonoBehaviour
{
    [SerializeField] GameObject m_content;
    [SerializeField] GameObject m_closeButton;

    public void Play()
    {
    }

    public void Stop()
    {
    }

    public void EnableCloseButton()
    {
        m_closeButton.SetActive(true);
    }
}



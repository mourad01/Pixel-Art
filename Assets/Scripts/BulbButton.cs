using UnityEngine;
using UnityEngine.UI;

public class BulbButton : MonoBehaviour
{
    [SerializeField] GameObject m_highlightedGrid;
    [SerializeField] Image m_image;
    [SerializeField] Sprite m_enableSprite;
    [SerializeField] Sprite m_disableSprite;

    void Start()
    {
        UpdateState();
    }

    public void Click()
    {
        AppData.BulbMode = !AppData.BulbMode;
        UpdateState();
    }

    void UpdateState()
    {
        m_image.sprite = ((!AppData.BulbMode) ? m_disableSprite : m_enableSprite);
        m_highlightedGrid.SetActive(AppData.BulbMode);
    }
}



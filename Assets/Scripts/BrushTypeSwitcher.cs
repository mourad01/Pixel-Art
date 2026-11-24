using System;
using System.Collections;
using UnityEngine;

public class BrushTypeSwitcher : MonoBehaviour
{
    [SerializeField] RectTransform m_content;

    [SerializeField] GameObject m_singualarImage;

    [SerializeField] GameObject m_pluralImage;

    private void Start()
    {
        ColorizationModeModel colorizationModeModel = WorkbookModel.Instance.ColorizationModeModel;
        colorizationModeModel.OnSpaceTypeChanged = (Action<ColorizationModeModel>)Delegate.Combine(colorizationModeModel.OnSpaceTypeChanged, new Action<ColorizationModeModel>(OnStateChangedHandler));
    }

    private void OnStateChangedHandler(ColorizationModeModel model)
    {
        base.StopAllCoroutines();
        base.StartCoroutine(ChangeSpriteCoroutine(model));
    }

    private IEnumerator ChangeSpriteCoroutine(ColorizationModeModel model)
    {
        Vector2 openedPos = Vector2.zero;
        Vector2 sizeDelta = ((RectTransform)base.transform).sizeDelta;
        Vector2 closedPos = new Vector2(0f, sizeDelta.y);
        float time = 0.1f;
        Vector2 speed = closedPos / time;
        yield return null;

        var deltaTime = Mathf.Min(0.05f, Time.deltaTime);
        var delta = speed * deltaTime;
        Vector2 closingPosition = m_content.anchoredPosition + delta;
        if (closingPosition.y > closedPos.y)
        {
            m_content.anchoredPosition = closedPos;
            switch (model.CurrentSpaceType)
            {
                case ColorizationModeModel.BrushType.Singular:
                    m_singualarImage.SetActive(true);
                    m_pluralImage.SetActive(false);
                    break;
                case ColorizationModeModel.BrushType.Plural:
                    m_singualarImage.SetActive(false);
                    m_pluralImage.SetActive(true);
                    break;
            }
            yield return null;

            deltaTime = Mathf.Min(0.05f, Time.deltaTime);
            delta = -speed * deltaTime;
            Vector2 openingPosition = m_content.anchoredPosition + delta;
            if (openingPosition.y < openedPos.y)
            {
                m_content.anchoredPosition = openedPos;
                yield break;
            }
            RectTransform content = m_content;
            content.anchoredPosition += delta;
            yield return null;
        }
        RectTransform movingContent = m_content;
        movingContent.anchoredPosition += delta;
        yield return null;
    }

    public void Click()
    {
        WorkbookModel.Instance.ColorizationModeModel.ChangeBrushType();
    }
}

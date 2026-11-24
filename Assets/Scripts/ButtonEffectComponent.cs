

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class ButtonEffectComponent : MonoBehaviour
{
    Color m_tempColor;

    [SerializeField] List<ButtonEffect> m_elements;

    public void ButtonDownHandler()
    {
        if (enabled)
        {
            foreach (ButtonEffect element in m_elements)
            {
                if (element.element != null)
                {
                    switch (element.buttonEffectType)
                    {
                        case ButtonEffectType.Color:
                            element.element.color = element.activeColor;
                            break;
                        case ButtonEffectType.HighlightColor:
                            m_tempColor = element.element.color;
                            element.element.color = element.activeColor;
                            break;
                        case ButtonEffectType.Sprite:
                            ((Image)element.element).sprite = element.activeSprite;
                            break;
                        case ButtonEffectType.OnOff:
                            element.element.gameObject.SetActive(false);
                            break;
                        case ButtonEffectType.OffOn:
                            element.element.gameObject.SetActive(true);
                            break;
                    }
                }
            }
        }
    }

    public void ButtonUpHandler()
    {
        if (enabled)
        {
            foreach (ButtonEffect element in m_elements)
            {
                if (element.element != null)
                {
                    switch (element.buttonEffectType)
                    {
                        case ButtonEffectType.Color:
                            element.element.color = element.defaultColor;
                            break;
                        case ButtonEffectType.HighlightColor:
                            element.element.color = m_tempColor;
                            break;
                        case ButtonEffectType.Sprite:
                            ((Image)element.element).sprite = element.defaultSprite;
                            break;
                        case ButtonEffectType.OnOff:
                            element.element.gameObject.SetActive(true);
                            break;
                        case ButtonEffectType.OffOn:
                            element.element.gameObject.SetActive(false);
                            break;
                    }
                }
            }
        }
    }
}



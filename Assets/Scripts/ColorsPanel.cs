using UnityObject = global::UnityEngine.Object;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ColorsPanel : MonoBehaviour
{
    public SimpleObjectPooler[] poolers;
    public Action<int, Color> OnSelectColor;

    List<Image> m_points;

    [SerializeField] LayoutElement m_layoutElement;
    [SerializeField] ColorsPage m_colorsPagePrefab;
    [SerializeField] Transform m_colorsPagesParent;
    [SerializeField] PositScrollRect m_positScrollRect;
    [SerializeField] RectTransform m_specPage;
    [SerializeField] RectTransform m_specPageInternal;
    [SerializeField] List<ColorsPage> m_colorsPages;
    [SerializeField] ColorPointer m_colorPointer;
    [SerializeField] Image m_pointPrefab;
    [SerializeField] RectTransform m_pointsParent;
    [SerializeField] float m_pointsDelta = 20f;

    public GameObject restColorPanel;
    public Image restColorIndicator;
    public Text restLabel;
    public Animator _resetTextAni;

    bool isWorking3D = false;
    bool colorButtonFirstClicked = true;
    void Start()
    {
        isWorking3D = false;
        if (SceneManager.GetActiveScene().name == "3DScene")
        {
            isWorking3D = true;
        }
        BackgroundMusic.PlayInGameBackground();
    }
    public void Init(List<Color> colors)
    {
        restColorPanel.SetActive(false);

        Vector2 sizeDelta = ((RectTransform)transform).sizeDelta;
        float panelHeight = 0f - ((RectTransform)transform).anchoredPosition.y + sizeDelta.y;
        int count = 0;
        count = 10;
        while (!(sizeDelta.x / (float)count / panelHeight * 2f >= 0.25f))
        {
            count--;
        }
        count++;
        int rowWidth = Mathf.Max(count, 5);
        float colorSize = sizeDelta.x / (float)rowWidth;
        int perPage = rowWidth * 2;
        int pageCount = Mathf.CeilToInt((float)colors.Count / (float)perPage);
        for (count = 0; count < pageCount; count++)
        {
            var range = colors.GetRange(count * perPage, Math.Min(perPage, colors.Count - count * perPage));
            var colorsPage = UnityObject.Instantiate(m_colorsPagePrefab);
            colorsPage.transform.SetParent(m_colorsPagesParent);
            colorsPage.transform.localScale = Vector3.one;
            colorsPage.gameObject.SetActive(true);
            m_colorsPages.Add(colorsPage);
            colorsPage.Init(range, count * perPage, rowWidth, colorSize, m_colorsPages.Count - 1);
            colorsPage.OnColorClick += OnColorClickedHandler;
        }
        float y = m_pointsParent.sizeDelta.y;
        if (colors.Count <= rowWidth)
        {
            var layoutElement = m_layoutElement;
            float height = colorSize + y;
            m_layoutElement.preferredHeight = height;
            layoutElement.minHeight = height;
        }
        else
        {
            var layoutElement = m_layoutElement;
            float height = colorSize * 2f + y;
            m_layoutElement.preferredHeight = height;
            layoutElement.minHeight = height;
        }
        m_specPage.sizeDelta = new Vector2(m_specPage.rect.width, m_layoutElement.minHeight);
        m_specPageInternal.sizeDelta = new Vector2(0f, colorSize * 2f);
        m_specPageInternal.anchoredPosition = new Vector2(0f, y);
        m_positScrollRect.Reinit(-1f, false);
        if (m_colorsPages.Count > 0)
        {
            m_colorsPages[0].SelectFirstColor();
            colorButtonFirstClicked = true;
            GeneratePoints();
            m_positScrollRect.OnSelectIndex += OnIndexChangedHandler;
            m_positScrollRect.SetIndex(m_specPage.gameObject.activeSelf ? 1 : 0);
        }
        else
        {
            m_colorPointer.gameObject.SetActive(false);
        }
        if (SceneManager.GetActiveScene().name == "2DScene")
        {
            var numberColoring = NewWorkbookManager.Instance.NumberColoring;
            numberColoring.OnColorComplete += OnColorCompleteHandler;
            numberColoring.OnCountLeft += OnColorCount;
            numberColoring.OnPaintedPixel += OnPaintPixel;
        }
        else
        {
            var gameController = UnitySingleton<GameController>.Instance;
            gameController.OnColorComplete += OnColorCompleteHandler;
            gameController.OnCountLeft += OnColorCount;
            gameController.OnPaintedPixel += OnPaintPixel;
            UnitySingleton<GameController>.Instance.UpdateColors();
        }

        if (m_colorsPages.Count > 0)
        {
            m_colorsPages[0].SelectFirstColor();
        }
    }

    void GeneratePoints()
    {
        int pageCount = m_colorsPages.Count + (m_specPage.gameObject.activeSelf ? 1 : 0);
        float startX = ((float)(-pageCount) / 2f + 0.5f) * m_pointsDelta;
        m_points = new List<Image>(pageCount);
        for (int i = 0; i < pageCount; i++)
        {
            var image = UnityObject.Instantiate(m_pointPrefab);
            image.gameObject.SetActive(true);
            image.transform.SetParent(m_pointsParent);
            image.transform.localScale = Vector2.one;
            ((RectTransform)image.transform).anchoredPosition = new Vector2(startX + (float)i * m_pointsDelta, 0f);
            m_points.Add(image);
        }
    }

    void OnColorClickedHandler(ColorImage colorImage, SpecialColorPosition pos)
    {
        restColorPanel.SetActive(true);

        if (SceneManager.GetActiveScene().name == "3DScene")
        {
            UnitySingleton<GameController>.Instance.SetCurrentIndexCollorPallete(colorImage.ColorIndex);
        }
        WorkbookModel.Instance.CurrentColorModel.UpdateColor(colorImage.Color);
        m_colorPointer.UpdateColor(colorImage, pos);

        if (isWorking3D)
        {
            bool stillAvailable = UnitySingleton<GameController>.Instance.CheckColorLeft(colorImage.ColorIndex, !colorButtonFirstClicked);
            restColorPanel.SetActive(stillAvailable);
            if (stillAvailable)
            {
                restColorPanel.GetComponent<Animator>().Play("RestPanelDefaultAni", -1, 0f);
            }
        }
        else
        {
            bool stillAvailable = NewWorkbookManager.Instance.NumberColoring.CheckColorLeft(colorImage.Color, !colorButtonFirstClicked);
            restColorPanel.SetActive(stillAvailable);
            if (stillAvailable)
            {
                restColorPanel.GetComponent<Animator>().Play("RestPanelDefaultAni", -1, 0f);
            }
        }

        restColorIndicator.color = colorImage.Color;

        colorButtonFirstClicked = false;
    }
    System.Collections.IEnumerator DestroyEffectRoutine(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        obj.SetActive(false);
    }
    void PlayParticle(Color color, Vector3 pos, GameObject particle, bool shouldChangeColor = true, bool repeat = false)
    {
        particle.transform.localScale = Vector3.one;
        particle.transform.position = pos;
        particle.SetActive(true);
        if (shouldChangeColor)
        {
            var component = particle.GetComponent<ParticleSystem>();
            var main = component.main;
            main.startColor = color;
            var transform = particle.transform.Find("2-2");
            if (transform != null)
            {
                var particles = (transform).GetComponent<ParticleSystem>();
                if (particles != null)
                {
                    var m = particles.main;
                    m.startColor = color;
                }
            }
        }
        if (!repeat)
        {
            base.StartCoroutine(DestroyEffectRoutine(particle));
        }
    }
    void OnPaintPixel(Color color)
    {
        AudioManager.Instance.PlayColor();

        if (isWorking3D)
        {
            PlayParticle(color, Camera.main.ScreenPointToRay(Input.mousePosition).origin, poolers[1].GetPooledGameObject(), true, false);
        }
        //else
        //{
        //	this.PlayParticle(color, Camera.main.ScreenToViewportPoint(Input.mousePosition), this.poolers[1].GetPooledGameObject(), true, false);
        //}
    }

    void OnColorCount(Color color, int restColorCount)
    {
        if (!restLabel.text.Equals(string.Format("{0}", restColorCount)))
        {
            restLabel.text = string.Format("{0}", restColorCount);
            _resetTextAni.Play("ResetTextChangeOnce", -1, 0f);
        }
        if (restColorCount == 0)
        {
            var a = restColorPanel.GetComponent<Animator>();
            if (a != null)
            {
                a.Play("Disappear", -1, 0f);
            }
        }
        else
        {
            //this.restColorIndicator.color = color;
        }
    }

    void OnColorCompleteHandler(Color color)
    {
        foreach (ColorsPage colorsPage in m_colorsPages)
        {
            colorsPage.DisableColor(color);
        }

        OnColorCount(color, 0);
    }

    void OnIndexChangedHandler(int index)
    {
        if (index == 0 && !AppData.SpecPageOpened)
        {
            AppData.SpecPageOpened = true;
        }
        if (index > 0)
        {
            WorkbookModel.Instance.SpecBoostersModel.BombMode = false;
            WorkbookModel.Instance.SpecBoostersModel.LassoMode = false;
        }
        if (index >= 0 && index < m_points.Count)
        {
            for (int i = 0; i < m_points.Count; i++)
            {
                m_points[i].SetAlpha((i != index) ? 0.7f : 1f);
            }
        }
    }
}



using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FilterWindow : BaseWindow
{
    ISavedWorkData m_savedWorkData;
    List<FilterInfo> m_filters;
    List<FilterElement> m_filterElements;
    int m_currentIndex = -1;
    float m_maxZoom = 1f;
    float m_currentImageZoom = 1f;

    [SerializeField] PositScrollRect m_positScrollRect;
    [SerializeField] RawImage m_image;
    [SerializeField] RawImage m_filterImage;
    [SerializeField] Text m_filterName;
    [SerializeField] FilterElement m_filterElementPrefab;
    [SerializeField] Texture2D m_emptyFilter;
    [SerializeField] FilterWIndowInputReceiver m_inputReceiver;

    //private Color[] m_originalTextureData;

    Vector2 _imageSize;
    protected override string WindowName
    {
        get
        {
            return "filtersWindow";
        }
    }

    protected void Awake()
    {
        m_image.material = new Material(Shader.Find("Custom/GreyTextureShader"));
        m_filterImage.material = new Material(Shader.Find("Custom/TilingShader"));
        m_positScrollRect.OnSelectIndex += OnSelectIndexHandler;
        m_positScrollRect.OnDragSelectIndex += OnSelectIndexHandler;
        m_positScrollRect.OnScaleElement += OnScaleHandler;
        m_filterElements = new List<FilterElement>();
        m_inputReceiver.OnScrolled += OnScrolledImageHandler;
        m_inputReceiver.OnDragged += OnImageDraggedHandler;
        m_inputReceiver.OnZoomed += OnZoomedImageHandler;
    }

    public void Init(ISavedWorkData savedWorkData, float maxZoom, Rect uvRect = default(Rect))
    {
        Rect rect = new Rect(0f, 0f, 1f, 1f);
        if (uvRect == default(Rect) || uvRect == rect)
        {
            uvRect = rect;
        }
        else
        {
            float x = (uvRect.min.x + 1f) / 2f;
            uvRect.center = new Vector2(x, (uvRect.min.y + 1f) / 2f);
        }
        AnalyticsManager.Instance.FilterWindowOpened();
        m_savedWorkData = savedWorkData;
        ReinitImage(uvRect);
        //filter elements are created only the first time
        if (m_filters == null)
        {
            m_filters = MainManager.Instance.FilterManager.GetFilters();
            foreach (FilterInfo filter in m_filters)
            {
                var filterElement = UnityObject.Instantiate(m_filterElementPrefab);
                filterElement.transform.SetParent(m_filterElementPrefab.transform.parent);
                filterElement.transform.localScale = Vector3.one;
                filterElement.gameObject.SetActive(true);
                filterElement.OnClick += OnFilterClickHandler;
                m_filterElements.Add(filterElement);
                if (filter.Texture != null)
                {
                    filterElement.Init(filter);
                }
                else
                {
                    filterElement.InitEmpty(m_emptyFilter);
                }
            }
        }
        m_positScrollRect.Reinit(0f, false);
        m_positScrollRect.SetContentSize();
        OnSelectIndexHandler(m_savedWorkData.FilterId);
        m_positScrollRect.SetIndex(m_savedWorkData.FilterId);
        m_maxZoom = maxZoom;
    }

    void ReinitImage(Rect uvRect)
    {
        var resTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        resTex.filterMode = FilterMode.Point;
        resTex.LoadImage(m_savedWorkData.Preview);
        m_image.material.SetTexture("_ResTex", resTex);
        if (resTex != null)
        {
            _imageSize = new Vector2(resTex.width, resTex.height);
        }
        m_image.enabled = true;
        m_filterImage.enabled = false;
        // library images have an url, user photos are loaded by id
        if (m_savedWorkData.ImageInfo.Url != null)
        {
            DataManager.Instance.GetImageAsset(m_savedWorkData.ImageInfo, delegate (bool res, Texture2D tex)
            {
                //m_originalTextureData = tex.GetPixels().ToArray();

                m_image.texture = tex;
                m_filterImage.material.mainTextureScale = new Vector2((float)resTex.width, (float)resTex.height);
                var filter = MainManager.Instance.FilterManager.GetFilter(m_savedWorkData.FilterId);
                if (filter != null)
                {
                    m_filterImage.texture = filter;
                    m_filterImage.enabled = true;
                    m_filterName.text = filter.name;

                    //repeat the filter once per image pixel
                    m_filterImage.material.SetFloat("RepeatX", _imageSize.x);
                    m_filterImage.material.SetFloat("RepeatY", _imageSize.y);
                }
                else
                {
                    m_filterImage.enabled = false;
                }
                m_image.uvRect = uvRect;
                m_filterImage.uvRect = m_image.uvRect;
                m_currentImageZoom = 1f / m_image.uvRect.width;

                ApplyFilter(filter);
            });
        }
        else
        {
            DataManager.Instance.GetPhotoAsset(m_savedWorkData.ImageInfo.Id, delegate (bool res, Texture2D tex)
            {
                //m_originalTextureData = tex.GetPixels().ToArray();

                m_image.texture = tex;
                m_filterImage.material.mainTextureScale = new Vector2((float)resTex.width, (float)resTex.height);
                var filter = MainManager.Instance.FilterManager.GetFilter(m_savedWorkData.FilterId);
                if (filter != null)
                {
                    m_filterImage.texture = filter;
                    m_filterImage.enabled = true;
                    m_filterName.text = filter.name;
                    m_filterImage.material.SetFloat("RepeatX", _imageSize.x);
                    m_filterImage.material.SetFloat("RepeatY", _imageSize.y);
                }
                else
                {
                    m_filterImage.enabled = false;
                }
                m_image.uvRect = uvRect;
                m_filterImage.uvRect = m_image.uvRect;
                m_currentImageZoom = 1f / m_image.uvRect.width;

                ApplyFilter(filter);

            });
        }
    }

    void ApplyFilter(Texture2D filter)
    {
        //if (this.m_image.texture != null)
        //{
        //	var tex = new Texture2D(this.m_image.texture.width, this.m_image.texture.height, ((Texture2D)this.m_image.texture).format, false);
        //	if (filter != null)
        //	{
        //		int cellSize = NumberColoring.CELL_SIZE;

        //		if (filter.height != cellSize || filter.width != cellSize)
        //		{
        //			var resizedFilter = new Texture2D(filter.width, filter.height, TextureFormat.ARGB32, false);
        //			Graphics.CopyTexture(filter, resizedFilter);
        //			resizedFilter.Resize(cellSize, cellSize);
        //			filter = resizedFilter;
        //		}
        //		var filterColors = filter.GetPixels().ToArray();
        //		for (int row = 0; row < this.m_image.texture.height; row += cellSize)
        //		{
        //			for (int column = 0; column < this.m_image.texture.width; column += cellSize)
        //			{
        //				int index = row * this.m_image.texture.width + column;
        //				Color color = this.m_originalTextureData[index];
        //				for (int i = 0; i < filterColors.Length; i++)
        //				{
        //					filterColors[i] = color;
        //				}
        //				tex.SetPixels(column, row, cellSize, cellSize, filterColors);
        //			}
        //		}
        //	}
        //	else
        //	{
        //		tex.SetPixels(m_originalTextureData);
        //	}
        //	this.m_image.texture = tex;
        //}
    }

    public void OnScrollRectScrolled(Vector2 pos)
    {
        if (m_filters.Count <= 0)
        {
            return;
        }
    }

    public void OnFilterClickHandler(FilterElement filterElement)
    {
        Debug.Log("m_positScrollRect.Moving: " + m_positScrollRect.Moving);
        if (!m_positScrollRect.Moving)
        {
            m_positScrollRect.MoveToIndex(m_filterElements.IndexOf(filterElement));
        }
    }

    void OnSelectIndexHandler(int index)
    {
        if (index >= 0 && index < m_filterElements.Count && index != m_currentIndex)
        {
            m_currentIndex = index;
            VibroWrapper.PlayVibro();
            var filter = MainManager.Instance.FilterManager.GetFilter(m_currentIndex);
            if (filter != null)
            {
                m_filterImage.texture = filter;
                m_filterImage.enabled = true;
                m_filterName.text = LocalizationManager.Instance.GetString(filter.name);

                m_filterImage.material.SetFloat("RepeatX", _imageSize.x);
                m_filterImage.material.SetFloat("RepeatY", _imageSize.y);
            }
            else
            {
                m_filterImage.enabled = false;
                m_filterName.text = string.Empty;
            }

            ApplyFilter(filter);
        }
    }

    void OnScaleHandler(int index, float scale, float maxScale)
    {
        if (index < m_filterElements.Count)
        {
            m_filterElements[index].Scale(scale, maxScale);
        }
    }

    void OnScrolledImageHandler(float delta)
    {
        // each wheel step zooms by 10%
        m_currentImageZoom *= Mathf.Pow(1.1f, delta);
        m_currentImageZoom = Mathf.Max(1f, m_currentImageZoom);
        m_currentImageZoom = Mathf.Min(m_currentImageZoom, m_maxZoom);
        Vector2 center = m_image.uvRect.center;
        Rect uvRect = default(Rect);
        uvRect.height = 1f / m_currentImageZoom;
        uvRect.width = uvRect.height;
        uvRect.center = center;
        m_image.uvRect = uvRect;
        CheckBorders();
        m_filterImage.uvRect = m_image.uvRect;
    }

    void OnZoomedImageHandler(float koef)
    {
        m_currentImageZoom *= koef;
        m_currentImageZoom = Mathf.Max(1f, m_currentImageZoom);
        m_currentImageZoom = Mathf.Min(m_currentImageZoom, m_maxZoom);
        Vector2 center = m_image.uvRect.center;
        Rect uvRect = default(Rect);
        uvRect.height = 1f / m_currentImageZoom;
        uvRect.width = uvRect.height;
        uvRect.center = center;
        m_image.uvRect = uvRect;
        CheckBorders();
        m_filterImage.uvRect = m_image.uvRect;
    }

    void OnImageDraggedHandler(Vector2 delta)
    {
        var canvas = m_image.canvas;
        Rect uvRect = m_image.uvRect;
        float pixelScale = canvas.pixelRect.height / (float)Screen.height;
        //convert the drag from pixels to uv space
        delta.x /= canvas.pixelRect.width / uvRect.width;
        delta.y /= canvas.pixelRect.width / uvRect.height;
        uvRect.center -= delta;
        m_image.uvRect = uvRect;
        CheckBorders();
        m_filterImage.uvRect = m_image.uvRect;
    }

    void CheckBorders()
    {
        Rect uvRect = m_image.uvRect;
        // push the visible rect back inside the 0..1 image
        if (uvRect.min.x < 0f)
        {
            float leftX = uvRect.center.x - uvRect.min.x;
            uvRect.center = new Vector2(leftX, uvRect.center.y);
        }
        if (uvRect.max.x > 1f)
        {
            float right = uvRect.center.x + 1f;
            float rightX = right - uvRect.max.x;
            uvRect.center = new Vector2(rightX, uvRect.center.y);
        }
        if (uvRect.min.y < 0f)
        {
            uvRect.center = new Vector2(uvRect.center.x, uvRect.center.y - uvRect.min.y);
        }
        if (uvRect.max.y > 1f)
        {
            float centerX = uvRect.center.x;
            float top = uvRect.center.y + 1f;
            uvRect.center = new Vector2(centerX, top - uvRect.max.y);
        }
        m_image.uvRect = uvRect;
    }

    public void OkButtonClick()
    {
        //keep the crop and filter for the share window
        m_savedWorkData.UvRect = new SerializableRect(m_image.uvRect);
        m_savedWorkData.FilterId = m_currentIndex;
        var shareWindow = WindowManager.Instance.OpenShareWindow();
        shareWindow.Init(m_savedWorkData);
        var filter = MainManager.Instance.FilterManager.GetFilter(m_currentIndex);
        AnalyticsManager.Instance.FilterWindowClosed((!(filter == null)) ? filter.name : "none");
        AudioManager.Instance.PlayClick();
    }

    public void CloseButtonClick()
    {
        WindowManager.Instance.CloseMe(this);
        var filter = MainManager.Instance.FilterManager.GetFilter(m_currentIndex);
        AnalyticsManager.Instance.BackButtonClicked();
        AnalyticsManager.Instance.FilterWindowClosed((!(filter == null)) ? filter.name : "none");
        AudioManager.Instance.PlayClick();
    }

    public override bool Close()
    {
        m_savedWorkData.FilterId = m_currentIndex;
        return base.Close();
    }
}



using System;
using System.IO;
using Tools.TextureOptimization;
using UnityEngine;
using UnityEngine.UI;

public class CreationWindow : BaseWindow
{
    Action<PhotoInfo> OnCreate;
    Texture2D m_fullTexture;
    Rect m_rect = new Rect(0f, 0f, 1f, 1f);
    Rect m_lastRect;
    float m_maxZoom = 5f;
    float m_currentImageZoom = 1f;
    bool m_subscribed;
    int m_maxSize = 1024;
    PhotoSource m_currentSource;

    [SerializeField] RawImage m_image;
    [SerializeField] RawImage m_readyImage;
    [SerializeField] CameraPlugin m_cameraPlugin;
    [SerializeField] GameObject m_firstButtonsBlock;
    [SerializeField] GameObject m_okButtonsBlock;
    [SerializeField] Slider m_qualitySlider;
    [SerializeField] CreationWindowInputReceiver m_inputReceiver;

    protected override string WindowName
    {
        get
        {
            return "createImageWindow";
        }
    }

    public void Init(Action<PhotoInfo> handler)
    {
        OnCreate = handler;
        if (!m_subscribed)
        {
            m_subscribed = true;
            m_inputReceiver.OnScrolled += OnScrolledImageHandler;
            m_inputReceiver.OnDragged += OnImageDraggedHandler;
            m_inputReceiver.OnZoomed += OnZoomedImageHandler;
            OnOpen += delegate
            {
                m_cameraPlugin.OnUpdateTexture += OnUpdateTextureHandler;
                m_cameraPlugin.Initilized();
                m_cameraPlugin.Quality = (m_qualitySlider.minValue + m_qualitySlider.maxValue) / 2f;
                m_qualitySlider.value = m_cameraPlugin.Quality;
            };
        }
        m_qualitySlider.value = (m_qualitySlider.minValue + m_qualitySlider.maxValue) / 2f;
        m_image.texture = null;
    }

    void OnUpdateTextureHandler(Texture2D tex, float angle)
    {
        GameHelper.CamScaleTexture(tex, 40, 1f, m_qualitySlider.value);
        m_image.rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
        m_image.texture = tex;
    }

    public void QualitySliderValueChanged(float value)
    {
        m_cameraPlugin.Quality = value;
        if (m_okButtonsBlock.activeSelf)
        {
            UpdateTexture(true);
        }
    }

    public void ChangeCameraButtonCLicked()
    {
        m_cameraPlugin.ChangeCamera();
    }

    public void PhotoButtonClicked()
    {
        var texture2D = m_cameraPlugin.TakeSnapshot();
        if (texture2D.width > m_maxSize || texture2D.height > m_maxSize)
        {
            float aspect = (float)texture2D.width / (float)texture2D.height;
            int height = (int)Mathf.Min((float)m_maxSize, (float)m_maxSize / aspect);
            GameHelper.CamScaleTexture(texture2D, height, aspect, 1f);
        }
        m_fullTexture = texture2D;
        m_currentImageZoom = 1f;
        m_rect = new Rect(0f, 0f, 1f, 1f);
        UpdateTextureScale();
        UpdateTexture(true);
        m_okButtonsBlock.SetActive(true);
        m_firstButtonsBlock.SetActive(false);
        m_cameraPlugin.Stop();
        m_image.rectTransform.localEulerAngles = Vector3.zero;
        m_currentSource = (PhotoSource)((!m_cameraPlugin.CurrentDevice.isFrontFacing) ? 2 : 3);
    }

    public void GalleryButtonClicked()
    {
        if (!UnitySingleton<PickerController>.Instance.IsInit())
        {
            UnitySingleton<PickerController>.Instance.InitComplete += AfterInitPicker;
            UnitySingleton<PickerController>.Instance.Subscribe(new Action<Texture2D>(GetImageFromGallery), null, null, null);
            UnitySingleton<PickerController>.Instance.Initilized();
        }
        else
        {
            UnitySingleton<PickerController>.Instance.Subscribe(new Action<Texture2D>(GetImageFromGallery), null, null, null);
            UnitySingleton<PickerController>.Instance.OpenGallery();
        }
    }

    void AfterInitPicker(bool isInit)
    {
        UnitySingleton<PickerController>.Instance.InitComplete -= AfterInitPicker;
        if (isInit)
        {
            UnitySingleton<PickerController>.Instance.OpenGallery();
        }
    }

    void GetImageFromGallery(Texture2D texture)
    {
        UnitySingleton<PickerController>.Instance.UnSubscribe(new Action<Texture2D>(GetImageFromGallery), null, null, null);
        m_fullTexture = texture;
        m_image.texture = texture;
        m_image.rectTransform.localEulerAngles = Vector3.zero;
        m_currentImageZoom = 1f;
        UpdateTextureScale();
        UpdateTexture(true);
        m_okButtonsBlock.SetActive(true);
        m_firstButtonsBlock.SetActive(false);
        m_cameraPlugin.Stop();
        m_currentSource = PhotoSource.Album;
    }

    void OnImageDraggedHandler(Vector2 delta)
    {
        if (m_okButtonsBlock.activeSelf)
        {
            var canvas = m_image.canvas;
            float pixelScale = canvas.pixelRect.height / (float)Screen.height;
            delta.x /= canvas.pixelRect.width / m_rect.width;
            delta.y /= canvas.pixelRect.width / m_rect.height; 
            m_rect.center -= delta; 
            CheckBorders();
            UpdateTexture(false);
        }
    }

    void OnScrolledImageHandler(float delta)
    {
        if (m_okButtonsBlock.activeSelf)
        {
            m_currentImageZoom *= Mathf.Pow(1.1f, delta);
            UpdateTextureScale();
            UpdateTexture(false);
        }
    }

    void OnZoomedImageHandler(float koef)
    {
        if (m_okButtonsBlock.activeSelf)
        {
            m_currentImageZoom *= koef;
            UpdateTextureScale();
            UpdateTexture(false);
        }
    }

    void UpdateTextureScale()
    {
        m_currentImageZoom = Mathf.Max(1f, m_currentImageZoom);
        m_currentImageZoom = Mathf.Min(m_currentImageZoom, m_maxZoom);
        Vector2 center = m_rect.center;
        if (m_fullTexture.width > m_fullTexture.height)
        {
            float aspect = (float)m_fullTexture.height / (float)m_fullTexture.width;
            m_rect = new Rect((1f - aspect) / 2f, 0f, aspect / m_currentImageZoom, 1f / m_currentImageZoom);
        }
        else
        {
            float value2 = (float)m_fullTexture.width / (float)m_fullTexture.height;
            m_rect = new Rect(0f, (1f - value2) / 2f, 1f / m_currentImageZoom, value2 / m_currentImageZoom);
        }
        m_rect.center = center;
        CheckBorders();
    }

    void CheckBorders()
    { 
        if (m_rect.min.x < 0f)
        {    
            m_rect.center = new Vector2(m_rect.center.x - m_rect.min.x, m_rect.center.y);
        } 
        if (m_rect.max.x > 1f)
        {    
            m_rect.center = new Vector2(m_rect.center.x + 1f - m_rect.max.x, m_rect.center.y);
        } 
        if (m_rect.min.y < 0f)
        {     
            m_rect.center = new Vector2(m_rect.center.x, m_rect.center.y - m_rect.min.y);
        } 
        if (m_rect.max.y > 1f)
        {   
            m_rect.center = new Vector2(m_rect.center.x, m_rect.center.y + 1f - m_rect.max.y);
        }
    }

    void UpdateTexture(bool force = false)
    {
        if (!(m_rect != m_lastRect) && !force)
        {
            return;
        }

        m_lastRect = m_rect;
        Texture2D texture2D = null;
        if (m_rect == new Rect(0f, 0f, 1f, 1f))
        {
            var texture2D2 = new Texture2D(m_fullTexture.width, m_fullTexture.height, TextureFormat.RGB24, false);
            texture2D2.filterMode = FilterMode.Point;
            texture2D = texture2D2;
            texture2D.SetPixels(m_fullTexture.GetPixels());
        }
        else
        {
            int x = (int)(m_rect.xMin * (float)m_fullTexture.width);
            int y = (int)(m_rect.yMin * (float)m_fullTexture.height);
            int a = (int)(m_rect.width * (float)m_fullTexture.width);
            int b = (int)(m_rect.height * (float)m_fullTexture.height); 

            int side = Mathf.Min(a, b);
            if (side >= m_fullTexture.width || side >= m_fullTexture.height)
            {

            }
            var pixels = m_fullTexture.GetPixels(x, y, side, side);

            var texture2D2 = new Texture2D(side, side, TextureFormat.RGB24, false);
            texture2D2.filterMode = FilterMode.Point;
            texture2D = texture2D2;
            texture2D.SetPixels(pixels);
            texture2D.Apply();
        }
        m_image.texture = texture2D;
        GameHelper.CamScaleTexture((Texture2D)m_image.texture, 40, 1f, m_qualitySlider.value); 
    }

    public void OkButtonClick()
    {
        string text = Guid.NewGuid().ToString();
        var texture2D = (Texture2D)m_image.texture;
        int colorCount = (int)Mathf.Lerp(15f, 50f, (m_qualitySlider.value - m_qualitySlider.minValue) / (m_qualitySlider.maxValue - m_qualitySlider.minValue));
        if (TextureColoring.CheckToNeedConverColor(texture2D, colorCount))
        {
            texture2D = TextureColorsReducer.Process(texture2D, colorCount);
        }
        File.WriteAllBytes(AppPathsConfig.DownloadsPath + text + ".png", texture2D.EncodeToPNG());
        var photoInfo = new PhotoInfo(text, m_currentSource);
        MainManager.Instance.SavedWorksList.AddPhoto(photoInfo);
        OnCreate.SafeInvoke(photoInfo);
    }

    public void CloseButtonClick()
    {
        WindowManager.Instance.CloseMe(this);
        AnalyticsManager.Instance.BackButtonClicked();
    }

    public override bool Close()
    {
        if (m_okButtonsBlock.activeSelf)
        {
            m_okButtonsBlock.SetActive(false);
            m_firstButtonsBlock.SetActive(true);
            m_cameraPlugin.Initilized();
            return false;
        }
        m_cameraPlugin.Stop();
        return base.Close();
    }
}



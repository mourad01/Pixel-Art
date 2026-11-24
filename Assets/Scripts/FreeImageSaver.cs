using UnityObject = global::UnityEngine.Object;


using System;
using UnityEngine;
using UnityEngine.UI;

public class FreeImageSaver : MonoBehaviour
{
    [SerializeField] Canvas m_canvas;
    [SerializeField] Camera m_freeImageCamera;
    [SerializeField] RawImage m_image;
    [SerializeField] RawImage m_filterImage;

    //[SerializeField]
    //private GameObject m_watermark;

    //[SerializeField]
    //private RectTransform m_watermarkTransform;

    protected void Awake()
    {
        m_image.material = new Material(Shader.Find("Custom/GreyTextureShader"));
        m_filterImage.material = new Material(Shader.Find("Custom/TilingShader"));
    }

    public void GetFilteredImage(ISavedWorkData swd, Action<byte[]> handler, bool antialiasing)
    {
        var resTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        resTex.filterMode = FilterMode.Point;
        resTex.LoadImage(swd.Preview);
        var filter = MainManager.Instance.FilterManager.GetFilter(swd.FilterId);
        if (swd.ImageInfo.Url != null)
        {
            DataManager.Instance.GetImageAsset(swd.ImageInfo, delegate (bool res, Texture2D tex)
            {
                GetFilteredImage(tex, resTex, filter, swd.UvRect1, 800, true, handler, antialiasing, 1f);
            });
        }
        else
        {
            DataManager.Instance.GetPhotoAsset(swd.ImageInfo.Id, delegate (bool res, Texture2D tex)
            {
                GetFilteredImage(tex, resTex, filter, swd.UvRect1, 800, true, handler, antialiasing, 1f);
            });
        }
    }

    public void GetFilteredImage(Texture2D grayTex, Texture2D resTex, Texture2D filter, Rect uvRect, int size, bool enableWatermark, Action<byte[]> handler, bool antialiasing, float watermarkPos = 1f)
    {

        m_image.material.SetTexture("_ResTex", resTex);
        m_image.enabled = true;
        if (filter != null)
        {
            m_filterImage.material.mainTextureScale = new Vector2((float)resTex.width, (float)resTex.height);
            m_filterImage.texture = filter;
            m_filterImage.enabled = true;

            m_filterImage.material.SetFloat("RepeatX", resTex.width);
            m_filterImage.material.SetFloat("RepeatY", resTex.height);
        }
        else
        {
            m_filterImage.enabled = false;
        }

        m_image.uvRect = uvRect;
        m_filterImage.uvRect = m_image.uvRect;
        m_image.texture = grayTex;
        //this.m_watermark.SetActive(enableWatermark && !IAPWrapper.Instance.NoAds && INPluginWrapper.Instance.GetAbTestGroup() != ABTestGroup.RewardedNo_ContentEasy);
        //RectTransform watermarkTransform = this.m_watermarkTransform;
        //Vector2 sizeDelta = this.m_watermarkTransform.sizeDelta;
        //watermarkTransform.anchoredPosition = new Vector2(sizeDelta.x * (0f - watermarkPos + 1f), 0f);
        handler.SafeInvoke(MakePngFromOurVirtualThingy(size, size, size, 100, m_freeImageCamera, antialiasing));
    }

    public void Get3DImage(byte[] bytes, int size, bool enableWatermark, Action<byte[]> handler, bool antialiasing, float watermarkPos = 1f)
    {
        var texture2D = new Texture2D(1, 1, TextureFormat.RGB24, false);
        texture2D.LoadImage(bytes);
        m_image.material.SetTexture("_ResTex", texture2D);
        m_image.enabled = true;
        m_image.uvRect = new Rect(0f, 0f, 1f, 1f);
        m_filterImage.uvRect = m_image.uvRect;
        m_image.texture = texture2D;
        //this.m_watermark.SetActive(enableWatermark && !IAPWrapper.Instance.NoAds && INPluginWrapper.Instance.GetAbTestGroup() != ABTestGroup.RewardedNo_ContentEasy);
        //RectTransform watermarkTransform = this.m_watermarkTransform;
        //Vector2 sizeDelta = this.m_watermarkTransform.sizeDelta;
        //watermarkTransform.anchoredPosition = new Vector2(sizeDelta.x * (0f - watermarkPos + 1f), 0f);
        handler.SafeInvoke(MakePngFromOurVirtualThingy(size, size, size, 100, m_freeImageCamera, antialiasing));
    }

    public static Texture2D MakeTexFromOurVirtualThingy(int width, int height, int maxSize, int pixelPerUnit, Camera cam)
    {
        int newHeight;
        int value2;
        if (width < height)
        {
            newHeight = maxSize;
            value2 = (int)((float)width / (float)height) * newHeight;
        }
        else
        {
            value2 = maxSize;
            newHeight = (int)((float)height / (float)width) * value2;
        }
        var renderTexture = cam.targetTexture = new RenderTexture(value2, newHeight, 24);
        cam.Render();
        RenderTexture.active = renderTexture;
        var texture2D = new Texture2D(value2, newHeight, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0f, 0f, (float)value2, (float)newHeight), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null;
        cam.targetTexture = null;
        UnityObject.Destroy(renderTexture);
        return texture2D;
    }

    public static byte[] MakePngFromOurVirtualThingy(int width, int height, int maxSize, int pixelPerUnit, Camera cam, bool antialising)
    {
        int newHeight;
        int value2;
        if (width < height)
        {
            newHeight = maxSize;
            value2 = (int)((float)width / (float)height) * newHeight;
        }
        else
        {
            value2 = maxSize;
            newHeight = (int)((float)height / (float)width) * value2;
        }
        var renderTexture = new RenderTexture(value2, newHeight, 24);
        renderTexture.antiAliasing = ((!antialising) ? 1 : 8);
        var renderTexture3 = cam.targetTexture = renderTexture;
        cam.Render();
        RenderTexture.active = renderTexture3;
        var texture2D = new Texture2D(value2, newHeight, TextureFormat.RGB24, true);
        texture2D.ReadPixels(new Rect(0f, 0f, (float)value2, (float)newHeight), 0, 0);
        RenderTexture.active = null;
        cam.targetTexture = null;
        var result = texture2D.EncodeToJPG(100);
        UnityObject.Destroy(renderTexture3);
        UnityObject.Destroy(texture2D);
        return result;
    }
}



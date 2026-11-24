

using System.Collections;
using UnityEngine;

public class ColoringVideo : MonoBehaviour
{
    [SerializeField] MeshRenderer m_grayRenderer;
    [SerializeField] MeshRenderer m_gridRenderer;
    [SerializeField] MeshRenderer m_resRenderer;
    [SerializeField] Camera m_videoCamera;

    int m_width;
    Color[] m_pixels;
    ISavedWorkData m_savedWorkData;

    public void Init(Texture2D tex, ISavedWorkData savedWorkData)
    {
        gameObject.SetActive(true);
        m_savedWorkData = savedWorkData;
        m_grayRenderer.sharedMaterial.mainTexture = tex;
        m_gridRenderer.sharedMaterial.mainTextureScale = new Vector2((float)tex.width, (float)tex.height);
        m_resRenderer.sharedMaterial = new Material(Shader.Find("tk2d/BlendVertexColor"));
        m_videoCamera.orthographicSize = NewWorkbookManager.Instance.CameraManager.DefaultOrtoSize;
        m_videoCamera.rect = NewWorkbookManager.Instance.CameraManager.DefaultRect;
        m_pixels = tex.GetPixels();
        m_width = tex.width;
        var texture2D = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
        texture2D.filterMode = FilterMode.Point;
        var startColors = new Color[m_pixels.Length];
        Color color = new Color(1f, 1f, 1f, 0f);
        Color white = Color.white;
        for (int i = 0; i < startColors.Length; i++)
        {
            if (m_pixels[i] == white)
            {
                startColors[i] = white;
            }
            else
            {
                startColors[i] = color;
            }
        }
        texture2D.SetPixels(startColors);
        texture2D.Apply();
        m_resRenderer.sharedMaterial.mainTexture = texture2D;
        base.StartCoroutine(ColoringCoroutine());
    }

    IEnumerator ColoringCoroutine()
    {
        int width = ((Texture2D)m_resRenderer.sharedMaterial.mainTexture).width;
        int height = ((Texture2D)m_resRenderer.sharedMaterial.mainTexture).height;
        int counter = 0;
        int stepSize = m_savedWorkData.History2.Steps.Count / 7 / 30;
        for (int i = 0; i < m_savedWorkData.History2.Steps.Count; i++)
        {
            var step = m_savedWorkData.History2.Steps[i];
            foreach (ShortVector2 vector in step.Vectors)
            {
                ((Texture2D)m_resRenderer.sharedMaterial.mainTexture).SetPixel(vector.X, vector.Y, m_pixels[vector.X + vector.Y * width]);
            }
            counter += step.Vectors.Count;
            if (counter >= stepSize)
            {
                ((Texture2D)m_resRenderer.sharedMaterial.mainTexture).Apply();
                yield return null;
            }
        }
        ((Texture2D)m_resRenderer.sharedMaterial.mainTexture).Apply();
        gameObject.SetActive(false);
    }
}

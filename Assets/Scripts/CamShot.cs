using System;
using UnityEngine;

public class CamShot : MonoBehaviour
{
    public int resWidth = 1000;
    public int resHeight = 1000;
    public Camera cam;

    bool takeHiResShot;
    Texture2D m_Texture;

    public event Action<byte[]> ColorPalleteComplete;

    public GameObject lights;

    void Start()
    {
        m_Texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
    }

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/Captures/Capture_{1}x{2}_{3}.png", Application.dataPath, width, height, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        //lights.SetActive(true);
        takeHiResShot = true;
        var shot = FreeImageSaver.MakePngFromOurVirtualThingy(400, 400, 400, 0, cam, true);
        OnColorPalleteComplete(shot);
        //lights.SetActive(false);
    }

    void OnColorPalleteComplete(byte[] shot)
    {
        var colorPalleteComplete = ColorPalleteComplete;
        if (colorPalleteComplete != null)
        {
            colorPalleteComplete(shot);
        }
    }
}



using UnityObject = global::UnityEngine.Object;
using System;
using System.Linq;
using UnityEngine;

public class CameraPlugin : MonoBehaviour
{
    WebCamTexture webcamTexture;
    string _deviceName;
    Texture2D texture;
    Texture2D gallery;
    bool isAvalibleCam;
    WebCamDevice[] devices;
    int indexCam;
    int orient;

    public Action<Texture2D, float> OnUpdateTexture;
    public Action<bool> InitComplete;

    bool m_force;

    public float Quality { get; set; }

    public WebCamDevice CurrentDevice
    {
        get
        {
            return devices[indexCam];
        }
    }

    public void Initilized()
    {
        //ask for the android camera permission first
        if (!UniAndroidPermission.IsPermitted(AndroidPermission.CAMERA))
        {
            UniAndroidPermission.RequestPermission(AndroidPermission.CAMERA, delegate
            {
                DebugLogger.Log("[CameraPlugin] RequestPremission complete");
                Init();
            }, delegate
            {
                DebugLogger.LogError("[CameraPlugin] RequestPremission Error");
                InitComplete.SafeInvoke(false);
            });
        }
        else
        {
            Init();
        }
    }

    void Init()
    {
        isAvalibleCam = true;
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
        devices = WebCamTexture.devices;
        if (devices == null || devices.Length == 0)
        {
            Debug.LogError("[CameraPlugin] devices == null || devices.Length == 0");
        }
        else
        {
            _deviceName = devices[indexCam].name;
            if (_deviceName != null)
            {
                webcamTexture = new WebCamTexture(_deviceName);
                try
                {
                    webcamTexture.Play();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                    Debug.LogError("[CameraPlugin] webcamTexture.Play");
                    return;
                }
                DebugLogger.Log("[CameraPlugin] Init Camera");
                InitComplete.SafeInvoke(true);
            }
            else
            {
                Debug.LogError("[CameraPlugin] _deviceName == null");
            }
        }
    }

    public Texture2D TakeSnapshot()
    {
        if (isAvalibleCam)
        {
            //crop a centered square from the camera frame
            int side = Mathf.Min(webcamTexture.width, webcamTexture.height);
            texture = new Texture2D(side, side, TextureFormat.RGB24, false)
            {
                filterMode = FilterMode.Point
            };
            texture.SetPixels(webcamTexture.GetPixels((webcamTexture.width - side) / 2, (webcamTexture.height - side) / 2, side, side));
            texture.Apply();
            isAvalibleCam = false;
            if (webcamTexture != null)
            {
                webcamTexture.Stop();
                UnityObject.Destroy(webcamTexture);
            }
            Debug.Log(orient);
            // undo the camera rotation so the photo is upright
            switch (orient)
            {
                case 90:
                    texture = GameHelper.Rotate(texture, false);
                    break;
                case 180:
                    texture = GameHelper.Rotate(texture, false);
                    texture = GameHelper.Rotate(texture, false);
                    break;
                case 270:
                    texture = GameHelper.Rotate(texture, true);
                    break;
                case -90:
                    texture = GameHelper.Rotate(texture, true);
                    break;
                case -180:
                    texture = GameHelper.Rotate(texture, true);
                    texture = GameHelper.Rotate(texture, true);
                    break;
                case -270:
                    texture = GameHelper.Rotate(texture, false);
                    break;
            }
            return texture;
        }
        return null;
    }

    public Texture2D GetTextureByQuality()
    {
        return texture;
    }

    public void ForceUpdate()
    {
        m_force = true;
        Update();
    }

    public void ChangeCamera()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
            UnityObject.Destroy(webcamTexture);
        }
        //cycle to the next camera (front / back)
        indexCam++;
        if (indexCam >= devices.Count())
        {
            indexCam = 0;
        }
        Init();
    }

    void Update()
    {
        if (isAvalibleCam)
        {
            if (webcamTexture == null)
            {
                Debug.LogError("[CameraPlugin] webcamTexture is null");
            }
            else
            {
                //live preview, a new square texture every frame
                int side = Mathf.Min(webcamTexture.width, webcamTexture.height);
                orient = -webcamTexture.videoRotationAngle;
                texture = new Texture2D(side, side, TextureFormat.ARGB32, false)
                {
                    filterMode = FilterMode.Point
                };
                texture.SetPixels(webcamTexture.GetPixels((webcamTexture.width - side) / 2, (webcamTexture.height - side) / 2, side, side));
                texture.Apply();
                OnUpdateTexture.SafeInvoke(texture, (float)orient);
            }
        }
    }

    public void Stop()
    {
        isAvalibleCam = false;
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
            UnityObject.Destroy(webcamTexture);
            webcamTexture = null;
        }
    }

    void OnDestroy()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
            UnityObject.Destroy(webcamTexture);
            webcamTexture = null;
        }
    }
}



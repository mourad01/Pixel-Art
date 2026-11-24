

using Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine; 

public class DataManager : MonoBehaviour
{
    public Action OnInternetAppeared;

    DateTime m_startTime = DateTime.Now; 

    [SerializeField] ServerConnector m_serverConnector;

    ImagesInfo m_imagesInfo; 

    public static DataManager Instance { get; private set; }

    public ImagesInfo AllImages
    {
        get
        {
            return m_imagesInfo;
        }
    }

    public bool ServerAvailable
    {
        get
        {
            return InternetConnection.IsAvailable && !ServerError;
        }
    }

    public bool ServerError { get; private set; }

    public bool Inited { get; private set; }

    void Awake()
    {
        Instance = this; 
    }

    public void Init()
    { 
        m_serverConnector.Init();
        m_serverConnector.OnInternetAppeared += delegate
        {
            OnInternetAppeared.SafeInvoke();
        };
    } 

    public void GetImages(bool force, Action<ImagesInfo> handler)
    {
        Debug.Log("GetImages 10");
        if (m_imagesInfo != null && !force)
        {
            handler.SafeInvoke(m_imagesInfo);
        }
        else
        {
            Debug.Log("Start loading GetImages");
            //SearchForLocalFile(AppPathsConfig.ImagesFile, delegate (ImagesInfo res)
            //{
            //	this.m_imagesInfo = res;
            //});
            base.StartCoroutine(m_serverConnector.GetLocalStreamData((res) =>
            {
                Debug.Log("GetImages this.m_imagesInfo = res; count = " + res.Count);
                m_imagesInfo = res;
                base.StartCoroutine(GetImagesListCoroutine(force, handler));
            }));
            base.StartCoroutine(LoadImages(handler));
        }
    }

    IEnumerator LoadImages(Action<ImagesInfo> handler)
    {
        yield return null;
        if (m_imagesInfo == null || m_imagesInfo.Images.Count == 0)
        {
           
            yield return new WaitForSeconds(0.5f);
            /*using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(imagesAsset.text)))
            {
                this.m_imagesInfo = Serializer.LoadFromTextStream<ImagesInfo>(stream);
            }*/
        }
        handler(m_imagesInfo);
    } 
    T CheckDownloadedFile<T>(string fileName)
    { 
        string path = AppPathsConfig.DownloadsPath + fileName;
        Debug.Log("CheckDownloadedFile " + path);
        if (File.Exists(path))
        {
            Debug.Log("CheckDownloadedFile OK");
            if (typeof(T) == typeof(ImagesInfo))
            {
                return Serializer.LoadFromTextFile<T>(path);
            }
            return Serializer.LoadFromFile<T>(path);
        }
        return default(T);
    }

    public ImageInfo GetImageInfo(string imageId)
    {
        return m_imagesInfo.Images.FirstOrDefault((ImageInfo a) => a.Id == imageId);
    }

    public void GetImageAsset(ImageInfo imageInfo, Action<bool, Texture2D> handler)
    {
        base.StartCoroutine(GetImageAssetCoroutine(imageInfo, handler));
    }

    public bool CheckLocalAsset(ImageInfo imageInfo)
    {
        string text = imageInfo.Id + ".png";
        return File.Exists(AppPathsConfig.DownloadsPath + text);
    }

    public void GetImageAsset3D(ImageInfo imageInfo, Action<bool, CashImage3D> handler)
    {
        base.StartCoroutine(GetImageAsset3DCoroutine(imageInfo, handler));
    }

    public void GetPhotoAsset(string id, Action<bool, Texture2D> handler)
    {
        base.StartCoroutine(GetPhotoAssetCoroutine(id, handler));
    } 

    public AssetBundle GetInappsWindow()
    {
        return null;
    }

    public AssetBundle GetTrialInappsWindow()
    {
        return null;
    } 
    void Log(string message)
    {
    }

    void LogError(string message)
    {
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        { 
        }
        else
        {
            m_startTime = DateTime.Now;
        }
    }  
    IEnumerator CheckEmbeddedVersion<T>(string fileName, Action<T> handler) 
    {
        Debug.Log("CheckEmbeddedVersion " + AppPathsConfig.StreamingAssetsPath + fileName);
        var www = new WWW(AppPathsConfig.StreamingAssetsPath + fileName);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("CheckEmbeddedVersion ok"); 
            var par = Serializer.LoadFromStream<T>((Stream)new MemoryStream(www.bytes));
            SystemExtends.SafeInvoke<T>(handler, par);
        }
        else
        {
            Debug.Log("CheckEmbeddedVersion error"); 
            SystemExtends.SafeInvoke<T>(handler, default(T)); 
        }
    }


    IEnumerator SearchForLocalFile<T>(string fileName, Action<T> handler)
    { 
        Debug.Log("SearchForLocalFile"); 
        var downloadedVersion = CheckDownloadedFile<T>(fileName);
        if (downloadedVersion != null && !downloadedVersion.Equals(default(T)))
        {
            SystemExtends.SafeInvoke<T>(handler, downloadedVersion);
            yield break;
        }
        StartCoroutine(CheckEmbeddedVersion<T>(fileName, (Action<T>)delegate (T res)
        { 
            if (res != null && !res.Equals(default(T)))
            {
                handler.SafeInvoke<T>(res);
            }
            else
            {
                handler.SafeInvoke<T>(default(T)); 
            }
        }));         yield return null;
    }

    IEnumerator GetImagesListCoroutine(bool force, Action<ImagesInfo> handler)
    {
        Debug.Log("GetImagesListCoroutine");
        
        if (!force)
        {
            yield return null;
            
            Debug.Log("GetImagesListCoroutine not force");
            StartCoroutine(SearchForLocalFile(AppPathsConfig.ImagesFile, delegate (ImagesInfo res)
            {
                Debug.Log("GetImagesListCoroutine SearchForLocalFile response");
                if (res != null)
                    m_imagesInfo = res;
                if (m_imagesInfo != null)
                {
                    handler.SafeInvoke(m_imagesInfo);
                }
            }));
            
            yield return null;
            
        }
        
        yield return null;
        m_serverConnector.GetImagesList(delegate (ImagesInfo imagesInfo)         {
            Debug.Log("GetImagesListCoroutine GetImagesList response");
            if (imagesInfo != null){
                Debug.Log("GetImagesListCoroutine GetImagesList response count = " + imagesInfo.Count);
            }
            if (imagesInfo != null && m_imagesInfo != null)
            {
                Debug.Log("GetImagesListCoroutine GetImagesList response OK");
                int count = m_imagesInfo.Images.Count;
                m_imagesInfo = imagesInfo;                 Serializer.SaveToTextFile(AppPathsConfig.DownloadsPath + AppPathsConfig.ImagesFile, imagesInfo, false);                 if (count != imagesInfo.Images.Count)
                {
                    handler.SafeInvoke(imagesInfo);
                }
            }
        });         yield return null;
    }
    IEnumerator GetImageAssetCoroutine(ImageInfo imageInfo, Action<bool, Texture2D> handler)
    {
        StartCoroutine(SearchForLocalFile(imageInfo.Id + ".png", delegate (CashImage foundAsset)
        { 
            if (foundAsset != null)
            {
                var texture2D = new Texture2D(1, 1, TextureFormat.RGB24, false);
                texture2D.filterMode = (FilterMode)(imageInfo.Is3D ? 1 : 0);
                texture2D.LoadImage(foundAsset.Bytes);
                handler.SafeInvoke(true, texture2D);
                return;
            }
            Texture2D serverTex = null; 
            m_serverConnector.GetImageAsset(imageInfo, delegate (bool res, byte[] bytes)
            { 
                if (res)
                {
                    try
                    {
                        string fileName = AppPathsConfig.DownloadsPath + imageInfo.Id + ".png";
                        Serializer.SaveToFile(fileName, new CashImage(bytes), false);
                        var cashImage = Serializer.LoadFromFile<CashImage>(fileName);
                        var texture2D2 = new Texture2D(1, 1, TextureFormat.RGB24, false);
                        texture2D2.filterMode = (FilterMode)(imageInfo.Is3D ? 1 : 0);
                        texture2D2.LoadImage(cashImage.Bytes);
                        serverTex = texture2D2;
                    }
                    catch(Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
                handler.SafeInvoke((UnityEngine.Object)serverTex != (UnityEngine.Object)null, serverTex);
            }); 
            handler.SafeInvoke((UnityEngine.Object)serverTex != (UnityEngine.Object)null, serverTex); 
        })); 
        yield return null;
    }

    IEnumerator GetImageAsset3DCoroutine(ImageInfo imageInfo, Action<bool, CashImage3D> handler)
    {          StartCoroutine(SearchForLocalFile(imageInfo.Id + ".png.vox", delegate (CashImage3D foundAsset)         {
            if (foundAsset != null)
            {
                handler.SafeInvoke(true, foundAsset);
                return;
            }
            try
            {
                string path = AppPathsConfig.DownloadsPath + imageInfo.Id + ".png.vox";
                if (File.Exists(path))
                {
                    var data = Serializer.LoadFromFile<CashImage3D>(path);
                    handler.SafeInvoke(true, data);
                    return;
                }
            }
            catch
            {
            } 
            m_serverConnector.GetImageAsset3D(imageInfo, delegate (bool res, byte[] bytes)
            { 
            CashImage3D customVox = null;
                if (res)
                {
                    var data = new CashImage3D(new CashImage(bytes));
                    string voxFilename = AppPathsConfig.DownloadsPath + imageInfo.Id + ".png.vox";

                    Serializer.SaveToFile(voxFilename, data);
                    if (handler == null)
                    {
                    }
                    else
                    {
                        customVox = data;
                    }
                }
                handler.SafeInvoke(customVox != null, customVox);
            });
        }));
        yield return null;  
    }
    IEnumerator GetPhotoAssetCoroutine(string id, Action<bool, Texture2D> handler)
    {
      
        yield return null;
        var fileName = id + ".png";
        if (File.Exists(AppPathsConfig.DownloadsPath + fileName))
        {
            
            var data = File.ReadAllBytes(AppPathsConfig.DownloadsPath + fileName);
            var texture2D = new Texture2D(1, 1, TextureFormat.RGB24, false);
            texture2D.filterMode = FilterMode.Point;
            texture2D.LoadImage(data);
            handler.SafeInvoke(true, texture2D);
        }
        else
        {
            handler.SafeInvoke(false, null);

        } 
    }
}

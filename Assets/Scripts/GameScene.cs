using UnityObject = global::UnityEngine.Object;


using Assets.Scripts.Navigation;
using Assets.Scripts.Navigation.Scenes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxDLL;

internal class GameScene : BaseScene
{
    [Range(0.01f, 5f)]
    [SerializeField] float sizePerVox = 1f;

    [SerializeField] Material voxMaterial;
    [SerializeField] Transform meshOrigin;
    [SerializeField] GameObject saveCamera;
    [SerializeField] Camera videoCamera;

    MVMainChunk v;
    Loader3D load3D;
    bool m_stopInit = true;
    ImageInfo m_imageInfo;

    [SerializeField] GameObject m_videoWaiter;

    public static GameScene Instance;

    public override SceneType SceneType
    {
        get
        {
            return SceneType.Game;
        }
    }

    public override void Start()
    {
        Instance = this;
        base.Start();
        //base.StartCoroutine(this.StopInitAndShowInterCoroutine());
    }

    public override void OnNavigatedTo(NavigationArgs args)
    {
        try { 
        var gameNavigationArgs = args as GameNavigationArgs;
        base.StartCoroutine(InitCoroutine(gameNavigationArgs.ImageInfo, gameNavigationArgs.Data, gameNavigationArgs.SavedWorkData, gameNavigationArgs.ImageOpenType));
        }
        catch(NullReferenceException e)
        {
            Debug.Log(e.Message);
        }
    }

    public override void OnNavigatedFrom()
    {
        UnitySingleton<BackButtonManager>.Instance.BackButtonAction -= ((BaseScene)this).BackButtonActions;
    }

    public override void BackButtonActions()
    {
        Debug.Log(" Touch BackButtonActions");
    }

    public void CheatColorAll()
    {
        var objectsOfType = UnityObject.FindObjectsOfType<VoxCubeItem>();
        var voxCubeItems = objectsOfType;
        foreach (VoxCubeItem voxCubeItem in voxCubeItems)
        {
            voxCubeItem.CheatTochCubeItem();
            UnitySingleton<GameController>.Instance.SetCubeItemProgress(voxCubeItem);
        }
    }

    void VoxLoadCompleteHandler(List<Color> list)
    {
        base.StartCoroutine(OpenUICoroutine(list));
    }

    void ClearVoxMeshes()
    {
        var componentsInChildren = gameObject.GetComponentsInChildren<MVVoxModelMesh>();
        var mvVoxModelMeshs = componentsInChildren;
        foreach (MVVoxModelMesh mVVoxModelMesh in mvVoxModelMeshs)
        {
            UnityObject.DestroyImmediate(mVVoxModelMesh.gameObject);
        }
        var mvVoxModelVoxels = gameObject.GetComponentsInChildren<MVVoxModelVoxel>();
        var voxels = mvVoxModelVoxels;
        foreach (MVVoxModelVoxel mVVoxModelVoxel in voxels)
        {
            UnityObject.DestroyImmediate(mVVoxModelVoxel.gameObject);
        }
    }

    void VoxLoad(byte[] data = null)
    {
        if (data != null)
        {
            load3D = new Loader3D();
            load3D.Init(new Action<Color[]>(LoadPalleteComplete), new Action<List<VoxCubeItem>>(LoadVoxCubeComplete));
            if (data != null)
            {
                load3D.Load(data);
            }
        }
        else
        {
            Debug.LogError("[MVVoxModel] Invalid file path or data");
        }
    }

    void LoadVoxCubeComplete(List<VoxCubeItem> items)
    {
        UnitySingleton<GameController>.Instance.VoxCubeItems = items;
    }

    void LoadPalleteComplete(Color[] colors)
    {
        UnitySingleton<GameController>.Instance.Initilized(colors);
    }

    void LoadCustomVox(CashImage3D vox = null)
    {
        UnitySingleton<GameController>.Instance.SetCustomData(vox);
        VoxLoad(vox.Vox);
        vox.Init(load3D.MV);

        //video and save cameras use the view saved with the model
        Vector3 position = vox.Position.ToVector3();
        videoCamera.transform.position = position;
        saveCamera.transform.position = position; 
        Quaternion rotation = vox.Rotation.ToQuaternion();
        videoCamera.transform.rotation = rotation;
        saveCamera.transform.rotation = rotation;
        var saveCam = saveCamera.GetComponent<Camera>(); 
        ((Component)videoCamera).GetComponent<Camera>().orthographicSize = vox.CamSize / 2f;
        saveCam.orthographicSize = vox.CamSize / 2f;
        Camera.main.orthographicSize = vox.CamSize / 1.5f;
        Camera.main.transform.position = Vector3.zero;
        Camera.main.transform.rotation = vox.Rotation.ToQuaternion();
        // ortho size 15..5 becomes 0..1 for the camera depth
        float z = Mathf.InverseLerp(15f, 5f, Camera.main.orthographicSize);
        UnitySingleton<GameController>.Instance.ChangeCamDepthPosition(z);
    }

    void OnFailLoad()
    {
        Debug.LogError("[MVVoxModel] Invalid load data from StreamingAssets");
    }

    void OnDestroy()
    {
    }

    //private IEnumerator StopInitAndShowInterCoroutine()
    //{
        /*if ((INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.None || INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentMedium_Old || INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentMedium_Revealed) && (this.m_imageInfo == null || this.m_imageInfo.CustomAccessStatus != 0) && !IAPWrapper.Instance.Subscribed && !AppData.UnlockedImages.Contains(this.m_imageInfo.Id))
        {
            this.m_stopInit = false;
            yield break;
        }
        if (!IAPWrapper.Instance.NoAds && AppData.TutorialCompleted)
        {
            yield return new WaitForSeconds(0.1f);
            if (AdsWrapper.Instance.ShowInter("preview"))
            {
                yield return new WaitForSeconds(1f); 
                this.m_stopInit = false;
                yield break;
            }
        }
        this.m_stopInit = false;*/
    //}
    IEnumerator InitCoroutine(ImageInfo info, CashImage3D vox, SavedWorkData3D savedWorkData, ImageOpenType imageOpenType)
    {  
        m_imageInfo = info;
        yield return null;

        //both branches continue, so the init below is never reached
        while (true)
        {
            if (m_stopInit)
            {
                yield return null;
                continue;
            }
            else
            {
                yield return null;
                continue;
            }
            bool video = false;
            /*if ((INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.None || INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentMedium_Old || INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentMedium_Revealed) && info.CustomAccessStatus != 0 && !IAPWrapper.Instance.Subscribed && !AppData.UnlockedImages.Contains(info.Id))
            {
                this.m_videoWaiter.SetActive(true); 
                AppData.UnlockedImages.Add(info.Id);
                var timer = 7f;
                while (timer >= 0f)
                { 
#if UNITY_EDITOR
                    video = true; 
                    //AdsWrapper.Instance.ShowVideo("image", null);
#elif UNITY_ANDROID
                    var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
                    {
                        if (AdsWrapper.Instance.IsVideoAvailable())
                        {
                            video = true;
                            AdsWrapper.Instance.ShowVideo("image", null);
                        }
                    });
#else
                    if (AdsWrapper.Instance.IsVideoAvailable())
                    {
                        video = true;
                        AdsWrapper.Instance.ShowVideo("image", null);
                    }	
#endif
                    yield return new WaitForSeconds(1f);

                    timer -= 1f;
                    if (video)
                    {
                        break;
                    } 
                }
            }*/
            m_videoWaiter.SetActive(false);
            WorkbookModel.Init();
            UnitySingleton<ProgressManager>.Instance.Init(info, saveCamera, savedWorkData);
            ClearVoxMeshes();
            UnitySingleton<GameController>.Instance.VoxLoadComplete += new Action<List<Color>>(VoxLoadCompleteHandler);
            UnitySingleton<GameController>.Instance.VideoCamera = videoCamera;
            LoadCustomVox(vox);
            AnalyticsManager.Instance.ImageOpened(info, (int)vox.CubesCount, vox.ColorsCount.Count, imageOpenType);
            yield break;
        }
    }
    IEnumerator OpenUICoroutine(List<Color> colors)
    {
        yield return null;
        var workbook = WindowManager.Instance.OpenWorkbook3D();
        workbook.Init(colors);
        yield return null;

        UnityObject.FindObjectOfType<CameraManager3D>().Init();
        if (!AppData.TutorialCompleted)
        {
            var tutorialWindow = WindowManager.Instance.OpenTutorial3D();
            if (tutorialWindow != null)
            {
                tutorialWindow.Init("picture", "3D");
                // hide the scene while the tutorial is open
                gameObject.SetActive(false);
                tutorialWindow.OnStartClose += delegate
                {
                    if (gameObject != null)
                    {
                        gameObject.SetActive(true);
                    }
                };
            }
        }
    }
}

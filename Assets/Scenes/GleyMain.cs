using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GleyMain : MonoBehaviour
{
    public int isrun = 1;
    private static GleyMain instance = new GleyMain();
    public static GleyMain Instance
    {
        get
        {
            return instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //Advertisements.Instance.Initialize();
        //Advertisements.Instance.ShowBanner(BannerPosition.TOP, BannerType.SmartBanner);
        
    }
   public void click()
    {
        //Advertisements.Instance.ShowInterstitial();
        AdsManager.instance.ShowInterstitial(null);
    }
    public void rewarded()
    {
        AdsManager.instance.ShowReward(null,null);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isrun == 1)
        {
            StartCoroutine(wait35s());
        }
    }
    IEnumerator wait35s()
    {

        isrun = 0;
        yield return new WaitForSeconds(35);
       AdsManager.instance.ShowInterstitial(null);
    
        isrun = 1;
    }
}

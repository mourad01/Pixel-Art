using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADS : MonoBehaviour
{
    string bannerAdUnitId = "3657079b8eb84a05"; // Retrieve the ID from your account

    // Start is called before the first frame update
    void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            
            // AppLovin SDK is initialized, start loading ads
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.TopCenter);

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.white);
            MaxSdk.ShowBanner(bannerAdUnitId);
        };

        MaxSdk.SetSdkKey("zEt4_M4PkG_cNWU_sR4p4RXZ6mE5AO4RAx8SQs1BVTZR5iMfLP54K_p1L4C6x88exH8F__Tr3QQ0TgrAyzCiPq");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

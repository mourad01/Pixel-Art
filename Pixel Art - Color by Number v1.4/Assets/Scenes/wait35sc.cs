using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class wait35sc : MonoBehaviour
{
    public int isrun=1;
    public Timer timer;
    public GameObject itert;
    private Interstitial interstitial;
    string adInter = "72d41e387a1a7fd4";
    private void Awake()
    {
        
    }
    private void Start()
    {

        interstitial = Interstitial.Instance;
       
        
    }
    private void Update()
    {
        if (isrun == 1)
        {
            StartCoroutine(wait35s());
        }
    }
   
    // Update is called once per frame
  IEnumerator wait35s()
    {

        isrun = 0;
        yield return new WaitForSeconds(35);
        interstitial.InitializeInterstitialAds();
        isrun = 1;
    }
}

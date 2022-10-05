using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class wait35sc : MonoBehaviour
{
    public int isrun=1;
   
  
   
    private void Awake()
    {
        
    }
    private void Start()
    {

        
       
        
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
        AdsManager.instance.ShowInterstitial(null);
        isrun = 1;
    }
}

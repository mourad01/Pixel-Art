using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class intershow : MonoBehaviour
{
	private Interstitial interstitial;
	public string adInter = "72d41e387a1a7fd4";
	private void Start()
	{
		interstitial = Interstitial.Instance;

	}
	public void Click()
	{
		interstitial.InitializeInterstitialAds();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class MobileAdManager : MonoBehaviour
{
	public static MobileAdManager Instance { get; private set; }
	
	private RewardedAd rewardedAd;

	public static event Action OnRewardEarned;

	private void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		MobileAds.Initialize(initStatus => { });

		CreateAndLoadAd();
	}

	private void CreateAndLoadAd()
	{
		Debug.Log("CreateAndLoadAd_1");
#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-8413101784746060/2258453671";
		//string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // test id
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
		string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // test id
#endif

		Debug.Log("CreateAndLoadAd_2");
		rewardedAd = new RewardedAd(adUnitId);

		Debug.Log("CreateAndLoadAd_3");
		rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		Debug.Log("CreateAndLoadAd_4");
		rewardedAd.LoadAd(new AdRequest.Builder().Build());
		Debug.Log("CreateAndLoadAd_5");
	}

	public void ShowRewardAd()
	{
		Debug.Log("ShowRewardAd_1");
		Debug.Log($"rewardedAd.IsLoaded() : {rewardedAd.IsLoaded()}");
		if (rewardedAd.IsLoaded())
		{
			Debug.Log("ShowRewardAd_2");
			rewardedAd.Show();
		}
		else
		{
			Debug.Log("ShowRewardAd_3");
			CreateAndLoadAd();
			Debug.Log("ShowRewardAd_4");
		}
	}

	private void HandleUserEarnedReward(object sender, Reward e)
	{
		OnRewardEarned?.Invoke();
	}

	private void HandleRewardedAdClosed(object sender, EventArgs e)
	{
		CreateAndLoadAd();
	}
}

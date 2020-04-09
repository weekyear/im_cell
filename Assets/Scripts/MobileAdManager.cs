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
#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-8413101784746060/3517986761";
		//string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // test id
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
		string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // test id
#endif

		rewardedAd = new RewardedAd(adUnitId);

		rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		rewardedAd.LoadAd(new AdRequest.Builder().Build());
	}

	public void ShowRewardAd()
	{
		if (rewardedAd.IsLoaded()) rewardedAd.Show();
		else CreateAndLoadAd();
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

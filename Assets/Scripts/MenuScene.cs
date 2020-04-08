using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
	[SerializeField] GameObject NoAdBtn;
	[SerializeField] GameObject VIPImage;
	[SerializeField] GameObject SettingMenu;
	[SerializeField] LoginWindow loginWindow;
	[SerializeField] CreateUserWindow createUserWindow;

	#region CALLBACKS
	private void Awake()
	{
		PlayfabManager.OnNewUser += OnNewUser;
		PlayfabManager.UserDataUpdated += UserDataUpdated;
	}

	private void Start()
	{
		if (!StoryManager.IsEndedCredit)
		{
			AudioManager.Instance?.StartMenuBgm();
		}

		if (!PlayfabManager.Instance.IsLogin)
		{
#if UNITY_EDITOR || ASTANDALONE
			loginWindow.Show();
#elif UNITY_ANDROID 
		PlayfabManager.Instance.Login();
#endif
		}

		UpdateAdButtons();
	}

	private void OnDestroy()
	{
		PlayfabManager.OnNewUser -= OnNewUser;
		PlayfabManager.UserDataUpdated -= UserDataUpdated;
	}
	#endregion

	private void OnNewUser()
	{
		createUserWindow.Show();
	}

	private void UserDataUpdated()
	{
		UpdateAdButtons();
	}

	private void UpdateAdButtons()
	{
		NoAdBtn.SetActive(!PlayfabManager.Instance.NoAd);
		VIPImage.SetActive(PlayfabManager.Instance.NoAd);
	}

	public void ShowSettingMenu()
	{
		SettingMenu.SetActive(true);
	}

	public void CloseSettingMenu()
	{
		AudioManager.Instance.PlayEffectSound("button_click_02");
		SettingMenu.SetActive(false);
	}

	public void StartGame()
	{
		if (!PlayfabManager.Instance.IsLogin)
		{
			ToastManager.Instance.Show("로그인 후에 플레이하실 수 있습니다.");
			PlayfabManager.Instance.Login();
			return;
		}

		AudioManager.Instance.PlayEffectSound("button_click_01");
		SceneManager.LoadScene("GameScene");
	}

	public void PurchasNoAd() => PlayfabManager.Instance.InAppPurchase("remove_ads");

	public void QuitGame()
	{
		AudioManager.Instance.PlayEffectSound("button_click_02");
		Application.Quit();
	}
}

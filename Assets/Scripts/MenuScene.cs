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
	[SerializeField] GameObject StartMenu;
	[SerializeField] GameObject SettingMenu;
	[SerializeField] LoginWindow loginWindow;
	[SerializeField] CreateUserWindow createUserWindow;

	public static bool IsNewGameStart;

	#region CALLBACKS
	private void Awake()
	{
		IsNewGameStart = false;

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
	
	public void ShowStartMenu()
	{
		AudioManager.Instance.PlayEffectSound("button_click_01");
		StartMenu.SetActive(true);

		var continueBtn = StartMenu.transform.Find("Buttons").Find("ContinueGameButton");
		continueBtn.Find("BtnTitle_1").GetComponent<Text>().text = $"스테이지 {PlayerPrefs.GetInt("SavedStage", 1)}";

		if (PlayerPrefs.GetInt("SavedStage", 1) == 1)
		{
			continueBtn.GetComponent<Button>().interactable = false;
		}
	}

	public void CloseStartMenu()
	{
		AudioManager.Instance.PlayEffectSound("button_click_02");
		StartMenu.SetActive(false);
	}

	public void ClickNewGameBtn()
	{
		IsNewGameStart = true;
		StartGame();
	}

	public void ClickContinueGameBtn()
	{
		IsNewGameStart = false;
		StartGame();
	}

	private void StartGame()
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

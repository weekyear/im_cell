using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
	[SerializeField] GameObject SettingMenu;
	[SerializeField] LoginWindow loginWindow;
	[SerializeField] CreateUserWindow createUserWindow;

	#region CALLBACKS
	private void Awake()
	{
		PlayfabManager.OnNewUser += OnNewUser;
	}

	private void OnNewUser()
	{
		createUserWindow.Show();
	}

	private void Start()
	{
		AudioManager.Instance.StartMenuBgm();

		if (!PlayfabManager.Instance.IsLogin)
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			loginWindow.Show();
#else
		PlayfabManager.Instance.Login();
#endif
		}
	}

	private void OnDestroy()
	{
		PlayfabManager.OnNewUser -= OnNewUser;
	}
#endregion

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

	public void QuitGame()
	{
		AudioManager.Instance.PlayEffectSound("button_click_02");
		Application.Quit();
	}
}

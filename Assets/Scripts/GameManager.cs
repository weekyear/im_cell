using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static List<bool> IsOpenChestList;

	[SerializeField] private GameObject MessageWindow;
    [SerializeField] private GameObject GameoverWindow;
    [SerializeField] private GameObject PauseWindow;
    [SerializeField] private GameObject ConfirmWindow;
    [SerializeField] private GameObject SaveConfirmWindow;
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject LossHealthBar;
    [SerializeField] private GameObject Player;
	[SerializeField] private int StartMapNum = 1;
	[SerializeField] private int EndMapNum;

	public static float Health = 100;
    public static float RecentHeal = 0;
    public static int RecentVirus = 0;

	public static float time;

    public static int EndedMapNum;
    public static int MapNum;
    public static int PassedMapNum;
    private int RevivalNum;

    private void Awake()
    {
		time = 0f;
        Health = 100;
        RecentHeal = 0;
        RecentVirus = 0;
        RevivalNum = 0;
        EndedMapNum = EndMapNum;

        SetShowAdBtn();

        if (MenuScene.IsNewGameStart == null)
        {
            MapNum = StartMapNum;
			IsOpenChestList = new List<bool>(new bool[11]);
		}
		else if (MenuScene.IsNewGameStart == true)
		{
			MapNum = 1;
			IsOpenChestList = new List<bool>(new bool[11]);
		}
        else
        {
			MapNum = PlayfabManager.Instance.Level;
			IsOpenChestList = PlayfabManager.Instance.ChestList;
		}

        GameObject.Find($"StageText").GetComponent<Text>().text = $"<Stage{MapNum}>";
        PassedMapNum = MapNum - 1;

		MobileAdManager.OnRewardEarned += OnRewardEarned;
		PlayerObserver.OnHealthChanged += HealthBarChanged;
        PlayerObserver.OnLossHealthChanged += LossHealthBarChanged;
        PlayerObserver.OnGameOver += GameOver;

        Instantiate(Resources.Load($"Map/Map_{MapNum}_"));
    }

    private void SetShowAdBtn()
    {
        if (PlayfabManager.Instance != null && PlayfabManager.Instance.NoAd)
        {
            RectTransform _rect = GameoverWindow.GetComponent<RectTransform>();
            _rect.sizeDelta = new Vector2(_rect.rect.width, 600f);
            GameoverWindow.transform.Find("ShowAdText").gameObject.SetActive(false);
            GameoverWindow.transform.Find("Buttons").Find("ShowAdButton").Find("Text").GetComponent<Text>().text = "부활";
            GameoverWindow.transform.Find("Buttons").Find("ShowAdButton").Find("Text").GetComponent<Text>().fontSize = 50;
        }
    }

	private void OnDestroy()
    {
		MobileAdManager.OnRewardEarned -= OnRewardEarned;
        PlayerObserver.OnHealthChanged -= HealthBarChanged;
        PlayerObserver.OnLossHealthChanged -= LossHealthBarChanged;
        PlayerObserver.OnGameOver -= GameOver;
    }

    void Update()
    {
        time += Time.deltaTime;

        if (Input.GetKey(KeyCode.P)) GamePause();
        if (Input.GetKey(KeyCode.R)) GameRestart();
        if (Input.GetKey(KeyCode.C)) Player.transform.position = new Vector3(30, 0, 0);
        if (Input.GetKey(KeyCode.O))
        {
            for (int i = 0; i < IsOpenChestList.Count; i++) IsOpenChestList[i] = true;
		}

    }

    public void GamePause()
    {
        if (!MessageWindow.activeSelf && !GameoverWindow.activeSelf)
        {
            if (PauseWindow.activeSelf)
            {
                AudioManager.Instance.PlayEffectSound("button_click_02");
                PauseWindow.SetActive(false);
                ChangeTimeScale(1);
            }
            else
            {
                AudioManager.Instance.PlayEffectSound("button_click_01");
                PauseWindow.SetActive(true);
                ChangeTimeScale(0);
            }

            PauseWindow.transform.Find("PauseTimerText").GetComponent<Text>().text = $"{TimeText} | {RevivalNum}회 부활";
        }
    }

    public static void ChangeTimeScale(int _timeScale)
    {
        Time.timeScale = _timeScale;
    }

    private void HealthBarChanged(float amount)
    {
        Health += amount;

        if (Health < 0) Health = 0;

        if (Health > 100) Health = 100;

        HealthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(Health * 7.25f, 60);

        SetPlayerScaleByHealth();
    }
    
    private void LossHealthBarChanged(float amount)
    {
        var sizeOfBar = new Vector2(0, 60);
        if (Mathf.Abs(amount) >= Health)
        {
            sizeOfBar.x = Health * 7.25f;
        }
        else
        {
            sizeOfBar.x = amount * -7.25f;
        }

        LossHealthBar.GetComponent<RectTransform>().sizeDelta = sizeOfBar;
    }

    private void SetPlayerScaleByHealth()
    {
        var playerScale = (0.85f + 0.0015f * Health) * 0.55f;

        Player.transform.localScale = new Vector3(Mathf.Sign(Player.transform.position.x) * playerScale, playerScale);
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.75f);

        ChangeTimeScale(0);

        GameoverWindow.transform.Find("GameoverTimerText").GetComponent<Text>().text = $"{TimeText} | {RevivalNum}회 부활";
        GameoverWindow.transform.Find("SaveStagePanel").Find("SaveStageDescription").GetComponent<Text>().text = $"저장된 스테이지 : Stage{PlayfabManager.Instance.Level}";
        GameoverWindow.SetActive(true);

        PlayerController.IsAnimatingDead = false;
    }

    public void ClickRestartBtn()
    {
        SetConfirmWindow("처음으로 돌아가면 지금 위치에서는 시작하지 못 합니다.\n\n첫 스테이지로 돌아갈까요?", new Action(() => GameRestart()));
        AudioManager.Instance.PlayEffectSound("button_click_01");
    }

    public void ClickMenuBtn()
    {
        SetConfirmWindow("홈화면으로 돌아가면 지금 위치에서는 시작하지 못 합니다.\n\n홈 화면으로 돌아갈까요?", new Action(() => LoadMenuScene()));
        AudioManager.Instance.PlayEffectSound("button_click_01");
    }

    public void ShowSaveConfirmWindow()
    {
        SaveConfirmWindow.SetActive(true);
    }

    public void CloseSaveConfirmWindow()
    {
        SaveConfirmWindow.SetActive(false);
    }

    public void SaveStage()
	{
		PlayfabManager.Instance.SaveStage(MapNum);
        GameoverWindow.transform.Find("SaveStagePanel").Find("SaveStageDescription").GetComponent<Text>().text = $"저장된 스테이지 : Stage{MapNum}";
        SaveConfirmWindow.SetActive(false);
        AudioManager.Instance.PlayEffectSound("button_click_01");
    }

    private void SetConfirmWindow(string _text, Action _action)
    {
        ConfirmWindow.SetActive(true);

        var confirmBtn = ConfirmWindow.transform.Find("Buttons").Find("ConfirmButton").GetComponent<Button>();
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(() => {
            _action();
        });

        ConfirmWindow.transform.Find("Content").GetComponent<Text>().text = _text;
    }

	public void CloseConfirmWindow()
    {
        ConfirmWindow.SetActive(false);
        AudioManager.Instance.PlayEffectSound("button_click_02");
    }

    private void GameRestart()
    {
        HealthBarChanged(200);

        SceneManager.LoadScene("GameScene");
        ChangeTimeScale(1);
    }
    
    private void LoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
        ChangeTimeScale(1);
    }

    public void ShowAd()
	{
		AudioManager.Instance.PlayEffectSound("button_click_01");

#if UNITY_EDITOR || UNITY_STANDALONE
		Revival();
		GameoverWindow.SetActive(false);
#elif UNITY_ANDROID
        Debug.Log("1");
		if (PlayfabManager.Instance.NoAd)
		{
            Debug.Log("2");
			Revival();
            Debug.Log("3");
			GameoverWindow.SetActive(false);
            Debug.Log("4");
		} else
		{
            Debug.Log("5");
			MobileAdManager.Instance?.ShowRewardAd();
            Debug.Log("6");
		}
#endif
    }

    private void Revival()
	{
		HealthBarChanged(200);
		RevivalNum += 1;
		ChangeTimeScale(1);
        Player.GetComponent<Animator>().SetBool("IsDead", false);
    }

	private void OnRewardEarned()
	{
		Revival();
		GameoverWindow.SetActive(false);
	}

	private bool IsDead
    {
        get
        {
            return Time.timeScale != 0 && Health <= 0 && Player.GetComponent<Rigidbody2D>().velocity.magnitude <= 0.01;
        }
    }

    private string TimeText
    {
        get
        {
            float min = Mathf.Floor(time / 60);
            float seconds = time % 60;
            return $"{min.ToString("00") }:{seconds.ToString("00.00")}";
        }
    }
}

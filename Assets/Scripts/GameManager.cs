using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject MessageWindow;
    [SerializeField] private GameObject GameoverWindow;
    [SerializeField] private GameObject PauseWindow;
    //[SerializeField] private GameObject GameEndingWindow;
    [SerializeField] private GameObject GameFinishedWindow;

    //[SerializeField] private Slider BgmSlider;
    //[SerializeField] private Slider EffectSlider;
    //[SerializeField] private Toggle ShowStoryToggle;
    //[SerializeField] private Text ShowStoryDescription;

    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject LossHealthBar;

    //// StoryWindow
    //[SerializeField] private GameObject StoryWindow;

    //private GameObject BoneSpeech;
    //private Text BoneEmoticon;
    //private TextMeshProUGUI BoneText;
    //private GameObject CellSpeech;
    //private Text CellEmoticon;
    //private TextMeshProUGUI CellText;

    //private int currentDialogIndex;
    //private string currentDialog;
    //private TextMeshProUGUI currentPlayingTextMeshPro;
    //private List<string> DialogList;
    //private List<string> EmoticonList;

    //[SerializeField] private GameObject Audio;
    //public static BgmAudio BgmAudio;
    //public static EffectAudio EffectAudio;

    [SerializeField] private GameObject Player;

    public static float Health = 100;
    public static float RecentHeal = 0;
    public static int RecentVirus = 0;

    private float time;

    [SerializeField] private int StartMapNum = 1;
    [SerializeField] private int EndMapNum;
    public static int EndedMapNum;
    public static int MapNum;
    public static int PassedMapNum;
    private int RevivalNum;
    private bool IsAnimatingDead;

    private void Awake()
    {
        Health = 100;
        RecentHeal = 0;
        RecentVirus = 0;
        RevivalNum = 0;
        EndedMapNum = EndMapNum;
        MapNum = StartMapNum;
        PassedMapNum = StartMapNum - 1;

        Chest.IsOpenChestList.Clear();
        for (int i = 0; i < 10; i++)
        {
            Chest.IsOpenChestList.Add(false);
        }

        PlayerObserver.OnHealthChanged += HealthBarChanged;
        PlayerObserver.OnLossHealthChanged += LossHealthBarChanged;
        //PlayerObserver.OnGameFinished += GameOver;

        Instantiate(Resources.Load($"Map/Map_{MapNum}_"));
    }

    private void OnDestroy()
    {
        PlayerObserver.OnHealthChanged -= HealthBarChanged;
        PlayerObserver.OnLossHealthChanged -= LossHealthBarChanged;
        //PlayerObserver.OnGameFinished -= GameOver;

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (Input.GetKey(KeyCode.P)) GamePause();
        if (Input.GetKey(KeyCode.C)) Player.transform.position = new Vector3(30, 0, 0);
        if (Input.GetKey(KeyCode.O))
        {
            for (int i = 0; i < Chest.IsOpenChestList.Count; i++)
            {
                Chest.IsOpenChestList[i] = true;
            }
        }

        if (IsDead && !IsAnimatingDead) StartCoroutine(GameOver());

    }

    public void GamePause()
    {
        if (!MessageWindow.activeSelf && !GameoverWindow.activeSelf)
        {
            if (PauseWindow.activeSelf)
            {
                AudioManager.EffectAudio.PlayEffectSound("button_click_02");
                PauseWindow.SetActive(false);
                ChangeTimeScale(1);
            }
            else
            {
                AudioManager.EffectAudio.PlayEffectSound("button_click_01");
                PauseWindow.SetActive(true);
                ChangeTimeScale(0);
            }

            PauseWindow.transform.Find("PauseTimerText").GetComponent<Text>().text = $"{time.ToString("F")} | {RevivalNum}회 부활";
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
        var playerScale = (0.8f + 0.002f * Health) * 0.6f;

        Player.transform.localScale = new Vector3(Mathf.Sign(Player.transform.position.x) * playerScale, playerScale);
    }

    private IEnumerator GameOver()
    {
        IsAnimatingDead = true;

        yield return new WaitForSeconds(1.75f);

        ChangeTimeScale(0);

        GameoverWindow.transform.Find("GameoverTimerText").GetComponent<Text>().text = $"{time.ToString("F")} | {RevivalNum}회 부활";
        GameoverWindow.SetActive(true);

        IsAnimatingDead = false;
    }

    public void ClickRestartBtn()
    {
        SetGameFinishedWindow(true);
        AudioManager.EffectAudio.PlayEffectSound("button_click_01");
    }

    public void ClickMenuBtn()
    {
        SetGameFinishedWindow(false);
        AudioManager.EffectAudio.PlayEffectSound("button_click_01");
    }

    private void SetGameFinishedWindow(bool isRestartBtn)
    {
        GameFinishedWindow.SetActive(true);

        var confirmBtn = GameFinishedWindow.transform.Find("Buttons").Find("ConfirmButton").GetComponent<Button>();
        confirmBtn.onClick.RemoveAllListeners();

        var content = GameFinishedWindow.transform.Find("Content").GetComponent<Text>();
        if (isRestartBtn)
        {
            confirmBtn.onClick.AddListener(GameRestart);
            content.text = "처음으로 돌아가면 지금 위치에서는 시작하지 못 합니다.\n\n첫 스테이지로 돌아갈까요?";
        }
        else
        {
            confirmBtn.onClick.AddListener(LoadMenuScene);
            content.text = "홈화면으로 돌아가면 지금 위치에서는 시작하지 못 합니다.\n\n홈 화면으로 돌아갈까요?";
        }
    }

    public void CloseGameFinishedWindow()
    {
        GameFinishedWindow.SetActive(false);
        AudioManager.EffectAudio.PlayEffectSound("button_click_02");
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
        HealthBarChanged(200);

        RevivalNum += 1;

        AudioManager.EffectAudio.PlayEffectSound("button_click_01");

        ChangeTimeScale(1);

        GameoverWindow.SetActive(false);
    }

    private bool IsDead
    {
        get
        {
            return Time.timeScale != 0 && Health <= 0 && Player.GetComponent<Rigidbody2D>().velocity.magnitude <= 0.01;
        }
    }
}

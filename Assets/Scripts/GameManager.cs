using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject MessageWindow;
    [SerializeField] private GameObject GameoverWindow;
    [SerializeField] private GameObject PauseWindow;

    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject LossHealthBar;

    // StoryWindow
    [SerializeField] private GameObject StoryWindow;

    private GameObject BoneSpeech;
    private Text BoneEmoticon;
    private TextMeshProUGUI BoneText;
    private GameObject CellSpeech;
    private Text CellEmoticon;
    private TextMeshProUGUI CellText;

    private int currentDialogIndex;
    private string currentDialog;
    private TextMeshProUGUI currentPlayingTextMeshPro;
    private List<string> DialogList;
    private List<string> EmoticonList;

    [SerializeField] private GameObject Audio;
    public static BgmAudio BgmAudio;
    public static EffectAudio EffectAudio;

    [SerializeField] private GameObject Player;

    public static float Health = 100;
    public static int RecentHeal = 0;
    public static int RecentVirus = 0;

    private float time;

    [SerializeField] private int StartMapNum = 1;
    [SerializeField] private int EndMapNum;
    public static int EndedMapNum;
    public static int MapNum;
    public static int PassedMapNum;

    private void Awake()
    {
        Health = 100;
        RecentHeal = 0;
        RecentVirus = 0;
        EndedMapNum = EndMapNum;
        MapNum = StartMapNum;
        PassedMapNum = StartMapNum - 1;

        if (GameObject.Find("Audio(Clone)") == null) DontDestroyOnLoad(Instantiate(Audio));

        PlayerObserver.OnHealthChanged += HealthBarChanged;
        PlayerObserver.OnLossHealthChanged += LossHealthBarChanged;
        PlayerObserver.OnChestOpened += ChestOpened;
        PlayerObserver.OnGameFinished += GameOver;

        MapManager.OnStoryShowed += StoryWindowOpened;

        // Get GameObject of StoryWindow
        BoneSpeech = StoryWindow.transform.Find("BoneSpeech").gameObject;
        BoneEmoticon = BoneSpeech.transform.Find("BoneEmoticon").GetComponent<Text>();
        BoneText = BoneSpeech.transform.Find("BoneText").GetComponent<TextMeshProUGUI>();
        CellSpeech = StoryWindow.transform.Find("CellSpeech").gameObject;
        CellEmoticon = CellSpeech.transform.Find("CellEmoticon").GetComponent<Text>();
        CellText = CellSpeech.transform.Find("CellText").GetComponent<TextMeshProUGUI>();

        Instantiate(Resources.Load($"Map/Map_{MapNum}_"));
    }

    // Start is called before the first frame update
    void Start()
    {
        BgmAudio = GameObject.Find("BgmAudio").GetComponent<BgmAudio>();
        EffectAudio = GameObject.Find("EffectAudio").GetComponent<EffectAudio>();

        BgmAudio.StartGameBgm();
    }

    private void OnDestroy()
    {
        PlayerObserver.OnHealthChanged -= HealthBarChanged;
        PlayerObserver.OnLossHealthChanged -= LossHealthBarChanged;
        PlayerObserver.OnChestOpened -= ChestOpened;
        PlayerObserver.OnGameFinished -= GameOver;

        MapManager.OnStoryShowed -= StoryWindowOpened;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (Input.GetKey(KeyCode.P)) GamePause();

        if (Time.timeScale != 0 && Health <= 0 && Player.GetComponent<Rigidbody2D>().velocity.magnitude <= 0.01) GameOver();
    }

    public void GamePause()
    {
        if (!MessageWindow.activeSelf && !GameoverWindow.activeSelf)
        {

            if (PauseWindow.activeSelf)
            {
                EffectAudio.PlayEffectSound("button_click_02");
                PauseWindow.SetActive(false);
                ChangeTimeScale(1);
            }
            else
            {
                EffectAudio.PlayEffectSound("button_click_01");
                PauseWindow.SetActive(true);
                ChangeTimeScale(0);
            }

            PauseWindow.transform.Find("PauseTimerText").GetComponent<Text>().text = time.ToString("F");
        }
    }

    private void ChangeTimeScale(int _timeScale)
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
        var playerScale = 0.8f + 0.002f * Health;

        Player.transform.localScale = new Vector3(Mathf.Sign(Player.transform.position.x) * playerScale, playerScale);
    }

    private void ChestOpened(string message)
    {
        MessageWindow.SetActive(true);
        MessageWindow.transform.Find("Content").GetComponent<Text>().text = message;
        ChangeTimeScale(0);
    }

    private void GameOver()
    {
        ChangeTimeScale(0);

        GameoverWindow.SetActive(true);

        GameoverWindow.transform.Find("GameoverTimerText").GetComponent<Text>().text = time.ToString("F");
    }

    public void CloseMessageWindow()
    {
        EffectAudio.PlayEffectSound("button_click_02");
        MessageWindow.SetActive(false);
        ChangeTimeScale(1);
    }

    public void GameRestart()
    {
        HealthBarChanged(200);

        EffectAudio.PlayEffectSound("button_click_01");

        SceneManager.LoadScene("GameScene");
        ChangeTimeScale(1);
    }
    
    public void LoadMenuScene()
    {
        EffectAudio.PlayEffectSound("button_click_01");
        SceneManager.LoadScene("MenuScene");
        ChangeTimeScale(1);
    }

    public void ShowAd()
    {
        HealthBarChanged(200);

        EffectAudio.PlayEffectSound("button_click_01");

        ChangeTimeScale(1);

        GameoverWindow.SetActive(false);
    }

    public void ShowStory(List<string> dialogList, List<string> emoticonList)
    {
        DialogList = dialogList;
        EmoticonList = emoticonList;

        ChangeTimeScale(0);

        currentDialogIndex = 0;

        //ShowEmoticon();

        ShowDialog();

    }

    private void ShowEmoticon()
    {
        if (EmoticonList.Count > 0)
        {
            var currentEmoticon = EmoticonList[currentDialogIndex];

            switch (currentEmoticon.Substring(0, 2))
            {
                case "셀:":
                    CellEmoticon.text = currentEmoticon.Substring(2);
                    break;
                case "본:":
                    BoneEmoticon.text = currentEmoticon.Substring(2);
                    break;
            }
        }
    }

    private void ShowDialog()
    {
        currentDialog = DialogList[currentDialogIndex];

        switch (currentDialog.Substring(0, 2))
        {
            case "셀:":
                CellSpeech.SetActive(true);
                currentPlayingTextMeshPro = CellText;
                StartCoroutine(ShowText(CellText, currentDialog.Substring(2)));
                break;
            case "본:":
                BoneSpeech.SetActive(true);
                currentPlayingTextMeshPro = BoneText;
                StartCoroutine(ShowText(BoneText, currentDialog.Substring(2)));
                break;
        }
    }

    private IEnumerator ShowText(TextMeshProUGUI textMeshPro, string dialog)
    {
        textMeshPro.text = string.Empty;
        foreach (char d in dialog)
        {
            if (textMeshPro.text.Length != dialog.Length && StoryWindow.activeSelf)
            {
                textMeshPro.text += d;
                EffectAudio.PlayEffectSound("typing");
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.pointerCurrentRaycast.gameObject.name)
        {
            case "StoryWindow":
            case "BoneSpeech":
            case "BoneEmoticon":
            case "BoneText":
            case "CellSpeech":
            case "CellEmoticon":
            case "CellText":
                if (currentPlayingTextMeshPro.text.Length == currentDialog.Substring(2).Length)
                {
                    if (DialogList.Count != currentDialogIndex + 1)
                    {
                        currentDialogIndex += 1;
                        ShowDialog();
                    }
                    else
                    {
                        CellSpeech.SetActive(false);
                        BoneSpeech.SetActive(false);
                        StoryWindow.SetActive(false);
                        currentDialog = string.Empty;

                        ChangeTimeScale(1);
                    }
                }
                else
                {
                    currentPlayingTextMeshPro.text = currentDialog.Substring(2);
                }
                break;
        }
    }

    private void StoryWindowOpened(List<string> dialogList, List<string> emoticonList)
    {
        if (MapNum > PassedMapNum)
        {
            StoryWindow.SetActive(true);
            ShowStory(dialogList, emoticonList);
        }
    }

    public void SkipStory()
    {
        CellSpeech.SetActive(false);
        BoneSpeech.SetActive(false);
        BoneText.text = string.Empty;
        CellText.text = string.Empty;
        StoryWindow.SetActive(false);

        ChangeTimeScale(1);
    }
}

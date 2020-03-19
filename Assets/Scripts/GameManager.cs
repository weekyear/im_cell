using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IPointerClickHandler
{
    public GameObject MessageWindow;
    public GameObject GameoverWindow;
    public GameObject PauseWindow;

    public GameObject HealthBar;
    public GameObject LossHealthBar;

    // StoryWindow
    public GameObject StoryWindow;

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

    public GameObject Audio;
    public static BgmAudio BgmAudio;
    public static EffectAudio EffectAudio;

    [SerializeField] private GameObject Player;

    public static float Health = 100;

    private float time;

    [SerializeField] private int StartMapNum = 1;
    public static int MapNum;
    public static int PassedMapNum;

    private void Awake()
    {
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
    }

    // Start is called before the first frame update
    void Start()
    {
        MapNum = StartMapNum;
        PassedMapNum = StartMapNum - 1;

        Instantiate(Resources.Load($"Map/Map_{MapNum}_"));

        BgmAudio = GameObject.Find("BgmAudio").GetComponent<BgmAudio>();
        EffectAudio = GameObject.Find("EffectAudio").GetComponent<EffectAudio>();

        BgmAudio.StartGameBgm();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (Input.GetKey(KeyCode.Escape)) GamePause();

        if (Time.timeScale != 0 && Health <= 0 && Player.GetComponent<Rigidbody2D>().velocity.magnitude <= 0) GameOver();
    }

    public void GamePause()
    {
        if (!MessageWindow.activeSelf && !GameoverWindow.activeSelf)
        {
            ChangeTimeScale();

            if (PauseWindow.activeSelf)
            {
                EffectAudio.PlayEffectSound("button_click_02");
                PauseWindow.SetActive(false);
            }
            else
            {
                EffectAudio.PlayEffectSound("button_click_01");
                PauseWindow.SetActive(true);
            }

            PauseWindow.transform.Find("PauseTimerText").GetComponent<Text>().text = time.ToString("F");
        }
    }

    private void ChangeTimeScale()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
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
        if (amount >= Health)
        {
            sizeOfBar.x = Health * -7.25f;
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
        ChangeTimeScale();
    }

    private void GameOver()
    {
        ChangeTimeScale();

        GameoverWindow.SetActive(true);

        PauseWindow.transform.Find("GameoverTimerText").GetComponent<Text>().text = time.ToString("F");
    }

    public void CloseMessageWindow()
    {
        EffectAudio.PlayEffectSound("button_click_02");
        MessageWindow.SetActive(false);
        ChangeTimeScale();
    }

    public void GameRestart()
    {
        EffectAudio.PlayEffectSound("button_click_01");
        SceneManager.LoadScene("GameScene");
    }
    
    public void LoadMenuScene()
    {
        EffectAudio.PlayEffectSound("button_click_01");
        SceneManager.LoadScene("MenuScene");
    }

    public void ShowAd()
    {
        HealthBarChanged(200);

        EffectAudio.PlayEffectSound("button_click_01");

        ChangeTimeScale();

        GameoverWindow.SetActive(false);
    }

    public void ShowStory(List<string> dialogList, List<string> emoticonList)
    {
        DialogList = dialogList;
        EmoticonList = emoticonList;

        ChangeTimeScale();

        currentDialogIndex = 0;

        ShowEmoticon();

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
            if (textMeshPro.text.Length != dialog.Length)
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

                        ChangeTimeScale();
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

        ChangeTimeScale();
    }
}

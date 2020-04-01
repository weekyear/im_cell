using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject GameEndingWindow;
    [SerializeField] private GameObject MessageWindow;

    // StoryWindow
    [SerializeField] private GameObject StoryWindow;
    [SerializeField] private GameObject Player;

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

    public static bool IsEndingCredit;
    private bool IsEndedCredit;
    private void Awake()
    {
        MapManager.OnStoryShowed += StoryWindowOpened;
        PlayerObserver.OnChestOpened += ChestOpened;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        // IsEndingCredit

        if (IsEndingCredit)
        {
            var creditSpeed = 1f;
            if (Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                creditSpeed = 10f;
            }

            var endingImage = GameEndingWindow.GetComponent<Image>();
            if (endingImage.color.a < 0.53f) endingImage.color = new Color(endingImage.color.r, endingImage.color.g, endingImage.color.b, endingImage.color.a + 0.2f * creditSpeed * Time.deltaTime);
            var endingTransform = GameEndingWindow.transform;
            endingTransform.position = new Vector3(endingTransform.position.x, endingTransform.position.y + 50f * creditSpeed * Time.deltaTime, endingTransform.position.z);

            if (endingTransform.position.y > 3500f)
            {
                endingImage.color = new Color(endingImage.color.r, endingImage.color.g, endingImage.color.b, endingImage.color.a + 0.1f * creditSpeed * Time.deltaTime);
                if (endingImage.color.a > 0.98f && !IsEndedCredit) StartCoroutine(WaitAndLoadMenuScene());
            }
        }
    }

    private void OnDestroy()
    {
        PlayerObserver.OnChestOpened -= ChestOpened;
        MapManager.OnStoryShowed -= StoryWindowOpened;
    }

    private void GameTrueEnding()
    {
        GameObject.Find("CM vcam").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 3.5f;
        AudioManager.BgmAudio.StartGameBgm_TrueEnding();

        var endingText = GameEndingWindow.transform.Find("EndingText").gameObject;
        var endingTitle = endingText.transform.Find("EndingTitle").GetComponent<Text>();
        var endingContent = endingText.transform.Find("EndingContent").GetComponent<Text>();

        endingTitle.text = "True Ending";
        endingContent.text = "<잔혹한 새하얀 진실>";
        StartCoroutine(ShowEndingCredit());
    }

    private IEnumerator ShowEndingCredit()
    {
        gameObject.transform.Find("StageText").gameObject.SetActive(false);
        gameObject.transform.Find("HealthHolder").gameObject.SetActive(false);
        gameObject.transform.Find("PauseButton").gameObject.SetActive(false);

        GameEndingWindow.SetActive(true);

        yield return new WaitForSeconds(2f);

        IsEndingCredit = true;
        IsEndedCredit = false;
    }

    private void ChestOpened(string message)
    {
        MessageWindow.SetActive(true);
        AudioManager.EffectAudio.PlayEffectSound("booksori");
        MessageWindow.transform.Find("Content").GetComponent<Text>().text = message;
        GameManager.ChangeTimeScale(0);

        if (GameManager.MapNum == 51)
        {
            Player.GetComponent<Animator>().SetBool("IsWhite", true);
            MessageWindow.transform.Find("QuitButton").GetComponent<Button>().onClick.AddListener(GameTrueEnding);
            GameManager.ChangeTimeScale(1);
        }
    }

    public void CloseMessageWindow()
    {
        AudioManager.EffectAudio.PlayEffectSound("button_click_02");
        MessageWindow.SetActive(false);
        GameManager.ChangeTimeScale(1);
    }

    private IEnumerator WaitAndLoadMenuScene()
    {
        IsEndedCredit = true;
        IsEndingCredit = false;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("MenuScene");
        GameManager.ChangeTimeScale(1);
    }

    public void ShowStory(List<string> dialogList, List<string> emoticonList)
    {
        DialogList = dialogList;
        EmoticonList = emoticonList;

        GameManager.ChangeTimeScale(0);

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
                AudioManager.EffectAudio.PlayEffectSound("typing");
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
                        SkipStory();
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
        if (GameManager.MapNum > GameManager.PassedMapNum)
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
        currentDialog = string.Empty;
        GameManager.ChangeTimeScale(1);

        if (GameManager.MapNum > 50 && Chest.IsOpenChestList.Contains(false))
        {
            StartCoroutine(ShowEndingCredit());
        }
    }
}

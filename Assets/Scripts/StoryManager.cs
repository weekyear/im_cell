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
    public static bool IsEndedCredit;
    private void Awake()
    {
        PlayerObserver.OnChestOpened += ChestOpened;
        PlayerObserver.OnGameEndingShowed += GameNormalEnding;
        MapManager.OnStoryShowed += StoryWindowOpened;

        // Get GameObject of StoryWindow
        BoneSpeech = StoryWindow.transform.Find("BoneSpeech").gameObject;
        BoneEmoticon = BoneSpeech.transform.Find("BoneEmoticon").GetComponent<Text>();
        BoneText = BoneSpeech.transform.Find("BoneText").GetComponent<TextMeshProUGUI>();
        CellSpeech = StoryWindow.transform.Find("CellSpeech").gameObject;
        CellEmoticon = CellSpeech.transform.Find("CellEmoticon").GetComponent<Text>();
        CellText = CellSpeech.transform.Find("CellText").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (IsEndingCredit)
        {
            GameEndingWindow.SetActive(true);

            var creditSpeed = 1f;
            if (Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                creditSpeed = 12f;
            }

            var endingImage = GameEndingWindow.GetComponent<Image>();
            if (endingImage.color.a < 0.53f) endingImage.color = new Color(endingImage.color.r, endingImage.color.g, endingImage.color.b, endingImage.color.a + 0.2f * creditSpeed * Time.deltaTime);
            var endingTransform = GameEndingWindow.transform.Find("EndingText");
            endingTransform.position = new Vector2(endingTransform.position.x, endingTransform.position.y + 50f * creditSpeed * Time.deltaTime);

            if (endingTransform.position.y > endingTransform.GetComponent<RectTransform>().rect.height + 1100f)
            {
                endingImage.color = new Color(endingImage.color.r, endingImage.color.g, endingImage.color.b, endingImage.color.a + 0.1f * creditSpeed * Time.deltaTime);
                if (endingImage.color.a >= 1.00f && !IsEndedCredit) StartCoroutine(WaitAndLoadMenuScene());
            }
        }
    }

    private void OnDestroy()
    {
        PlayerObserver.OnChestOpened -= ChestOpened;
        PlayerObserver.OnGameEndingShowed -= GameNormalEnding;
        MapManager.OnStoryShowed -= StoryWindowOpened;
    }

    private void GameTrueEnding()
    {
        GameObject.Find("CM vcam").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 3.5f;
        AudioManager.Instance.StartGameBgm_TrueEnding();

        var endingText = GameEndingWindow.transform.Find("EndingText").gameObject;
        var endingTitle = endingText.transform.Find("EndingTitle").GetComponent<Text>();
        var endingContent = endingText.transform.Find("EndingContent").GetComponent<Text>();

        endingTitle.text = "True Ending";
        endingContent.text = "<잔혹하게 다가온 새하얀 진실>";
        StartCoroutine(ShowEndingCredit());
    }

    private IEnumerator ShowEndingCredit()
    {
        gameObject.transform.Find("StageText").gameObject.SetActive(false);
        gameObject.transform.Find("HealthHolder").gameObject.SetActive(false);
        gameObject.transform.Find("PauseButton").gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        IsEndingCredit = true;
        IsEndedCredit = false;
    }

    private void GameNormalEnding()
    {
        if (GameManager.MapNum > 50 && Chest.IsOpenChestList.Contains(false))
        {
            StartCoroutine(ShowEndingCredit());
        }
    }

    private void ChestOpened(string message)
    {
        MessageWindow.SetActive(true);
        AudioManager.Instance.PlayEffectSound("booksori");
        MessageWindow.transform.Find("Content").GetComponent<Text>().text = message;
        GameManager.ChangeTimeScale(0);

        if (GameManager.MapNum == 51)
        {
            GameManager.ChangeTimeScale(1);
            Player.GetComponent<Animator>().SetBool("IsWhite", true);
            Player.gameObject.layer = LayerMask.NameToLayer("PlayerInvincible");
            MessageWindow.transform.Find("QuitButton").GetComponent<Button>().onClick.AddListener(GameTrueEnding);
        }
    }

    public void CloseMessageWindow()
    {
        AudioManager.Instance.PlayEffectSound("button_click_02");
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
                AudioManager.Instance.PlayEffectSound("typing");
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
		StoryWindow.SetActive(true);
		ShowStory(dialogList, emoticonList);
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

        GameNormalEnding();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoryWindow : MonoBehaviour
{
    [SerializeField] private Text Title;
    [SerializeField] private Text Content;

    private int currentStoryNum;

    public void ShowWindow()
    {
        if (!PlayfabManager.Instance.IsLogin)
        {
            ToastManager.Instance.Show("로그인이 필요합니다");
            return;
        }

        var storyNum = PlayerPrefs.GetInt("StoryNum", 0);
        if (PlayfabManager.Instance.Level > storyNum)
        {
            storyNum = PlayfabManager.Instance.Level;
            PlayerPrefs.SetInt("StoryNum", storyNum);
        }

        if (storyNum == 0) Content.text = "게임을 시작해주세요";

        ShowStoryText(storyNum);

        gameObject.SetActive(true);
        AudioManager.Instance.PlayEffectSound("button_click_01");
    }
    public void HideWindow()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.PlayEffectSound("button_click_02");
    }

    private void ShowStoryText(int mapNum)
    {
        currentStoryNum = mapNum;
        Title.text = $"Story {mapNum}";
        Content.text = Lean.Localization.LeanLocalization.GetTranslationText($"Story{mapNum}");
    }

    public void ShowNextStoryText()
    {
        if (currentStoryNum < PlayerPrefs.GetInt("StoryNum", 0))
        {
            Debug.Log("ShowNextStoryText_1");
            ShowStoryText(currentStoryNum + 1);
        }
        AudioManager.Instance.PlayEffectSound("button_click_01");
    }

    public void ShowPreviousStoryText()
    {
        if (currentStoryNum > 1)
        {
            ShowStoryText(currentStoryNum - 1);
        }
        AudioManager.Instance.PlayEffectSound("button_click_01");
    }
}

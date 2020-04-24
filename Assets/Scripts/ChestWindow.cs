using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChestWindow : MonoBehaviour
{
    [SerializeField] private Text Title;
    [SerializeField] private Text Content;

    private int currentChestIndex;
    private List<bool> chestCheckList = new List<bool>();
    // Start is called before the first frame update

    public void ShowWindow()
    {
        if (!PlayfabManager.Instance.IsLogin)
        {
            ToastManager.Instance.Show("로그인이 필요합니다");
            return;
        }

        for (int i = 0; i < 11; i++)
        {
            bool isOpen = false;
            if (PlayerPrefs.GetInt($"Chest{i}", 0) == 1 || PlayfabManager.Instance.ChestList[i]) isOpen = true;
            chestCheckList.Add(isOpen);
        }

        ShowChestText(0);
        gameObject.SetActive(true);
    }
    public void HideWindow()
    {
        gameObject.SetActive(false);
    }

    private void ShowChestText(int chestIndex)
    {
        var content = Lean.Localization.LeanLocalization.GetTranslationText($"Chest");
        var Contents = content.Split('\n').ToList();
        currentChestIndex = chestIndex;
        Title.text = $"박스 {chestIndex + 1}";
        if (chestCheckList[currentChestIndex] == true)
        {
            Content.text = Contents[currentChestIndex];
            Content.alignment = TextAnchor.MiddleLeft;
        }
        else
        {
            Content.text = "-";
            Content.alignment = TextAnchor.MiddleCenter;
        }
    }

    public void ShowNextChestText()
    {
        if (currentChestIndex < 10)
        {
            ShowChestText(currentChestIndex + 1);
        }
    }

    public void ShowPreviousChestText()
    {
        if (currentChestIndex > 0)
        {
            ShowChestText(currentChestIndex - 1);
        }
    }
}

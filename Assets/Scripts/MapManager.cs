using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] List<string> DialogList;
    [SerializeField] List<string> EmoticonList;
    // Start is called before the first frame update
    private static List<string> _dialogList;
    private static List<string> _emoticonList;
    public static event Action<List<string>, List<string>> OnStoryShowed;
    void Start()
    {
        if (IsShownStoryAlways || !IsShownStoryAlways && PlayerPrefs.GetInt("ShownMapNum", 0) < GameManager.MapNum)
        {
            _dialogList = DialogList;
            _emoticonList = EmoticonList;

            StoryShowed();

            PlayerPrefs.SetInt("ShownMapNum", GameManager.MapNum);
        }
    }


    public static void StoryShowed()
    {
        OnStoryShowed?.Invoke(_dialogList, _emoticonList);
    }

    private bool IsShownStoryAlways
    {
        get { return PlayerPrefs.GetInt("IsShownStoryAlways", 0) == 1; }
    }
}

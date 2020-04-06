using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static List<string> DialogList;
    private static List<string> EmoticonList;
    public static event Action<List<string>, List<string>> OnStoryShowed;
    public static event Action OnVirusBeKilled;

    void Start()
	{
		OnVirusBeKilled += RemoveMaceWall;

		AudioManager.Instance?.StartBgm_GameScene();

		if (GameManager.MapNum > 45)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = new Color(0.5f, 0.75f, 0.75f, 0.56f);
        }

		if (GameManager.MapNum > 50)
		{
			PlayfabManager.Instance.SaveLevel(51);
			PlayfabManager.Instance.ReportScore(GameManager.time);
			PlayerObserver.GameEndingShowed();
		}

		if (SettingManager.IsShownStoryAlways || IsFirstStory)
        {
            var dialog = Lean.Localization.LeanLocalization.GetTranslationText($"Story{GameManager.MapNum}");
            DialogList = dialog.Split('\n').ToList();
            StoryShowed();
		}

		if (GameManager.PassedMapNum < GameManager.MapNum) GameManager.PassedMapNum = GameManager.MapNum;
	}

    private void OnDestroy()
    {
        OnVirusBeKilled -= RemoveMaceWall;
    }

    // Event
    public static void StoryShowed()
    {
        OnStoryShowed?.Invoke(DialogList, EmoticonList);
    }
    
    public static void VirusBeKilled()
    {
        OnVirusBeKilled?.Invoke();
    }

    private void RemoveMaceWall()
    {
        if (!IsExistVirus)
        {
            var maceWall = gameObject.transform.Find("MaceWall");
            Destroy(maceWall.gameObject);
            GameManager.RecentVirus = GameManager.MapNum;
        }
    }

    private bool IsExistVirus
    {
        get
        {
            for (int i = 0; i < 6; i++)
            {
                if (gameObject.transform.Find($"Virus ({i})")?.Find("Virus_Sprite").gameObject.activeSelf == true) return true;
            }

            return false;
        }
    }

	public bool IsFirstStory => GameManager.MapNum > PlayfabManager.Instance.Level && GameManager.MapNum > GameManager.PassedMapNum;
}

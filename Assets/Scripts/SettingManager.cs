using UnityEngine;

public class SettingManager
{
	private const string BGM_VOLUME_KEY = "BgmVolume";
	private const string EFFECT_VOLUME_KEY = "EffectVolume";
	private const string STORY_KEY = "IsShownStoryAlways";

	public static bool IsShownStoryAlways
	{
		get
		{
			return PlayerPrefs.GetInt(STORY_KEY, 1) == 1;
		}
		set
		{
			PlayerPrefs.SetInt(STORY_KEY, value? 1: 0);
		}
	}

	public static float BgmVolume
	{
		get
		{
			return PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f);
		}
		set
		{
			PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
			AudioManager.Instance.UpdateBgmVolume();
		}
	}

	public static float EffectVolume
	{
		get
		{
			return PlayerPrefs.GetFloat(EFFECT_VOLUME_KEY, 0.5f);
		}
		set
		{
			PlayerPrefs.SetFloat(EFFECT_VOLUME_KEY, value);
			AudioManager.Instance.UpdateEffectVolume();
		}
	}
}

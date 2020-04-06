using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
	[SerializeField] private Slider BgmSlider;
	[SerializeField] private Slider EffectSlider;
	[SerializeField] private Toggle ShowStoryToggle;
	[SerializeField] private Text ShowStoryDescription;

	private void Awake()
	{
		BgmSlider.onValueChanged.AddListener(value => SettingManager.BgmVolume = value);

		EffectSlider.onValueChanged.AddListener(value =>
		{
			SettingManager.EffectVolume = value;
			if (gameObject.activeSelf) AudioManager.Instance.PlayEffectSound("jump_06");
		});

		ShowStoryToggle.onValueChanged.AddListener(value =>
		{
			if (value)
			{
				SettingManager.IsShownStoryAlways = true;
				ShowStoryDescription.text = "해당 맵의 스토리를 항상 표시합니다.";
				if (gameObject.activeSelf) AudioManager.Instance.PlayEffectSound("button_click_01");
			}
			else
			{
				SettingManager.IsShownStoryAlways = false;
				ShowStoryDescription.text = "한 번 본 스토리는 다시 표시하지 않습니다.";
				if (gameObject.activeSelf) AudioManager.Instance.PlayEffectSound("button_click_02");
			}
		});
	}

	private void OnEnable()
	{
		BgmSlider.value = SettingManager.BgmVolume;
		EffectSlider.value = SettingManager.EffectVolume;
		ShowStoryToggle.isOn = SettingManager.IsShownStoryAlways;
	}
}
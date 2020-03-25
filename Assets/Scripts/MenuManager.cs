using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject SettingMenu;
    public Slider BgmSlider;
    public Slider EffectSlider;
    public Toggle ShowStoryToggle;
    public Text ShowStoryDescription;

    public GameObject Audio;

    private BgmAudio BgmAudio;
    private EffectAudio EffectAudio;


    private void Awake()
    {
        if (GameObject.Find("Audio(Clone)") == null)
        {
            DontDestroyOnLoad(Instantiate(Audio));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        BgmAudio = GameObject.Find("BgmAudio").GetComponent<BgmAudio>();
        EffectAudio = GameObject.Find("EffectAudio").GetComponent<EffectAudio>();

        BgmAudio.StartMenuBgm();

        // SettingMenu
        BgmSlider.value = PlayerPrefs.GetFloat("BgmVolume", 0.5f);
        EffectSlider.value = PlayerPrefs.GetFloat("EffectVolume", 0.5f);

        if (PlayerPrefs.GetInt("IsShownStoryAlways", 0) == 1)
        {
            ShowStoryToggle.isOn = true;
        }
        else
        {
            ShowStoryToggle.isOn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    // SettingMenu
    public void OpenSettingMenu()
    {
        EffectAudio.PlayEffectSound("button_click_01");
        SettingMenu.SetActive(true);
    }

    public void CloseSettingMenu()
    {
        EffectAudio.PlayEffectSound("button_click_02");
        SettingMenu.SetActive(false);
    }

    public void OnBgmSliderValueChanged()
    {
        PlayerPrefs.SetFloat("BgmVolume", BgmSlider.value);
        BgmAudio.SetBgmVolume(BgmSlider.value);
    }
    
    public void OnEffectSliderValueChanged()
    {
        PlayerPrefs.SetFloat("EffectVolume", EffectSlider.value);
        EffectAudio.SetEffectVolume(EffectSlider.value);

        if (SettingMenu.activeSelf) EffectAudio.PlayEffectSound("jump_06");
    }

    public void OnStoryToggleValueChanged()
    {
        if (ShowStoryToggle.isOn)
        {
            PlayerPrefs.SetInt("IsShownStoryAlways", 1);
            ShowStoryDescription.text = "해당 맵의 스토리를 항상 표시합니다.";
            if (SettingMenu.activeSelf) EffectAudio.PlayEffectSound("button_click_01");
        }
        else
        {
            PlayerPrefs.SetInt("IsShownStoryAlways", 0);
            ShowStoryDescription.text = "한 번 본 스토리는 다시 표시하지 않습니다.";
            if (SettingMenu.activeSelf) EffectAudio.PlayEffectSound("button_click_02");
        }
    }


    public void StartGame()
    {
        EffectAudio.PlayEffectSound("button_click_01");
        SceneManager.LoadScene("GameScene");
    }

    public void TurnOffGame()
    {
        EffectAudio.PlayEffectSound("button_click_02");
        Application.Quit();
    }
}
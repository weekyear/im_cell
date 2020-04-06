using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

    [SerializeField] BgmAudio BgmAudio;
    [SerializeField] EffectAudio EffectAudio;

    private void Awake()
    {
        if (Instance != null)
		{
			Destroy(this);
			return;
		} else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
    }
    
    void Start()
	{
		BgmAudio?.SetVolume();
		EffectAudio.SetVolume();
	}

	public void StartMenuBgm() => BgmAudio.StartMenuBgm();

	public void StartBgm_GameScene() => BgmAudio.StartBgm_GameScene();

	public void PlayEffectSound(string fileName) => EffectAudio?.PlayEffectSound(fileName);

	public void UpdateBgmVolume() => BgmAudio.SetVolume();

	public void UpdateEffectVolume() => EffectAudio.SetVolume();

	public void StopBgm() => BgmAudio.StopBgm();

	public void StartGameBgm_TrueEnding() => BgmAudio.StartGameBgm_TrueEnding();
}
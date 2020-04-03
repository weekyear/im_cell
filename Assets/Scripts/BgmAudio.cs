using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmAudio : MonoBehaviour
{
    [SerializeField] private AudioClip MenuBgmClip;
    [SerializeField] private List<AudioClip> GameBgmClips_Spring;
    [SerializeField] private List<AudioClip> GameBgmClips_Autumn;
    [SerializeField] private List<AudioClip> GameBgmClips_Winter;
    [SerializeField] private AudioClip GameBgmClip_Final;
    [SerializeField] private AudioClip GameBgmClip_NormalEnding;
    [SerializeField] private AudioClip GameBgmClip_TrueEnding;

    private int currentBgmClipsNum = -1;

    private AudioSource audioSource;

    private int currentBgmIndex = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        SetBgmVolume(PlayerPrefs.GetFloat("BgmVolume", 0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            currentBgmIndex += 1;

            if (currentBgmIndex > 1) currentBgmIndex = 0;

            switch (currentBgmClipsNum)
            {
                case 1:
                    PlayBgm(GameBgmClips_Spring[currentBgmIndex]);
                    break;
                case 2:
                    PlayBgm(GameBgmClips_Autumn[currentBgmIndex]);
                    break;
                case 3:
                    PlayBgm(GameBgmClips_Winter[currentBgmIndex]);
                    break;
            }
        }
    }

    public void StartBgm_GameScene()
    {
        if (GameManager.MapNum < 21)
        {
            StartGameBgm_Spring();
        }
        else if (GameManager.MapNum < 37)
        {
            StartGameBgm_Autumn();
        }
        else if (GameManager.MapNum < 46)
        {
            StartGameBgm_Winter();
        }
        else if (GameManager.MapNum < 50)
        {
            StartGameBgm_Final();
        }
        else if (GameManager.MapNum < 51)
        {
            StopBgm();
        }
        else
        {
            StartGameBgm_NormalEnding();
        }
    }

    public void StartMenuBgm()
    {
        if (currentBgmClipsNum != 0)
        {
            currentBgmClipsNum = 0;
            audioSource.loop = true;

            PlayBgm(MenuBgmClip);
        }
    }

    private void StartGameBgm_Spring()
    {
        if (currentBgmClipsNum < 1)
        {
            currentBgmClipsNum = 1;
            currentBgmIndex = 0;
            audioSource.loop = false;
            PlayBgm(GameBgmClips_Spring[currentBgmIndex]);
        }
    }

    private void StartGameBgm_Autumn()
    {
        if (currentBgmClipsNum < 2)
        {
            currentBgmClipsNum = 2;
            currentBgmIndex = 0;
            audioSource.loop = false;
            PlayBgm(GameBgmClips_Autumn[currentBgmIndex]);
        }
    }

    private void StartGameBgm_Winter()
    {
        if (currentBgmClipsNum < 3)
        {
            currentBgmClipsNum = 3;
            currentBgmIndex = 0;
            audioSource.loop = false;
            PlayBgm(GameBgmClips_Winter[currentBgmIndex]);
        }
    }

    private void StartGameBgm_Final()
    {
        if (currentBgmClipsNum < 4)
        {
            currentBgmClipsNum = 4;
            audioSource.loop = true;
            PlayBgm(GameBgmClip_Final);
        }
    }

    private void StartGameBgm_NormalEnding()
    {
        audioSource.loop = true;
        PlayBgm(GameBgmClip_NormalEnding);
    }

    public void StartGameBgm_TrueEnding()
    {
        audioSource.loop = true;
        PlayBgm(GameBgmClip_TrueEnding);
    }

    public void SetBgmVolume(float value)
    {
        audioSource.volume = value;
    }

    private void PlayBgm(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void StopBgm()
    {
        audioSource.Stop();
    }
}

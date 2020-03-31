using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmAudio : MonoBehaviour
{
    [SerializeField] private AudioClip MenuBgmClip;
    [SerializeField] private List<AudioClip> GameBgmClips_1;
    [SerializeField] private AudioClip GameBgmClip_2;
    [SerializeField] private AudioClip GameBgmClip_3;
    [SerializeField] private AudioClip GameBgmClip_4;

    private int currentBgmClipsNum;

    private AudioSource audioSource;

    private bool IsGamePlaying;
    private int currentBgmIndex = 0;

    private void Awake()
    {
        currentBgmClipsNum = 0;
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        SetBgmVolume(PlayerPrefs.GetFloat("BgmVolume", 0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGamePlaying && !audioSource.isPlaying)
        {
            currentBgmIndex += 1;

            if (currentBgmIndex > 2) currentBgmIndex = 0;

            PlayBgm(GameBgmClips_1[currentBgmIndex]);
        }
    }

    public void StartMenuBgm()
    {
        audioSource.loop = true;

        PlayBgm(MenuBgmClip);

        IsGamePlaying = false;
    }

    public void StartGameBgm_1()
    {
        if (!IsGamePlaying && currentBgmClipsNum != 1)
        {
            currentBgmClipsNum = 1;
            audioSource.loop = false;
            currentBgmIndex = 0;
            PlayBgm(GameBgmClips_1[currentBgmIndex]);
            IsGamePlaying = true;
        }
    }

    public void StartGameBgm_2()
    {
        if (currentBgmClipsNum != 2)
        {
            currentBgmClipsNum = 2;
            audioSource.loop = true;
            PlayBgm(GameBgmClip_2);
            IsGamePlaying = true;
        }
    }

    public void StartGameBgm_3()
    {
        if (currentBgmClipsNum != 3)
        {
            currentBgmClipsNum = 3;
            audioSource.loop = true;
            PlayBgm(GameBgmClip_3);
            IsGamePlaying = true;
        }
    }
    
    public void StartGameBgm_4()
    {
        if (currentBgmClipsNum != 4)
        {
            currentBgmClipsNum = 4;
            audioSource.loop = true;
            PlayBgm(GameBgmClip_4);
            IsGamePlaying = true;
        }
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
        IsGamePlaying = false;
    }
}

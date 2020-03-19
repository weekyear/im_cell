using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmAudio : MonoBehaviour
{
    public AudioClip MenuBgmClip;
    public List<AudioClip> GameBgmClips;

    private AudioSource audioSource;

    private bool IsGamePlaying;
    private int currentBgmIndex = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGamePlaying && !audioSource.isPlaying)
        {
            currentBgmIndex += 1;

            if (currentBgmIndex > 2) currentBgmIndex = 0;

            PlayBgm(GameBgmClips[currentBgmIndex]);
        }
    }

    public void StartMenuBgm()
    {
        audioSource.loop = true;

        PlayBgm(MenuBgmClip);

        IsGamePlaying = false;
    }

    public void StartGameBgm()
    {
        if (!IsGamePlaying)
        {
            audioSource.loop = false;
            currentBgmIndex = 0;
            PlayBgm(GameBgmClips[currentBgmIndex]);
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
}

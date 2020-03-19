using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAudio : MonoBehaviour
{
    public List<AudioClip> EffectClips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetEffectVolume(PlayerPrefs.GetFloat("EffectVolume"));
    }

    public void SetEffectVolume(float value)
    {
        audioSource.volume = value;
    }

    public void PlayEffectSound(string fileName)
    {
        foreach (var clip in EffectClips)
        {
            if (clip.name.Contains(fileName))
            {
                audioSource.PlayOneShot(clip);
                return;
            }
        }
    }
}

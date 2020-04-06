using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAudio : MonoBehaviour
{
	[SerializeField] AudioSource audioSource;
	[SerializeField] List<AudioClip> EffectClips;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetVolume()
    {
		audioSource.volume = SettingManager.EffectVolume;
    }

    public void PlayEffectSound(string fileName)
    {
        foreach (var clip in EffectClips)
        {
            if (clip.name.Contains(fileName))
            {
                audioSource.PlayOneShot(clip);
                break;
            }
        }
    }
}

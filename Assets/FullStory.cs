using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullStory : MonoBehaviour
{
    public int MIN;
    public int MAX;
    private void OnEnable()
    {
        string story = "";
        for (int i = MIN; i <= MAX; i++)
        {
            story += Lean.Localization.LeanLocalization.GetTranslationText($"map{i}") + "\n\n";
        }

        Debug.LogError(story);
    }
}
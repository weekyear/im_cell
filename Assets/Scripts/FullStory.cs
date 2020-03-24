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
            story += $"<Stage {i}>" + "\n\n" + Lean.Localization.LeanLocalization.GetTranslationText($"Story{i}") + "\n\n";

        }

        Debug.LogError(story);
    }
}
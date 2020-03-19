using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObserver : MonoBehaviour
{
    public static event Action<float> OnLossHealthChanged;
    public static event Action<float> OnHealthChanged;
    public static event Func<IEnumerator> OnDamaged;
    public static event Action<string> OnChestOpened;
    public static event Action OnGameFinished;


    public static void LossHealthChanged(float amountOfChange)
    {
        OnLossHealthChanged?.Invoke(amountOfChange);
    }

    public static void HealthChanged(float amountOfChange)
    {
        OnHealthChanged?.Invoke(amountOfChange);
    }
    public static IEnumerator Damaged()
    {
        return OnDamaged?.Invoke();
    }

    public static void ChestOpened(string message)
    {
        OnChestOpened?.Invoke(message);
    }
    
    public static void GameFinished()
    {
        OnGameFinished?.Invoke();
    }
}

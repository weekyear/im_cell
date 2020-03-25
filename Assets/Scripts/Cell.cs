using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private bool IsLastCell;
    // Start is called before the first frame update
    void Start()
    {
        if (IsLastCell)
        {
            if (GameManager.MapNum <= GameManager.RecentHeal) Destroy(gameObject);
        }
        else
        {
            if (GameManager.MapNum - 0.5f <= GameManager.RecentHeal) Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            Destroy(gameObject);
            PlayerObserver.HealthChanged(50);

            GameManager.RecentHeal = IsLastCell ? GameManager.MapNum : GameManager.MapNum - 0.5f;
            GameManager.EffectAudio.PlayEffectSound("item");
        }
    }

    private bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }
}
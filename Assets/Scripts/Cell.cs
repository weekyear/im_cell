using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private static int RecentHeal = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.MapNum <= RecentHeal)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            Destroy(gameObject);
            PlayerObserver.HealthChanged(40);
            RecentHeal = GameManager.MapNum;
            GameManager.EffectAudio.PlayEffectSound("item");
        }
    }

    private bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }
}

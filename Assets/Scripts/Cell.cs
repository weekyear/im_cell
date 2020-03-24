using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.MapNum <= GameManager.RecentHeal)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            Destroy(gameObject);
            PlayerObserver.HealthChanged(50);
            GameManager.RecentHeal = GameManager.MapNum;
            GameManager.EffectAudio.PlayEffectSound("virus_killed");
        }
    }

    private bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }
}

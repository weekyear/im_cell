using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsPlayer(collision))
        {
            var particleSys = collision.collider.gameObject.GetComponent<ParticleSystem>();
            particleSys.Clear();
            particleSys.Play();

            StartCoroutine(PlayerObserver.Damaged());
            PlayerObserver.HealthChanged(-5);
        }
    }

    private bool IsPlayer(Collision2D collision)
    {
        return collision.gameObject.CompareTag("Player");
    }
}

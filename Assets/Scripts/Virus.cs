using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.MapNum <= GameManager.RecentVirus)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            gameObject.transform.Find("Virus_Sprite").gameObject.SetActive(false);

            var particleSys = gameObject.GetComponent<ParticleSystem>();
            particleSys.Clear();
            particleSys.Play();

            MapManager.VirusBeKilled();

            PlayerObserver.HealthChanged(5);

            AudioManager.Instance.PlayEffectSound("virus_killed");

            StartCoroutine(ReserveDestroyObject(gameObject));
        }
    }

    private IEnumerator ReserveDestroyObject(GameObject _gameObject)
    {
        yield return new WaitForSeconds(2.00f);

        Destroy(_gameObject);
    }

    private bool IsPlayer(Collider2D collision)
    {
        return collision.gameObject.CompareTag("Player");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLine : MonoBehaviour
{
    [SerializeField] private bool IsFinishLine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ChangingMapNum < GameManager.EndedMapNum + 1)
        {
            if (IsPlayer(collision) || IsPlayerInvincible(collision))
            {
                SwitchMap(collision.gameObject);
            }
        }
        else
        {
            PlayerObserver.GameFinished();
        }
    }

    private bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }
    
    private bool IsPlayerInvincible(Collider2D collision)
    {
        return collision.CompareTag("PlayerInvincible");
    }

    private void SwitchMap(GameObject player)
    {
        Destroy(GameObject.Find($"Map_{GameManager.MapNum}_(Clone)"));
        player.transform.position = new Vector2(SpawnPosition, player.transform.position.y);
        Instantiate(Resources.Load($"Map/Map_{ChangingMapNum}_"));

        GameObject.Find($"StageText").GetComponent<Text>().text = $"<Stage{ChangingMapNum}>";
        GameManager.MapNum = ChangingMapNum;
    }

    private float SpawnPosition
    {
        get
        {
            if (IsFinishLine)
            {
                return -30.5f;
            }
            else
            {
                return 30.5f;
            }
        }
    }

    private int ChangingMapNum
    {
        get
        {
            if (IsFinishLine)
            {
                return GameManager.MapNum + 1;
            }
            else
            {
                return GameManager.MapNum - 1;
            }
        }
    }
}

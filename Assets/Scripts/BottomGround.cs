using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomGround : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeInvincibleToPlayer(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ChangeInvincibleToPlayer(collision);
    }

    private void ChangeInvincibleToPlayer(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerInvincible"))
        {
            var rigid2D = collision.collider.GetComponent<Rigidbody2D>();
            rigid2D.velocity = Vector2.zero;
            StartCoroutine(PlayerObserver.PlayerActivated());
        }
    }
}


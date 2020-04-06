using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGround : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.Instance.PlayEffectSound("splash");

        collision.collider.gameObject.layer = LayerMask.NameToLayer("PlayerInvincible");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var rigid2D = collision.collider.GetComponent<Rigidbody2D>();
        rigid2D.velocity = new Vector2(-18f, 0);
    }
}

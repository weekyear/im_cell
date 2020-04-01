using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGround : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.EffectAudio.PlayEffectSound("splash");
        collision.collider.gameObject.layer = LayerMask.NameToLayer("PlayerInvincible");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var rigid2D = collision.collider.GetComponent<Rigidbody2D>();
        rigid2D.velocity = Vector2.zero;
        rigid2D.position = new Vector2(rigid2D.position.x - 0.35f, rigid2D.position.y);
    }
}

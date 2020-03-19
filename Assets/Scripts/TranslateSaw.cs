using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateSaw : MonoBehaviour
{
    [SerializeField] private Vector2 StartPos;
    [SerializeField] private Vector2 EndPos;
    [SerializeField] private float Speed;
    private bool IsFlip;

    // Update is called once per frame
    void Update()
    {
        var direction = Vector3.Normalize(EndPos - StartPos);

        if (transform.position.x < StartPos.x)
        {
            IsFlip = true;
        }
        else if (transform.position.x > EndPos.x)
        {
            IsFlip = false;
        }

        var speed = IsFlip ? Speed * 1 : Speed * -1;
        transform.Translate(direction * speed);
    }
}

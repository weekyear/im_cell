using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSaw : MonoBehaviour
{
    [SerializeField] private float RotateSpeed;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, RotateSpeed * Time.deltaTime, Space.Self);
    }
}

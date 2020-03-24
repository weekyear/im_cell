using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceWall : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.MapNum <= GameManager.RecentVirus)
        {
            Destroy(gameObject);
        }
    }
}

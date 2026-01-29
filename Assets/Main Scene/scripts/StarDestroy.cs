using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarDestroy : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}

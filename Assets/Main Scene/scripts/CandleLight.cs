using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleLight : MonoBehaviour
{
    public bool isLit = true;
   /* private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Find the flame child
            Transform flame = other.transform.Find("Flame"); 
            if (flame != null && !flame.gameObject.activeSelf)
            {
                // Light it up
                flame.gameObject.SetActive(true);
            }
        }
    }
   */
}

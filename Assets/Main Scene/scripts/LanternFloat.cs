using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LanternFloat : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float rotateSpeed = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Move upward
            transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

            // Small rotation for realism
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }
}

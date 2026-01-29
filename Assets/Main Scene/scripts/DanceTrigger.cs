using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceTrigger : MonoBehaviour
{
    public AudioSource musicSource;       
    public Light[] spotlights;           
    public float lightIntensity = 5f;
    private bool activated = false;
    public float rotationSpeed = 50f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;
            musicSource.Play();

            foreach (Light spot in spotlights)
            {
                spot.intensity = lightIntensity; // Turn lights on
            }
            Debug.Log("Dance Trigger Activated: Lights On and Music Playing");
        }
    }
    void Update()
    {
        if (activated)
        {
            foreach (Light spot in spotlights)
            {
                spot.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && activated)
        {
            musicSource.Stop();

            foreach (Light spot in spotlights)
            {
                spot.intensity = 0; // Turn lights off
            }

            Debug.Log("Dance Trigger Deactivated: Lights Off and Music Stopped");
            activated = false;
        }
    }
}

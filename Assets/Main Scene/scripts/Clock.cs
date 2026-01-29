using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class Clock : MonoBehaviour
{
    public Transform hourHand;
    public Transform minuteHand;

    public float hourSpeed = 30f;   // Degrees per second (adjust as needed)
    public float minuteSpeed = -60f; // Negative = counter-clockwise

    void Update()
    {
        //hourHand.localRotation = Quaternion.Euler(0, -hourSpeed * Time.deltaTime, 0);
        //minuteHand.localRotation = Quaternion.Euler(0, -minuteSpeed * Time.deltaTime, 0);
        hourHand.Rotate(0f, 0f, -hourSpeed * Time.deltaTime);    // Z axis = clock face
        minuteHand.Rotate(0f, 0f, -minuteSpeed * Time.deltaTime);

    }

}

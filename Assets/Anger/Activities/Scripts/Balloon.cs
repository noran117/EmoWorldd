using UnityEngine;

public class Balloon : MonoBehaviour
{
    public Color balloonColor;

    private void Start()
    {
        GetComponent<Renderer>().material.color = balloonColor;
    }
}

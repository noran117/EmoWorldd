using UnityEngine;

public class SaberColor : MonoBehaviour
{
    public SaberColorType color = SaberColorType.Red;

    void Start()
    {
        Debug.Log(gameObject.name + " -> My SaberColor is " + color);
    }
}
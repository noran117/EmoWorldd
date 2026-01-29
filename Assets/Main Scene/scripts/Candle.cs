using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : MonoBehaviour
{
    [SerializeField] private GameObject flame; // drag your flame child here
    [SerializeField] private float lightDelay = 0.3f; // delay before lighting (chain effect)

    public bool isLit = false;

    void Start()
    {
        if (flame != null)
            flame.SetActive(isLit); // make sure it matches initial state
    }

    private void OnTriggerEnter(Collider other)
    {
        // If player holder or another lit candle touches this
        Candle otherCandle = other.GetComponent<Candle>();
        CandleLight lighter = other.GetComponent<CandleLight>();

        if ((otherCandle != null && otherCandle.isLit) || (lighter != null && lighter.isLit))
        {
            if (!isLit)
                Invoke(nameof(LightUp), lightDelay); // wait before lighting
        }
    }

    public void LightUp()
    {
        isLit = true;
        if (flame != null)
            flame.SetActive(true);
    }
}

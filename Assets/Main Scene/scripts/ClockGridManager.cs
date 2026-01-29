using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockGridManager : MonoBehaviour
{
    public GameObject clockPrefab;
    public int rows = 6;
    public int cols = 6;
    public float spacing = 2f;

    private Clock[,] clocks;
    void Start()
    {
        clocks = new Clock[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 pos = new Vector3(col * spacing, -row * spacing, 0);
                GameObject clockObj = Instantiate(clockPrefab, pos, Quaternion.identity, transform);
                clocks[row, col] = clockObj.GetComponent<Clock>();
            }
        }

        //StartCoroutine(LoopPatterns());
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class facesscript : MonoBehaviour
{
    public Texture[] faces;          // صور التعابير
    public float switchTime = 2f;    // الوقت بين كل تعبير وآخر

    private Renderer rend;
    private int index = 0;
    private float timer = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (faces.Length > 0)
            rend.material.mainTexture = faces[0];
    }

    void Update()
    {
        if (faces.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= switchTime)
        {
            timer = 0f;
            index = (index + 1) % faces.Length;
            rend.material.mainTexture = faces[index];
        }
    }

   
    }

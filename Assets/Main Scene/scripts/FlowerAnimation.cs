using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerAnimation : MonoBehaviour
{
    public int petalCount = 5;       
    public int sphereCount = 200;   
    public float radius = 5f;      
    public float animationSpeed = 1f; 
    public Gradient colorGradient;   
    //public Light sculptureLight;     

    private GameObject[] spheres;

    void Start()
    {
        spheres = new GameObject[sphereCount];

        for (int i = 0; i < sphereCount; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); // ممكن اعملها اي عنصر بدي ياه
            sphere.transform.parent = transform;
            sphere.transform.localPosition = Vector3.zero;
            sphere.transform.localScale = Vector3.one * 0.2f;
            spheres[i] = sphere;
        }
    }

    void Update()
    {
        float time = Time.time * animationSpeed;

        for (int i = 0; i < spheres.Length; i++)
        {
            float angle = i * Mathf.PI * 2f / sphereCount;

            float k = petalCount; 
            float r = radius * Mathf.Cos(k * angle + time); 

            Vector3 position = new Vector3(
                r * Mathf.Cos(angle),
                Mathf.Sin(time + i * 0.1f), 
                r * Mathf.Sin(angle)
            );

            spheres[i].transform.localPosition = position;

            float t = Mathf.Abs(Mathf.Sin(time + i * 0.1f));
            Color color = colorGradient.Evaluate(t);
            spheres[i].GetComponent<Renderer>().material.color = color;
        }

        //// تحديث لون الإضاءة وشدتها
        //if (sculptureLight != null)
        //{
        //    sculptureLight.color = colorGradient.Evaluate(Mathf.PingPong(time * 0.5f, 1f));
        //    sculptureLight.intensity = Mathf.Abs(Mathf.Sin(time)) * 2f + 1f;
        //}
    }


}

using UnityEngine;
using System.Collections;

public class Basket : MonoBehaviour
{

    public float fadeDuration = 2f;
    Material mat;
    public GameObject stones;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;

        Color c = mat.color;
        c.a = 0;
        mat.color = c;
    }

    public void StartFade()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0;
        Color c = mat.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            mat.color = c;
            yield return null;
        } 

        if (stones != null)
            stones.SetActive(true);
    }
}

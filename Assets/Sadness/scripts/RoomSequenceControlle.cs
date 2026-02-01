using System.Collections;
using UnityEngine;

public class RoomSequenceController : MonoBehaviour
{
    [Header("Room Objects")]
    public Renderer[] roomRenderers; 

    [Header("Textures")]
    public Texture normalTexture;
    public Texture sadTexture;
    public Texture hopefulTexture;

    [Header("Falling Objects")]
    public Rigidbody[] fallingObjects;

    [Header("Audio")]
    public AudioSource fallingSound;

    [Header("Light")]
    public Light windowLight;

    private Vector3[] originalPositions;

    void Start()
    {
        originalPositions = new Vector3[fallingObjects.Length];
        for (int i = 0; i < fallingObjects.Length; i++)
        {
            originalPositions[i] = fallingObjects[i].transform.position;
            fallingObjects[i].isKinematic = true;
        }

        windowLight.intensity = 0;
    }

    public void StartRoomSequence()
    {
        StartCoroutine(RoomSequence());
    }

    IEnumerator RoomSequence()
    {
        fallingSound.Play();

        foreach (Rigidbody rb in fallingObjects)
        {
            rb.isKinematic = false;
        }

        yield return new WaitForSeconds(6f);

        fallingSound.Stop();

        yield return new WaitForSeconds(2f);

        StartCoroutine(ResetObjects());

        yield return StartCoroutine(ChangeRoomTexture(sadTexture, 4f));

        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(ChangeRoomTexture(hopefulTexture, 5f));
        StartCoroutine(IncreaseLight());
    }

    IEnumerator ResetObjects()
    {
        float t = 0;
        float duration = 4f;

        Vector3[] startPos = new Vector3[fallingObjects.Length];

        for (int i = 0; i < fallingObjects.Length; i++)
        {
            startPos[i] = fallingObjects[i].transform.position;
            fallingObjects[i].isKinematic = true;
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            for (int i = 0; i < fallingObjects.Length; i++)
            {
                fallingObjects[i].transform.position =
                    Vector3.Lerp(startPos[i], originalPositions[i], lerp);
            }

            yield return null;
        }
    }

    IEnumerator ChangeRoomTexture(Texture targetTexture, float duration)
    {
        foreach (Renderer r in roomRenderers)
        {
            r.material.mainTexture = targetTexture;
        }

        yield return new WaitForSeconds(duration);
    }

    IEnumerator IncreaseLight()
    {
        float t = 0;
        float duration = 5f;

        while (t < duration)
        {
            t += Time.deltaTime;
            windowLight.intensity = Mathf.Lerp(0, 1.5f, t / duration);
            yield return null;
        }
    }
}

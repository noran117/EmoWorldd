using UnityEngine;
using System.Collections;

public class BallSpawner : MonoBehaviour
{
    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public Transform parentContainer;
    public float spawnInterval = 3f;

    private bool isSpawning = false;
    private Coroutine spawnCoroutine;
    private bool initialized = false;

    private void Awake()
    {
        isSpawning = false;
        initialized = false;
    }

    private void Start()
    {
        initialized = true;
    }

    public void ToggleSpawning()
    {
        if (!initialized) return;

        if (isSpawning)
            StopSpawning();
        else
            StartSpawning();
    }

    public void StartSpawning()
    {
        if (isSpawning) return;

        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnBalls());

        Debug.Log("Ball spawning started");
    }

    public void StopSpawning()
    {
        if (!isSpawning) return;

        isSpawning = false;
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        Debug.Log("Ball spawning stopped");
    }

    private IEnumerator SpawnBalls()
    {
        while (isSpawning)
        {
            if (ballPrefab != null && spawnPoint != null)
            {
                GameObject newBall = Instantiate(
                    ballPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation,
                    parentContainer 
                );

                BallColor randomColor = newBall.GetComponent<BallColor>();
                if (randomColor != null)
                    randomColor.SetRandomColor();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

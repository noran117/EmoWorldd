using UnityEngine;

public class SugarProcessor : MonoBehaviour
{
    
    public enum MachineState
    {
        Idle,               
        WaitingForSugar,    
        Processing         
    }

    [Header("State")]
    public MachineState currentState = MachineState.Idle;

    [Header("Machine Settings")]
    public GameObject machineSpinningPart;
    public float idleSpinSpeed = 50f;
    public float processingSpinSpeed = 200f;

    [Header("Effects")]
    public ParticleSystem sugarEffect;
    public AudioSource machineSound;
    public AudioSource spawnCandySound;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Cotton Candy")]
    public GameObject cottonCandyPrefab;

    private float currentSpinSpeed;

    void Update()
    {
        if (machineSpinningPart != null && currentState != MachineState.Idle)
        {
            machineSpinningPart.transform.Rotate(
                0f,
                currentSpinSpeed * Time.deltaTime,
                0f
            );
        }
    }

    public void StartMachine()
    {
        if (currentState != MachineState.Idle)
            return;

        currentState = MachineState.WaitingForSugar;
        currentSpinSpeed = idleSpinSpeed;

        Debug.Log("Machine started - waiting for sugar");

        if (machineSound != null)
            machineSound.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState != MachineState.WaitingForSugar)
            return;

        if (other.CompareTag("Sugar"))
        {
            other.gameObject.SetActive(false);
            StartProcessing();
        }
    }

    private void StartProcessing()
    {
        currentState = MachineState.Processing;
        currentSpinSpeed = processingSpinSpeed;

        Debug.Log("Processing cotton candy");

        if (sugarEffect != null)
            sugarEffect.Play();

        Invoke(nameof(SpawnCottonCandy), 3f);
    }

    private void SpawnCottonCandy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        Transform chosenPoint =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(
            cottonCandyPrefab,
            chosenPoint.position,
            chosenPoint.rotation
        );

        if (spawnCandySound != null)
            spawnCandySound.Play();

        if (sugarEffect != null)
            sugarEffect.Stop();

        if (machineSound != null)
            machineSound.Stop();

        currentState = MachineState.Idle;
        currentSpinSpeed = 0f;

        Debug.Log("Machine finished");
    }
}

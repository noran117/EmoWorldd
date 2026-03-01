using UnityEngine;
using System.Collections.Generic;

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
    public List<GameObject> spawnPoints = new();

    private float currentSpinSpeed;
    private Color currentCandyColor;

    //private void Start()
    //{
    //    StartMachine(); // Start the machine immediately for testing purposes, we can remove this later and add it to the start button 
    //}

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
        sugarEffect.Stop();

        //Debug.Log("[SugarProcessor]: Start Machine");
        if (currentState != MachineState.Idle)
            return;

        currentState = MachineState.WaitingForSugar;
        currentSpinSpeed = idleSpinSpeed;

        //Debug.Log("Machine started - waiting for sugar");

        if (machineSound != null)
            machineSound.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("[SugarProcessor]: Trigger Enter");

        if (currentState != MachineState.WaitingForSugar)
            return;

        // Debug.Log("[SugarProcessor]: Trigger Waiting for Sugar");

        if (other.CompareTag("Sugar"))
        {
            //Debug.Log("[SugarProcessor]: Trigger Sugar entered");

            Renderer rend = other.GetComponentInChildren<Renderer>();

            if (rend == null)
                return;

            Material mat = rend.material;

            if (mat.HasProperty("_BaseColor"))
            {
                currentCandyColor = mat.GetColor("_BaseColor");
            }


            other.gameObject.SetActive(false);

            StartProcessing();
        }
    }

    private void StartProcessing()
    {
        currentState = MachineState.Processing;
        currentSpinSpeed = processingSpinSpeed;

        // Debug.Log("Processing cotton candy");
        ApplyColor();
        if (sugarEffect != null)
            sugarEffect.Play();

        Invoke(nameof(SpawnCottonCandy), 3f);
    }

    private void SpawnCottonCandy()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        GameObject chosenPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        ApplyColorToChosenPoint(chosenPoint);
        chosenPoint.SetActive(true);
        spawnPoints.Remove(chosenPoint);


        if (spawnCandySound != null)
            spawnCandySound.Play();

        if (sugarEffect != null)
            sugarEffect.Stop();

        if (machineSound != null)
            machineSound.Stop();

        currentState = MachineState.WaitingForSugar;
        currentSpinSpeed = 0f;
        sugarEffect.Stop();

        Debug.Log("Machine finished");
    }
    private void ApplyColor()
    {
        if (sugarEffect == null) return;

        // Particle
        if (sugarEffect != null)
        {
            var main = sugarEffect.main;
            main.startColor = currentCandyColor;
        }

    }
    private void ApplyColorToChosenPoint(GameObject chosenPoint)
    {
        if (chosenPoint == null)
            return;

        CandyColorPart colorPart = chosenPoint.GetComponentInChildren<CandyColorPart>();

        if (colorPart != null)
        {
            Renderer rend = colorPart.GetComponent<Renderer>();

            if (rend != null)
            {
                Material mat = rend.material;

                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", currentCandyColor);
                }
            }
        }
    }
}

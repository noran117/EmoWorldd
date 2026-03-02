using UnityEngine;
using System.Collections;

public class RepairToyBargaining : MonoBehaviour
{
    public GameObject brokenModel;
    public GameObject fixedModel;
    public ParticleSystem repairParticles;

    [Header("Repair Settings")]
    public float repairDelay = 1.2f;

    private bool repaired = false;
    private bool canRepair = false;

    private void Start()
    {
        if (brokenModel != null)
            brokenModel.SetActive(false);

        if (fixedModel != null)
            fixedModel.SetActive(false);
    }

    public void EnableRepair()
    {
        canRepair = true;
    }

    public void RepairAfterPuzzleCompleted()
    {
        if (repaired) return;
        if (!canRepair) return;

        StartCoroutine(RepairRoutine());
    }

    IEnumerator RepairRoutine()
    {
        repaired = true;

        repairParticles?.Play();

        yield return new WaitForSeconds(repairDelay);

        if (brokenModel != null) brokenModel.SetActive(false);
        if (fixedModel != null) fixedModel.SetActive(true);

        GameStateManager.Instance.ChangeState(GameState.Depression);
    }
}

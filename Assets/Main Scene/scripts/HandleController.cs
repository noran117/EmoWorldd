using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float minRotation = 36f;
    public float maxRotation = -36f;

    public GameObject[] controlledBlocks;
    public static bool[] blockStates = new bool[4];
    public static HandleController leaderHandle;

    //private Vector3 initialDoorPosition;

    private bool isToggled = false;
    private float targetAngle;

    void Awake()
    {

        if (leaderHandle == null)
        {
            leaderHandle = this;
            Debug.Log(this.name + " has been assigned as the LEADER.");
        }
    }

    void Start()
    {
        targetAngle = transform.localEulerAngles.z;
        targetAngle = NormalizeAngle(targetAngle);

    }

    void OnMouseDown()
    {
        isToggled = !isToggled;
        targetAngle = isToggled ? maxRotation : minRotation;
        StartCoroutine(RotateHandle());
        ToggleBlocks();
    }

    private IEnumerator RotateHandle()
    {
        float startAngle = NormalizeAngle(transform.localEulerAngles.z);
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAngle = Mathf.Lerp(startAngle, targetAngle, elapsedTime / duration);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    private void ToggleBlocks()
    {
        foreach (GameObject block in controlledBlocks)
        {
            int index = int.Parse(block.name.Substring(block.name.Length - 1)) - 1;
            blockStates[index] = !blockStates[index];
            block.transform.position += blockStates[index] ? Vector3.up * 1f : Vector3.down * 1f;
        }

        if (leaderHandle != null)
            leaderHandle.CheckPuzzleSolved();
    }

    public void CheckPuzzleSolved()
    {
        foreach (bool state in blockStates)
        {
            if (!state)
                return;
        }
        Debug.Log("Puzzle Solved! Bridge is complete!");
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

}

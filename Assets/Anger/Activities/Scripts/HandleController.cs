using System.Collections;
using UnityEngine;

public class HandleController : MonoBehaviour
{
    [Header("Group 1")]
    public GameObject[] group1;

    [Header("Group 2")]
    public GameObject[] group2;

    [Header("Group 3")]
    public GameObject[] group3;

    [Header("Group 4")]
    public GameObject[] group4;

    public float moveHeight = 2f;
    public float moveSpeed = 2f;
    private Vector3[][] originalPositions;

    void Start()
    {
        originalPositions = new Vector3[4][];

        originalPositions[0] = SavePositions(group1);
        originalPositions[1] = SavePositions(group2);
        originalPositions[2] = SavePositions(group3);
        originalPositions[3] = SavePositions(group4);
    }

    Vector3[] SavePositions(GameObject[] group)
    {
        Vector3[] positions = new Vector3[group.Length];
        for (int i = 0; i < group.Length; i++)
        {
            positions[i] = group[i].transform.position;
        }
        return positions;
    }
    // ===== GROUP 1 =====
    public void Group1Up() => StartCoroutine(MoveGroup(group1, originalPositions[0], true));
    public void Group1Down() => StartCoroutine(MoveGroup(group1, originalPositions[0], false));

    // ===== GROUP 2 =====
    public void Group2Up() => StartCoroutine(MoveGroup(group2, originalPositions[1], true));
    public void Group2Down() => StartCoroutine(MoveGroup(group2, originalPositions[1], false));

    // ===== GROUP 3 =====
    public void Group3Up() => StartCoroutine(MoveGroup(group3, originalPositions[2], true));
    public void Group3Down() => StartCoroutine(MoveGroup(group3, originalPositions[2], false));

    // ===== GROUP 4 =====
    public void Group4Up() => StartCoroutine(MoveGroup(group4, originalPositions[3], true));
    public void Group4Down() => StartCoroutine(MoveGroup(group4, originalPositions[3], false));

    IEnumerator MoveGroup(GameObject[] group, Vector3[] basePositions, bool up)
    {
        float elapsed = 0f;
        float duration = 1f;

        Vector3 offset = up ? Vector3.up * moveHeight : Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < group.Length; i++)
            {
                Vector3 target = basePositions[i] + offset;
                group[i].transform.position =
                    Vector3.Lerp(group[i].transform.position, target, Time.deltaTime * moveSpeed);
            }

            yield return null;
        }
    }

}

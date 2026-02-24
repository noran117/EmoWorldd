using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HandleController : MonoBehaviour
{

    [System.Serializable]
    public class LeverGroup
    {
        public GameObject[] pieces;   // القطع المرتبطة بهذا المقبض
        public bool correctState;     // الحالة المطلوبة للحل (true = Up)
        [HideInInspector] public bool currentState;
        [HideInInspector] public Vector3[] basePositions;
    }

    public List<LeverGroup> leverGroups = new List<LeverGroup>();

    public float moveHeight = 2f;
    public float moveSpeed = 3f;

    private bool puzzleSolved = false;

    void Start()
    {
        foreach (var group in leverGroups)
        {
            group.basePositions = new Vector3[group.pieces.Length];

            for (int i = 0; i < group.pieces.Length; i++)
            {
                // نحسب مكان المشي السفلي
                group.basePositions[i] =
                    group.pieces[i].transform.position - Vector3.up * moveHeight;
            }
        }
    }
    public void LeverUp(int index)
    {
        if (puzzleSolved) return;

        leverGroups[index].currentState = true;
        StartCoroutine(MoveGroup(index, true));
        CheckPuzzle();
    }

    public void LeverDown(int index)
    {
        if (puzzleSolved) return;

        leverGroups[index].currentState = false;
        StartCoroutine(MoveGroup(index, false));
        CheckPuzzle();
    }
    // يتم استدعاؤها من الـ Lever
    public void LeverChanged(int index, bool isUp)
    {
        if (puzzleSolved) return;

        leverGroups[index].currentState = isUp;

        StartCoroutine(MoveGroup(index, isUp));
        CheckPuzzle();
    }

    IEnumerator MoveGroup(int index, bool moveUp)
    {
        var group = leverGroups[index];

        float elapsed = 0f;
        float duration = 0.6f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < group.pieces.Length; i++)
            {
                Vector3 target = moveUp
                    ? group.basePositions[i] + Vector3.up * moveHeight
                    : group.basePositions[i];

                group.pieces[i].transform.position =
                    Vector3.Lerp(group.pieces[i].transform.position, target, Time.deltaTime * moveSpeed);
            }

            yield return null;
        }
    }

    void CheckPuzzle()
    {
        for (int i = 0; i < leverGroups.Count; i++)
        {
            if (leverGroups[i].currentState != leverGroups[i].correctState)
                return;
        }

        puzzleSolved = true;
        Debug.Log("Puzzle Solved! Bridge Complete!");
    }

}

/*
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
    */
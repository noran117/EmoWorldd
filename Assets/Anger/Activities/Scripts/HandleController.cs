using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HandleController : MonoBehaviour
{
    [System.Serializable]
    public class BridgeGroup
    {
        public GameObject lever;
        public List<Transform> pieces = new List<Transform>();
    }

    [Header("Bridge Groups")]
    public List<BridgeGroup> bridgeGroups = new List<BridgeGroup>();

    [Header("Movement Settings")]
    public float moveAmount = 1f;        // المقدار اللي بترفع/بتنزل فيه القطع
    public float moveDuration = 0.5f;    // مدة الحركة بالثواني

    [Header("Movement Sounds")]
    public AudioSource moveSound;

    [Header("Puzzle Completion")]
    public Collider bridgeBarrier;     // drag your invisible wall here
    public AudioSource solvedSound;
    public float solvedThreshold = 0.1f; // tolerance for position check

    private bool isMoving = false;

    [Header("Target Settings")]
    public float targetY = 0f;          // المستوى المطلوب

    // يستدعيها كيوب التحكم ويرسل رقم المجموعة
    public void RaiseGroup(int groupIndex)
    {
        if (isMoving) return;
        if (groupIndex < 0 || groupIndex >= bridgeGroups.Count) return;

        StartCoroutine(MovePieces(bridgeGroups[groupIndex].pieces, Vector3.up * moveAmount));
    }

    public void LowerGroup(int groupIndex)
    {
        if (isMoving) return;
        if (groupIndex < 0 || groupIndex >= bridgeGroups.Count) return;

        StartCoroutine(MovePieces(bridgeGroups[groupIndex].pieces, Vector3.down * moveAmount));
    }

    private IEnumerator MovePieces(List<Transform> pieces, Vector3 direction)
    {
        isMoving = true;

        List<Vector3> startPositions = new List<Vector3>();
        List<Vector3> targetPositions = new List<Vector3>();

        foreach (Transform piece in pieces)
        {
            startPositions.Add(piece.position);

            float newY = piece.position.y + direction.y;

            // منع النزول تحت المستوى المطلوب
            if (direction.y < 0)
                newY = Mathf.Max(newY, targetY);

            targetPositions.Add(new Vector3(
                piece.position.x,
                newY,
                piece.position.z));
        }

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);

            for (int i = 0; i < pieces.Count; i++)
            {
                pieces[i].position =
                    Vector3.Lerp(startPositions[i], targetPositions[i], t);
            }
            yield return null;
        }

        for (int i = 0; i < pieces.Count; i++)
            pieces[i].position = targetPositions[i];

        moveSound?.Play();

        CheckPuzzleSolved();

        isMoving = false;
    }

    private void CheckPuzzleSolved()
    {
        foreach (BridgeGroup group in bridgeGroups)
        {
            foreach (Transform piece in group.pieces)
            {
                // if any piece is NOT at the target Y, puzzle is unsolved
                if (Mathf.Abs(piece.position.y - targetY) > solvedThreshold)
                    return;
            }
        }

        // All pieces are at targetY — puzzle solved!
        if (bridgeBarrier != null)
            bridgeBarrier.enabled = false;

        solvedSound?.Play();

    }
}
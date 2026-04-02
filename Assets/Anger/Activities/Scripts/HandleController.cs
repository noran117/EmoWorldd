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
    public float maxY = 0f;

    // يستدعيها كيوب التحكم ويرسل رقم المجموعة
    public void RaiseGroup(int groupIndex)
    {
        if (isMoving) return;
        if (groupIndex < 0 || groupIndex >= bridgeGroups.Count) return;

        StartCoroutine(MovePieces(bridgeGroups[groupIndex].pieces, moveAmount));
    }

    public void LowerGroup(int groupIndex)
    {
        if (isMoving) return;
        if (groupIndex < 0 || groupIndex >= bridgeGroups.Count) return;

        StartCoroutine(MovePieces(bridgeGroups[groupIndex].pieces, -moveAmount));
    }

    private IEnumerator MovePieces(List<Transform> pieces, float amount)
    {
        isMoving = true;

        List<Vector3> startPositions = new List<Vector3>();
        List<Vector3> targetPositions = new List<Vector3>();

        foreach (Transform piece in pieces)
        {
            startPositions.Add(piece.localPosition);

            float newY = Mathf.Round(piece.localPosition.y) + amount;

            // منع النزول تحت الحد الأدنى
            if (amount < 0)
                newY = Mathf.Max(newY, maxY);

            // منع الرفع فوق الحد الأقصى
            if (amount > 0)
                newY = Mathf.Min(newY, targetY);

            targetPositions.Add(new Vector3(
                piece.localPosition.x,
                newY,
                piece.localPosition.z));
        }

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);

            for (int i = 0; i < pieces.Count; i++)
            {
                pieces[i].localPosition = Vector3.Lerp(startPositions[i], targetPositions[i], t);

            }
            yield return null;
        }

        for (int i = 0; i < pieces.Count; i++)
            pieces[i].localPosition = targetPositions[i];

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
                if (Mathf.Abs(piece.localPosition.y - targetY) > solvedThreshold)
                    return;
            }
        }

        if (bridgeBarrier != null)
            bridgeBarrier.enabled = false;

        solvedSound?.Play();
    }
}
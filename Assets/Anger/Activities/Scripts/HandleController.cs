using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HandleController : MonoBehaviour
{
[Header("Bridge Pieces")]
    public List<Transform> bridgePieces = new List<Transform>();

    [Header("Movement Settings")]
    public float moveAmount = 2f;        // المقدار اللي بترفع/بتنزل فيه القطع
    public float moveDuration = 0.5f;    // مدة الحركة بالثواني
    public bool animateMovement = true;  // تحريك سلس أو فوري

    // لتتبع الحالة الأصلية لكل قطعة
    private List<Vector3> originalPositions = new List<Vector3>();
    private bool isMoving = false;

    void Start()
    {
        // حفظ المواقع الأصلية لكل القطع
        foreach (Transform piece in bridgePieces)
        {
            originalPositions.Add(piece.position);
        }
    }
      // ترفع كل القطع لمقدار معين
    public void RaisePieces()
    {
        if (isMoving) return;

        if (animateMovement)
            StartCoroutine(MovePieces(Vector3.up * moveAmount));
        else
            MovePiecesInstant(Vector3.up * moveAmount);
    }
    // تنزّل كل القطع لنفس المقدار
    public void LowerPieces()
    {
        if (isMoving) return;

        if (animateMovement)
            StartCoroutine(MovePieces(Vector3.down * moveAmount));
        else
            MovePiecesInstant(Vector3.down * moveAmount);
    }
    // ترجع القطع لمواقعها الأصلية
    public void ResetPieces()
    {
        if (isMoving) return;

        if (animateMovement)
            StartCoroutine(ResetToOriginal());
        else
        {
            for (int i = 0; i < bridgePieces.Count; i++)
                bridgePieces[i].position = originalPositions[i];
        }
    }
 // حركة سلسة باستخدام Coroutine
    private IEnumerator MovePieces(Vector3 direction)
    {
        isMoving = true;

        List<Vector3> startPositions = new List<Vector3>();
        List<Vector3> targetPositions = new List<Vector3>();

        foreach (Transform piece in bridgePieces)
        {
            startPositions.Add(piece.position);
            targetPositions.Add(piece.position + direction);
        }

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);

            for (int i = 0; i < bridgePieces.Count; i++)
                bridgePieces[i].position = Vector3.Lerp(startPositions[i], targetPositions[i], t);

            yield return null;
        }

        // تأكد إنها وصلت للموقع الصح
        for (int i = 0; i < bridgePieces.Count; i++)
            bridgePieces[i].position = targetPositions[i];

        isMoving = false;
    }

    // حركة فورية بدون animation
    private void MovePiecesInstant(Vector3 direction)
    {
        foreach (Transform piece in bridgePieces)
            piece.position += direction;
    }
 // رجوع سلس للمواقع الأصلية
    private IEnumerator ResetToOriginal()
    {
        isMoving = true;

        List<Vector3> startPositions = new List<Vector3>();

        foreach (Transform piece in bridgePieces)
            startPositions.Add(piece.position);

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);

            for (int i = 0; i < bridgePieces.Count; i++)
                bridgePieces[i].position = Vector3.Lerp(startPositions[i], originalPositions[i], t);

            yield return null;
        }

        for (int i = 0; i < bridgePieces.Count; i++)
            bridgePieces[i].position = originalPositions[i];

        isMoving = false;
    }

}
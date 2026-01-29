using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheelmovement : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform[] points;       // نقاط المسار
    public float speed = 3f;         // سرعة الحركة
    public int[] jumpIndices;        // كل النقاط اللي بينط عندها
    public float jumpHeight = 3f;    // ارتفاع القفزة

    private int currentIndex = 0;
    private Vector3 startPos;
    private Vector3 endPos;
    private float t = 0f;
    private bool isJumping = false;

    void Start()
    {
        if (points.Length > 0)
            transform.position = points[0].position;
    }
    void Update()
    {
        if (points.Length < 2) return;

        if (t == 0f)
        {
            startPos = points[currentIndex].position;
            endPos = points[(currentIndex + 1) % points.Length].position;

            // 👇 التأكد إذا النقطة التالية وحدة من النقاط اللي فيها قفزة
            isJumping = false;
            foreach (int jumpIndex in jumpIndices)
            {
                if (currentIndex + 1 == jumpIndex)
                {
                    isJumping = true;
                    break;
                }
            }
        }

        t += Time.deltaTime * (speed / Vector3.Distance(startPos, endPos));
        t = Mathf.Clamp01(t);

        // ✨ الحركة
        Vector3 movePos = Vector3.Lerp(startPos, endPos, t);

        // ✨ القفزة (قوس)
        if (isJumping)
        {
            float arc = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            movePos.y += arc;
        }

        transform.position = movePos;

        // تدوير الحوت
        Vector3 direction = endPos - startPos;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-direction), Time.deltaTime * 2);

        // لما يوصل للنقطة
        if (t >= 1f)
        {
            t = 0f;
            currentIndex++;
            if (currentIndex >= points.Length - 1)
                currentIndex = 0;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class duckmovementfinal : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform[] waypoints;     // النقاط اللي بتمشي عليها البطة
    public float moveSpeed = 2f;      // سرعة الحركة
    public float rotateSpeed = 5f;    // سرعة لفّة البطة
    public float reachDistance = 0.1f; // مسافة اعتبر عندها إنّي وصلت للنقطة

    int currentIndex = 0;   // رقم النقطة الحالية
    bool forward = true;   // true = رايحة من 0 → آخر نقطة، false = راجعة العكس

    void Start()
    {
        // لو في نقاط، حط البطة على أول نقطة بالبداية
        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 1) تحديد الهدف الحالي
        Transform target = waypoints[currentIndex];

        // 2) تحريك البطة نحو الهدف
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // 3) تدوير البطة باتجاه الحركة (بس على محور Y)
        Vector3 direction = (target.position - transform.position);
        direction.y = 0f; // عشان ما ترفع رأسها لفوق/لتحت
        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );
        }

        // 4) لما توصل للنقطة → حدّد النقطة التالية أو اعكس الاتجاه
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= reachDistance)
        {
            if (forward)
            {
                currentIndex++;

                // لو وصلنا لآخر نقطة → اعكس الاتجاه وابدأ ترجع
                if (currentIndex >= waypoints.Length)
                {
                    currentIndex = waypoints.Length - 2; // النقطة اللي قبل الأخيرة
                    forward = false;
                }
            }
            else
            {
                currentIndex--;

                // لو رجعنا لأول نقطة → اعكس الاتجاه وابدأ تطلع تاني
                if (currentIndex < 0)
                {
                    currentIndex = 1; // النقطة الثانية
                    forward = true;
                }
            }

        }
    }
}
    

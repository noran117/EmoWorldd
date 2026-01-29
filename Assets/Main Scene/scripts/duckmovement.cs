using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class duckmovement : MonoBehaviour
{
  




    public Transform[] points;    // نقاط المسار
    public float speed = 2f;      // سرعة الحركة
    public float rotationSpeed = 5f; // سرعة دوران الجسم بشكل ناعم

    private int index = 0;
    private int direction = 1;
    private float startY;

    void Start()
    {
        if (points.Length == 0) return;

        transform.position = points[0].position;
        startY = transform.position.y;
    }

    void Update()
    {
        if (points.Length == 0) return;

        // النقطة الحالية
        Transform target = points[index];

        // نحافظ على الارتفاع
        Vector3 targetPos = new Vector3(target.position.x, startY, target.position.z);

        // الحركة باتجاه النقطة
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // ---- دوران ناعم Smooth Rotation ----
        Vector3 directionToLook = targetPos - transform.position;
        directionToLook.y = 0; // ما نخليه يطّلع لفوق أو لتحت

        if (directionToLook != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        // ---------------------------------------

        // التغيير بين النقاط
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            index += direction;

            if (index >= points.Length)
            {
                direction = -1;
                index = points.Length - 2;
            }
            else if (index < 0)
            {
                direction = 1;
                index = 1;
            }
        }
    }
}





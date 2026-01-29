using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointsscript : MonoBehaviour
{
    //public List<GameObject> waypoints = new List<GameObject>();
    //public float speed = 2;
    //int index = 0;

    public Transform[] corners; // أربع نقاط كحدود المنطقة
    public float speed = 2f;
    public float waitTime = 2f;

    private Vector3 targetPosition;
    private float timer;

    private float minX, maxX, minY, maxY, minZ, maxZ;

    // Start is called before the first frame update
    void Start()
    {
        if (corners.Length < 4)
        {
            Debug.LogError("You must assign 4 corner points!");
            return;
        }

        // حساب حدود المنطقة من الأربع نقاط
        minX = Mathf.Min(corners[0].position.x, corners[1].position.x, corners[2].position.x, corners[3].position.x);
        maxX = Mathf.Max(corners[0].position.x, corners[1].position.x, corners[2].position.x, corners[3].position.x);
        minY = Mathf.Min(corners[0].position.y, corners[1].position.y, corners[2].position.y, corners[3].position.y);
        maxY = Mathf.Max(corners[0].position.y, corners[1].position.y, corners[2].position.y, corners[3].position.y);
        minZ = Mathf.Min(corners[0].position.z, corners[1].position.z, corners[2].position.z, corners[3].position.z);
        maxZ = Mathf.Max(corners[0].position.z, corners[1].position.z, corners[2].position.z, corners[3].position.z);

        PickNewTarget();
    }

        // Update is called once per frame
        void Update()
    {
        ////Vector3 destination = waypoints[index].transform.position;
        ////Vector3 newPos = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        ////transform.position = newPos;

        ////float distance = Vector3.Distance(transform.position, destination);
        ////if (distance <= 0.05f)
        ////{
        ////    if (index < waypoints.Count - 1)
        ////    {
        ////        index++;
        ////    }
        ////    else
        ////    {
        ////        index = 0; // ✅ رجع للبداية (لوب)
        ////    }
        ////}
        //if (waypoints.Count == 0) return;

        //Vector3 destination = waypoints[index].transform.position;

        //// الحركة نحو النقطة
        //Vector3 newPos = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        //transform.position = newPos;

        //// دوران باتجاه النقطة فقط على المحور Y
        //Vector3 direction = destination - transform.position;
        //direction.y = 0; // تجاهل الارتفاع
        //if (direction.magnitude > 0.1f)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        //}

        //// التبديل للنقطة التالية
        //float distance = Vector3.Distance(transform.position, destination);
        //if (distance <= 0.05f)
        //{
        //    if (index < waypoints.Count - 1)
        //        index++;
        //    else
        //        index = 0; // ترجع للبداية (لوب)
        //}


        // التحرك نحو الهدف
        //transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        //transform.LookAt(targetPosition);

        //if (Vector3.Distance(transform.position, targetPosition) < 0.2f || timer <= 0)
        //{
        //    PickNewTarget();
        //}

        //timer -= Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        // دوران ناعم نحو الهدف
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
        }

        // إذا قربت من الهدف، اختاري نقطة جديدة
        if (Vector3.Distance(transform.position, targetPosition) < 0.3f || timer <= 0)
        {
            PickNewTarget();
        }

        timer -= Time.deltaTime;
    }
    void PickNewTarget()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        float z = Random.Range(minZ, maxZ);

        targetPosition = new Vector3(x, y, z);
        timer = waitTime;
    }
}

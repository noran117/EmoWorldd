using UnityEngine;

public class DuckMovementFinal : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float rotateSpeed = 6f;
    public float reachDistance = 0.15f;
    public float turnSpeed = 180f; // سرعة اللفة عند النهاية

    int currentIndex = 0;
    bool forward = true;
    bool isTurning = false;

    Quaternion targetTurnRotation;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        if (isTurning)
        {
            HandleTurning();
            return;
        }

        Transform target = waypoints[currentIndex];

        // ===== حركة =====
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // ===== دوران باتجاه الهدف =====
        Vector3 dir = (target.position - transform.position);
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRot,
                rotateSpeed * Time.deltaTime
            );
        }

        // ===== وصلت للنقطة =====
        if (Vector3.Distance(transform.position, target.position) <= reachDistance)
        {
            UpdateWaypointIndex();
        }
    }

    void UpdateWaypointIndex()
    {
        if (forward)
        {
            currentIndex++;

            // ===== آخر نقطة → لف =====
            if (currentIndex >= waypoints.Length)
            {
                forward = false;
                currentIndex = waypoints.Length - 2;
                StartTurn();
            }
        }
        else
        {
            currentIndex--;

            // ===== أول نقطة → لف =====
            if (currentIndex < 0)
            {
                forward = true;
                currentIndex = 1;
                StartTurn();
            }
        }
    }

    void StartTurn()
    {
        isTurning = true;

        // الاتجاه للنقطة الجاية بعد الانعكاس
        Vector3 nextDir = waypoints[currentIndex].position - transform.position;
        nextDir.y = 0f;

        targetTurnRotation = Quaternion.LookRotation(nextDir);
    }


    void HandleTurning()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetTurnRotation,
            turnSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(transform.rotation, targetTurnRotation) < 1f)
        {
            isTurning = false;
        }
    }
}

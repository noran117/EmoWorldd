using UnityEngine;

public class StartMove : MonoBehaviour
{
 public Animator [] animators;
    private BNG.MoveToWaypoint moveScript;
    private bool wasMoving = false; 
    private void Awake()
    {
        moveScript = GetComponent<BNG.MoveToWaypoint>();
    }
    void Update()
    {
        if (moveScript == null|| animators == null) return;

        bool isCurrentlyMoving = moveScript.IsActive && !moveScript.ReachedDestination;

        // إذا الحالة تغيرت فقط
        if (isCurrentlyMoving != wasMoving)
        {
            wasMoving = isCurrentlyMoving;
            foreach (var animator in animators)
            {
                animator.SetBool("isMoving", isCurrentlyMoving);
            }

            Debug.Log(isCurrentlyMoving ? "moving" : "not moving");
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player entered");
            moveScript.IsActive = true;
           // GetComponent<BNG.MoveToWaypoint>().IsActive = true;
        }
    }

}

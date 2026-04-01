using UnityEngine;

public class StartMove : MonoBehaviour
{
    public Animator[] animators;
    public AudioSource sound;
    public GameObject collider;
    private BNG.MoveToWaypoint moveScript;
    private bool wasMoving = false;
    private void Awake()
    {
        moveScript = GetComponent<BNG.MoveToWaypoint>();
    }
    void Update()
    {
        if (moveScript == null || animators == null) return;

        bool isCurrentlyMoving = moveScript.IsActive && !moveScript.ReachedDestination;

        // إذا الحالة تغيرت فقط
        if (isCurrentlyMoving != wasMoving)
        {
            wasMoving = isCurrentlyMoving;
            foreach (var animator in animators)
            {
                animator.SetBool("isMoving", isCurrentlyMoving);
            }
            if (isCurrentlyMoving && sound != null)
            {
                if (!sound.isPlaying)
                    sound.Play();
            }
            else
            {
                if (sound == null) return;
                if (sound.isPlaying){
                    sound.Stop();
                    collider.SetActive(false);
                }
            }

            //Debug.Log(isCurrentlyMoving ? "moving" : "not moving");
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (collider != null)
            {
                collider.SetActive(true);
            }
            moveScript.IsActive = true;

        }
    }
   

}

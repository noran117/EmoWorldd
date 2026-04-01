using UnityEngine;

public class StartMove : MonoBehaviour
{
    public Animator[] animators;
    public AudioSource sound;
    public GameObject hiddenCollider;
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
                }
            }

            //Debug.Log(isCurrentlyMoving ? "moving" : "not moving");
        }
        if(moveScript.ReachedDestination)
        {
            if (hiddenCollider == null) return;
            if (hiddenCollider.activeSelf)
            {
                hiddenCollider.SetActive(false);
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hiddenCollider != null)
            {
                hiddenCollider.SetActive(true);
            }
            moveScript.IsActive = true;

        }
    }
   

}

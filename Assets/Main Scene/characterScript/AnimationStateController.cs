using UnityEngine;
using BNG;

public class AnimationStateController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform playerHead;  

    [Header("Movement")]
    public float followDistance = 1.5f;
    public float moveSpeed = 1.5f;

    private bool greeted = false;

    int isNPCWalkingHash;

    AudioSource audio;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (!greeted)
        {
            animator.SetTrigger("Greet");
            greeted = true;
        }

        isNPCWalkingHash = Animator.StringToHash("IsWalking");
    }

    void Update()
    {
        FollowPlayer();
    }

void FollowPlayer()
{
    if (playerHead == null) return;

    float distance = Vector3.Distance(transform.position, playerHead.position);

    if (distance > followDistance)
    {
        animator.SetBool(isNPCWalkingHash, true);

        Vector3 direction = (playerHead.position - transform.position).normalized;
        direction.y = 0; 

        transform.position += direction * moveSpeed * Time.deltaTime;

        // التفات ناعم
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }
    else
    {
        animator.SetBool(isNPCWalkingHash, false);
    }
}
    /// <summary>
/// AnimationStateController.PlayDialogue(dialogueClip);
/// </summary>

    public void PlayDialogue(AudioSource audio)
    {
        audio.Play();

        animator.SetBool("IsTalking", true);

        StartCoroutine(WaitForVoiceEnd());
    }
    private System.Collections.IEnumerator WaitForVoiceEnd()
    {
        yield return new WaitWhile(() => audio.isPlaying);

        animator.SetBool("IsTalking", false);
    }
    //  تفاعل البوابة
    public void ReactToGate()
    {
        animator.SetTrigger("NearGate");
    }

    // تفاعل الفقاعة
    public void ReactToBubble()
{
    animator.SetTrigger("BubblePopped");
}
    //// بالكود تبع الفقاعة 
    //public CompanionController companion;

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        //نضيف هاد السطر
    //        companion.ReactToBubble();

    //    }
    //}

    
}

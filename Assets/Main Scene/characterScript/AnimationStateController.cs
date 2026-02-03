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
    public float gravity = 9.8f;

    [Header("Look Settings")]
    public float lookSpeed = 5f;


    private CharacterController controller;
    private Vector3 velocity;

    private bool greeted = false;
    int isNPCWalkingHash;

    AudioSource audio;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        if (!greeted)
        {
            animator.SetTrigger("Greet");
            greeted = true;
        }

        isNPCWalkingHash = Animator.StringToHash("IsWalking");
    }

    void Update()
    {
        ApplyGravity();
        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (playerHead == null) return;

        Vector3 targetPos = playerHead.position;
        targetPos.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance > followDistance)
        {
            animator.SetBool(isNPCWalkingHash, true);

            Vector3 direction = (targetPos - transform.position).normalized;
            controller.Move(direction * moveSpeed * Time.deltaTime);

            //  وهو ماشي: يطلع لقدام
            RotateTowards(direction);
        }
        else
        {
            animator.SetBool(isNPCWalkingHash, false);

            //  وهو واقف: يطلع على اللاعب
            RotateTowards((targetPos - transform.position).normalized);
        }
    }

    void RotateTowards(Vector3 direction)
    {
        direction.y = 0;
        if (direction.magnitude < 0.01f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * lookSpeed
        );
    }


    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ===== Dialogue =====
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

    // ===== Reactions =====
    public void ReactToGate()
    {
        animator.SetTrigger("NearGate");
    }

    public void ReactToBubble()
    {
        animator.SetTrigger("BubblePopped");
    }
}

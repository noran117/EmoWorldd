using UnityEngine;

public class ShellOpen : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private bool hasOpened = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasOpened && other.CompareTag("Player"))
        {
            animator.SetBool("Open", true);  
            hasOpened = true;

            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}

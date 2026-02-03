using UnityEngine;

public class ShellOpen : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private bool isOpen = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            /*isOpen = !isOpen;
            animator.SetBool("Open", isOpen);
            Debug.Log("Shell state changed: " + (isOpen ? "Opened" : "Closed"));
            if (audioSource != null)
            {
                audioSource.Play();
            }*/
            OpenShell();
            Invoke(nameof(CloseShell), 5f);
        }
    }

    void OpenShell()
    {
        animator.SetBool("Open", true);
        Debug.Log("Shell opened.");
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    void CloseShell()
    {
        animator.SetBool("Open", false);
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        Debug.Log("Shell closed.");
    }
}

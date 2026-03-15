using UnityEngine;

public class ShellOpen : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private bool isOpen = false;
    public followPlayer companion;



    public GameObject openMessage;  
    public GameObject thanksMessage;
    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (openMessage != null)
            openMessage.SetActive(true);

        if (thanksMessage != null)
            thanksMessage.SetActive(false);
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
            Invoke(nameof(CloseShell), 10f);
        }
    }

    void OpenShell()
    {
        animator.SetBool("Open", true);
        /*
                if (companion != null)
                {
                    companion.SetSurprise();
                }
        */

       
        if (openMessage != null)
            openMessage.SetActive(false);

        // اظهار شكرا
        if (thanksMessage != null)
            thanksMessage.SetActive(true);

        if (companion != null)
        {
            companion.ReactToShell();
        }




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

using UnityEngine;

public class ShellOpen : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private bool isOpen = false;

    public followPlayer companion;

    public GameObject openMessage;
    public GameObject thanksMessage;

    private bool playerInside = false; 

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // البداية
        if (openMessage != null)
            openMessage.SetActive(true);

        if (thanksMessage != null)
            thanksMessage.SetActive(false);
    }

   private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("PlayerHand"))
    {
        if (!playerInside && !isOpen) 
        {
            playerInside = true;
            OpenShell();
        }
    }
}



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            playerInside = false; 
        }
    }

    void OpenShell()
    {
        isOpen = true;

        animator.SetBool("Open", true);

        if (thanksMessage != null)
        {
           // thanksMessage.SetActive(false);
            thanksMessage.SetActive(true);
        }

        if (openMessage != null)
            openMessage.SetActive(false);

        if (companion != null)
            companion.ReactToShell();

        Debug.Log("Shell opened.");

        if (audioSource != null)
            audioSource.Play();
        
        CancelInvoke();
        Invoke(nameof(CloseShell),5f);
    }

    void CloseShell()
    {
        isOpen = false;

        animator.SetBool("Open", false);

        if (audioSource != null)
            audioSource.Stop();

        if (openMessage != null)
            openMessage.SetActive(true);

        if (thanksMessage != null)
            thanksMessage.SetActive(false);
    }
}




























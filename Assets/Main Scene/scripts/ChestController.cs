using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("References")]
    public GameObject winCup;
    public ParticleSystem openParticles;
    public AudioSource openSound;
    public Animator animator;

    private bool isOpened = false;

    private void Start()
    {
        if (winCup != null)
            winCup.SetActive(false);
    }

    public void OpenChest()
    {
        if (isOpened) return;

        isOpened = true;

        if (openSound != null)
            openSound.Play();

        if (animator != null)
            animator.SetTrigger("Open");
    }

    public void ShowWinCup()
    {
        if (winCup != null)
            winCup.SetActive(true);

        if (openParticles != null)
            openParticles.Play();
    }
}

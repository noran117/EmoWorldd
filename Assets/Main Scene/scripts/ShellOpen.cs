using UnityEngine;

public class ShellOpen : MonoBehaviour
{
    public Animator part1Animator;
    public Animator part2Animator;
    public Animator part3Animator;
    public string animationTriggerName = "OpenShell";
    private bool hasOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasOpened && other.CompareTag("Player"))
        {
            part1Animator.SetTrigger(animationTriggerName);
            part2Animator.SetTrigger(animationTriggerName);
            part3Animator.SetTrigger(animationTriggerName);
            hasOpened = true;
        }
    }
}

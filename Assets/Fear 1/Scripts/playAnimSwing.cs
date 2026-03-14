using UnityEngine;

public class playAnimSwing : MonoBehaviour
{
    public Animator anim;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Swing", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Swing", false);
        }
    }

}

using UnityEngine;

public class playAnimSwing : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Animator anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Swing");
            }
        }
    }
}

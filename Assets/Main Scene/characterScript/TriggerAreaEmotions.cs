using UnityEngine;

public class TriggerAreaEmotions : MonoBehaviour
{
    AnimationStateController anim;

    // الصوت الذي تريد تشغيله
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the area!");

            // تشغيل الصوت والأنيميشن
            anim.PlayDialogue(audioSource);
            anim.ReactToGate();
                //companion.PlayDialogue(clip);  
                //companion.ReactToGate();
            
        }
    }
}

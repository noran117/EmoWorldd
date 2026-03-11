using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SimpleGhost : MonoBehaviour
{
    private Animator Anim;
    private int count = 0;

    [SerializeField] private MaterialReveal areaReveal;
    [SerializeField] private ParticleSystem vanishEffect;
    enum RevealType
    {
        Magical,
        Smooth
    }
    [SerializeField] private RevealType revealType = RevealType.Smooth;
    [SerializeField] private SkinnedMeshRenderer[] MeshR;
    [SerializeField] private AudioSource vanishGhostSound;
    [SerializeField] private AudioSource revealAreaSound;
    [SerializeField] private GameObject fireworkEffect;

    private static readonly int IdleState = Animator.StringToHash("Base Layer.idle");
    private static readonly int DissolveState = Animator.StringToHash("Base Layer.dissolve");

    private float Dissolve_value = 1f;
    private bool DissolveFlg = false;

    void Start()
    {
        Anim = GetComponent<Animator>();
        Anim.CrossFade(IdleState, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Triggered");
        if (other.CompareTag("PlayerHand") && !DissolveFlg)
        {
            Anim.CrossFade(DissolveState, 0.1f);
            DissolveFlg = true;
            count++;
            if (vanishGhostSound != null)
                vanishGhostSound.Play();
            StartCoroutine(DissolveRoutine());
            if (count == 1)
            {
                FindObjectOfType<BigEffectController>().PlayFinalEffect();
            }
        }
    }
    private IEnumerator DissolveRoutine()
    {
        //Debug.Log("Dissolve Routine");

        while (Dissolve_value > 0f)
        {
            Dissolve_value -= Time.deltaTime;

            foreach (var mesh in MeshR)
            {
                mesh.material.SetFloat("_Dissolve", Dissolve_value);
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        if (vanishEffect != null)
        {
            vanishEffect.transform.parent = null;
            vanishEffect.Play();
        }
        if (revealAreaSound != null)
            revealAreaSound.Play();
        if (areaReveal != null)
        {
            if (revealType == RevealType.Magical)
                areaReveal.RevealMagical();
            else
                areaReveal.RevealSmooth();
        }

        //gameObject.SetActive(false);
        Anim.enabled = false;
        foreach (var rend in MeshR)
            rend.enabled = false;
    }



}

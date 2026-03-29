using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System;


public class SimpleGhost : MonoBehaviour
{
    private Animator Anim;

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

    private static readonly string animAttack = "attack";
    private static readonly string animDissolve = "dissolve";
    private bool isAttacking = false;

    private float Dissolve_value = 1f;
    private bool DissolveFlg = false;

    public Transform player;
    public float triggerDistance = 2f;

    void Start()
    {
        Anim = GetComponent<Animator>();
    }
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < triggerDistance && !isAttacking)
        {
            StartCoroutine(LookAtPlayer());
            Anim.SetTrigger("attackk");
            isAttacking = true;
        }
    }
    IEnumerator LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);     
        yield return new WaitForSeconds(0.5f);
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Triggered");
        if (other.CompareTag("PlayerHand") && !DissolveFlg)
        {
            Anim.SetBool(animDissolve, true);
            DissolveFlg = true;
            if (vanishGhostSound != null)
                vanishGhostSound.Play();
            StartCoroutine(DissolveRoutine());
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

        BigEffectController.Instance.GhostDestroyed();

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

        Anim.enabled = false;
        foreach (var rend in MeshR)
            rend.enabled = false;

        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }



}
 



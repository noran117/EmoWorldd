using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SimpleGhost : MonoBehaviour
{
    private Animator Anim;

    [SerializeField] private MaterialReveal areaReveal;
    enum RevealType
    {
        Magical,
        Smooth
    }
    [SerializeField] private RevealType revealType = RevealType.Smooth;

    [SerializeField] private SkinnedMeshRenderer[] MeshR;

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
        Debug.Log("Triggered");
        if (other.CompareTag("Player") && !DissolveFlg)
        {
            Debug.Log("Player");
            Anim.CrossFade(DissolveState, 0.1f);
            DissolveFlg = true;
            StartCoroutine(DissolveRoutine());
        }
    }
    private IEnumerator DissolveRoutine()
    {
        Debug.Log("Dissolve Routine");

        while (Dissolve_value > 0f)
        {
            Dissolve_value -= Time.deltaTime;

            foreach (var mesh in MeshR)
            {
                mesh.material.SetFloat("_Dissolve", Dissolve_value);
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

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

using UnityEngine;
using System.Collections;

public class SimpleGhost : MonoBehaviour
{
    private Animator Anim;

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
        if (other.CompareTag("Player") && !DissolveFlg)
        {
            Anim.CrossFade(DissolveState, 0.1f);
            DissolveFlg = true;
            StartCoroutine(DissolveRoutine());
        }
    }
 private IEnumerator DissolveRoutine()
    {
        while (Dissolve_value > 0f)
        {
            Dissolve_value -= Time.deltaTime;

            foreach (var mesh in MeshR)
            {
                mesh.material.SetFloat("_Dissolve", Dissolve_value);
            }

            yield return null; 
        }

        gameObject.SetActive(false);
    }
  
}

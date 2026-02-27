using UnityEngine;
using System.Collections;

public class DissolveOnTrigger : MonoBehaviour
{
    private MeshRenderer rend;
    private SkinnedMeshRenderer srend;
    private Material mat;
    private bool triggered = false;
    public float step = 0.01f;
    public float waittime = 0.001f;
    public bool hideOnDone = false;

    private void Awake()
    {
        if (!TryGetComponent<MeshRenderer>(out rend))
        {
            if (TryGetComponent<SkinnedMeshRenderer>(out srend))
            {
                mat = srend.material;
                return;
            }
        }
        else
        {
            mat = rend.material;
        }

    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Triggered");
        if(mat == null) return;
        if(!(other.tag == "Player")) return;

        Debug.Log("player");

        if(triggered) return;
        Debug.Log("already dissolved");


        StartCoroutine(dissolveEffect(hideOnDone));
        // rend.material.SetFloat("_Dissolve", 1);
        triggered = true;
    }
    
    IEnumerator dissolveEffect(bool hideOnDone)
    {
        Debug.Log("coroutine");

        for (float i = 0; i < 1; i+= step)
        {
            mat.SetFloat("_Dissolve", i);
            yield return new WaitForSeconds(waittime);
        }

        this.gameObject.SetActive(hideOnDone);
    }
}

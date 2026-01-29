using UnityEngine;

public class SugarInput : MonoBehaviour
{
    public GameObject cottonCandyPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sugar"))
        {
            Destroy(other.gameObject);
            MakeCottonCandy();
        }
    }

    void MakeCottonCandy()
    {
        Instantiate(cottonCandyPrefab, transform.position, Quaternion.identity);
    }
}

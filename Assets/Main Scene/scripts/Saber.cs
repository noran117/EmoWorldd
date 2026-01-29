using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Saber : MonoBehaviour
{
    // Dont forget to set the layer for the hand which tje cube will be its child in the inspector!
    public LayerMask layer;
    private Vector3 previousPos;

    void Start() { }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1, layer))
        {
            if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130)
            {
                Destroy(hit.transform.gameObject);
            }
        }

        previousPos = transform.position;
    }

}

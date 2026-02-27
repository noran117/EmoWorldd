//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

//public class Saber : MonoBehaviour
//{
//    // Dont forget to set the layer for the hand which tje cube will be its child in the inspector!
//    public LayerMask layer;
//    private Vector3 previousPos;

//    void Start() { }

//    void Update()
//    {
//        RaycastHit hit;
//        if (Physics.Raycast(transform.position, transform.forward, out hit, 1, layer))
//        {
//            if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130)
//            {
//                Destroy(hit.transform.gameObject);
//            }
//        }

//        previousPos = transform.position;
//    }

//}
using UnityEngine;

public class Saber : MonoBehaviour
{
    public float minCutSpeed = 1.0f;
    public float minCutAngle = 100f;

    Vector3 prevPos;

    void Update()
    {
        prevPos = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        var note = other.GetComponentInParent<NoteHitState>();
        if (note == null) return;

        if (!note.canBeHit) return;

        Vector3 velocity = (transform.position - prevPos) / Mathf.Max(Time.deltaTime, 0.0001f);
        if (velocity.magnitude < minCutSpeed) return;

        float angle = Vector3.Angle(velocity, other.transform.up);
        if (angle < minCutAngle) return;

        note.wasHit = true;

        Destroy(other.gameObject);
    }
}

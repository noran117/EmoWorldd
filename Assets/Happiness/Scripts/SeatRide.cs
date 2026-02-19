using System.Collections;
using UnityEngine;

public class SeatRide : MonoBehaviour
{
    public TrainMove trainMove;

    [Header("XR Rig")]
    public Transform xrRoot;       
    public Transform xrCamera;     

    [Header("Seat on Train")]
    public Transform seatPoint;    

    private Transform originalParent;

    private CharacterController cc;
    private bool wasCcEnabled;

    private MonoBehaviour[] allBehaviours;
    private bool[] wasEnabled;

    private bool riding = false;
    private bool finishing = false;

    private void Awake()
    {
        if (xrRoot != null)
        {
            cc = xrRoot.GetComponentInChildren<CharacterController>(true);
            allBehaviours = xrRoot.GetComponentsInChildren<MonoBehaviour>(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (xrRoot != null && other.transform.root != xrRoot) return;
        if (trainMove == null) return;

        trainMove.StartMoving();

        if (xrRoot == null || xrCamera == null || seatPoint == null) return;
        if (riding || finishing) return;

        riding = true;
        originalParent = xrRoot.parent;

        DisableMovement();
        AlignRigToPoint(seatPoint.position, seatPoint.eulerAngles.y);

        xrRoot.SetParent(trainMove.transform, true);
    }

    private void Update()
    {
        if (!riding || finishing || trainMove == null) return;

        if (!trainMove.IsMoving)
        {
            StartCoroutine(FinishRideDropAtDestination());
        }
    }

    private IEnumerator FinishRideDropAtDestination()
    {
        finishing = true;
        riding = false;

        Vector3 dropPos = trainMove.destination.position;
        float dropYaw = trainMove.destination.eulerAngles.y;

        xrRoot.SetParent(originalParent, true);

        AlignRigToPoint(dropPos, dropYaw);

        if (cc != null) cc.enabled = wasCcEnabled;

        yield return null;

        RestoreBehaviours();

        yield return null;

        AlignRigToPoint(dropPos, dropYaw);

        finishing = false;
    }

    private void AlignRigToPoint(Vector3 targetWorldPos, float targetYaw)
    {
        if (xrRoot == null || xrCamera == null) return;

        float camYaw = xrCamera.eulerAngles.y;
        xrRoot.Rotate(0f, targetYaw - camYaw, 0f);

        Vector3 offset = xrCamera.position - xrRoot.position;
        xrRoot.position = targetWorldPos - offset;
    }

    private void DisableMovement()
    {
        if (cc != null)
        {
            wasCcEnabled = cc.enabled;
            cc.enabled = false;
        }

        if (allBehaviours == null) return;

        wasEnabled = new bool[allBehaviours.Length];

        for (int i = 0; i < allBehaviours.Length; i++)
        {
            var b = allBehaviours[i];
            if (b == null) continue;

            wasEnabled[i] = b.enabled;

            string n = b.GetType().Name;

            bool looksLikeMovement =
                n.Contains("Locomotion") ||
                n.Contains("Teleport") ||
                n.Contains("Continuous") ||
                n.Contains("Move") ||
                n.Contains("SnapTurn") ||
                n.Contains("Turn");

            if (looksLikeMovement)
                b.enabled = false;
        }
    }

    private void RestoreBehaviours()
    {
        if (allBehaviours == null || wasEnabled == null) return;

        for (int i = 0; i < allBehaviours.Length; i++)
        {
            var b = allBehaviours[i];
            if (b == null) continue;

            b.enabled = wasEnabled[i];
        }
    }
}

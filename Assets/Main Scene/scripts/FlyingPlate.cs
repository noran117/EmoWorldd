using UnityEngine;

public class FlyingPlate : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 1f;

    private CharacterController playerController;
    private bool xrOnPlatform = false;
    private bool goingToB = true;

    void Start()
    {
        GameObject pcObj = GameObject.Find("PlayerController");

        if (pcObj == null)
        {
            Debug.LogError("[Platform] PlayerController NOT FOUND");
            return;
        }

        playerController = pcObj.GetComponent<CharacterController>();

        if (playerController == null)
        {
            Debug.LogError("[Platform] CharacterController NOT FOUND");
        }
    }

    void FixedUpdate()
    {
        if (!xrOnPlatform || playerController == null) return;

        Transform target = goingToB ? pointB : pointA;

        Vector3 oldPos = transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.fixedDeltaTime
        );

        Vector3 delta = transform.position - oldPos;
        playerController.Move(delta);

       
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            goingToB = !goingToB; 
        }
    }

    public void XR_EnterPlatform()
    {
        xrOnPlatform = true;
    }

    public void XR_ExitPlatform()
    {
        xrOnPlatform = false;
    }
}

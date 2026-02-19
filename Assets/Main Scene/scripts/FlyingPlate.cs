using UnityEngine;

public class FlyingPlate : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform pointD;
    private int currentIndex = 0;
    private Transform[] path;
    public float speed = 1f;
    public float moveDuration = 2f;
    public float arcHeight = 2f;
    private CharacterController playerController;
    private bool xrOnPlatform = false;
    private float timer = 0f;
    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        GameObject pcObj = GameObject.Find("PlayerController");

        if (pcObj == null)
        {
            Debug.LogError("[Platform] PlayerController NOT FOUND");
            return;
        }

        playerController = pcObj.GetComponent<CharacterController>();
        path = new Transform[] { pointA, pointB, pointD };
        if (playerController == null)
        {
            Debug.LogError("[Platform] CharacterController NOT FOUND");
        }
    }

    void FixedUpdate()
    {
        if (!xrOnPlatform || playerController == null) return;
        if (currentIndex >= path.Length) return;

        if (timer == 0f)
        {
            startPos = transform.position;
            endPos = path[currentIndex].position;
        }

        timer += Time.fixedDeltaTime;
        float progress = timer / moveDuration;

        Vector3 oldPos = transform.position;

        if (progress >= 1f)
        {
            transform.position = endPos;
            timer = 0f;
            currentIndex++;
        }
        else
        {
            Vector3 linearPos = Vector3.Lerp(startPos, endPos, progress);

            float heightOffset = Mathf.Sin(progress * Mathf.PI) * arcHeight;

            linearPos.y += heightOffset;

            transform.position = linearPos;
        }

        Vector3 delta = transform.position - oldPos;
        playerController.Move(delta);
    }

    public void XR_EnterPlatform()
    {
        xrOnPlatform = true;
        currentIndex = 0;
        timer = 0f;
    }

    public void XR_ExitPlatform()
    {
        xrOnPlatform = false;
    }
    void OnTriggerEnter(Collider other)
    {
        //if (other.GetComponent<CharacterController>())

        if (other.CompareTag("Player"))
        {
            XR_EnterPlatform();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            XR_ExitPlatform();
        }
    }
}
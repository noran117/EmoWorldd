using UnityEngine;

public class CompanionFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0.6f, 0f, 1.2f);
    public float smoothSpeed = 1.5f;

    void Update()
    {
        Vector3 targetPos = player.position + player.TransformDirection(offset);
        targetPos.y = player.position.y - 1.04f;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
        //transform.LookAt(player.position);
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(lookDir),
                Time.deltaTime * smoothSpeed
            );
    }
}

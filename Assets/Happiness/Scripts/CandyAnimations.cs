using UnityEngine;

public class CandyDance : MonoBehaviour
{
    [Header("������� ������� (���� ����)")]
    public Transform donutMesh;
    public float donutFloatSpeed = 2f;
    public float donutFloatHeight = 0.15f;

    [Header("������� ������ (��� ���� ����)")]
    public Transform cherryTransform;
    public float cherryDanceSpeed = 4f; // ���� �����
    public float cherryDanceAngle = 20f; // ��� �������

    private Vector3 donutStartPos;
    private Quaternion cherryStartRot;
    private float randomOffset;

    void Start()
    {
        if (donutMesh) donutStartPos = donutMesh.localPosition;
        if (cherryTransform) cherryStartRot = cherryTransform.localRotation;

        // ����� ������ ���� ������ �� ���� ��������� ����
        randomOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float time = Time.time + randomOffset;

        // 1. ���� ������� (Y Position)
        if (donutMesh)
        {
            float newY = donutStartPos.y + Mathf.Sin(time * donutFloatSpeed) * donutFloatHeight;
            donutMesh.localPosition = new Vector3(donutStartPos.x, newY, donutStartPos.z);
        }

        // 2. ���� ������ (Z Rotation - ��� ���� �����)
        if (cherryTransform)
        {
            // ���� Sin ���� ����� ���� ���� �����
            float angle = Mathf.Sin(time * cherryDanceSpeed) * cherryDanceAngle;

            // ����� ������� ��� ���� Z (���� �� ������ ������� ������)
            cherryTransform.localRotation = cherryStartRot * Quaternion.Euler(0, 0, angle);
        }
    }
}
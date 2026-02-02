using UnityEngine;

public class CandyDance : MonoBehaviour
{
    [Header("≈⁄œ«œ«  «·œÊ‰«  (ÿ«·⁄ ‰«“·)")]
    public Transform donutMesh;
    public float donutFloatSpeed = 2f;
    public float donutFloatHeight = 0.15f;

    [Header("≈⁄œ«œ«  «·ﬂ—“… (—ﬁ’ Ì„Ì‰ ‘„«·)")]
    public Transform cherryTransform;
    public float cherryDanceSpeed = 4f; // ”—⁄… «·Â“…
    public float cherryDanceAngle = 20f; // ﬁÊ… «·„Ì·«‰

    private Vector3 donutStartPos;
    private Quaternion cherryStartRot;
    private float randomOffset;

    void Start()
    {
        if (donutMesh) donutStartPos = donutMesh.localPosition;
        if (cherryTransform) cherryStartRot = cherryTransform.localRotation;

        // √Ê›”  ⁄‘Ê«∆Ì ⁄‘«‰ «·Õ—ﬂ… „«  ﬂÊ‰ „Ìﬂ«‰ÌﬂÌ… »Õ …
        randomOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float time = Time.time + randomOffset;

        // 1. Õ—ﬂ… «·œÊ‰«  (Y Position)
        if (donutMesh)
        {
            float newY = donutStartPos.y + Mathf.Sin(time * donutFloatSpeed) * donutFloatHeight;
            donutMesh.localPosition = new Vector3(donutStartPos.x, newY, donutStartPos.z);
        }

        // 2. Õ—ﬂ… «·ﬂ—“… (Z Rotation - —ﬁ’ Ì„Ì‰ Ê‘„«·)
        if (cherryTransform)
        {
            // Õ—ﬂ… Sin  ⁄ÿÌ  √—ÃÕ ‰«⁄„ Ì„Ì‰ ÊÌ”«—
            float angle = Mathf.Sin(time * cherryDanceSpeed) * cherryDanceAngle;

            // »‰ÿ»ﬁ «·„Ì·«‰ ⁄·Ï „ÕÊ— Z ( √ﬂœ „‰ «·„ÕÊ— «·„‰«”» ·„Ã”„ﬂ)
            cherryTransform.localRotation = cherryStartRot * Quaternion.Euler(0, 0, angle);
        }
    }
}
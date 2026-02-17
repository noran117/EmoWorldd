using UnityEngine;

public class LollipopJustSwing : MonoBehaviour
{
    [Header("≈⁄œ«œ«  «· „«Ì·")]
    public float swingAngle = 7f;    // ﬂ„ œ—Ã…  „Ì· Ì„Ì‰ Ê‘„«·
    public float swingSpeed = 1f;    // ”—⁄… «·Õ—ﬂ…

    private Quaternion startRot;
    private float randomOffset;

    void Start()
    {
        // Õ›Ÿ «·œÊ—«‰ «·√’·Ì ··„’«’…
        startRot = transform.rotation;
        // —ﬁ„ ⁄‘Ê«∆Ì ⁄‘«‰ „« Ì Õ—ﬂÊ« ﬂ·Â„ »‰›” «· ÊﬁÌ 
        randomOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        // Õ”«» “«ÊÌ… «·„Ì·«‰ »‰«¡ ⁄·Ï «·Êﬁ 
        float angle = Mathf.Sin(Time.time * swingSpeed + randomOffset) * swingAngle;

        //  ÿ»Ìﬁ «·„Ì·«‰ ⁄·Ï „ÕÊ— Z (Ì„Ì‰ Ê‘„«·)
        // ≈–« „«·  ··√„«„ »œ· «·Ì„Ì‰° €Ì— «·‹ Z ›Ì «·”ÿ— «··Ì  Õ  ·‹ X
        transform.rotation = startRot * Quaternion.Euler(0, 0, angle);
    }
}
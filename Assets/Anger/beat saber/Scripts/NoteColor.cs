using UnityEngine;

public class NoteColor : MonoBehaviour
{
    public SaberColorType color = SaberColorType.Red;

    public Renderer rend;
    public Material redMat;
    public Material blueMat;

    void Awake()
    {
        if (rend == null)
            rend = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        Debug.Log(gameObject.name + " -> NoteColor = " + color);

        if (rend == null)
        {
            Debug.LogWarning(gameObject.name + " -> Renderer is NULL");
            return;
        }

        if (color == SaberColorType.Red && redMat != null)
            rend.material = redMat;
        else if (color == SaberColorType.Blue && blueMat != null)
            rend.material = blueMat;
    }
}
using UnityEngine;

public class NoteColor : MonoBehaviour
{
    public SaberColorType color = SaberColorType.Red;

    public Renderer rend;
    public Material redMat;
    public Material blueMat;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
    }
    void Start()
    {
        if (!rend) rend = GetComponentInChildren<Renderer>();

        if (rend)
        {
            if (color == SaberColorType.Red && redMat) rend.material = redMat;
            if (color == SaberColorType.Blue && blueMat) rend.material = blueMat;
        }
    }
}
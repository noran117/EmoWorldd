using UnityEngine;

public class ImageScreenController : MonoBehaviour
{
    public Texture[] images;
    private int index = 0;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (images.Length > 0)
        {
            rend.material.mainTexture = images[0];
        }
    }

    public void NextImage()
    {
        if (images.Length == 0) return;

        index++;
        if (index >= images.Length)
            index = 0;

        rend.material.mainTexture = images[index];
    }
}

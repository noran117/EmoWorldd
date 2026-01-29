using UnityEngine;

public class BallColor : MonoBehaviour
{
    public Color[] colors;
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetRandomColor()
    {
        if (colors.Length > 0)
        {
            Color randomCol = colors[Random.Range(0, colors.Length)];
            rend.material.color = randomCol;
        }
    }
}

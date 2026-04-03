using UnityEngine;

public class HappinessBall : MonoBehaviour
{
    public float lifetime = 10f;
    public Color[] colors;
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
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

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VRTwoPointsLine : MonoBehaviour
{
    [Header("Line Points")]
    public Transform pointA;
    public Transform pointB;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        // 1. ÊİÚíá ÅÍÏÇËíÇÊ ÇáÚÇáã áÖãÇä ÇáÏŞÉ
        line.useWorldSpace = true;
        line.positionCount = 2;

        // 2. ÊÃßÏ ãä Ãä ÇáÜ Shader ãÊæÇİŞ ãÚ URP
        if (line.sharedMaterial == null || line.sharedMaterial.shader.name.Contains("Standard"))
        {
            line.material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        }
    }

    void LateUpdate()
    {
        if (!pointA || !pointB) return;

        // 3. ÅÚØÇÁ ÇáãæÇŞÚ ÇáÚÇáãíÉ ãÈÇÔÑÉ Ïæä ÊÍæíáÇÊ ãÚŞÏÉ
        line.SetPosition(0, pointA.position);
        line.SetPosition(1, pointB.position);
    }
}
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public Material normalSkybox;    // ÇáÓßÇí ÈæßÓ ÇáØÈíÚí
    public Material accidentSkybox;  // ÇáÓßÇí ÈæßÓ ÇáÃÓæÏ / ÍÇÏË

    void Update()
    {
        // ÊÌÑÈÉ: ÇÖÛØí Úáì ÒÑ "A" áÊÛííÑ ÇáÓßÇí ÈæßÓ
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeSkybox(accidentSkybox);
        }

        // ÊÌÑÈÉ: ÇÖÛØí Úáì ÒÑ "S" ááÑÌæÚ ááäåÇÑ ÇáØÈíÚí
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeSkybox(normalSkybox);
        }
    }

    void ChangeSkybox(Material newSkybox)
    {
        RenderSettings.skybox = newSkybox;
        DynamicGI.UpdateEnvironment(); // åĞÇ ãåã ÌÏÇğ áÊÍÏíË ßá ÇáÃáæÇä
    }
}

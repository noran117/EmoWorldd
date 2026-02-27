using UnityEngine;
using UnityEngine.UI;

public class SafeSkyboxLightController : MonoBehaviour
{
    public Slider slider;

    [Header("Safe Lighting Range")]
    public float minExposure = 0.3f;   // √€„ﬁ Õœ „”„ÊÕ
    public float maxExposure = 1f;   // √› Õ Õœ „”„ÊÕ

    void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0.5f;

        slider.onValueChanged.AddListener(ChangeExposure);
    }

    void ChangeExposure(float value)
    {
        float safeExposure = Mathf.Lerp(minExposure, maxExposure, value);
        RenderSettings.skybox.SetFloat("_Exposure", safeExposure);
        DynamicGI.UpdateEnvironment();
    }
}

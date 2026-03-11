using UnityEngine;
using TMPro;
using ArabicSupport; // ÊÃßÏ Ãä ãáİÇÊ ÇáãßÊÈÉ ãæÌæÏÉ İí ÇáãÔÑæÚ

[ExecuteInEditMode] // åĞÇ ÇáÓØÑ ÓíÌÚáß ÊÑì ÇáäÊíÌÉ İæÑÇğ ÍÊì ÈÏæä ÊÔÛíá ÇááÚÈÉ
public class ArabicFixerTMP : MonoBehaviour
{
    [TextArea(3, 10)]
    public string originalText; // åäÇ ÊßÊÈ ÇáäÕ ÇáØÈíÚí

    private TextMeshProUGUI tmpComponent;

    void OnValidate() // ÊÚãá İæÑÇğ ÚäÏ ÊÛííÑ Ãí ÍÑİ İí ÇáÜ Inspector
    {
        UpdateText();
    }

    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (tmpComponent == null) tmpComponent = GetComponent<TextMeshProUGUI>();

        if (tmpComponent != null && !string.IsNullOrEmpty(originalText))
        {
            // ÏÇáÉ Fix åí ÇáÊí ÊÊæáì ãåãÉ ŞáÈ ÇáäÕ æÊæÕíá ÇáÍÑæİ
            tmpComponent.text = ArabicFixer.Fix(originalText, false, false);
        }
    }
}
using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsToggleButtonBehaviour : MonoBehaviour
{
    [SerializeField] string EnabledText;
    [SerializeField] string DisabledText;


    [SerializeField] Image ButtonImage;
    [SerializeField] TMP_Text ButtonText;

    public void Enable(bool value)
    {
        ButtonImage.material = value ? null : VisualContent.Instance.GrayScaleMaterial;
        ButtonText.text = Locales.Get(value ?  EnabledText : DisabledText);
    }
}

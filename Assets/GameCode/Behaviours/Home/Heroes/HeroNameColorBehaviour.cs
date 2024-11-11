using Legacy.Database;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class HeroNameColorBehaviour : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI NameText;
        [SerializeField, Range(-1.0f, 1.0f)] private float TextGradientUpLighter;
        [SerializeField, Range(-1.0f, 1.0f)] private float TextGradientDownLighter;

        internal void SetLighters(float DownLighter, float UpLighter)
        {
            TextGradientDownLighter = DownLighter;
            TextGradientUpLighter = UpLighter;
        }

        internal void SetColor(Color color)
        {
            var color1ForText = color * (1.0f + TextGradientDownLighter);
            var color2ForText = color * (1.0f + TextGradientUpLighter);
            NameText.colorGradient = new VertexGradient(color2ForText, color2ForText, color1ForText, color1ForText);
        }

        internal void SetName(string title, Color color)
        {
            SetColor(color);
            NameText.text = Locales.Get(title);
        }
    }
}

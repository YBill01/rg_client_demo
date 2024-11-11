using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class LootCurrencyBehaviour : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI ValueText;
        [SerializeField] Image Icon;
        public void Init(CurrencyType type, int valueMin, uint valueMax = 0)
        {
            Icon.sprite = VisualContent.Instance.GetCurrencyIcon(type);
            string text = valueMin.ToString();
            if(valueMax > valueMin)
            {
                text += $" - {valueMax}";
            }
            if (type == CurrencyType.Soft && valueMin > 0)
                text = "+" + text;
            ValueText.text = text;
        }
    }
}

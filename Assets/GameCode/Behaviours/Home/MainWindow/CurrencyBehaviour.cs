using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class CurrencyBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private Image Icon;

        internal void SetValue(int v)
        {
            text.text = LegacyHelpers.FormatByDigits(v.ToString());
        }

    }
}

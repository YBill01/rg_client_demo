using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class DailyDealsBasicCardBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text amount;

        protected DailyDealsOfferBehaviour offerParent;

        public virtual void InitData(PlayerDailyDealsItem item, DailyDealsOfferBehaviour offer)
        {
            offerParent = offer;
        }
        
        protected void SetAmount(string text)
        {
            amount.text = /*"<size=50%>x </size>" + */ "X" + LegacyHelpers.FormatByDigits(text);
        }
    }
}
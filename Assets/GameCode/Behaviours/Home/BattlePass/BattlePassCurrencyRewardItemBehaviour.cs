using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassCurrencyRewardItemBehaviour : BattlePassRewardItemBasicBehaviour
    {
        [SerializeField] 
        private TMP_Text count;
        [SerializeField] 
        private GameObject regularItem;
        [SerializeField] 
        private GameObject scaledItem;

        public void Init(ushort amount)
        {
            count.text = amount.ToString();
        }

        public override void ScaleToCurrentState()
        {
            regularItem.SetActive(false);
            scaledItem.SetActive(true);
        }

        public override void ScaleToRegularState()
        {
            regularItem.SetActive(true);
            scaledItem.SetActive(false);
        }
    }
}
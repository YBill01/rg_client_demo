using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
    public class DailyDealsCurrenciesCardBehaviour : DailyDealsBasicCardBehaviour
    {
        [SerializeField]
        private Color32 raysColor;
        [SerializeField]
        private Color32 lightColor;
        
        private const string COINS_OFFER = "locale:835";
        private const string GEMS_OFFER = "locale:832";
        
        public override void InitData(PlayerDailyDealsItem item, DailyDealsOfferBehaviour offer)
        {
            base.InitData(item, offer);

            var currencyAmount = item.treasure.count;
            SetAmount(currencyAmount.ToString());
            
            switch (item.type)
            {
                case DailyDealsTreasureType.Hard:
                    offerParent.SetTitle(GEMS_OFFER);
                    break;
                case DailyDealsTreasureType.Soft:
                    offerParent.SetTitle(COINS_OFFER);
                    break;
            }
            
            offerParent.SetRaysImageColor(raysColor);
            offerParent.SetLightImageColor(lightColor);
        }
    }
}
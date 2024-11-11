using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
    public class DailyDealsCardBehaviour : DailyDealsBasicCardBehaviour
    {
        [Serializable]
        private struct ColorsForRarity
        {
            public CardRarity Rarity;
            public Color32 RaysColor;
            public Color32 LightColor;
        }
        
        [SerializeField]
        private CardViewBehaviour viewBehaviour;
        [SerializeField]
        private CardTextDataBehaviour cardText;
        [SerializeField]
        private List<ColorsForRarity> colorsForRarities;

        private BinaryCard currentCard;

        public override void InitData(PlayerDailyDealsItem item, DailyDealsOfferBehaviour offer)
        {
            base.InitData(item, offer);
            
            if (Cards.Instance.Get(item.treasure.tid, out currentCard))
            {
                offerParent.SetTitle(currentCard.title);
                ChangeVisualizationBasedOnRarity(currentCard.rarity);

                UpdateCardData();
            }
            
            SetAmount(item.treasure.count.ToString());
        }

        public void UpdateCardData()
        {
            viewBehaviour.Init(currentCard);

            var cardItem = ClientWorld.Instance.GetExistingSystem<HomeSystems>().UserProfile.Inventory.GetCardData(currentCard.index);
            viewBehaviour.SetLabelNew();
            if (cardItem.index != 0)
            {
                cardText.SetProgressBar(cardItem.level, cardItem.count);
            }
        }

        private void ChangeVisualizationBasedOnRarity(CardRarity rarity)
        {
            var colors = colorsForRarities.Find(x => x.Rarity.Equals(rarity));
            offerParent.SetRaysImageColor(colors.RaysColor);
            offerParent.SetLightImageColor(colors.LightColor);
        }
    }
}
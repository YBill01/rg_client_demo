using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class DailyDealsOfferBehaviour : BasicOfferBehaviour
    {
        public const string OFFER_TYPE_FREE = "type_free";
        public const string OFFER_TYPE_REGULAR = "type_regular";

        [SerializeField] 
        protected Transform cardParent;
        [SerializeField] 
        private GameObject boughtItems;
        [SerializeField] 
        private GameObject freeLabelItems;
        [SerializeField] 
        private GameObject coinsIcon;
        [SerializeField] 
        private GameObject gemsIcon;
        [SerializeField] 
        private GameObject buttonImage;
        [SerializeField] 
        private Image raysImage;
        [SerializeField] 
        private Image lightImage;

        PlayerDailyDealsItem offer;

        private string offerType;
        

        private DailyDealsBasicCardBehaviour currentCard;

        public void CreateCard(DailyDealsBasicCardBehaviour cardPrefab, PlayerDailyDealsItem offer)
        {
            this.offer = offer;
            currentCard = Instantiate(cardPrefab, cardParent);
            currentCard.InitData(offer, this);
        }

        public void SetBoughtState(bool isBought)
        {
            buyButton.interactable = !isBought;
            if (offerType == DailyDealsOfferBehaviour.OFFER_TYPE_REGULAR)
			{
                if (isBought)
                {
                    string cardID = offer.treasure.tid.ToString();
                    int cardCount = (int)offer.treasure.count;
                    CurrencyType currencyType = CurrencyType.Soft;
                    if(offer.hard > 0)
                    {
                        currencyType = CurrencyType.Hard;
                    }
                    int cardPrice = currencyType == CurrencyType.Soft ? (int)offer.soft : (int)offer.hard;

                    AnalyticsManager.Instance.InAppPurchase(cardID, InAppPurchaseType.BuyCards.ToString(), cardCount, cardPrice, currencyType.ToString());
                }
                buttonImage.SetActive(!isBought);
            }
            boughtItems.SetActive(isBought);
			if (freeLabelItems != null)
			{
				freeLabelItems.SetActive(!isBought);
			}
		}

        public void SetHardCurrencyPrice(bool isHard)
        {
            coinsIcon.SetActive(!isHard);
            gemsIcon.SetActive(isHard);
        }

        public void SetOfferType(string value)
        {
            offerType = value;
        }

        public void SetRaysImageColor(Color32 newColor)
        {
            //raysImage.color = newColor;
        }

        public void SetLightImageColor(Color32 newColor)
        {
            //lightImage.color = newColor;
        }

        public DailyDealsBasicCardBehaviour GetCard()
        {
            return currentCard;
        }
    }
}
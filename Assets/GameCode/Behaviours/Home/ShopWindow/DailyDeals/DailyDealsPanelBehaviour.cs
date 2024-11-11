using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using UnityEngine;
using static Legacy.Client.LootBoxWindowBehaviour;

namespace Legacy.Client
{
    public class DailyDealsPanelBehaviour : ShopPanelBehaviour
    {
        [SerializeField] 
        private UITimerBehaviour timer;
        [SerializeField] 
        private DailyDealsOfferBehaviour freeOfferPrefab;
        [SerializeField] 
        private DailyDealsOfferBehaviour regularOfferPrefab;
        [SerializeField] 
        private DailyDealsCurrenciesCardBehaviour coinsCardPrefab;
        [SerializeField] 
        private DailyDealsCurrenciesCardBehaviour gemsCardPrefab;
        [SerializeField] 
        private DailyDealsCardBehaviour cardPrefab;
        [SerializeField] 
        private DailyDealsLootBoxCardBehaviour lootBoxCardPrefab;
        [SerializeField] 
        private BasicSectionBehaviour sectionParent;
        [SerializeField]
        private RectTransform sectionParentNew;

        private List<DailyDealsOfferBehaviour> createdOffers;

        protected override void InitData()
        {
            base.InitData();
            
            createdOffers = new List<DailyDealsOfferBehaviour>();
            timer.SetFinishedTime(profile.dailyDeals.nextGeneration);
            var dailyOffers = profile.dailyDeals.offers;
            
            for (int i = 0; i < dailyOffers.Count; i++)
            {
                var offerPrefab = dailyOffers[i].hard == 0 & dailyOffers[i].soft == 0 ? freeOfferPrefab : regularOfferPrefab;
                CreateOffer(dailyOffers[i], offerPrefab, (ushort)i, sectionParentNew);
            }
        }

        private void CreateOffer(PlayerDailyDealsItem dailyDealsItem, DailyDealsOfferBehaviour offerPrefab, ushort index, RectTransform parentNew = null)
        {
            var offer = Instantiate(offerPrefab, parentNew);
            //var offer = Instantiate(offerPrefab, sectionParent.GetOffersHolder());

            switch (dailyDealsItem.type)
            {
                case DailyDealsTreasureType.Soft:
                    offer.CreateCard(coinsCardPrefab, dailyDealsItem);
                    break;
                case DailyDealsTreasureType.Hard:
                    offer.CreateCard(gemsCardPrefab, dailyDealsItem);
                    break;
                case DailyDealsTreasureType.Cards:
                    offer.CreateCard(cardPrefab, dailyDealsItem);
                    SubscribeToUpdateEvent(offer);
                    break;
                case DailyDealsTreasureType.LootBoX:
                    offer.CreateCard(lootBoxCardPrefab, dailyDealsItem);
                    break;
            }

            offer.SetOfferType((dailyDealsItem.hard == 0 && dailyDealsItem.soft == 0) ? DailyDealsOfferBehaviour.OFFER_TYPE_FREE : DailyDealsOfferBehaviour.OFFER_TYPE_REGULAR);
            offer.SetOfferIndex(index);
            offer.SetBoughtState(dailyDealsItem.buyed);
            
            if (dailyDealsItem.hard > 0 || dailyDealsItem.soft > 0)
            {
                var isHardCurrency = dailyDealsItem.hard > 0;
                offer.SetHardCurrencyPrice(isHardCurrency);
                offer.SetBuyButtonText(isHardCurrency ? dailyDealsItem.hard.ToString() : dailyDealsItem.soft.ToString());
            }

            offer.BuyButtonClick += OnOfferBuyButtonClick;
            
            createdOffers.Add(offer);
        }

        private void SubscribeToUpdateEvent(DailyDealsOfferBehaviour offer)
        {
            if (offer.GetCard() is DailyDealsCardBehaviour card)
            {
                profile.PlayerProfileUpdated.AddListener(card.UpdateCardData);
            }
        }

        private void OnOfferBuyButtonClick(ushort offerIndex, BasicOfferBehaviour offerBehaviour)
        {
            if (profile.dailyDeals.offers.Count > offerIndex)
            {
                var item = profile.dailyDeals.offers[offerIndex];

                if (item.buyed)
                {
                    // Add here message with locale "Already bought"
                    return;
                }

                var isFree = item.hard == 0 && item.soft == 0;
                if (isFree)
                {
                    BuyItem(offerIndex, offerBehaviour);
                }
                else
                {
                    switch (item.type)
                    {
                        case DailyDealsTreasureType.Soft:
                        case DailyDealsTreasureType.Hard:
                            // Check if currency open correctly.
                            // Now we don't have currency in DailyDeals offers
                            parentShopWindow.OpenConfirmDailyPurchasePopup(item, offerIndex, offerBehaviour, BuyItem);
                            break;
                        case DailyDealsTreasureType.Cards:
                            parentShopWindow.OpenConfirmDailyPurchasePopup(item, offerIndex, offerBehaviour, BuyItem);
                            break;
                        case DailyDealsTreasureType.LootBoX:
                            if (createdOffers[offerIndex].GetCard() is DailyDealsLootBoxCardBehaviour lootBoxCard)
                            {
                                var boxToOpen = lootBoxCard.GetBox();
                                parentShopWindow.OpenLootBoxPopUpWindow(boxToOpen, item, offerIndex, offerBehaviour, BuyItem);                      
                            }
                            break;
                    }
                }
            }
        }

        private void BuyItem(ushort offerIndex, BasicOfferBehaviour offerBehaviour)
        {
            var offer = profile.dailyDeals.offers[offerIndex];
            if (PlayerHasEnoughMoney(offer.hard, offer.soft))
            {
                profile.BuyDailyDeal(offerIndex);

                createdOffers[offerIndex].SetBoughtState(true);

                switch (offer.type)
                {
                    case DailyDealsTreasureType.Soft:
                        RewardParticlesBehaviour.Instance.Drop(offerBehaviour.transform.position, 10, LootCardType.Soft);
                        break;
                    case DailyDealsTreasureType.Hard:
                        RewardParticlesBehaviour.Instance.Drop(offerBehaviour.transform.position, 10, LootCardType.Hard);
                        break;
                    case DailyDealsTreasureType.Cards:
                        byte amount = 10; //(byte) offer.treasure.count;
                        RewardParticlesBehaviour.Instance.Drop(offerBehaviour.transform.position, amount, LootCardType.Cards);
                        break;
                    case DailyDealsTreasureType.LootBoX:
                        if (createdOffers[offerIndex].GetCard() is DailyDealsLootBoxCardBehaviour lootBoxCard)
                        {
                            var boxToOpen = lootBoxCard.GetBox();
                            parentShopWindow.OpenLootBoxWindow(boxToOpen);
                            boxToOpen.ChangeState(LootBoxBehaviour.BoxState.SlotsFull);
                        }
                        break; 
                }
            }
            else
            {
                WindowManager.Instance.OpenNotEnoughCoinsWindow(offer.soft - profile.Stock.getItem(CurrencyType.Soft).Count);
                //parentShopWindow.RedirectToSection(offer.hard == 0 ? RedirectMenuSection.BankCoins : RedirectMenuSection.BankGems);
            }
        }

        private bool PlayerHasEnoughMoney(uint hard, uint soft)
        {
            return hard <= profile.Stock.getItem(CurrencyType.Hard).Count &&
                   soft <= profile.Stock.getItem(CurrencyType.Soft).Count;
        }

        public override void ClearData()
        {
            base.ClearData();
            
            if (createdOffers == null) return;

            for (int i = 0; i < createdOffers.Count; i++)
            {
                createdOffers[i].BuyButtonClick -= OnOfferBuyButtonClick;
                
                if (createdOffers[i].GetCard() is DailyDealsCardBehaviour card)
                {
                    profile.PlayerProfileUpdated.RemoveListener(card.UpdateCardData);
                }
                
                Destroy(createdOffers[i].gameObject);
            }
            
            createdOffers.Clear();
        }
    }
}
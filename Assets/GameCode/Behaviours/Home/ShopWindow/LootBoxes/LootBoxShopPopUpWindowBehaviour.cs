using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Legacy.Database;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class LootBoxShopPopUpWindowBehaviour : WindowBehaviour
    {
        private enum ShopType
        {
            DailyDeals,
            LootBoxShop
        }

        [SerializeField] 
        private Animator WindowAnimator;
        [SerializeField] 
        private LootBoxPopUpDataBehaviour LootDataBehaviour;
        [SerializeField] 
        private TMP_Text ButtonPrice;
        [SerializeField] 
        private RectTransform BoxContainer;
        [SerializeField, Range(0.0f, 1.0f)] 
        private float ScaleMultiplier;
        [SerializeField] private GameObject coinsIcon;
        [SerializeField] private GameObject gemsIcon;

        private LootBoxViewBehaviour lootBoxView;
        private ShopWindowBehaviour shopWindow;
        private ProfileInstance profile;
        private BinaryMarketLoot boxOffer;
        private PlayerDailyDealsItem dailyItem;

        private ShopType shopType;
        private ushort index;
        private ushort hardPrice;
        private ushort offerIndex;
        private Action<ushort, BasicOfferBehaviour> onButtonClicked;
        private BasicOfferBehaviour offerBehaviour;

        public override void Init(Action callback)
        {
            callback();
        }

        public void SetOffer(LootBoxViewBehaviour box, BinaryMarketLoot offer, ShopWindowBehaviour shop,
            ProfileInstance profile)
        {
            SetOffer(box, shop, profile);
            boxOffer = offer;
            shopType = ShopType.LootBoxShop;

            hardPrice = boxOffer.hardPrice;
            index = boxOffer.index;
        }

        public void SetOffer(LootBoxViewBehaviour box, PlayerDailyDealsItem item, ShopWindowBehaviour shop,
            ProfileInstance profile, ushort offerIndex, BasicOfferBehaviour offerBehaviour, 
            Action<ushort, BasicOfferBehaviour> onButtonClicked)
        {
            SetOffer(box, shop, profile);
            dailyItem = item;
            shopType = ShopType.DailyDeals;

            hardPrice = (ushort) item.hard;            
            index = item.treasure.tid;
            this.offerIndex = offerIndex;
            this.offerBehaviour = offerBehaviour;
            this.onButtonClicked = onButtonClicked;
        }

        private void SetOffer(LootBoxViewBehaviour box, ShopWindowBehaviour shop, ProfileInstance profile)
        {
            lootBoxView = box;
            shopWindow = shop;
            this.profile = profile;
        }

        public void MissClick()
        {
            DestroyImmediate(lootBoxView.gameObject);
            WindowManager.Instance.ClosePopUp();
        }

        public void BuyClick()
        {
            WindowManager.Instance.ClosePopUp();
            (parent as ShopWindowBehaviour).SaveClickedData(boxOffer.lootbox, hardPrice);

            switch (shopType)
            {
                case ShopType.LootBoxShop:
                    BuyByHard();
                    break;
                case ShopType.DailyDeals:
                    onButtonClicked?.Invoke(offerIndex, offerBehaviour);
                    break;
            }
        }

        private void BuyByHard()
        {
            if (hardPrice <= profile.Stock.getItem(CurrencyType.Hard).Count)
            {
                profile.BuyLootBox(index);
                shopWindow.OpenLootBoxWindow(lootBoxView);
            }
            else
            {
                shopWindow.RedirectToSection(RedirectMenuSection.BankGems);
            }
        }
        
        protected override void SelfOpen()
        {
            ReParentClickedBox();
            if(lootBoxView != null)
            {
                switch (shopType)
                {
                    case ShopType.LootBoxShop:
                        LootDataBehaviour.ResetData();
                        if (Loots.Instance.Get(boxOffer.lootbox, out BinaryLoot binaryBox))
                            LootDataBehaviour.Init(binaryBox, boxOffer.arena, true);
                        SetBuyButtonPrice();
                        break;
                    case ShopType.DailyDeals:
                        LootDataBehaviour.ResetData();
                        var arenaIndex = profile.CurrentArena.index;
                        if (Loots.Instance.Get(dailyItem.treasure.tid, out BinaryLoot loot))
                            LootDataBehaviour.Init(loot, arenaIndex, true);
                        SetBuyButtonPrice();
                        break;
                }
            }
            gameObject.SetActive(true);
        }

        private void ReParentClickedBox()
        {
          
            var reset = lootBoxView.gameObject.GetComponent<RectScaleToBehaviour>();
            if (reset)
            {
                reset.Reset();
                Debug.Log("Reset RectScaleToBehaviour Scale");
            }
            lootBoxView = Instantiate(lootBoxView.gameObject, BoxContainer).GetComponent<LootBoxViewBehaviour>();
            lootBoxView.Init(LootBoxBehaviour.BoxState.Opening, lootBoxView.BinaryData);
           // lootBoxView.SetScaleMultiplier(ScaleMultiplier); // до ресет функции в RectScale
            lootBoxView.SetScaleMultiplier(0.5f);  
            lootBoxView.SetPopUpLayer(true);
        }

        private void SetBuyButtonText(string text)
        {
            ButtonPrice.text = LegacyHelpers.FormatByDigits(text);
        }

        private void SetBuyButtonPrice()
        {
            if (hardPrice > 0 || dailyItem.soft > 0)
            {
                var isHardCurrency = hardPrice > 0;
                SetHardCurrencyPrice(isHardCurrency);
                SetBuyButtonText(isHardCurrency ? hardPrice.ToString() : dailyItem.soft.ToString());
            }
        }

        private void SetHardCurrencyPrice(bool isHard)
        {
            coinsIcon.SetActive(!isHard);
            gemsIcon.SetActive(isHard);
        }

        protected override void SelfClose()
        {
            WindowAnimator.Play("Close");
        }
    }
}
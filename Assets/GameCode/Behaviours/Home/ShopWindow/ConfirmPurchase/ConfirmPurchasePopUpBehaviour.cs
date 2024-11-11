using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class ConfirmPurchasePopUpBehaviour : WindowBehaviour
    {
        [SerializeField] private Animator WindowAnimator;
        //[SerializeField] private ConfirmPurchaseCardBehaviour ConfirmPurchaseCard;
        [SerializeField] private ConfirmPurchaseCardBehaviour LootCard;
        [SerializeField] private ConfirmPurchaseCurrencyBehaviour CurrencyCard;
        [SerializeField] private RectTransform CardContainer;


        [Space(10)]
        [SerializeField] private TMP_Text ButtonPrice;
        [SerializeField] private TMP_Text title;
        [SerializeField] private GameObject coinsIcon;
        [SerializeField] private GameObject gemsIcon;
        [SerializeField, Range(0.0f, 1.0f)] private float ScaleMultiplier;

        private PlayerDailyDealsItem item;
        private BinaryBank bankItem;
        private ushort offerIndex;
        private BasicOfferBehaviour offerBehaviour;
        private Action<ushort, BasicOfferBehaviour> onButtonClicked;
        private ConfirmPurchaseBasicCardBehaviour cardView;

        public override void Init(Action callback)
        {
            title.text = Locales.Get("locale:1957");
            callback();
        }

        public void SetData(PlayerDailyDealsItem item, ushort offerIndex, BasicOfferBehaviour offerBehaviour, 
            Action<ushort, BasicOfferBehaviour> onButtonClicked)
        {
            this.item = item;
            this.offerIndex = offerIndex;
            this.offerBehaviour = offerBehaviour;
            this.onButtonClicked = onButtonClicked;

            UpdateDailyData();
        }

        public void SetData(BinaryBank bankItem, ushort offerIndex, BasicOfferBehaviour offerBehaviour,
    Action<ushort, BasicOfferBehaviour> onButtonClicked)
        {
            this.bankItem = bankItem;
            this.offerIndex = offerIndex;
            this.offerBehaviour = offerBehaviour;
            this.onButtonClicked = onButtonClicked;

            UpdateBankData();
        }

        private void UpdateDailyData()
        {
            cardView = Instantiate(LootCard, CardContainer);
            cardView.InitData(item);

            SetBuyButtonPrice(item.hard, item.soft);
        }

        private void UpdateBankData()
        {
            cardView = Instantiate(CurrencyCard, CardContainer);
            cardView.InitData(bankItem);

            SetBuyButtonPrice(bankItem.hardPrice, 0);
        }

        private void SetBuyButtonPrice(uint hard, uint soft)
        {
            if (hard > 0 || soft > 0)
            {
                var isHardCurrency = hard > 0;
                SetHardCurrencyPrice(isHardCurrency);
                SetBuyButtonText(isHardCurrency ? hard.ToString() : soft.ToString());
            }
        }

        private void SetHardCurrencyPrice(bool isHard)
        {
            coinsIcon.SetActive(!isHard);
            gemsIcon.SetActive(isHard);
        }

        private void SetBuyButtonText(string text)
        {
            ButtonPrice.text = LegacyHelpers.FormatByDigits(text);
        }

        public void MissClick()
        {
            WindowManager.Instance.ClosePopUp();
        }

        public void BuyClick()
        {
            WindowManager.Instance.ClosePopUp();
            onButtonClicked?.Invoke(offerIndex, offerBehaviour);

            // redirect?

            //if (boxOffer.hardPrice <= profile.Stock.getItem(CurrencyType.Hard).Count)
            //{
            //    profile.BuyLootBox(boxOffer.index);
            //    shopWindow.OpenLootBoxWindow(lootBoxView);
            //}
            //else
            //{
            //    shopWindow.RedirectToSection(RedirectMenuSection.BankGems);
            //}
        }

        protected override void SelfOpen()
        {
            gameObject.SetActive(true);
        }

        protected override void SelfClose()
        {
            if (cardView != null) Destroy(cardView.gameObject);
            WindowAnimator.Play("Close");
        }
    }
}

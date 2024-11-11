using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class NotEnoughCoinsWindowBehaviour : WindowBehaviour
    {
        [SerializeField] Animator WindowAnimator;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text subtitleText;
        [SerializeField] private TMP_Text countBttnBuy;
        protected ProfileInstance profile;
        private int needCountSoft=0;
        private int needCountHard=0;
        private ShopWindowBehaviour shopWindow;
        public override void Init(Action callback)
        {
            titleText.text= Locales.Get("locale:1909");
            profile = ClientWorld.Instance.Profile;
            callback();
        }

        protected override void SelfClose()
        {
            WindowAnimator.Play("Close");
        }

        protected override void SelfOpen()
        {

            needCountSoft =Convert.ToInt32(settings["count"]);
            subtitleText.text = Locales.Get("locale:1912", $"<color=#f6c01a><size=130%>{(needCountSoft).ToString()}</size></color>");
            needCountHard = (int)Math.Ceiling((decimal)needCountSoft / 16);
            if (needCountHard < 1)
                needCountHard = 1;
            countBttnBuy.text = (needCountHard).ToString();
            gameObject.SetActive(true);
        }

        public void OnBuy()
        {
            if (profile.Stock.CanTake(CurrencyType.Hard, (uint)needCountHard))
            {
                AnalyticsManager.Instance.SoftExchange(needCountSoft, needCountHard);
                profile.BuyNotEnoughCoins(needCountSoft);
                WindowManager.Instance.CloseNotEnoughCoinsWindow();
                WindowManager.Instance.ClosePopUp();
            }
            else
            {
                WindowManager.Instance.ClosePopUp();
                WindowManager.Instance.MainWindow.OpenShopWithSection(RedirectMenuSection.BankGems);
            }
        }

        public void MissClick()
        {
              WindowManager.Instance.ClosePopUp();
        }
    }
}
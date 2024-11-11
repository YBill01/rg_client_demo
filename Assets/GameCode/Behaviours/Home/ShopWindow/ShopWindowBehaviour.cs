using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Legacy.Database;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ShopWindowBehaviour : WindowBehaviour
    {
        [SerializeField] 
        private List<ShopPanelBehaviour> ShopPanels;
        [SerializeField] 
        private List<ShopMenuButtonBehaviour> ShopButtons;
        [SerializeField] 
        public ShopScrollBehaviour ScrollPanel;
        [SerializeField] 
        public RectTransform MainRect;

        public LootBoxViewBehaviour BoxToOpen;
        
        private ProfileInstance profile;
        private RedirectMenuSection? redirectSection;

        public override void Init(Action callback)
        {
            profile = ClientWorld.Instance.GetExistingSystem<HomeSystems>().UserProfile;
            ScrollPanel.Init();
            EnableThisGameObject(true);
            EnableThisGameObject(false);
            callback();
        }

        private void EnableThisGameObject(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void OpenLootBoxWindow(LootBoxViewBehaviour boxToOpen)
        {
            BoxToOpen = boxToOpen;
            WindowManager.Instance.OpenWindow(childs_windows[0]);
        }

        public void OpenLootBoxPopUpWindow(LootBoxViewBehaviour boxToOpen, BinaryMarketLoot offer)
        {
            BoxToOpen = boxToOpen;
            if (childs_windows[2] is LootBoxShopPopUpWindowBehaviour popUpWindowBehaviour)
            {
                popUpWindowBehaviour.SetOffer(boxToOpen, offer, this, profile);
            }
            WindowManager.Instance.OpenWindow(childs_windows[2]);
        }

        public void OpenLootBoxPopUpWindow(LootBoxViewBehaviour boxToOpen, PlayerDailyDealsItem item, ushort offerIndex, BasicOfferBehaviour offerBehaviour,
            Action<ushort, BasicOfferBehaviour> onButtonClicked)
        {
            BoxToOpen = boxToOpen;
            if (childs_windows[2] is LootBoxShopPopUpWindowBehaviour popUpWindowBehaviour)
            {
                popUpWindowBehaviour.SetOffer(boxToOpen, item, this, profile, offerIndex, offerBehaviour, onButtonClicked);
            }
            WindowManager.Instance.OpenWindow(childs_windows[2]);
        }

        public void OpenConfirmDailyPurchasePopup(PlayerDailyDealsItem item, ushort offerIndex, BasicOfferBehaviour offerBehaviour, 
            Action<ushort, BasicOfferBehaviour>  onButtonClicked)
        {
            if (childs_windows[3] is ConfirmPurchasePopUpBehaviour popUpWindowBehaviour)
            {
                popUpWindowBehaviour.SetData(item, offerIndex, offerBehaviour, onButtonClicked);
            }
            WindowManager.Instance.OpenWindow(childs_windows[3]);
        }

        public void OpenConfirmBankPurchasePopup(BinaryBank item, ushort offerIndex, BasicOfferBehaviour offerBehaviour,
            Action<ushort, BasicOfferBehaviour> onButtonClicked)
        {
            if (childs_windows[3] is ConfirmPurchasePopUpBehaviour popUpWindowBehaviour)
            {
                popUpWindowBehaviour.SetData(item, offerIndex, offerBehaviour, onButtonClicked);
            }
            WindowManager.Instance.OpenWindow(childs_windows[3]);
        }

        public void OpenBattlePassWindow()
        {
            WindowManager.Instance.OpenWindow(childs_windows[1]);
        }
        
        public bool GetClickedBox(out LootBoxViewBehaviour clickedBox)
        {
            clickedBox = BoxToOpen;
            return clickedBox != null;
        }

        public void RedirectToSection(RedirectMenuSection section)
        {
            redirectSection = section;
            CheckForRedirect();
        }

        protected override void SelfOpen()
        {
            if(profile == null)
            {
                profile = ClientWorld.Instance.GetExistingSystem<HomeSystems>().UserProfile;
            }

            if (Shop.Instance.BattlePass.GetCurrent() == null || profile.battlePass.isPremiumBought)
			{
                CustomPanelClose(typeof(BattlePassPanelBehaviour));
            }
            
            if (profile.actions.GetActualActionsList().Count == 0)
            {
                CustomPanelClose(typeof(BigOffersPanelBehaviour));
            }

            if (profile.dailyDeals.offers.Count == 0)
            {
                CustomPanelClose(typeof(DailyDealsPanelBehaviour));
            }

            redirectSection = null;
            EnableThisGameObject(true);
            InitScrollItems();
            SetupScrollItems();
            ScrollPanel.SelectFirst();
        }

        private void InitScrollItems()
        {
            for (int i = 0; i < ShopPanels.Count; i++)
            {
                ShopPanels[i].Init(profile);
            }
        }

        private void SetupScrollItems()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollPanel.contentTransform);
            
            for (int i = 0; i < ShopButtons.Count; i++)
            {
                for (int j = 0; j < ShopPanels.Count; j++)
                {
                    if (ShopButtons[i].MenuType.Equals(ShopPanels[j].MenuType))
                    {
                        ScrollPanel.AddPanelToScrollList(ShopPanels[j], ShopButtons[i], ShopButtons[i].MenuType, i);
                        ShopButtons[i].Init(ShopPanels[j]);
                        ShopPanels[j].SetOffersOrder();
                        break;
                    }
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollPanel.contentTransform);
            ScrollPanel.SetupPanelsPositions();
        }

        private void CheckForRedirect()
        {
            if (redirectSection != null)
            {
				if (firstOpenFix)
				{
                    firstOpenFix = false;
                    StartCoroutine(CallRedirectWait());
                }
				else
				{
                    CallRedirect();
                }
            }
        }

        private IEnumerator CallRedirectWait()
        {
            yield return new WaitForSeconds(0.1f);

            CallRedirect();
        }

        private bool firstOpenFix = true;
        private void CallRedirect()
        {
            SetupScrollItems();

            switch (redirectSection)
            {
                case RedirectMenuSection.BankGems:
                    RedirectToBankGemsSection();
                    break;
                case RedirectMenuSection.BankCoins:
                    RedirectToBankCoinsSection();
                    break;
                case RedirectMenuSection.BankLoots:
                    RedirectToBankLootsSection();
                    break;
            }
        }

		public ShopPanelBehaviour GetPanelOnType(Type type)
		{
            for (byte i = 0; i < ShopPanels.Count; i++)
			{
                if (ShopPanels[i].GetType() == type)
				{
                    return ShopPanels[i];
                }
			}

            return null;
        }

		public void CustomPanelClose(byte index)
		{
			ShopPanels[index].gameObject.SetActive(false);
			ShopPanels.RemoveAt(index);

			ShopButtons[index].gameObject.SetActive(false);
			ShopButtons.RemoveAt(index);
		}

        public void CustomPanelClose(Type type)
        {
            ShopPanelBehaviour panel = GetPanelOnType(type);
            if (panel != null)
			{
                int index = ShopPanels.IndexOf(panel);
                CustomPanelClose((byte)index);
            }
        }

        private void RedirectToBankGemsSection()
        {
            var bankPanel = GetBankPanelBehaviour();
            if (bankPanel != null)
            {
                ScrollPanel.SelectSection(ShopMenuType.Coins, bankPanel.GetGemsSectionPosition());
                //var targetPos = bankPanel.GetGemsSectionPosition();
                //ScrollPanel.SelectSection(ShopMenuType.Bank, targetPos);
            }
        }

        private BankPanelBehaviour GetBankPanelBehaviour()
        {
            return (BankPanelBehaviour) ShopPanels.Find(x => x as BankPanelBehaviour);
        }

        private void RedirectToBankCoinsSection()
        {
            var bankPanel = GetBankPanelBehaviour();
            if (bankPanel != null)
            {
                ScrollPanel.SelectSection(ShopMenuType.Coins, bankPanel.GetCoinsSectionPosition());
                //var targetPos = bankPanel.GetCoinsSectionPosition();
                //ScrollPanel.SelectSection(ShopMenuType.Bank, targetPos);
            }
        }

        private void RedirectToBankLootsSection()
        {
            var bankPanel = (ShopLootBoxesPanelBehaviour) ShopPanels.Find(x => x as ShopLootBoxesPanelBehaviour);
            if (bankPanel != null)
            {
                var targetPos = bankPanel.GetChestSectionParent();
                //ScrollPanel.SelectSection(ShopMenuType.Bank, targetPos);
            }
        }

        protected override void SelfClose()
        {
            ClearScrollItems();
            EnableThisGameObject(false);
        }

        private void ClearScrollItems()
        {
            for (int i = 0; i < ShopPanels.Count; i++)
            {                
                ShopPanels[i]?.ClearData();
            }
            
            ScrollPanel.ClearData();
        }

        public void SaveClickedData(ushort boughtChest, int purchasePrice)
        {
            BoughtChest     = boughtChest;
            PurchasePrice   = purchasePrice;
        }

        private ushort  BoughtChest;
        private int     PurchasePrice;

        public void SendAnalytic()
        {  
            if(BoughtChest > 0 && PurchasePrice > 0)
            {
                AnalyticsManager.Instance.InAppPurchase(BoughtChest.ToString(), InAppPurchaseType.BuyChest.ToString(), 1, PurchasePrice, CurrencyType.Hard.ToString());
            }
            BoughtChest = 0;
            PurchasePrice = 0;
        } 
    }
}
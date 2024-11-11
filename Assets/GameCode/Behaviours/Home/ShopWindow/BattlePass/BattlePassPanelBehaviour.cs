using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using Unity.Collections;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassPanelBehaviour : ShopPanelBehaviour
    {
        [SerializeField] 
        private BattlePassOfferBehaviour offerPrefab;
        [SerializeField] 
        private BasicSectionBehaviour sectionParent;
        [SerializeField]
        private RectTransform sectionParentNew;
        [SerializeField] 
        private GameObject sectionSubtitle;

        private BattlePassOfferBehaviour createdOffer;
        
        protected override void InitData()
        {
            base.InitData();

            var battlePassData = Shop.Instance.BattlePass.GetCurrent();

            if (battlePassData != null)
                CreateOffer(battlePassData);
            else 
                sectionSubtitle.SetActive(true);
        }

        private void CreateOffer(BinaryBattlePass battlePass)
        {
            //createdOffer = Instantiate(offerPrefab, sectionParent.GetOffersHolder());
            createdOffer = Instantiate(offerPrefab, sectionParentNew);
            createdOffer.SetOfferIndex(battlePass.index);
            var offerInfo = IAPManager.Instance.GetBattlePassMetadata();
            createdOffer.SetTitle(battlePass.title);
            createdOffer.SetBuyButtonText(offerInfo);
            createdOffer.SetTimer(battlePass.timeEnd);
            createdOffer.SetBoughtState(profile.battlePass.isPremiumBought);
            createdOffer.BuyButtonClick += OnOfferButtonClick;
        }

        private void OnOfferButtonClick(ushort offerIndex, BasicOfferBehaviour offerBehaviour)
        {
            if (Shop.Instance.BattlePass.GetCurrent() == null)
            {
                parentShopWindow.CustomPanelClose(typeof(BattlePassPanelBehaviour));

                return;
            }

#if UNITY_EDITOR
            OnBoughtCallback();
#else
            IAPManager.Instance.BuyBattlePass((receipt) => {
                StartCoroutine(DelayedBuy(receipt));
            });
#endif
        }

        //Delay For socket reconnection after buy window on devices
        IEnumerator DelayedBuy(FixedString4096 receipt)
        {
            yield return new WaitForSeconds(0.5f);
            OnBoughtCallback(receipt);
        }

        private void OnBoughtCallback(FixedString4096 receipt = default)
        {
            ClientWorld.Instance.Profile.BuyBattlePass(receipt);
            GameBoughtCallback();
        }

        private void GameBoughtCallback()
        {
            createdOffer.SetBoughtState(true);
            StartCoroutine(OpenBattlePassWindow());
        }

        private IEnumerator OpenBattlePassWindow()
        {
            yield return new WaitForSeconds(1.0f);
            parentShopWindow.OpenBattlePassWindow();

        }

        public override void ClearData()
        {
            base.ClearData();

            if (createdOffer == null) return;
            
            createdOffer.BuyButtonClick -= OnOfferButtonClick;
            Destroy(createdOffer.gameObject);
        }
    }
}
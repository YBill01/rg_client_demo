using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Legacy.Client.LootBoxWindowBehaviour;

namespace Legacy.Client
{
    public class BankPanelBehaviour : ShopPanelBehaviour
    {
        public OfferType offerType;
        public enum OfferType
        {
            Gems,
            Coins
        }

        [SerializeField] 
        private BankOfferBehaviour gemsOfferPrefab1;
        [SerializeField] 
        private BankOfferBehaviour gemsOfferPrefab2;

        [SerializeField] 
        private BankOfferBehaviour coinsOfferPrefab1;
        [SerializeField] 
        private BankOfferBehaviour coinsOfferPrefab2;

        [Header("VFX data")]
        [SerializeField]
        private List<VFXData> VFXDataList;
        [System.Serializable]
        public struct VFXData
        {
            public string Name;
            public GameObject VFXPrefab;
        }
        public VFXData GetVFXData(string name)
        {
            foreach (VFXData masksData in VFXDataList)
            {
                if (masksData.Name != name) continue;
                return masksData;
            }
            return VFXDataList[0];
        }

        [SerializeField]
        private RectTransform coinsSection1;
        [SerializeField]
        private RectTransform coinsSection2;
        [SerializeField]
        private RectTransform coinsSection3;

        [SerializeField]
        private RectTransform gemsSection1;
        [SerializeField]
        private RectTransform gemsSection2;
        [SerializeField]
        private RectTransform gemsSection3;
        
        
        [SerializeField]
        private BasicSectionBehaviour gemsSectionParent;
        //[SerializeField]
        //private BankOfferBehaviour coinsOfferPrefab;
        [SerializeField]
        private BasicSectionBehaviour coinsSectionParent;
        [SerializeField]
        private BankOfferBehaviour shardsOfferPrefab;
        [SerializeField]
        private BasicSectionBehaviour shardsSectionParent;

        private string nameOffer = "";

        private List<BankOfferBehaviour> createdOffers;
        
        protected override void InitData()
        {
            base.InitData();
            
            createdOffers = new List<BankOfferBehaviour>();

            foreach (var binaryBank in Shop.Instance.Bank.Values)
            {
                switch (binaryBank.type)
                {
                    case CurrencyType.Hard:
                    {
                            /*if (offerType != OfferType.Gems) 
                            {
                                break;
                            }*/

                            nameOffer = Locales.Get("locale:832");

                            SetupGemsOffer(binaryBank);
                        break;
                    }
                    case CurrencyType.Soft:
                    {
                            /*if (offerType != OfferType.Coins)
                            {
                                break;
                            }*/

                            nameOffer = Locales.Get("locale:835");

                            SetupCoinsOffer(binaryBank);
                        break;
                    }
                    //case CurrencyType.Shards:
                    //{
                        //SetupShardsOffer(binaryBank);
                        //break;
                    //}
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelContent);
        }

        private void SetupGemsOffer(BinaryBank offer)
        {
            if (offer.order <= 2)
            {
                CreateOffer(offer, gemsOfferPrefab1, gemsSectionParent, gemsSection1);
            }
            else if (offer.order <= 4)
            {
                CreateOffer(offer, gemsOfferPrefab1, gemsSectionParent, gemsSection2);
            }
            else
            {
                CreateOffer(offer, gemsOfferPrefab2, gemsSectionParent, gemsSection3);
            }
        }

        private void CreateOffer(BinaryBank offer, BankOfferBehaviour prefab, BasicSectionBehaviour parent, RectTransform parentNew = null)
        {
            BankOfferBehaviour offerItem;
            if (parentNew != null)
            {
                offerItem = Instantiate(prefab, parentNew);
            }
            else
            {
                offerItem = Instantiate(prefab, parent.GetOffersHolder());
            }

            //offerItem.SetAmount(offer.count.ToString());
            /*string nameOffer = "";
            if (offerType == OfferType.Coins)
            {
                nameOffer = Locales.Get("locale:835");
            }
            else if (offerType == OfferType.Gems) 
            {
                nameOffer = Locales.Get("locale:832");
            }*/

            offerItem.SetTitle(("<size=120%>" + offer.count.ToString() + "</size >") + " " + nameOffer);
            //offerItem.SetTitle(offer.title);
            offerItem.SetMainImage(offer.preview);
            offerItem.SetMainImageVFX(GetVFXData(offer.preview));
            offerItem.SetOfferIndex(offer.index);
            var price = offer.price == 0 ? offer.hardPrice : offer.price;
            if(offer.price > 0)
            {
                var offerInfo = IAPManager.Instance.GetItemMetadata(offer.storeKeys.android);
                offerItem.SetBuyButtonText(offerInfo);
            }
            else
            {
                offerItem.SetBuyButtonText(offer.hardPrice.ToString());
            }
            offerItem.SaveSiblingIndex(offer.order - 1);
            offerItem.BuyButtonClick += OnOfferBuyButtonClick;
            
            createdOffers.Add(offerItem);
        }

        private void OnOfferBuyButtonClick(ushort offerIndex, BasicOfferBehaviour offerBehaviour)
        {
            if (Shop.Instance.Bank.Get(offerIndex, out BinaryBank offer))
            {
                switch (offer.type)
                {
                    case CurrencyType.Hard:
                        BuyItem(offerIndex, offerBehaviour);
                        break;
                    //case CurrencyType.Shards:
                    case CurrencyType.Soft:                  
                        parentShopWindow.OpenConfirmBankPurchasePopup(offer, offerIndex, offerBehaviour, BuyItem);
                        break;
                }                
            }
        }

        private void BuyItem(ushort offerIndex, BasicOfferBehaviour offerBehaviour)
        {
            if (Shop.Instance.Bank.Get(offerIndex, out BinaryBank offer))
            {
                if (offer.price > 0)
                {
#if UNITY_EDITOR
                    BuyOffer(offer, offerBehaviour);
#else
                    IAPManager.Instance.BuyCustomKey(offer.storeKeys.GetStoreKey(), (receipt) => {
                        StartCoroutine(DelayedBuyOffer(offer, offerBehaviour, receipt));
                    });
#endif
                }
                else
                {
                    BuyOffer(offer, offerBehaviour);
                }
            }
        }

        //Delay For socket reconnection after buy window on devices
        IEnumerator DelayedBuyOffer(BinaryBank offer, BasicOfferBehaviour offerBehaviour, FixedString4096 receipt)
        {
            yield return new WaitForSeconds(0.5f);
            BuyOffer(offer, offerBehaviour, receipt);
        }

        private BinaryBank pendingOffer;
        private BasicOfferBehaviour pendingOfferBehaviour;
        private void BuyOffer(BinaryBank offer, BasicOfferBehaviour offerBehaviour, FixedString4096 receipt = default)
        {
            pendingOffer = offer;
            pendingOfferBehaviour = offerBehaviour;
            
            if (receipt != default)
            {
                Debug.Log($"Receipt from type hard: {receipt}");
                profile.BuyFromBank(offer.index, receipt);
                CollectOferrsParticles();
            }
            else if (offer.hardPrice <= profile.Stock.getItem(CurrencyType.Hard).Count)
            {
                Debug.Log($"Receipt from type nonHard: {receipt}");

                AnalyticsManager.Instance.InAppPurchase("soft_" + offer.count, InAppPurchaseType.BuySoft.ToString(), (int)offer.count, offer.hardPrice, CurrencyType.Hard.ToString());

                profile.BuyFromBank(offer.index);
                CollectOferrsParticles();
            }
            else
            {
                parentShopWindow.RedirectToSection(RedirectMenuSection.BankGems);
            }
        }

        private void CollectOferrsParticles()
        {
            Debug.Log($"pendingOffer: {pendingOffer}");
            Debug.Log($"pendingOfferBehaviour: {pendingOfferBehaviour}");
            Debug.Log($"pendingOffer.type: {pendingOffer.type}");
            if (pendingOffer != null && pendingOfferBehaviour != null)
            {
                switch (pendingOffer.type)
                {
                    case CurrencyType.Rating:
                        RewardParticlesBehaviour.Instance.Drop(pendingOfferBehaviour.transform.position, 10, LootCardType.Rating);
                        break;
                    case CurrencyType.Soft:
                        RewardParticlesBehaviour.Instance.Drop(pendingOfferBehaviour.transform.position, 10, LootCardType.Soft);
                        break;
                    case CurrencyType.Hard:
                        RewardParticlesBehaviour.Instance.Drop(pendingOfferBehaviour.transform.position, 10, LootCardType.Hard);
                        break;
                    //case CurrencyType.Shards:
                        //RewardParticlesBehaviour.Instance.Drop(pendingOfferBehaviour.transform.position, 10, LootCardType.Shards);
                        //break;
                    case CurrencyType.HeroExp:
                        RewardParticlesBehaviour.Instance.Drop(pendingOfferBehaviour.transform.position, 10, LootCardType.Exp);
                        break;
                    default:
                        break;
                }
            }
            pendingOffer = null;
            pendingOfferBehaviour = null;
        }
        private void SetupCoinsOffer(BinaryBank offer)
        {
            if (offer.order <= 1)
            {
                CreateOffer(offer, coinsOfferPrefab1, gemsSectionParent, coinsSection1);
            }
            else if (offer.order <= 2)
            {
                CreateOffer(offer, coinsOfferPrefab1, gemsSectionParent, coinsSection2);
            }
            else
            {
                CreateOffer(offer, coinsOfferPrefab2, gemsSectionParent, coinsSection3);
            }


            //CreateOffer(offer, coinsOfferPrefab, coinsSectionParent);
        }

        private void SetupShardsOffer(BinaryBank offer)
        {
            CreateOffer(offer, shardsOfferPrefab, shardsSectionParent);
        }

        public override void SetOffersOrder()
        {
            base.SetOffersOrder();

            for (int i = 0; i < createdOffers.Count; i++)
            {
                createdOffers[i].SetSiblingIndex();
            }
        }

        public float GetGemsSectionPosition()
        {
            return int.MinValue; // TODO to new window...
            //return gameObject.GetComponentInParent<ShopWindowBehaviour>().ScrollPanel.minContentPosition;
            //return parentShopWindow.ScrollPanel.minContentPosition;
            //ShopScrollItem item = parentShopWindow.ScrollPanel.panelsList.Find(x => x.menuType.Equals(ShopMenuType.Coins));
            /*if (item != null)
            {
                return parentShopWindow.ScrollPanel.minContentPosition;
                //return item.position - (coinsSection3.position.x + coinsSection3.rect.width + 160);
            }*/

            //return 0.0f;
            //return gemsSectionParent.GetSectionPosition();
        }

        public float GetCoinsSectionPosition()
        {
            return 0.0f;
            //return coinsSectionParent.GetSectionPosition();
        }

        public float GetShardsSectionPosition()
        {
            return shardsSectionParent.GetSectionPosition();
        }

        public override void ClearData()
        {
            base.ClearData();
            
            if (createdOffers == null) return;

            for (int i = 0; i < createdOffers.Count; i++)
            {
                createdOffers[i].BuyButtonClick -= OnOfferBuyButtonClick;
                Destroy(createdOffers[i].gameObject);
            }
            
            createdOffers.Clear();
        }
    }
}

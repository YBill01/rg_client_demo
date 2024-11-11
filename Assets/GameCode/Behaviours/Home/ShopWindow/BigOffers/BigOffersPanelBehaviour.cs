using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static Legacy.Client.LootBoxWindowBehaviour;

namespace Legacy.Client
{
    public class BigOffersPanelBehaviour : ShopPanelBehaviour
    {
        [SerializeField]
        private List<BigOffersOfferBehaviour> bigOffersPrefabs;
        [SerializeField]
        private List<RectTransform> bigOffersSectionsPrefabs;

        private List<BigOffersOfferBehaviour> createdOffers;
        protected override void InitData()
        {
            base.InitData();

            var profile = ClientWorld.Instance.Profile;

            createdOffers = new List<BigOffersOfferBehaviour>();

            var actions = profile.actions.GetActualActionsList();
            //var actions = Shop.Instance.Actions.GetActualActionsList(); // old...

            byte index = 0;

			foreach (var item in actions)
			{
                Rewards.Instance.Get(item.Value.treasure, out BinaryReward reward);

                switch (item.Value.type)
                {
                    case 0:
                        {
                            CreateOffer(item.Value, bigOffersPrefabs[0], bigOffersSectionsPrefabs[index], reward);
                            index++;

                            break;
                        }
                    case 1:
                        {
                            CreateOffer(item.Value, bigOffersPrefabs[1], bigOffersSectionsPrefabs[2], reward);

                            break;
                        }
                    case 2:
                        {
                            CreateOffer(item.Value, bigOffersPrefabs[2], bigOffersSectionsPrefabs[2], reward);

                            break;
                        }
                }

                /*byte countItems = 0;

                if (Loots.Instance.Get(reward.lootbox, out BinaryLoot loot))
				{
                    countItems++;
                }

                countItems += (byte) (reward.hard > 0 ? 1 : 0);
                countItems += (byte) (reward.soft > 0 ? 1 : 0);
                //countItems += (byte) (reward.shard > 0 ? 1 : 0);

				switch (countItems)
				{
                    case 1:
                        {
                            CreateOffer(item.Value, bigOffersPrefabs[0], bigOffersSectionsPrefabs[index], reward);
                            index++;

                            break;
						}
                    case 2:
						{
                            CreateOffer(item.Value, bigOffersPrefabs[1], bigOffersSectionsPrefabs[2], reward);

                            break;
						}
                    case 3:
						{
                            CreateOffer(item.Value, bigOffersPrefabs[2], bigOffersSectionsPrefabs[2], reward);

                            break;
						}
                    case 4:
						{
                            CreateOffer(item.Value, bigOffersPrefabs[3], bigOffersSectionsPrefabs[2], reward);

                            break;
						}
				}*/

				//Loots.Instance.Get(reward.lootbox, out BinaryLoot loot);

				//var lootName = loot.GetTitle();


                //CreateOffer(item.Value, chestsOfferPrefabs[i], chestsSectionParent, sectionParent);

            }

            //Shop.Instance.Actions.Count
            /*var count = Shop.Instance.Actions.Count;
            for (ushort i = 0; i < count; i++)
            {
                Shop.Instance.Actions.Get(i, out BinaryAction action);
                {


                    var c = action.index;

                }
            }*/

            /*Loots.Instance.Get(action.treasure, out BinaryLoot loot_box){
                
            }*/

            clickLocked = false;

            LayoutRebuilder.ForceRebuildLayoutImmediate(panelContent);
        }

        private void ReDrawPanel()
		{
            ClearData();
            InitData();
        }

        private void CreateOffer(BinaryAction action, BigOffersOfferBehaviour actionPrefab, RectTransform parent, BinaryReward reward)
        {
            BigOffersOfferBehaviour offerItem = Instantiate(actionPrefab, parent);
            offerItem.SetTitle(action.title);
            offerItem.SetArena(1.ToString());// в акциях нет арен...
            
            offerItem.CreateItems(reward);

            offerItem.SetOfferIndex(action.index);

            //var price = action.price == 0 ? action.hardPrice : action.price;
            if (action.price.soft > 0)
            {
                offerItem.SetBuyButtonText(action.price.soft.ToString());
            }
            else if(action.price.hard > 0)
            {
                offerItem.SetBuyButtonText(action.price.hard.ToString());
			}
			else
			{
                var productData = IAPManager.Instance.GetItemMetadata(action.store_keys.android);
                offerItem.SetBuyButtonText(LegacyHelpers.FormatByDigits(productData.localizedPriceString));
            }

            offerItem.BuyButtonClick += OnOfferBuyButtonClick;

            createdOffers.Add(offerItem);
        }

        private void OnOfferBuyButtonClick(ushort offerIndex, BasicOfferBehaviour offerBehaviour)
        {
            if (Shop.Instance.Actions.Get(offerIndex, out BinaryAction action))
            {
                if (action.price.soft > 0)
                {

                }
                else if (action.price.hard > 0)
                {

                }
                else
                {
                    BuyItem(offerIndex, offerBehaviour);
                }
            }
        }

        // блокировка повторного вызова пуркаше уведомления
        private bool clickLocked = false;
		private void BuyItem(ushort offerIndex, BasicOfferBehaviour offerBehaviour)
		{
			if (clickLocked)
			{
                return;
			}

			if (Shop.Instance.Actions.Get(offerIndex, out BinaryAction offer))
			{
#if UNITY_EDITOR
                BuyOffer(offer, offerBehaviour);
#else
                IAPManager.Instance.BuyCustomKey(offer.store_keys.GetStoreKey(), (receipt) => {
                    StartCoroutine(DelayedBuyOffer(offer, offerBehaviour, receipt));
                });
#endif
			}
		}

        //Delay For socket reconnection after buy window on devices
        IEnumerator DelayedBuyOffer(BinaryAction offer, BasicOfferBehaviour offerBehaviour, FixedString4096 receipt)
        {
            yield return new WaitForSeconds(0.5f);
            BuyOffer(offer, offerBehaviour, receipt);
        }

        private BinaryAction pendingOffer;
        private BigOffersOfferBehaviour pendingOfferBehaviour;
        private BinaryReward binaryReward;

		//public object LootDataBehaviour { get; set; }
		//public bool IsOpenedLootBox { get; set; }

		private void BuyOffer(BinaryAction offer, BasicOfferBehaviour offerBehaviour, FixedString4096 receipt = default)
        {
            clickLocked = true;

            pendingOffer = offer;
            pendingOfferBehaviour = (BigOffersOfferBehaviour)offerBehaviour;

            if (receipt != default)
            {
                Debug.Log($"Action from store key: {receipt}");
                profile.BuyAction(offer.index, receipt);
                CollectOferrsParticles();
			}
			else
			{
                profile.BuyAction(offer.index);
                CollectOferrsParticles();
            }
        }

        private void CollectOferrsParticles()
        {
            Debug.Log($"pendingAction: {pendingOffer}");
            Debug.Log($"pendingOfferBehaviour: {pendingOfferBehaviour}");

            if (Rewards.Instance.Get(pendingOffer.treasure, out BinaryReward binaryReward))
            {
                this.binaryReward = binaryReward;

                if (binaryReward.soft > 0)
                {
                    RewardParticlesBehaviour.Instance.Drop(pendingOfferBehaviour.itemCoin.GetCoinView().position, 10, LootCardType.Soft);
                }

                if (binaryReward.hard > 0)
                {
                    RewardParticlesBehaviour.Instance.Drop(pendingOfferBehaviour.itemHard.GetHardView().position, 10, LootCardType.Hard);
                }

				if (binaryReward.lootbox > 0)
				{
					if (binaryReward.soft > 0 || binaryReward.hard > 0)
					{
						StartCoroutine("CollectLootBoxWithWait");
					}
					else
					{
                        CollectLootBox();
					}
				}

                //var pendingOfferT = null;

                //pendingOffer = null;
                //pendingOfferBehaviour = null;
            }
        }

        private void CollectLootBox()
		{
            profile.BuyActionLootBox(pendingOffer.index);

            LootBoxViewBehaviour boxView = pendingOfferBehaviour.itemLootBox.GetLootBoxView();
            boxView.Init(LootBoxBehaviour.BoxState.Opening, boxView.BinaryData);
            parentShopWindow.OpenLootBoxWindow(boxView);
        }

        IEnumerator CollectLootBoxWithWait()
        {
            yield return new WaitForSecondsRealtime(2.25f);

            if (binaryReward.lootbox > 0)
			{
                CollectLootBox();
            }
            else
            {
                ReDrawPanel();
                if (WindowManager.Instance.CurrentWindow is ShopWindowBehaviour)
                {
                    var window = WindowManager.Instance.CurrentWindow as ShopWindowBehaviour;
                    if (profile.actions.GetActualActionsList().Count == 0)
                    {
                        window.CustomPanelClose(typeof(BigOffersPanelBehaviour));
                    }
                }
            }
        }

        public override void ClearData()
        {
            base.ClearData();

            if (createdOffers == null) return;

            for (int i = 0; i < createdOffers.Count; i++)
            {
                createdOffers[i].ClearData();
                createdOffers[i].BuyButtonClick -= OnOfferBuyButtonClick;
                Destroy(createdOffers[i].gameObject);
            }

            createdOffers.Clear();
        }

    }
}
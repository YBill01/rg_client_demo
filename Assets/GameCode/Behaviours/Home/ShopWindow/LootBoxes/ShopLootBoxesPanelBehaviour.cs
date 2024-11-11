using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Legacy.Client;
using Legacy.Database;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ShopLootBoxesPanelBehaviour : ShopPanelBehaviour
    {
        [SerializeField] 
        private List<LootBoxOfferBehaviour> chestsOfferPrefabs;
        [SerializeField] 
        private BasicSectionBehaviour chestsSectionParent;
        [SerializeField]
        private RectTransform sectionParent;

        private List<LootBoxOfferBehaviour> createdOffers;
        
        protected override void InitData()
        {
            base.InitData();
            
            createdOffers = new List<LootBoxOfferBehaviour>();

            int i = 0;
            var profile = ClientWorld.Instance.Profile;
            var shopBoxes = Shop.Instance.LootBox.Values
                .Where(x => x.arena == profile.CurrentArena.number + 1 )
                .OrderBy(x => x.hardPrice);

            foreach (var binaryLoot in shopBoxes)
            {
                if (i < chestsOfferPrefabs.Count)
                {
                    //CreateOffer(binaryLoot, chestsOfferPrefabs[i], chestsSectionParent);
                    CreateOffer(binaryLoot, chestsOfferPrefabs[i], chestsSectionParent, sectionParent);
                    i++;
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelContent);
        }

        private void CreateOffer(BinaryMarketLoot offer, LootBoxOfferBehaviour prefab, BasicSectionBehaviour parent, RectTransform parentNew = null)
        {
            LootBoxOfferBehaviour offerItem = Instantiate(prefab, parentNew/*parent.GetOffersHolder()*/);
            offerItem.SetTitle(offer.title);
            offerItem.SetOfferIndex(offer.index);
            var price = offer.hardPrice;
            offerItem.SetBuyButtonText(price.ToString());
            offerItem.SetArena(offer.arena.ToString());
            Loots.Instance.Get(offer.lootbox, out BinaryLoot loot);
            offerItem.SetChest(loot);
            offerItem.BuyButtonClick += OnOfferBuyButtonClick;
            
            createdOffers.Add(offerItem);
        }

        private void OnOfferBuyButtonClick(ushort offerIndex, BasicOfferBehaviour offerBehaviour)
        {
            if (Shop.Instance.LootBox.Get(offerIndex, out BinaryMarketLoot offer))
            {
                var boxToOpen = createdOffers.Find(x => x.GetOfferIndex().Equals(offerIndex)).GetBox();
                parentShopWindow.OpenLootBoxPopUpWindow(boxToOpen, offer);
            }
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

        public float GetChestSectionParent()
        {
            //return sectionParent.GetSectionPosition();
            return chestsSectionParent.GetSectionPosition();
        }
    }
}
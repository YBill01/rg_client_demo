using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Legacy.Client
{
    public class BigOffersOfferBehaviour : BasicOfferBehaviour
    {
        public const string IMAGE_SOFT = "Soft_1";
        public const string IMAGE_HARD = "Hard_2";

        [SerializeField]
        private TMP_Text arena;
        [SerializeField]
        private List<RectTransform> rewardParents;
        [SerializeField]
        private BigOffersItemBechaviour itemPrefab;

        private List<BigOffersItemBechaviour> createdItems;

        public BigOffersItemBechaviour itemCoin;
        public BigOffersItemBechaviour itemHard;
        public BigOffersItemBechaviour itemLootBox;

        public void SetType(string text)
        {
            arena.text = Locales.Get("locale:712") + " " + text;
        }

        public void SetArena(string text)
        {
            arena.text = Locales.Get("locale:712") + " " + text;
        }

        public void CreateItems(BinaryReward reward)
        {
            createdItems = new List<BigOffersItemBechaviour>();

            BigOffersItemBechaviour item;

            byte index = 0;

            if (Loots.Instance.Get(reward.lootbox, out BinaryLoot loot))
            {
                item = Instantiate(itemPrefab, rewardParents[index]);
                item.SetAmount(1.ToString());// всего в акции может быть один сундук...
                item.SetChest(loot, ((reward.soft > 0 && reward.hard > 0) ? 0.7f : 0.5f));

                itemLootBox = item;
                createdItems.Add(item);

                index++;
            }

            if (reward.soft > 0)
			{
                item = Instantiate(itemPrefab, rewardParents[index]);
                item.SetAmount(reward.soft.ToString());
                item.SetImage(IMAGE_SOFT);

                itemCoin = item;
                createdItems.Add(item);

                index++;
            }

            if (reward.hard > 0)
            {
                item = Instantiate(itemPrefab, rewardParents[index]);
                item.SetAmount(reward.hard.ToString());
                item.SetImage(IMAGE_HARD);

                itemHard = item;
                createdItems.Add(item);

                //index++;
            }



        }

        public void ClearData()
        {
            if (createdItems == null) return;

            for (int i = 0; i < createdItems.Count; i++)
            {
                Destroy(createdItems[i].gameObject);
            }

            createdItems.Clear();
        }
    }
}
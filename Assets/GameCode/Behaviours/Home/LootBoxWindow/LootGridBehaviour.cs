using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Legacy.Client.LootBoxWindowBehaviour;

namespace Legacy.Client {
    public class LootGridBehaviour : MonoBehaviour
    {
        [SerializeField] GameObject LootCardPrefab;
        [SerializeField] LootBoxWindowBehaviour LootWindow;
        [SerializeField] Transform LootCardParent;

        [SerializeField] RectTransform UpRowRect;
        [SerializeField] RectTransform DownRowRect;

        private byte LootCount = 0;

        public void ClearGrid()
        {
            LootCount = 0;
            for (int i = 0; i < UpRowRect.childCount; i++)
            {
                Destroy(UpRowRect.GetChild(i).gameObject);
            }
            for (int i = 0; i < DownRowRect.childCount; i++)
            {
                Destroy(DownRowRect.GetChild(i).gameObject);
            }
        }

        public LootCardBehaviour AddLoot(uint count, ushort index, LootCardType type)
        {
            RectTransform parentRow = DownRowRect;
            if(LootCount % 2 > 0)
            {
                parentRow = UpRowRect;
            }
            LootCardPrefab.SetActive(false);
            var lootCard = Instantiate(LootCardPrefab, LootCardParent).GetComponent<LootCardBehaviour>();
            lootCard.Init(index, count, type, LootCount);
            LootCount++;
            return lootCard;
        }

        internal void BackToGrid(LootCardBehaviour lootCard)
        {
            RectTransform LootRect = lootCard.GetComponent<RectTransform>();
            RectTransform parentRow = DownRowRect;
            if (lootCard.indexInGrid % 2 > 0)
            {
                parentRow = UpRowRect;
            }
            LootRect.SetParent(parentRow);
        }
    }
}

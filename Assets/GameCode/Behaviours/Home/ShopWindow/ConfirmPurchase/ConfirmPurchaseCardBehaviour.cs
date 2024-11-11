using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class ConfirmPurchaseCardBehaviour : ConfirmPurchaseBasicCardBehaviour
    {
        [SerializeField] private LootCardBehaviour lootCard;

        protected override void UpdateData()
        {
            var id = data.treasure.tid;
            var count = data.treasure.count;
            lootCard.Init(id, count, LootBoxWindowBehaviour.LootCardType.Cards, 1);
        }

    }
}

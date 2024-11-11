using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Legacy.Client
{
    public class DailyDealsLootBoxCardBehaviour : DailyDealsBasicCardBehaviour
    {
        [SerializeField] 
        private Transform boxParent;
        [SerializeField]
        private Color32 raysColor;
        [SerializeField]
        private Color32 lightColor;

        private LootBoxViewBehaviour boxView;
        
        public override void InitData(PlayerDailyDealsItem item, DailyDealsOfferBehaviour offer)
        {
            base.InitData(item, offer);
            
            Loots.Instance.Get(item.treasure.tid, out BinaryLoot loot);
            var boxState = item.buyed ? LootBoxBehaviour.BoxState.SlotsFull : LootBoxBehaviour.BoxState.Opening;
            SetChest(loot, boxState);
            
            offerParent.SetTitle(loot.title);
            offerParent.SetRaysImageColor(raysColor);
            offerParent.SetLightImageColor(lightColor);
        }
        
        private void SetChest(BinaryLoot binary, LootBoxBehaviour.BoxState boxState)
        {
            var loaded = Addressables.InstantiateAsync($"Loots/{binary.prefab}LootBox.prefab", boxParent);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
            {
                boxView = async.Result.GetComponent<LootBoxViewBehaviour>();
                boxView.Init(boxState, binary);
                boxView.SetScaleMultiplier(.6f);
            };
        }

        public LootBoxViewBehaviour GetBox()
        {
            return boxView;
        }
    }
}
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ArenaRewardLootBehaviour : ArenaBasicRewardBehaviour
    {
        [SerializeField]
        private Transform BoxHolder;

        private LootBoxViewBehaviour boxView;
        private ArenaRewardBehaviour.RewardState state = ArenaRewardBehaviour.RewardState.Locked;
        private BinaryLoot loot;
        
        internal void Init(ushort lootbox, Action onLoadCompleet = null)
        {
            if (Loots.Instance.Get(lootbox, out loot))
            {
                SetChest(loot.prefab, onLoadCompleet);
            }
        }
        
        private void SetChest(string prefab,Action onLoadCompleet=null)
        {
            var loaded = Addressables.InstantiateAsync($"Loots/{prefab}LootBox.prefab", BoxHolder);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
            {
                boxView = async.Result.GetComponent<LootBoxViewBehaviour>();
                boxView.Init(LootBoxBehaviour.BoxState.Opening, loot);
                boxView.SetScaleMultiplier(.7f);
                SetBoxState();
                if (onLoadCompleet != null)
                    onLoadCompleet();
            };
        }

        private void SetBoxState()
        {
            switch (state)
            {
                case ArenaRewardBehaviour.RewardState.Active:
                    SetActiveBoxState();
                    break;
                case ArenaRewardBehaviour.RewardState.Completed:
                    SetCompleteBoxState();
                    break;
                case ArenaRewardBehaviour.RewardState.Locked:
                    SetLockedBoxState();
                    break;
            }
        }

        public LootBoxViewBehaviour GetBox()
        {
            return boxView;
        }

        public override void SetActiveState()
        {
            base.SetActiveState();
            state = ArenaRewardBehaviour.RewardState.Active;
            
            if (boxView != null)
                SetActiveBoxState();
        }

        private void SetActiveBoxState()
        {
            boxView.Init(LootBoxBehaviour.BoxState.Opening, loot);
        }

        public override void SetCompleteState()
        {
            base.SetCompleteState();
            state = ArenaRewardBehaviour.RewardState.Completed;

            if (boxView != null)
                SetCompleteBoxState();
        }

        private void SetCompleteBoxState()
        {
            boxView.Init(LootBoxBehaviour.BoxState.SlotsFull, loot);
        }

        public override void SetLockedState()
        {
            base.SetLockedState();
            state = ArenaRewardBehaviour.RewardState.Locked;

            if (boxView != null)
                SetLockedBoxState();
        }

        private void SetLockedBoxState()
        {
            boxView.Init(LootBoxBehaviour.BoxState.Closed, loot);
        }

        public void ResetAfterOpening()
        {
            state = ArenaRewardBehaviour.RewardState.Completed;
            SetChest(loot.prefab);
        }
    }
}

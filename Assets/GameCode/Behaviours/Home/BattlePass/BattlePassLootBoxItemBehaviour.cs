using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Legacy.Client
{
    public class BattlePassLootBoxItemBehaviour : BattlePassRewardItemBasicBehaviour
    {
        [SerializeField] 
        public Transform lootBoxHolder;

        private LootBoxViewBehaviour boxView;

        private float regularScale = .5f;
        private float currentScale = .6f;
        private float initScale = .5f;
        public LootBoxBehaviour.BoxState initState = LootBoxBehaviour.BoxState.SlotsFull;
        private BinaryLoot loot;
        private bool isCollected;

        public void Init(ushort index)
        {
            if (Loots.Instance.Get(index, out loot))
            {
                SetChest(loot.prefab);
            }
        }

        private void SetChest(string prefab)
        {
            var loaded = Addressables.InstantiateAsync($"Loots/{prefab}LootBox.prefab", lootBoxHolder);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
            {
                boxView = async.Result.GetComponent<LootBoxViewBehaviour>();
                boxView.Init(initState, loot);
                boxView.SetScaleMultiplier(initScale);
                var posBehaviour = async.Result.GetComponent<RectPositionToBehaviour>();
                posBehaviour.SetTargetPosition(new Vector2(-15, 5));
            };
        }

        public override void ScaleToCurrentState()
        {
            if (boxView == null)
            {
                initScale = currentScale;
            }
            else
            {
                boxView.SetScaleMultiplier(currentScale);
            }
        }

        public override void ScaleToRegularState()
        {

            if (boxView != null)
            {
                if (WindowManager.Instance.CurrentWindow is LootBoxWindowBehaviour)
                    boxView.SetScaleMultiplier(1.0f);
                else
                    boxView.SetScaleMultiplier(regularScale);

            }
        }

        public void SetCollectedState()
        {
            isCollected = true;
            
            if (boxView == null)
                initState = LootBoxBehaviour.BoxState.SlotsFull;
            else
            {
                boxView.ChangeState(LootBoxBehaviour.BoxState.SlotsFull);
            }
        }

        public void SetAvailableState()
        {
            if (boxView == null)
                initState = LootBoxBehaviour.BoxState.Opening;
            else
                boxView.ChangeState(LootBoxBehaviour.BoxState.Opening);
        }

        public void SetClosedState()
        {
            if (boxView == null)
                initState = LootBoxBehaviour.BoxState.SlotsFull;
            else
                boxView.ChangeState(LootBoxBehaviour.BoxState.SlotsFull);
        }

        public LootBoxViewBehaviour GetLootBox()
        {
            return boxView;
        }

        public void ResetAfterOpening()
        {
            initState = LootBoxBehaviour.BoxState.SlotsFull;
            SetChest(loot.prefab);
        }
    }
}
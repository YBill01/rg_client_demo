using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Legacy.Client
{
    public class LootBoxOfferBehaviour : BasicOfferBehaviour
    {
        [SerializeField] 
        private TMP_Text arena;
        [SerializeField] 
        protected Transform boxParent;

        private LootBoxViewBehaviour boxView;

        public void SetArena(string text)
        {
            arena.text = Locales.Get("locale:712") + " " + text;
        }

        public void SetChest(BinaryLoot loot)
        {
            var loaded = Addressables.InstantiateAsync($"Loots/{loot.prefab}LootBox.prefab", boxParent);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
            {
                boxView = async.Result.GetComponent<LootBoxViewBehaviour>();
                boxView.Init(LootBoxBehaviour.BoxState.Opening, loot);
                boxView.SetScaleMultiplier(.7f);
            };
        }

        public LootBoxViewBehaviour GetBox()
        {
            return boxView;
        }

        public void Clear()
        {
            Destroy(boxView.gameObject);
        }

    }
}
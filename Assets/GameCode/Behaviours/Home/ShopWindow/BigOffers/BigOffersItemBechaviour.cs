using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Legacy.Client
{
    public class BigOffersItemBechaviour : MonoBehaviour
    {
        [SerializeField]
        protected TMP_Text amount;
        [SerializeField]
        protected Image mainImage;
        [SerializeField]
        protected RectTransform content;

        protected LootBoxViewBehaviour boxView;

        public void SetAmount(string text)
        {
            amount.text = "X" + text;
        }

        public void SetImage(string imageName)
        {
            mainImage.gameObject.SetActive(true);
            mainImage.sprite = VisualContent.Instance.BankCurrenciesAtlas.GetSprite(imageName);
        }

        public void SetChest(BinaryLoot binaryloot, float scale)
        {
            mainImage.gameObject.SetActive(false);

			var loaded = Addressables.InstantiateAsync($"Loots/{binaryloot.prefab}LootBox.prefab", content);
			loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
			{
				boxView = async.Result.GetComponent<LootBoxViewBehaviour>();
				boxView.Init(LootBoxBehaviour.BoxState.Opening, binaryloot);
				boxView.SetScaleMultiplier(scale);
			};
		}

        public void SetCard()
        {

        }

        public Transform GetCoinView()
        {
            return this.transform;
        }
        public Transform GetHardView()
        {
            return this.transform;
        }
        public LootBoxViewBehaviour GetLootBoxView()
		{
            return boxView;
        }

        public void Clear()
        {
            if (boxView != null)
			{
                Destroy(boxView.gameObject);
            }
        }
    }
}
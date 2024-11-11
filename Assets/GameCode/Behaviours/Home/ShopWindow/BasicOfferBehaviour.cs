using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Purchasing;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class BasicOfferBehaviour : MonoBehaviour
    {
        public event Action<ushort, BasicOfferBehaviour> BuyButtonClick; 
        
        [SerializeField] 
        private TMP_Text title;
        [SerializeField] protected LegacyButton buyButton;
        [SerializeField] 
        private TMP_Text buyButtonText;

        private ushort index;
        private int orderIndex;

        public void SetOfferIndex(ushort index)
        {
            this.index = index;
        }

        public void SetTitle(string text)
        {
            title.text = Locales.Get(text);
        }

        public void SetBuyButtonText(string text)
        {
            buyButtonText.text = LegacyHelpers.FormatByDigits(text);
        }

        public void SetBuyButtonText(ProductMetadata productData)
        {
            buyButtonText.text = LegacyHelpers.FormatByDigits(productData.localizedPriceString);
        }

        public void SaveSiblingIndex(int index)
        {
            orderIndex = index;
        }

        public void SetSiblingIndex()
        {
            gameObject.transform.SetSiblingIndex(orderIndex);
        }

        public ushort GetOfferIndex()
        {
            return index;
        }

        private void OnEnable()
        {
            buyButton.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            BuyButtonClick?.Invoke(index,this);
        }

        private void OnDisable()
        {
            buyButton.onClick.RemoveListener(OnButtonClick);
        }
    }
}
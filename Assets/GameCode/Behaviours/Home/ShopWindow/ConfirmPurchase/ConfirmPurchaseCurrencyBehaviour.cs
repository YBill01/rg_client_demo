using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class ConfirmPurchaseCurrencyBehaviour : ConfirmPurchaseBasicCardBehaviour
    {
        [SerializeField] private BankOfferBehaviour bankOffer;

        protected override void UpdateData()
        {
            bankOffer.SetAmount(bankData.count.ToString());
            bankOffer.SetTitle(bankData.title);
            bankOffer.SetMainImage(bankData.preview);
            bankOffer.SetOfferIndex(bankData.index);
            var price = bankData.price == 0 ? bankData.hardPrice : bankData.price;
            if (bankData.price > 0)
            {
                var offerInfo = IAPManager.Instance.GetItemMetadata(bankData.storeKeys.android);
                bankOffer.SetBuyButtonText(offerInfo);
            }
            else
            {
                bankOffer.SetBuyButtonText(bankData.hardPrice.ToString());
            }
            bankOffer.SaveSiblingIndex(bankData.order - 1);
        }
    }
}

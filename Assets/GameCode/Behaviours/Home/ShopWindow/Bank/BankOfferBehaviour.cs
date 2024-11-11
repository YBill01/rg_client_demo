using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Legacy.Client.BankPanelBehaviour;

public class BankOfferBehaviour : BasicOfferBehaviour
{
    [SerializeField] 
    private TMP_Text amount;
    [SerializeField] 
    protected Image mainImage;

    public void SetAmount(string text)
    {
        amount.text = "<size=50%>x</size> " + LegacyHelpers.FormatByDigits(text);
    }

    public void SetMainImage(string imageName)
    {
        mainImage.sprite = VisualContent.Instance.BankCurrenciesAtlas.GetSprite(imageName);          
    }
    public void SetMainImageVFX(VFXData vfxData)
    {
        Instantiate(vfxData.VFXPrefab, mainImage.gameObject.transform);
    }
}

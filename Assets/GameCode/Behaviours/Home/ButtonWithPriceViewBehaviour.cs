using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWithPriceViewBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI PriceText;

    private uint value;
    private string realPrice;

    [SerializeField] LegacyButton LegacyButton;

    [SerializeField]
    private Image Icon;
    [SerializeField]
    private Image back;
    private ProfileInstance profile;

    [SerializeField]
    private Color NotEnaughColor;

   // public bool isEnaugh=true;

    [SerializeField]
    private CurrencyType type;

    internal void SetPrice(uint value)
    {
        this.value = value;
        UpdateView();
    }

    internal void SetHeroPrice(HeroPrice price)
    {
        if (price.isSoft)
        {
            value = price.soft;
            type = CurrencyType.Soft;
        }
        else if(price.isHard)
        {
            value = price.hard;
            type = CurrencyType.Hard;
        }
        else if (price.isReal)
        {
            type = CurrencyType.Real;
            realPrice = IAPManager.Instance.GetItemMetadata(price.store_key).localizedPriceString;
        }
        else
        {
            value = 0;
        }
        UpdateView();
    }
    public void SetGrayMaterial(bool setGray)
    {
        back.material = setGray ? VisualContent.Instance.GrayScaleMaterial : null;
    }
    //public bool isCloseLook = false;
    internal void UpdateView()
    {

        if (profile == null)
        {
            profile = ClientWorld.Instance.Profile;
        }
        if (value == 0) return;
        if(profile.Stock.CanTake(type, value))
        {
            PriceText.color = Color.white;
            LegacyButton.isLocked = false;
            LegacyButton.interactable = true;
            LegacyButton.LoockedOnClick = null;
        }
        else
        {
            PriceText.color = NotEnaughColor;
             LegacyButton.isLocked = true;
             LegacyButton.interactable = false;
              if (!profile.IsBattleTutorial && type == CurrencyType.Hard)
                {
                    LegacyButton.LoockedOnClick = OpenBankWindow;
                }
             else
              if (!profile.IsBattleTutorial && type == CurrencyType.Soft)
                {
                    LegacyButton.LoockedOnClick = NotEnoughCoins;
                }
                else
                {
                    LegacyButton.localeAlert = Locales.Get("locale:1642", (value - profile.Stock.GetCount(type)).ToString());
                }
            if (_isNotLockedClick)
            {
                LegacyButton.interactable = true;
                LegacyButton.isLocked = false;
                LegacyButton.LoockedOnClick = null;
            }

        }

        if (type > 0)
        {
            Icon.gameObject.SetActive(true);
            Icon.sprite = VisualContent.Instance.GetCurrencyIcon(type);
            PriceText.text = LegacyHelpers.FormatByDigits(type > 0 ? value.ToString() : realPrice);
        }
        else
        {
            Icon.gameObject.SetActive(false);
            PriceText.text = realPrice;
        }
    }
    public void LoockedOnClick(Action action)
    {
        LegacyButton.LoockedOnClick = NotEnoughCoins;
    }
    private bool _isNotEnoughtCoins = true;
    private bool _isNotLockedClick = false;
    public void IsNotLockedClick(bool value = true)
    {
        _isNotLockedClick = value;
    }
    public void IsNotEnoughtCoins(bool value = true)
    {
        _isNotEnoughtCoins = value;
    }
    public void OpenBankWindow()
    {
       if (WindowManager.Instance.CurrentWindow is LootBoxPopUpWindowBehaviour)
            WindowManager.Instance.ClosePopUp();
          WindowManager.Instance.MainWindow.OpenShopWithSection(RedirectMenuSection.BankGems);

    }
    public void NotEnoughCoins()
    {
        if (_isNotEnoughtCoins)
        {
            WindowManager.Instance.OpenNotEnoughCoinsWindow(value - profile.Stock.GetCount(type), OnPlayerProfileWait);
        }
    }

    private void OnPlayerProfileWait()
    {
        profile.PlayerProfileUpdated.AddListener(OnPlayerProfileUpdated);
    }
    private void OnPlayerProfileUpdated()
    {
        UpdateView();
        profile.PlayerProfileUpdated.RemoveListener(OnPlayerProfileUpdated);
    }

}

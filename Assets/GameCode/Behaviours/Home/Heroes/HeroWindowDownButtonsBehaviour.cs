using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class HeroWindowDownButtonsBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject BuyButton;
        [SerializeField] private GameObject UpgradeButton;
        [SerializeField] private ButtonWithPriceViewBehaviour UpgradePriceButton;
        [SerializeField] private ButtonWithPriceViewBehaviour BuyPriceButton;
        [SerializeField] private LegacyButton UpgradeLegacyButton;
        [SerializeField] private GameObject UpgradeEffect;
        [SerializeField] private LegacyButton SelectButton;
        [SerializeField] private TextMeshProUGUI selecctedButtonText;
        [SerializeField] private GameObject UseButton;
        [SerializeField] private GameObject LockedTextObject;
        [SerializeField] private TextMeshProUGUI ArenaText;

        [SerializeField] private HeroArrowButtonBehaviour LeftArrow;
        [SerializeField] private HeroArrowButtonBehaviour RightArrow;
        private PlayerProfileHero playerHero;

        private BinaryHero BinaryHero;

        internal void InitButtons(BinaryHero binaryHero)
        {
            BinaryHero = binaryHero;
            BuyButton.SetActive(false);
            UseButton.SetActive(false);
            UpgradeButton.SetActive(false);
            LockedTextObject.SetActive(false);
            playerHero = null;
        }

        public void NeedOpen(int number)
        {
            LockedTextObject.SetActive(true);
            ArenaText.text = Locales.Get("locale:1360", number);
        }

        public void NeedBuy()
        {
            BuyButton.SetActive(true);
            BuyPriceButton.SetHeroPrice(BinaryHero.price);
        }
        public (bool, bool) CanUpdate
        {
            get
            {
                return (playerHero != null && playerHero.level < ClientWorld.Instance.Profile.Level.level,
                playerHero != null &&  playerHero.UpdatePrice <= ClientWorld.Instance.Profile.Stock.getItem(CurrencyType.Soft).Count);
            }
            set { }
        }
        public bool CanBuy
        {
            get
            {
                if (BinaryHero.price.isHard)
                {
                    return ClientWorld.Instance.Profile.Stock.CanTake(CurrencyType.Hard, BinaryHero.price.hard);
                }else if (BinaryHero.price.isSoft)
                {
                    return ClientWorld.Instance.Profile.Stock.CanTake(CurrencyType.Soft, BinaryHero.price.soft);
                }
                else
                {
                    return true;
                }
            }
        }
        public void Exists(PlayerProfileHero hero, ProfileInstance profile)
        {
            playerHero = hero;

            UpdateSelectButtonText(profile);

            UseButton.SetActive(true);
            UpgradeButton.SetActive(true);
            UpgradePriceButton.SetPrice(playerHero.UpdatePrice);
            (bool, bool) canUpdate = (
                playerHero.level < ClientWorld.Instance.Profile.Level.level,
                 playerHero.UpdatePrice <= ClientWorld.Instance.Profile.Stock.getItem(CurrencyType.Soft).Count);

            UpgradeLegacyButton.isLocked = !canUpdate.Item1 && canUpdate.Item2 ;
            if (!canUpdate.Item1)
            {
                UpgradePriceButton.IsNotEnoughtCoins(false);
                UpgradeLegacyButton.localeAlert = Locales.Get("locale:1936");
            }
            else
            {
                UpgradePriceButton.IsNotEnoughtCoins(true);
                UpgradeLegacyButton.localeAlert = "";
            }
           
            UpgradeLegacyButton.interactable = canUpdate.Item1 && canUpdate.Item2 || !canUpdate.Item2;
            UpgradeLegacyButton.GetComponent<ButtonWithPriceViewBehaviour>().SetGrayMaterial(!canUpdate.Item1);
            UpgradeEffect.SetActive(canUpdate.Item1 && canUpdate.Item2);
            UpgradePriceButton.UpdateView();
            if (!canUpdate.Item1)
            {
                UpgradeLegacyButton.isLocked = !canUpdate.Item1;
                UpgradePriceButton.IsNotEnoughtCoins(false);
                UpgradeLegacyButton.localeAlert = Locales.Get("locale:1936");
            }
        }

        public void SetOrdersForChoosenHero(ProfileInstance profile)
        {
            UpdateSelectButtonText(profile);
        }

        public void SetLeftArrow(string name, Color color)
        {
            LeftArrow.SetHero(name, color);
        }

        public void UpdateSelectButtonText(ProfileInstance profile)
        {
            BinaryHero.GetLockedByArena(out BinaryBattlefields binaryArena);
            byte number = Settings.Instance.Get<ArenaSettings>().GetNumber(binaryArena.index);

            var isSelectedHero = profile.SelectedHero == BinaryHero.index;
            var isAvailableHero = profile.CurrentArena.number + 1 >= number;
            var isInteractableButton = !isSelectedHero && isAvailableHero ? true : false;
            SelectButton.targetGraphic.enabled = isInteractableButton;

            var selectedText = isSelectedHero ? Locales.Get("locale:934") : Locales.Get("locale:1960"/*"locale:931"*/);
            var selectedColor = isSelectedHero ? new Color32(41, 176, 206, 255) : new Color32(255, 255, 255, 255);
            selecctedButtonText.text = selectedText;
            selecctedButtonText.color = selectedColor;
        }

        public void SetRightArrow(string name, Color color)
        {
            RightArrow.SetHero(name, color);
        }

        public void UpgradeButtonVisible(bool value = true)
        {
            UpgradeLegacyButton.gameObject.SetActive(value);
        }
        internal void EnableLeftArrow(bool v)
        {
            LeftArrow.gameObject.SetActive(v);
        }

        internal void EnableRightArrow(bool v)
        {
            RightArrow.gameObject.SetActive(v);
        }
    }
}
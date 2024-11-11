using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassPremiumPassPanelBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private GameObject activateButton;
        [SerializeField] 
        private GameObject activatedText;

        private BattlePassWindowBehaviour windowBehaviour;
        private ProfileInstance profile;
        private bool isPremiumBought;

        public void Init(ProfileInstance profileInstance, BattlePassWindowBehaviour battlePassWindow)
        {
            windowBehaviour = battlePassWindow;
            profile = profileInstance;
            isPremiumBought = profile.battlePass.isPremiumBought;
            SetUpActivateButton();
            profile.PlayerProfileUpdated.AddListener(OnPlayerProfileUpdated);
        }

        private void SetUpActivateButton()
        {
            activateButton.SetActive(!isPremiumBought);
            activatedText.SetActive(isPremiumBought);
        }

        private void OnPlayerProfileUpdated()
        {
            if (profile.battlePass.isPremiumBought != isPremiumBought)
            {
                isPremiumBought = profile.battlePass.isPremiumBought;
                SetUpActivateButton();
            }
        }

        public void ActivateButtonClick()
        {
            if (windowBehaviour.isTutor)
            {
             //   PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:1483"));
                return;
            }
            windowBehaviour.OpenShop();
        }

        private void OnDestroy()
        {
            profile.PlayerProfileUpdated.RemoveListener(OnPlayerProfileUpdated);
        }
    }
}
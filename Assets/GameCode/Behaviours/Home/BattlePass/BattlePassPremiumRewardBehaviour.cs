using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassPremiumRewardBehaviour : BattlePassBasicRewardBehaviour
    {
        [SerializeField] 
        private GameObject lockImage;
        
        protected override void SetCollectedState()
        {
            lockedState.SetActive(true);
            currentState.SetActive(false);
        }

        protected override void ShowLockedLootBox()
        {
            battlePassLevel.onPremShow();
        }
        protected override void SetLockedState()
        {
            lockedState.SetActive(true);
            currentState.SetActive(false);
        }

        protected override void SetCurrentState()
        {
            lockedState.SetActive(false);
            currentState.SetActive(true);
        }

        public void SetPremiumLockedState(bool isPremiumBought)
        {
            UpdateSwitchState();
            lockImage.SetActive(!isPremiumBought);

            if (!isPremiumBought)
                button.interactable = false;
            
            if(timer.gameObject.activeInHierarchy)
                timer.gameObject.SetActive(isPremiumBought);
        }

        public void EnablePremiumReward()
        {
            button.interactable = true;
        }        
    }
}
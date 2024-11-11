using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassOfferBehaviour : BasicOfferBehaviour
    {
        [SerializeField] 
        private UITimerBehaviour timer;
        [SerializeField] 
        private GameObject boughtImage;
        [SerializeField] 
        private GameObject PricePanel;

        public void SetTimer(DateTime endTime)
        {
            timer.SetFinishedTime(endTime);
        }

        public void SetBoughtState(bool bought)
        {
            //boughtImage.SetActive(bought);
            //PricePanel.SetActive(!bought);
            buyButton.interactable = !bought;
        }
    }
}
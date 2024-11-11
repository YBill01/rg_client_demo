using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ArenaBasicRewardBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private GameObject activeLight;
        [SerializeField] 
        private TextMeshProUGUI amount;
        [SerializeField] 
        private Animator rewardAnim;

        private bool isSeen;
        public float position;
        
        public virtual void SetActiveState()
        {
            //activeLight.SetActive(true);
        }

        public virtual void SetCompleteState()
        {
            //activeLight.SetActive(false);
        }

        public virtual void SetLockedState()
        {
            //activeLight.SetActive(false);
        }

        public void SetInitialScale()
        {
            rewardAnim.enabled = false;
            transform.localScale = Vector3.zero;
            isSeen = false;
        }
        public void SetScaleNewRewardArena()
        {
            rewardAnim.enabled = true;
        }
        public void SetPosition(float pos)
        {
            position = pos;
        }

        public bool HadFirstAppearance()
        {
            return isSeen;
        }

        public float GetPosition()
        {
            return position;
        }

        public void PlayAnimationOnFirstAppearance(bool value = true)
        {
            isSeen = value;
            rewardAnim.enabled = value;
        }

        protected void SetAmount(string text)
        {
            amount.text = "<size=50%>x </size>" + LegacyHelpers.FormatByDigits(text);
        }
    }
}
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class ChangeValueBehaviour : MonoBehaviour, IParticleReciever
    {
        private float oldValue;
        private float currentValue;
        private float newValue;

        bool inited = false;
        bool WaitParticles = false;        

        [SerializeField, Range(0.0f, 1.0f)] float LerpSpeed = 0.15f;

        [SerializeField] TextMeshProUGUI ValueText;
        [SerializeField] AudioSource source;
        [SerializeField] DelayedRewardTargetBehaviour DelayedRewardTarget;
        [SerializeField] private CurrencyType currencyType = CurrencyType.Soft; //SET WITH YOUR HANDS WHERE EVER YOU NEED

        float ParticlesPercentage = 0.0f;

        void Update()
        {
            float tempValue;
            if (WaitParticles)
            {
                tempValue = Mathf.Lerp(oldValue, newValue, ParticlesPercentage);  
            }
            else
            {
                tempValue = Mathf.Lerp(currentValue, newValue, LerpSpeed);                
            }
            if (tempValue != currentValue)
            {
                currentValue = tempValue;
                UpdateView();

            }

        }

        private void UpdateView()
        {
            ValueText.text = LegacyHelpers.FormatByDigits(Mathf.RoundToInt(currentValue).ToString());
        }
        private void UpdateView2()
        {
            ValueText.text = LegacyHelpers.FormatByDigits(Mathf.RoundToInt(currentValue).ToString());
            SoundsPlayerManager.Instance.PlaySound(SoundName.Buy);
        }

        public void SetValue(int value)
        {
            newValue = value;
            if(newValue < currentValue)  // списание валюты.
            {
                currentValue = value;
                UpdateView2();
                return;
            }
            if (!inited)
            {
                inited = true;
                if (DelayedRewardTarget != null)
                {
                    var holdValue = DelayedRewardTarget.CheckHold();
                    if (holdValue > 0)
                    {
                        currentValue = value - holdValue;
                        ChangeWithParticles();
                    }
                    else
                    {
                        currentValue = value;
                    }
                }
                else
                {
                    currentValue = value;
                }
                UpdateView();
            }
            else
            {
               
                if (DelayedRewardTarget != null)
                {
                    var holdValue = DelayedRewardTarget.CheckHold();
                    if (holdValue > 0)
                    {
                        currentValue = value - holdValue;
                        ChangeWithParticles();
                    }
                }
            }   
        }

        public void ParticleCame(float percentageComplete)
        {
            ParticlesPercentage = percentageComplete;
            if(ParticlesPercentage == 1.0f)
            {
                DelayedRewardTarget?.DropHoldValue();
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(FinishParticles());
                }
                else
                {
                    ResetParticles();
                }
            }
        }

        void ResetParticles()
        {
            WaitParticles = false;
            ParticlesPercentage = 0.0f;
        }
        public void ChangeWithParticles()
        {
            WaitParticles = true;
            oldValue = currentValue;
        }
        IEnumerator FinishParticles()
        {
            yield return new WaitForSeconds(0.1f);
            ResetParticles();
        }
    }
}

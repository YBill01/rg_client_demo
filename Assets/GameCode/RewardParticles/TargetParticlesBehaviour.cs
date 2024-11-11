using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Legacy.Client {

    /// <summary>
    /// Use when need to customize behaviour when reward particles came to object.
    /// </summary>
    public interface IParticleReciever
    {
        /// <summary>
        /// Must Do something when every Reward Particle Came To Object.
        /// Calls when every particle come to target.
        /// </summary>
        /// <param name="percentageComplete">Percentage from 0 to 1 that represents completion of full particles. 1 means that all particles already came.</param>
        void ParticleCame(float percentageComplete);

        /// <summary>
        /// Get receiver to know that particles start to fly to this receiver.
        /// </summary>
        void ChangeWithParticles();
    }

    public class TargetParticlesBehaviour : MonoBehaviour
    {
        [SerializeField] RectTransform PunchRect;
        [SerializeField] RectTransform target;
        [SerializeField, Range(0.0f, 2.0f)] float punchPower;
        [SerializeField, Range(0.0f, 1.0f)] float punchDuration;
        [SerializeField] Ease punchEase;
        [SerializeField] ParticleSystem IncomeParticles;
        private IParticleReciever[] particleReceiver;
        FadeElementBehaviour fader;

        float PercentageComplete;

        byte ParticlesLeft = 0;
        byte TotalParticles = 0;

        private void Start()
        {
            particleReceiver = GetComponentsInChildren<IParticleReciever>();
            fader = GetComponent<FadeElementBehaviour>();
        }

        public void ParticleCame(UnityEvent OnParticleCame)
        {
            ParticlesLeft--;

            if (PunchRect != null)
            {
                PunchRect.DOKill();
                PunchRect.localScale = Vector3.one;
                PunchRect.DOPunchScale(Vector3.one * punchPower, punchDuration)
                    .SetEase(punchEase);
            }
            PercentageComplete = 1.0f - (float)ParticlesLeft / (float)TotalParticles;

            if (IncomeParticles != null)
            {
                IncomeParticles.Stop();
                IncomeParticles.Play();
            }

            if (particleReceiver != null)
            {
                foreach (var receiver in particleReceiver)
                {
                    receiver.ParticleCame(PercentageComplete);
                }
            }
            if(ParticlesLeft < 1)
            {
                fader?.TryOff();
            OnParticleCame.Invoke();
            }

        }

        public Vector3 GetTargetPosition()
        {
            return target?.position ?? Vector3.zero;
        }

        internal void WaitForParticles(byte count)
        {
            TotalParticles = count;
            ParticlesLeft = count;
            if (particleReceiver != null)
            {
                foreach (var receiver in particleReceiver)
                {
                    receiver.ChangeWithParticles();
                }
            }
            fader?.Enable(true);
        }
    }
}

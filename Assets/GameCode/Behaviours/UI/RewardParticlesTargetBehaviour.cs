using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class RewardParticlesTargetBehaviour : MonoBehaviour
    {
        [SerializeField] Animator IconAnimator;
        [SerializeField] ParticleSystem AppearParticles;
        public void Appear()
        {
            IconAnimator?.Play("ParticlesComes");
            if (!AppearParticles.isPlaying)
            {
                AppearParticles.Play();
            }
        }
    }

    

}

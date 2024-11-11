using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
    public class SkillEffect : MonoBehaviour
    {
        [Tooltip("PlaySkillEffect_1")]
        [SerializeField] private ParticleSystem[] effectsArr_1;
        [SerializeField] private ParticleSystem[] effectsArr_2;
        [SerializeField] private ParticleSystem[] effectsArr_3;
        private void PlaySkillEffect_1()
        {
            if(effectsArr_1 != null)
            {
                foreach (var ps in effectsArr_1)
                {
                    ps.ResetAndPlay();
                }
            }
        }
        private void PlaySkillEffect_2()
        {
            if (effectsArr_2 != null)
            {
                foreach (var ps in effectsArr_2)
                {
                    ps.ResetAndPlay();
                }
            }
        }
        private void PlaySkillEffect_3()
        {
            if (effectsArr_3 != null)
            {
                foreach (var ps in effectsArr_3)
                {
                    ps.ResetAndPlay();
                }
            }
        }
    }
}

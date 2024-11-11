using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Legacy.Client
{
    public class ParticleRewardBehaviour : MonoBehaviour
    {
        ParticleSystem particleSystem;

        void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                var sheet = particleSystem.textureSheetAnimation;
                sheet.fps = Random.Range(20, 30);
            }
        }

        public void SetFPS(byte fps)
        {
            if (particleSystem != null)
            {
                var sheet = particleSystem.textureSheetAnimation;
                sheet.fps = fps;
            }
        }
    }
}

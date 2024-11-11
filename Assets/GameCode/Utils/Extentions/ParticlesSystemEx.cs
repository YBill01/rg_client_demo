using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public static class ParticlesSystemEx
    {
        public static void ResetAndPlay(this ParticleSystem vfx)
        {
            vfx.Stop();
            vfx.Clear();
            vfx.Play();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpParticleShakeController : MonoBehaviour
{
    public void OnParticleSystemStopped()
    {
        LevelUpHeroBehavior.Instance.StartCoroutine(LevelUpHeroBehavior.Instance.ShakeCoroutine());
    }
}

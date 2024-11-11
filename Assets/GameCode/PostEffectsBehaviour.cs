using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectsBehaviour : MonoBehaviour
{
    [SerializeField] ParticleSystem CenterRaysRounded;
    [SerializeField] ParticleSystem FullScreenStasrs;

    public static PostEffectsBehaviour Instance;

    void Awake()
    {
        Instance = this;
    }

    public void LootBoxWindowFinish(bool enable)
    {
        CenterRaysRounded.gameObject.SetActive(enable);
        FullScreenStasrs.gameObject.SetActive(enable);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalahardBehaviour : MonoBehaviour
{
    [SerializeField] private float HealEffectDuration;
    [SerializeField] private GameObject HealEffectSwordPart;
    [SerializeField] private GameObject HealEffectHeroPart;

    public void PlayHealEffect()
    {
        HealEffectHeroPart.SetActive(true);
        HealEffectSwordPart.SetActive(true);

        StartCoroutine(StopEffect());

        IEnumerator StopEffect()
        {
            yield return new WaitForSeconds(HealEffectDuration);
            HealEffectSwordPart.SetActive(false);
            HealEffectHeroPart.SetActive(false);
        }
    }
}

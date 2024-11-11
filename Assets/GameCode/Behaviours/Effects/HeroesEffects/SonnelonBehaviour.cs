using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EZCameraShake.CameraShaker;

public class SonnelonBehaviour : MonoBehaviour
{
    [Header("ShakeCamera Settings")]
    [SerializeField] ShakeSettings ShakeSettings;
    [SerializeField] private float MagmaEffectDuration;
    [SerializeField] private float AuraEffectDuration;
    [SerializeField] private GameObject MagrmaHeroEffect;
    [SerializeField] private GameObject AuraHeroEffect;

    public void PlayMagmaEffect()
    {
        MagrmaHeroEffect.SetActive(true);

        StartCoroutine(StopEffect());

        IEnumerator StopEffect()
        {
            yield return new WaitForSeconds(MagmaEffectDuration);
            MagrmaHeroEffect.SetActive(false);
        }
    }
    public void PlayAuraEffect()
    {
        AuraHeroEffect.SetActive(true);

        StartCoroutine(StopEffect());

        IEnumerator StopEffect()
        {
            yield return new WaitForSeconds(AuraEffectDuration);
            AuraHeroEffect.SetActive(false);
        }
    }
    public void ShakeCamera()
    {
        ShakeSettings.Shake();
    }
}

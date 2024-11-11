using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleFlagAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private GameObject geometry;
    [SerializeField] private GameObject destroyedFlagPrefab;

    private GameObject particleObj;
    private float currentBlendValue;
    private float foundBlendValue;
    private bool isDestroyed = false;
    private static float explosionTime = 0.2f;
    private static float effectTimeTime = 3.5f;

    private void Start()
    {
        particleObj = this.GetComponentInChildren<EmptyParticleFlagComponent>().gameObject;
        particleObj.SetActive(false);
    }

    void Update()
    {
        if (currentBlendValue.AboutEquals(foundBlendValue, 0.001f))
        {
            var value = UnityEngine.Random.value;
            foundBlendValue = curve.Evaluate(value);
        }

        currentBlendValue = Mathf.Lerp(currentBlendValue, foundBlendValue, 0.01f);
        animator.SetFloat("Blend", currentBlendValue);
    }

    public IEnumerator BurnEffect()
    {
        if (!isDestroyed)
        {
            particleObj.gameObject.SetActive(true);
            yield return new WaitForSeconds(explosionTime);
            Geometry(false);
            SpawnDestoyedFlag();
            yield return new WaitForSeconds(effectTimeTime - explosionTime);
            Geometry(true);
            isDestroyed = true;
            this.gameObject.SetActive(false);
        }

    }

    private void SpawnDestoyedFlag()
    {
      var go = GameObject.Instantiate(destroyedFlagPrefab, this.transform.position, this.transform.rotation, this.transform.parent);
    }
    private void Geometry(bool flag)
    {
        geometry.SetActive(flag);
    }
}

public static class FloatExtension
{
    public static bool AboutEquals(this float value1, float value2, double precalculatedContextualEpsilon)
    {
        return Math.Abs(value1 - value2) <= precalculatedContextualEpsilon;
    }

}

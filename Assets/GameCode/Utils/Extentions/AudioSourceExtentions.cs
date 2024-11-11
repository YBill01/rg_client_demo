using System.Collections;
using UnityEngine;
using Legacy.Client;
public static class AudioSourceExtentions
{
    public static void Play(this AudioSource sorce, float delay)
    {
        var mono = SoundManager.Instance.GetComponent<MonoBehaviour>();
        mono.StartCoroutine(DelayedEx(delay, sorce.Play));
    }

    private static IEnumerator DelayedEx(float delay, System.Action method)
    {
        yield return new WaitForSeconds(delay);
        method();
    }
}

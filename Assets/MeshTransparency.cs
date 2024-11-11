using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MeshTransparency : MonoBehaviour
{
    [Range(0f,1f)]
    public float Alpha = 1;
    private MeshRenderer[] meshRenderers;
    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
    }

    private float prevAlpha;
    void Update()
    {
        if (prevAlpha == Alpha) return;
        prevAlpha = Alpha;
        foreach(var mr in meshRenderers)
            LegacyHelpers.SetAlpha(mr, Alpha);
    }
}

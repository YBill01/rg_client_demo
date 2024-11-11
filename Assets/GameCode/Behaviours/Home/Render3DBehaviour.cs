using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Render3DBehaviour : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)]
    float lerpSpeed = 0.2f;

    Vector3 currentScale = Vector3.one;
    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, lerpSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, currentScale, lerpSpeed);
    }

    internal void SetScale(float scale3DRender)
    {
        currentScale.x = scale3DRender;
        currentScale.y = scale3DRender;
    }
}

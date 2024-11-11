using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectPositionToBehaviour : MonoBehaviour
{
    [SerializeField] Vector2 TargetPosition = Vector2.zero;
    [SerializeField, Range(0.0f, 1.0f)] float LerpSpeed = 0.15f;

    RectTransform rect;

    public void SetLerpSpeed(float value)
    {
        LerpSpeed = Mathf.Clamp01(value);
    }

    public void SetTargetPosition(Vector2 pos)
    {
        TargetPosition = pos;
    }

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    void Update()
    {
        if (rect != null)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, TargetPosition, LerpSpeed);
        }
    }
}

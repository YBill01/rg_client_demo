using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectScaleToBehaviour : MonoBehaviour
{
    float ScaleMultiplier = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float LerpSpeed = 0.15f;

    RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        StartScale = rect.localScale;

    }

    Vector2 CurrentScale = Vector2.one;
    Vector2 StartScale=Vector2.zero;
   
    void Update()
    {
        CurrentScale = StartScale * ScaleMultiplier;

        rect.localScale = Vector2.Lerp(rect.localScale, CurrentScale, LerpSpeed);        
    }
    public void Reset()
    {
        rect.localScale=new Vector3(StartScale.x, StartScale.y, rect.localScale.z);
        Debug.Log("reset");
    }

    public void SetScaleMultiplier(float multiplier)
    {
        ScaleMultiplier = multiplier;
    }

    public void SetLerpSpeed(float speed)
    {
        LerpSpeed = speed;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpinnerBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform SpinnerImageRect;

    [SerializeField, Range(0.0f, 50.0f)]
    private float RotationSpeed;

    void Update()
    {
        SpinnerImageRect.Rotate(0.0f, 0.0f, -RotationSpeed);
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardParticleRotationBehaviour : MonoBehaviour
{
    [SerializeField, Range(0, 20)] public byte minRotationSpeed = 5;
    [SerializeField, Range(0, 30)] public byte maxRotationSpeed = 30;
    [SerializeField] ParticleSystem ParticleSystem;
    [SerializeField] RotationAxis rotationAxis = RotationAxis.Up;

    public enum RotationAxis
    {
        Up,
        Foward,
        Right
    }

    RectTransform rect;

    private float rotationSpeed;
    float RotationTimer = 0.0f;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
    }
    void Update()
    {
        Vector3 rotationVector = Vector3.up;
        switch (rotationAxis)
        {
            case RotationAxis.Right:
                rotationVector = Vector3.right;
                break;
            case RotationAxis.Foward:
                rotationVector = Vector3.forward;
                break;
        }
        if (RotationTimer > 0)
        {
            RotationTimer -= Time.deltaTime;
            if (ParticleSystem != null)
            {
                var textureSheet = ParticleSystem.textureSheetAnimation;
                textureSheet.fps = rotationSpeed;
            }
            else
            {
                rect.Rotate(rotationVector, -rotationSpeed);
            }
        }
        else
        {
            if (ParticleSystem != null)
            {
                //var textureSheet = ParticleSystem.textureSheetAnimation;
                //textureSheet.fps = 0;
            }
            else
            {
                rect.rotation = Quaternion.Lerp(rect.rotation, Quaternion.identity, rotationSpeed / 500);
            }
        }
    } 

    public void SetRotationTime(float RotateTime)
    {
        RotationTimer = RotateTime;
    }
}

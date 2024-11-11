using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public Positions aspect;

    [SerializeField] CameraShaker shaker;
    [SerializeField] private Positions aspect24_11;
    [SerializeField] private Positions aspect16_9;
    [SerializeField] private Positions aspect3_2;
    [SerializeField] private Positions aspect4_3;

    [Serializable]
    public struct Positions
    {
        public PositionData max;
        public PositionData min;
    }

    [Serializable]
    public struct PositionData
    {
        public Vector3 cameraPosition;
        public float deltaX;
        public float deltaZ;
        public float getZ;
    }

    private float lastAspect;
    private float limitCoef = 0.75f;


    private void Start()
    {
        UpdateAspect();
    }

    private void UpdateAspect()
    {
        if (Camera.main.aspect >= 1.85)
        {
            aspect = aspect24_11;
        }
        else if (Camera.main.aspect >= 1.7)
        {
            aspect = aspect16_9;
        }
        else if (Camera.main.aspect >= 1.5)
        {
            aspect = aspect3_2;
        }
        else
        {
            aspect = aspect4_3;
        }
        transform.position = GetPositionByAspect(true);
        shaker.SetRest();
    }

    public Vector3 GetPositionByAspect(bool max = false)
    {
        return max ? aspect.max.cameraPosition : aspect.min.cameraPosition;
    }
    public (float, float) GetCurrentPositionDelta()
    {
        var coef = (transform.position.y - aspect.min.cameraPosition.y) / (aspect.max.cameraPosition.y - aspect.min.cameraPosition.y);

        (float, float) deltaXZ = (Math.Abs(aspect.min.deltaX + ((aspect.max.deltaX - aspect.min.deltaX) * coef)),
                     Math.Abs(aspect.min.deltaZ + ((aspect.max.deltaZ - aspect.min.deltaZ) * coef)));

        return deltaXZ;
    }

    public float GetAdaptableMoveToZoomSpeed(float speed)
    {
        var coef = (transform.position.y - aspect.min.cameraPosition.y) / (aspect.max.cameraPosition.y - aspect.min.cameraPosition.y);
        var lastSpeed = coef > limitCoef ? speed * coef : speed * limitCoef;
        return lastSpeed;
    }

    public Vector3 GetPosition(float currnetY)
    {
        var coef = (currnetY - aspect.min.cameraPosition.y) / (aspect.max.cameraPosition.y - aspect.min.cameraPosition.y);

        return aspect.min.cameraPosition + ((aspect.max.cameraPosition - aspect.min.cameraPosition) * coef);
    }

    public Vector3 GetPersentagePosition(float percentages)
    {
        return aspect.min.cameraPosition + ((aspect.max.cameraPosition - aspect.min.cameraPosition) * ((100 - percentages) / 100));
    }

    private void Update()
    {
        if (lastAspect != Camera.main.aspect)
        {
            //  transform.localPosition = new Vector3(0, aspect.min.cameraPosition.y, aspect.min.cameraPosition.z);
            //GetComponent<CameraShake>().SetStartPosition(GetComponent<CameraShake>().shakeContainer.position);
            lastAspect = Camera.main.aspect;
            UpdateAspect();
        }
    }

}

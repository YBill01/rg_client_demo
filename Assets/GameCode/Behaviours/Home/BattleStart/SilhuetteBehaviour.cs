using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SilhuetteBehaviour : MonoBehaviour
{
    [SerializeField, Range(0.0f, 2.0f)]
    private float ChangeTimerMultiplier;
    [SerializeField, Range(0.0f, 1.0f)]
    private float SinFunctionAngleChangeSpeed;
    [SerializeField]
    private float currentAngleToSinFunction;
    [SerializeField]
    private float ChangeTime;
    [SerializeField]
    private float currentTimer = 0.0f;

    [SerializeField]
    private HeroIconBehaviour IconImage;


    private int CurrentSilhuetteIndex;

    [SerializeField]
    private BattleStartWindowBehaviour BattleStartWindow;

    public bool Enabled;
    void Update()
    {
        if (Enabled)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer > ChangeTime)
            {
                if (Enabled)
                {
                    NextSilhuette();
                    ChangeTime = (Mathf.Sin(currentAngleToSinFunction) + 2) * ChangeTimerMultiplier;
                    currentAngleToSinFunction += SinFunctionAngleChangeSpeed;
                    currentTimer = 0.0f;
                }
            }
        }
    }

    internal void DisableSilhuatte()
    {
        Enabled = false;
        IconImage.RemoveMaterial();
    }

    private void NextSilhuette()
    {
        if (Enabled)
        {
            IconImage.Set(VisualContent.Instance.Heroes[CurrentSilhuetteIndex].StartBattleIcon);

            if (++CurrentSilhuetteIndex == VisualContent.Instance.Heroes.Count)
            {
                CurrentSilhuetteIndex = 0;
            }
        }
    }
}

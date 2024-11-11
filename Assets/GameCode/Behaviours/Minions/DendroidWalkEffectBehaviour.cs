using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DendroidWalkEffectBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject rightStepDust;
    [SerializeField] private GameObject leftStepDust;

    public void LeftStep()
    {
        leftStepDust.SetActive(true);
        rightStepDust.SetActive(false);

    }

    public void RightStep()
    {
        rightStepDust.SetActive(true);
        leftStepDust.SetActive(false);
    }
}

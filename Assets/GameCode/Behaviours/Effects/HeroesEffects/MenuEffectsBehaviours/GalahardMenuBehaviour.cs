using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalahardMenuBehaviour : MonoBehaviour
{
    [SerializeField] GameObject Sparkles;

    private void OnDisable()
    {
        Sparkles.SetActive(false);
    }

    void SetSparkles(int value)
    {
        Sparkles.SetActive(value > 0);        
    }
}

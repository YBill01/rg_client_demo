using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarEmptyBehaviour : MonoBehaviour
{

    [SerializeField] private bool canFly;

    public bool CanFly()
    {
        return canFly;
    }
}

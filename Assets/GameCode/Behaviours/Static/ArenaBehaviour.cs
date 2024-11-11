using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject bridge1;

    [SerializeField]
    private GameObject bridge2;

    void OnEnable()
    {
        GetComponentInParent<StaticColliders>().SetBridges(bridge1, bridge2);
    }
}

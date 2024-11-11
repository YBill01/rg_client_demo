using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapInitBehaviour : InitBehaviour
{
    [SerializeField] private GameObject TrpFlyEffect;
    public override void Init()
    {
        SpaWnHeight = 0f;
        ObjectPooler.instance.MinionBack(this.gameObject);
        GetComponent<MinionInitBehaviour>().DoMinionVisible();
        if (GetComponent<InitBehaviour>()) GetComponent<InitBehaviour>().DoMinionVisible();

    }
    public override void Spawn()
    {
        //GetComponent<MinionInitBehaviour>().DoMinionVisible();
        //if (GetComponent<InitBehaviour>()) GetComponent<InitBehaviour>().DoMinionVisible();
    }
}

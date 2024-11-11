using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBehaviour : MonoBehaviour
{
   [SerializeField] private GameObject spineDustEffect;
   [SerializeField] private GameObject handLEffect;
   [SerializeField] private GameObject handREffect;

    private void Update()
    {
        var epb = GetComponent<EntityProxyBehaviour>();
        if (!epb || epb.Entity == null) return;
        var selfEntity = epb.Entity;

        var EM = ClientWorld.Instance.EntityManager;
        if (!EM.HasComponent<MinionData>(selfEntity)) return;

        var md = EM.GetComponentData<MinionData>(selfEntity);
        if (md.state == MinionState.Move)
        {
            spineDustEffect.SetActive(true);
            handLEffect.SetActive(true);
            handREffect.SetActive(true);
        }
        else
        {
            spineDustEffect.SetActive(false);
            handLEffect.SetActive(false);
            handREffect.SetActive(false);
        }
    }
}

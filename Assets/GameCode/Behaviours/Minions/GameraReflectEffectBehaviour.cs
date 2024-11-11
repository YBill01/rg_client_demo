using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameraReflectEffectBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject reflectParticle;
    [SerializeField] private Transform particleForceFieldHit;
    [SerializeField] private Transform particleForceFieldBack;

    private BattleBucketsSystem _buckets;
    void OnEnable()
    {
        _buckets = ClientWorld.Instance.GetOrCreateSystem<BattleBucketsSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        DoSmth();
    }


    private void DoSmth()
    {
        var _proxy = GetComponent<EntityProxyBehaviour>();
        if (_proxy == null) return;
        if (ClientWorld.Instance.EntityManager.HasComponent<Legacy.Database.MinionData>(_proxy.Entity))
        {
            var ss = ClientWorld.Instance.EntityManager.GetComponentData<Legacy.Database.MinionData>(_proxy.Entity);



            switch (ss.state)
            {
                case MinionState.Skill1:
                    reflectParticle.SetActive(true);
                    if (_buckets.Minions.TryGetValue(ss.atarget, out MinionClientBucket bucket))
                        if (ClientWorld.Instance.EntityManager.HasComponent<Transform>(bucket.entity))
                        {
                            var targetPos = ClientWorld.Instance.EntityManager.GetComponentObject<Transform>(bucket.entity).position;
                            var elevatedPosition = new Vector3(targetPos.x, targetPos.y + 1.5f, targetPos.z);
                            particleForceFieldHit.position = elevatedPosition;
                            particleForceFieldBack.position = elevatedPosition;
                        }
                    break;
                default:
                    reflectParticle.SetActive(false);
                    break;
            }
        }

    }
}

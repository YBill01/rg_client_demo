using Legacy.Client;
using Legacy.Database;
using Unity.Entities;
using UnityEngine;


public class IrvaTsunamiBehaviour : MonoBehaviour
{
    public GameObject Particles;
    void OnEnable()
    {
        var _proxy = GetComponent<EntityProxyBehaviour>();
        if (_proxy == null) return;
        var _effectData = ClientWorld.Instance.EntityManager.GetComponentData<EffectData>(_proxy.Entity);
        var _buckets = ClientWorld.Instance.GetOrCreateSystem<BattleBucketsSystem>();
        if (_buckets.Minions.TryGetValue(_effectData.source, out MinionClientBucket bucket))
        {
            var component = ClientWorld.Instance.EntityManager.GetComponentObject<Transform>(bucket.entity);
            if (component.GetComponent<IrvaBehaviour>())
            {
                Particles.transform.position = component.position;
            Particles.transform.LookAt(-component.position);
            }
        }
    }
}

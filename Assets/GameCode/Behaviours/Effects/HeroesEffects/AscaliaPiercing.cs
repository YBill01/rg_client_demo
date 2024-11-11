using Legacy.Client;
using Legacy.Database;
using Unity.Entities;
using UnityEngine;

public class AscaliaPiercing : MonoBehaviour
{
    public GameObject Particles;
    void OnEnable()
    {
        var _proxy = GetComponent<EntityProxyBehaviour>();
        if (_proxy == null) return;
        var _effectData = ClientWorld.Instance.EntityManager.GetComponentData<EffectData>(_proxy.Entity);
        transform.position = new Vector3(transform.position.x, 1f, transform.position.y);
        var _buckets = ClientWorld.Instance.GetOrCreateSystem<BattleBucketsSystem>();
        if (_buckets.Minions.TryGetValue(_effectData.source, out MinionClientBucket bucket))
        {
            var component = ClientWorld.Instance.EntityManager.GetComponentObject<Transform>(bucket.entity);
            var ascalia = component.GetComponent<AscaliaBehaviour>();
            if (ascalia != null)
                ascalia.Piercing(transform);
            //Particles.transform.position = new Vector3(bucket.position.x, transform.position.y, bucket.position.y);
            //Particles.transform.LookAt(transform);
        }
    }

}

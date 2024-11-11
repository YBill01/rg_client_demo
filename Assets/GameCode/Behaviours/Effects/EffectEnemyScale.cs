using Legacy.Client;
using Legacy.Database;
using Unity.Entities;
using UnityEngine;

public class EffectEnemyScale : MonoBehaviour
{
    void OnEnable()
    {
        var _proxy = GetComponent<EntityProxyBehaviour>();
        if (_proxy == null) return;
        var _effectData = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EffectData>(_proxy.Entity);
        transform.position = new Vector3(transform.position.x, 1f, transform.position.y);
        var _buckets = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BattleBucketsSystem>();
        if (_buckets.Minions.TryGetValue(_effectData.source, out MinionClientBucket bucket))
        {
            var scale = new Vector3(1, 1, 1);
            if (World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentObject<MinionPanel>(bucket.entity).IsEnemy)
            {
                scale = new Vector3(-1, 1, 1);
            }
            
            transform.localScale = scale;
            foreach (var tf in GetComponentsInChildren<Transform>())
            {
                tf.localScale = scale;
            }
        }
    }

}

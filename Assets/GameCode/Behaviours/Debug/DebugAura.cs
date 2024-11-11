using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
    public class DebugAura : MonoBehaviour
    {
        void OnEnable()
        {
            var _proxy = GetComponent<EntityProxyBehaviour>();
            if (_proxy != null)
            {
                if (ClientWorld.Instance.EntityManager.HasComponent<Legacy.Database.EffectRadius>(_proxy.Entity))
                {
                    var _component = ClientWorld.Instance.EntityManager.GetComponentData<Legacy.Database.EffectRadius>(_proxy.Entity);
                    gameObject.transform.localScale = new Vector3(_component.radius, _component.radius, _component.radius);
                }
            }
        }
    }
}

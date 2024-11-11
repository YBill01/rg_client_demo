
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
	public class EffectRadius : MonoBehaviour
	{
		[SerializeField]
		Transform radiusView;

		void OnEnable()
		{
			if (radiusView == null)
				radiusView = transform;

			var _proxy = GetComponent<EntityProxyBehaviour>();
			if (_proxy == null) return;
			if (World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<Legacy.Database.EffectRadius>(_proxy.Entity))
			{
				var effect = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Legacy.Database.EffectRadius>(_proxy.Entity);
				radiusView.localScale = new Vector3(effect.radius, effect.radius, effect.radius);
			}

		}
	}
}
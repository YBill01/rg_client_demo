using System.Collections;
using Unity.Entities;
using UnityEngine;

using Legacy.Database;


public class EffectDurationBehaviour : MonoBehaviour
{
	void Start()
	{
		var _proxy = GetComponent<EntityProxyBehaviour>();
        var _manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var _replicated = _manager.GetComponentData<EntityDatabase>(_proxy.Entity);
        var _effect = _manager.GetComponentData<EffectData>(_proxy.Entity);
        if (Effects.Instance.Get(_replicated.db, out BinaryEffect binary))
		{
			StartCoroutine(Dispose((ushort)binary.duration._value(_effect.level)));
		}
	}

	private IEnumerator Dispose(ushort duration)
	{
		yield return new WaitForSeconds(duration * 0.001f);
		var _system = GetComponentInChildren<ParticleSystem>();
		_system.Stop(true);
	}
}


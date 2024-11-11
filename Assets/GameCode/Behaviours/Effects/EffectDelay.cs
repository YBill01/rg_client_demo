using System.Collections;
using Unity.Entities;
using UnityEngine;
using Legacy.Database;

public class EffectDelay : MonoBehaviour
{
	public GameObject Particles;
	private ParticleSystem _particles;

    void OnEnable()
    {
        var _proxy = GetComponent<EntityProxyBehaviour>();
        if (_proxy == null) return;
        _particles = Particles.GetComponent<ParticleSystem>();
        if (_particles != null)
        {
            _particles.Stop(true);
            
            var _repl = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EntityDatabase>(_proxy.Entity);
            if (Effects.Instance.Get(_repl.db, out BinaryEffect effect))
            {
                StartCoroutine(Delay(effect));
            }
        }
    }

	IEnumerator Delay(BinaryEffect effect)
	{
		yield return new WaitForSeconds((float)effect.delay / 1000);
		_particles.Play(true);
        yield return new WaitForSeconds(0.3f);
    }

}

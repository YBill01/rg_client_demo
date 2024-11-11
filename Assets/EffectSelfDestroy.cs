using UnityEngine;

public class EffectSelfDestroy : MonoBehaviour
{
	private bool inited;
	private ParticleSystem[] particleSystems;
	void Start()
	{
		particleSystems = GetComponents<ParticleSystem>();
	}
	void Update()
	{
		int effectsLeft = particleSystems.Length;
		
		for (int k = 0; k < particleSystems.Length; ++k)
		{
			var s = particleSystems[k];
			if (!inited)
			{
				if (s.particleCount > 0)
				{
					inited = true;
					return;
				}
				continue;
			}
			if (s.particleCount == 0)
			{
				DestroyImmediate(gameObject);
				effectsLeft--;
			}
		}
		if (effectsLeft == 0)
		{
			DestroyImmediate(gameObject);
		}
	}
}

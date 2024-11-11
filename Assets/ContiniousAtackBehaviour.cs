using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContiniousAtackBehaviour : MonoBehaviour
{
	public string drainPrefabName;
	public GameObject orbGameObject;
	void Start()
	{
	}

	void Update()
	{
		if (!_started) return;

		UpdateDrainPosition();
	}

	private void UpdateDrainPosition()
	{
		currentDrainObject.transform.localPosition = _target.localPosition + Vector3.up;
		currentDrainObject.transform.LookAt(transform.localPosition + Vector3.up);
		var dist = Vector3.Distance(transform.localPosition, _target.localPosition);
		SetupDrainLine(dist);
	}

	private struct ParticleData
	{
		public float startLife;
		public int maxParticles;
	}

	private static List<ParticleData> listToSetup = new List<ParticleData>();
	private void SetupDrainLine(float distance)
	{
		SetupBasicParticleParams();

		var pList = currentDrainObject.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < pList.Length; i++)
		{
			var p = pList[i];
			var settings = listToSetup[i];
			var m = p.main;
			m.startLifetimeMultiplier = settings.startLife * distance;
			m.maxParticles = Mathf.Max((int)(settings.maxParticles * distance), 1);
		}

	}

	private void SetupBasicParticleParams()
	{
		if (listToSetup.Count != 0) return;
		var pList = currentDrainObject.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < pList.Length; i++)
		{
			var smain = pList[i].main;
			listToSetup.Add(new ParticleData
			{
				startLife = smain.startLifetimeMultiplier,
				maxParticles = smain.maxParticles
			});
		}
	}

	public void ResetTarget(Transform target)
	{
		if (target == _target) return;
		_target = target;
	}

	private Transform _target;
	private bool _started;
	private GameObject currentDrainObject;
	public void StartAttack(Transform target)
	{
		if (_started) return;
		_target = target;
		StartAttack();
	}

	public void StartAttack()
	{
		if (_started) return;
		_started = true;
		if(currentDrainObject != null)
		{
			LegacyHelpers.TurnParticlesOff(currentDrainObject);
		}
		currentDrainObject = ObjectPooler.instance.GetEffect(drainPrefabName);
		currentDrainObject.SetActive(true);
		LegacyHelpers.TurnParticlesOn(currentDrainObject);
		LegacyHelpers.TurnParticlesOn(orbGameObject);
	}

	public void StopAttack()
	{
		if (!_started) return;
		_started = false;
		LegacyHelpers.TurnParticlesOff(orbGameObject);
		LegacyHelpers.TurnParticlesOff(currentDrainObject);
	}
}

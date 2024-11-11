using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemControlBehaviour : MonoBehaviour
{
	public float minDieTime;
	public Transform followTransform;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		UpdateTransform();
		UpdateLife();
    }

	private void UpdateTransform()
	{
		if (followTransform == null) return;
		this.transform.localPosition = followTransform.localPosition + new Vector3(0, 0, -0.5f);
	}

	private float leftTime;
	private void OnEnable()
	{
		leftTime = minDieTime;
		var systems = GetComponentsInChildren<ParticleSystem>(true);
		foreach (var s in systems)
		{
			s.Play();
		}
	}

	private void UpdateLife()
	{
		leftTime -= Time.deltaTime;
		if (leftTime > 0) return;
		var systems = GetComponentsInChildren<ParticleSystem>(true);
		foreach(var s in systems)
		{
			s.Stop();
		}
	}
}


using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueSkill : MonoBehaviour
{
	[SerializeField]
	public float delay = 1000;
	[SerializeField]
	public float duration = 1000;
	//private Animator animator;
	[SerializeField]
	private GameObject fxc1;
	[SerializeField]
	private string wavePrefab;
    [SerializeField]
	private WaveProgressScript wps;
    void Start()
    {
		//animator = GetComponent<Animator>();
	}

    void Update()
    {
		if (!active) return;
		if (!CheckDelayDone()) return;
		if(wawing)
		{
			UpdateWave();
			return;
		}
		wawing = true;
		//animator.SetBool("Skill1", false);

		LegacyHelpers.TurnParticlesOff(fxc1);

		WaweTimeLeft = duration;
		wps.StartWave();

	}

	private bool wawing;
	private bool CheckDelayDone()
	{
		WaveDelayLeft -= Time.deltaTime;
		return WaveDelayLeft <= 0;
	}

	private void UpdateWave()
	{
		WaweTimeLeft -= Time.deltaTime;
		if (WaweTimeLeft <= 0)
		{
			WaweTimeLeft = 0;
			active = false;
			wawing = false;
			wps.StopWave();
		}
		wps.Progress = (duration - WaweTimeLeft) / duration;
	}

	private bool active;
	private float WaveDelayLeft = 0;
	private float WaweTimeLeft = 0;
	public void StartWawe()
	{
		if (active) return;
		active = true;
		WaveDelayLeft = delay;
		fxc1.SetActive(true);

		LegacyHelpers.TurnParticlesOn(fxc1);
	}

	public void StopWave()
	{
		active = false;
		WaweTimeLeft = 0;

		active = false;
		wawing = false;
		if (wps == null) return;
		wps.StopWave();
	}
}

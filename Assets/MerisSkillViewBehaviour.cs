using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerisSkillViewBehaviour : MonoBehaviour
{
	[SerializeField]
	private GameObject Skill1InitFX;
	[SerializeField]
	private ParticleSystem[] Skill2InitFX;
	[SerializeField]
	private GameObject strikeFX;
	[SerializeField]
	private GameObject underFX;
	public void Skill1TriggerInit()
	{
		FXON(Skill1InitFX);
	}

	public void Skill1TriggerStrike()
	{
		FXOFF(Skill1InitFX);
	}

	private void FXON(GameObject gameObject)
	{
		var list = gameObject.GetComponentsInChildren<ParticleSystem>();
		foreach (var v in list)
		{
			v.Play();
		}
	}

	private void FXOFF(GameObject gameObject)
	{
		var list = gameObject.GetComponentsInChildren<ParticleSystem>();
		foreach (var v in list)
		{
			v.Stop();
		}
	}

	public void PlaySecondSkillVFX()
    {
		foreach (var vfx in Skill2InitFX)
		{
			vfx.Play();
		}
	}
	public void StopSecondSkillVFX()
	{
		foreach (var vfx in Skill2InitFX)
		{
			vfx.Stop();
		}
	}
}

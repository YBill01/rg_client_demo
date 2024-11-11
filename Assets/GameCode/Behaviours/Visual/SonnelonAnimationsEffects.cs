using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonnelonAnimationsEffects : MonoBehaviour
{
	[SerializeField]
	private GameObject staffEnchantFX;
	[SerializeField]
	private GameObject strikeFX;
	[SerializeField]
	private GameObject underFX;
	[SerializeField]
	private GameObject underCoverFX;
	[SerializeField]
	private GameObject coverFinishFX;
	public void Skill2TriggerInit()
	{
		FXON(staffEnchantFX);
		FXON(underFX);
	}

	public void Skill2TriggerStrike()
	{
		FXOFF(staffEnchantFX);
		FXON(strikeFX);
		FXOFF(underFX);
	}

	public void Skill1TriggerInit()
	{
		FXON(underCoverFX);
	}

	public void Skill1TriggerStrike()
	{
		FXOFF(underCoverFX);
		FXON(coverFinishFX);
	}

	private void FXON(GameObject gameObject)
	{
		var list = gameObject.GetComponentsInChildren<ParticleSystem>();
		foreach(var v in list)
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
}

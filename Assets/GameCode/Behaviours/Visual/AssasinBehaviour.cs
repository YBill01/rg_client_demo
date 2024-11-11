using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssasinBehaviour : MonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer[] meshes;
	void Update()
	{
		if(revealing)
		{
			UpdateRevealProgress();
			return;
		}
		if(hiding)
		{
			UpdateHideProgress();
		}
	}

	private float switchStateDelay;
	public Material normalMaterial;
	public Material assasinMaterial;
	public GameObject HideFX;
	public GameObject ContinuousHideFX;
	public GameObject UnhideFX;
	public float transformTime;
	private float currentTime;
	private bool hiding;
	private bool revealing;
	public void Hide()
	{
		//if (!switchStateCheck()) return;
		if (hiding) return;
		//switchStateDelay = 0.1f;
		revealing = false;
		hiding = true;
		currentTime = 0;
		SetMaterial(assasinMaterial);
		SetAlpha(0);

		//LegacyHelpers.TurnParticlesOn(HideFX);
		//LegacyHelpers.TurnParticlesOn(ContinuousHideFX);
	}

	private bool switchStateCheck()
	{
		switchStateDelay -= Time.deltaTime;
		if (switchStateDelay > 0) return false;
		return true;
	}

	private void SetMaterial(Material material)
	{
		List<Material> materials = new List<Material>();
		foreach (var m in meshes)
			foreach(var mat in m.materials)
				materials.Add(mat);
		materials.All(x => x = material);
	}

	private void UpdateHideProgress()
	{
		if (currentTime >= transformTime)
		{
			return;
		}
		currentTime += Time.deltaTime;
		SetAlpha((currentTime / transformTime) * 0.5f);
	}

	private void UpdateRevealProgress()
	{
		if (currentTime >= transformTime)
		{
			revealing = false;
			hiding = false;
			return;
		}
		currentTime += Time.deltaTime;
		//SetAlpha((currentTime / transformTime));
	}

	private void SetAlpha(float alpha)
	{
		foreach (var m in meshes)
		{
			SetAlphaToMaterial(m.material, alpha);
			for (int i = 0; i < m.materials.Length; i++)
			{
				SetAlphaToMaterial(m.materials[i], alpha);
			}
		}
	}

	private void SetAlphaToMaterial(Material material, float alpha)
	{
		//var c = material.color;
		//var c = material.GetColor("_BaseColor");
		//c.a = alpha;
		//material.color = c;
		//material.SetColor("_BaseColor", c);
	}


	public void Unhide()
	{
		//if (!switchStateCheck()) return;
		if (revealing) return;
		if (!hiding) return;
		revealing = true;
		currentTime = 0;
		//switchStateDelay = 0.1f;
		SetMaterial(normalMaterial);
		//SetAlpha(0);

		//LegacyHelpers.TurnParticlesOff(ContinuousHideFX);
		//LegacyHelpers.TurnParticlesOn(UnhideFX);
	}

	private void OnDisable()
	{
		//LegacyHelpers.TurnParticlesOff(HideFX);
		//LegacyHelpers.TurnParticlesOff(ContinuousHideFX);
		//LegacyHelpers.TurnParticlesOff(UnhideFX);

		SetMaterial(normalMaterial);

		currentTime = 0;
		revealing = false;
		hiding = false;
	}
}

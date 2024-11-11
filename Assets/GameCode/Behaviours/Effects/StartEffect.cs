using System;
using System.Collections;
using UnityEngine;

public class StartEffect : MonoBehaviour
{
	public GameObject StartEffectObject;
	public Material WaitMaterial;
	private Material RegularMaterial;
	private SkinnedMeshRenderer[] renderers;

	void Awake()
	{
		SetupRenderers();
    }

    void Update()
	{
	}

	private void SetupRenderers()
	{
		if (renderers != null) return;
		renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
		if (renderers.Length == 0) return;
		RegularMaterial = renderers[0].material;
	}

	private bool _wait;

    public bool Flying { get; internal set; }

    public void Reset()
    {
        StartEffectObject.SetActive(false);
    }
    public void SetWait(bool Value)
	{

        SetupRenderers();
		if (renderers.Length == 0) return;
		if (_wait == Value) return;
		_wait = Value;

        if (_wait)
		{
			foreach (SkinnedMeshRenderer mr in renderers)
			{
				mr.material = WaitMaterial;
			}
		}
		else
		{
			foreach (SkinnedMeshRenderer mr in renderers)
			{
				mr.material = RegularMaterial;
			}
            GetComponent<UnderUnitCircle>().Circle.gameObject.SetActive(false);
            if (Flying)
            {
                StartEffectObject.transform.position = StartEffectObject.transform.position + new Vector3(0f, 2.5f, 0f);
            }
            StartEffectObject.SetActive(true);
			Debug.LogError($"<color=red>StartEffectObject</color>");
        }
	}

    internal void SetDefaults()
    {
        Reset();        
    }
}

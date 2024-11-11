using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BattleCanvasLoader : MonoBehaviour
{

	public AssetReference Instance;

	void Start()
    {
		Instance.InstantiateAsync(transform);

	}

	private void OnDestroy()
	{
		Instance.ReleaseAsset();
	}
}

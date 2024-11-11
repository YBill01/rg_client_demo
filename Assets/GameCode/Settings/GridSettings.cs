using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName="GameLegacy/GridSettings")]
public class GridSettings : ScriptableObject
{
	static GridSettings _instance = null;
	public static GridSettings Instance
	{
		get
		{
			if (!_instance)
				_instance = (GridSettings)Resources.Load("Settings/GridSettings");
			return _instance;
		}
	}

	public float OutRadius = 2.0f;
	public float InnerRadius = 1.72f;
	public GameObject Hex;
}

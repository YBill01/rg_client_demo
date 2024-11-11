using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaticCanvas : MonoBehaviour
{
	public static StaticCanvas instance;

	public GameObject Terrain;
	public GameObject AllyTerrain;
	public GameObject EnemyTerrain;
    [NonSerialized]
	public Bounds TerrainBounds;
	private BoxCollider TerrainCollider;

	public GameObject floorSprite;

	public GameObject FinishWindow;

	public Slider ManaSlider;

	[SerializeField]
	private Slider ManaAttachedSlider;

	[SerializeField]
	private Slider ManaAttachedSliderNeed;
	void Awake()
	{
		TerrainBounds = Terrain.GetComponent<BoxCollider>().bounds;
		instance = this;
	}

	public void SetAttached(byte cost) {
		ManaAttachedSlider.gameObject.SetActive(true);
		ManaAttachedSlider.value = cost;
	}

	public void UnsetAttached()
	{
		ManaAttachedSlider.gameObject.SetActive(false);
	}
	public void ToggleHighLight(bool all, bool _switch)
	{
		if (all)
		{
            Terrain.GetComponent<MeshRenderer>().enabled = _switch;
        }
		else
		{
            var renderers = AllyTerrain.GetComponentsInChildren<MeshRenderer>();
            for (var i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = _switch;
            }
        }
		
	}

	public bool IsCasted()
	{
		var m_Raycaster = GetComponent<GraphicRaycaster>();
		var m_EventSystem = GetComponent<EventSystem>();
		var m_PointerEventData = new PointerEventData(m_EventSystem);
		//Set the Pointer Event Position to that of the mouse position
		m_PointerEventData.position = Input.mousePosition;

		//Create a list of Raycast Results
		List<RaycastResult> results = new List<RaycastResult>();

		//Raycast using the Graphics Raycaster and mouse click position
		m_Raycaster.Raycast(m_PointerEventData, results);
		foreach(RaycastResult RR in results)
		{
			if (RR.gameObject.transform.IsChildOf(transform)) return true;
		}
		//return results.Count > 0;
		return false;
	}

	internal void ShowFinishWindow()
	{
		Instantiate(FinishWindow, transform);
	} 

}

using Unity.Entities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace Legacy.Client
{
	//[DisableAutoCreation]
	[AlwaysUpdateSystem]

	public class InputSystem : ComponentSystem
	{
		public static bool ButtonsClickable = true;
		public string UITag;
		public string UIName;

		protected override void OnCreate()
		{
		}

		protected override void OnDestroy()
		{
			GRC = null;
			GRCList.Clear();
			base.OnDestroy();
		}

		private TouchResult[] _hitResults;
		protected override void OnUpdate()
		{
			FindUI();
			if (GRC == null) return;

			UpdateTouch();
		}

		
		private GraphicRaycaster GRC;
		private List<GraphicRaycaster> GRCList = new List<GraphicRaycaster>();
		private void FindUI()
		{
			GameObject[] Interfaces = GameObject.FindGameObjectsWithTag("MainUITag");
			GRCList.Clear();
			foreach (GameObject GO in Interfaces)
			{
				GRC = GO.GetComponent<GraphicRaycaster>();
				if (GRC != null)
					GRCList.Add(GRC);
				//break;
			}

		}

		public TouchResult[] HitResults { get => _hitResults; }

		public void Clear()
		{
			//GRC = null;
			GRCList.Clear();
		}

		public struct TouchResult
		{
			public Vector3 Position3d;
			public Vector2 Position2d;
			public GameObject Target;
		}

		public static bool UITouched;
		private void UpdateTouch()
		{
			TouchResult[] ui = UITouch();
			UITouched = ui.Length > 0;
			TouchResult[] scene = SceneTouch();
			TouchResult[] final = new TouchResult[ui.Length + scene.Length];
			ui.CopyTo(final,0);
			scene.CopyTo(final, ui.Length);
			_hitResults = final;
		}

		private TouchResult[] UITouch()
		{
			PointerEventData ped = new PointerEventData(null);
			ped.position = Input.mousePosition;
			List<RaycastResult> pre_results = new List<RaycastResult>();
			List<RaycastResult> results = new List<RaycastResult>();
			foreach(var g in GRCList)
			{
				g.Raycast(ped, pre_results);
				results.AddRange(pre_results);
			}
			//GRC.Raycast(ped, results);
			
			TouchResult[] result = new TouchResult[results.Count];
			for (int i = 0; i < results.Count; i++)
			{
				RaycastResult RR = results[i];
				result[i] =
					new TouchResult
					{
						Position2d = RR.screenPosition,
						Position3d = Vector3.zero,
						Target = RR.gameObject
					};
			}
			return result;
		}

		private TouchResult[] SceneTouch()
		{
			Ray _input_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] _hits = Physics.RaycastAll(_input_ray);

			TouchResult[] result = new TouchResult[_hits.Length];
			for (int i = 0; i < _hits.Length; i++)
			{
				RaycastHit RH = _hits[i];
				result[i] =
					new TouchResult
					{
						Position2d = Vector2.zero,
						Position3d = RH.point,
						Target = RH.transform.gameObject
					};
			}
			return result;
		}
	}
}

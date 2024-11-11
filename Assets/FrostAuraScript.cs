using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
	[RequireComponent(typeof(MinionInitBehaviour))]
	public class FrostAuraScript : MonoBehaviour
	{
		[SerializeField]
		private GameObject fxGameObject;
		private MinionInitBehaviour initBehaviour;
		void Start()
		{
			initBehaviour = GetComponent<MinionInitBehaviour>();
			var _proxy = GetComponent<EntityProxyBehaviour>();
			//if (_proxy != null)
			//{
			//	if (ClientWorld.Instance.EntityManager.HasComponent<Legacy.Effects.EffectRadius>(_proxy.Entity))
			//	{
			//		var _component = ClientWorld.Instance.EntityManager.GetComponentData<Legacy.Effects.EffectRadius>(_proxy.Entity);
			//		radius = _component.radius;
			//		fxGameObject.transform.localScale *= radius;
			//	}
			//}
			radius = 3;
			//fxGameObject.transform.localScale *= radius;
		}

		private void OnEnterBattle()
		{
			Debug.Log("Golem Entered battle");
			initBehaviour.AtBattleEvent.RemoveListener(OnEnterBattle);
			initBehaviour.OutBattleEvent.AddListener(OnOutFromBattle);
			//StartFX();
			_active = true;
		}

		private void OnOutFromBattle()
		{
			Debug.Log("Golem Left battle");
			initBehaviour.AtBattleEvent.AddListener(OnEnterBattle);
			initBehaviour.OutBattleEvent.RemoveListener(OnOutFromBattle);
			//StopFX();
			_active = false;
		}

		private void OnEnable()
		{
			if (initBehaviour == null)
			{
				initBehaviour = GetComponent<MinionInitBehaviour>();
			}
			clearListeners();
			if (initBehaviour.atBattle)
			{
				OnEnterBattle();
				return;
			}
			initBehaviour.AtBattleEvent.AddListener(OnEnterBattle);
		}

		private void OnDisable()
		{
			clearListeners();
			_active = false;
			KillOwnEffects();
			return;
			var ps = fxGameObject.GetComponent<ParticleSystem>();
			fxGameObject.SetActive(false);
			if (ps == null) return;
			ps.Stop();
		}

		private void clearListeners()
		{
			initBehaviour.AtBattleEvent.RemoveListener(OnEnterBattle);
			initBehaviour.OutBattleEvent.RemoveListener(OnOutFromBattle);
		}

		private bool _active;
		private void StartFX()
		{
			fxGameObject.SetActive(true);
			var ps = fxGameObject.GetComponent<ParticleSystem>();
			if (ps == null) return;
			ps.Play();
		}

		private void StopFX()
		{
			var ps = fxGameObject.GetComponent<ParticleSystem>();
			if (ps == null) return;
			ps.Stop();
		}

		private void OnDestroy()
		{
			clearListeners();
			_active = false;
			KillOwnEffects();
			return;
			var ps = fxGameObject.GetComponent<ParticleSystem>();
			fxGameObject.SetActive(false);
			if (ps == null) return;
			ps.Stop();
		}

		private void KillOwnEffects()
		{
			foreach (var m in MinionInitBehaviour.MinionsList)
			{
				//if (m.gameObject == gameObject) continue;
				//if ((m.transform.localPosition - transform.localPosition).magnitude > radius)
				//{
				DoMinionStop(m.gameObject);
				//}
			}
		}

		private static List<MinionInitBehaviour> affectedList = new List<MinionInitBehaviour>();

		private void UpdateAffectedMinions()
		{
			UpdateActive();
		}

		private void UpdateActive()
		{
			foreach (var m in affectedList)
			{
				DoMinionPlay(m.gameObject);
			}
		}

		private void DoMinionStop(GameObject minionGO)
		{
			var a = minionGO.transform.Find("FrostAuraSmall");
			if (a == null) return;
			var ps = a.GetComponent<ParticleSystem>();
			if (ps != null)
			{
				if (ps.isPlaying)
					ps.Stop();
			}
			//if (a.gameObject.activeSelf)
			//	a.gameObject.SetActive(false);
		}

		private void DoMinionPlay(GameObject minionGO)
		{
			var a = minionGO.transform.Find("FrostAuraSmall");
			if (a == null) return;
			if (!a.gameObject.activeSelf)
				a.gameObject.SetActive(true);
			var ps = a.GetComponent<ParticleSystem>();
			if (ps == null) return;
			if (ps.isPlaying) return;
			ps.Play();
		}

		private float radius;
		private void UpdateMinionsList()
		{
			foreach (var m in MinionInitBehaviour.MinionsList)
			{
				if (m.gameObject == gameObject) continue;
				//Vector3 drawPos1 = gameObject.transform.localPosition + new Vector3(0, -2, 0);
				//Vector3 drawPos2 = gameObject.transform.localPosition + new Vector3(0, 2, 0);
				//var up = Vector3.up * 2;
				//var down = Vector3.down * 2;
				//Debug.DrawLine(drawPos1 + up, drawPos2 + up, Color.white, 1);
				//Debug.DrawLine(drawPos1 + up, drawPos1 + down, Color.white, 1);
				//Debug.DrawLine(drawPos2 + up, drawPos2 + down, Color.white, 1);
				if (!m.atBattle) continue;
				if (!m.gameObject.activeSelf) continue;
				if ((m.transform.localPosition - transform.localPosition).magnitude < radius)
				{
					if (!affectedList.Contains(m))
					{
						affectedList.Add(m);
					}
				}
			}
		}

		private static float UpdateTime = 0;
		public void Update()
		{
			//if (!_active) return;
			if (!initBehaviour.atBattle)
			{
				return;
			}
			UpdateMinionsList();
			if (UpdateTime < Time.time - 0.06f)
			{
				KillOwnEffects();
				UpdateAffectedMinions();
				affectedList.Clear();
				UpdateTime = Time.time;
			}
		}
	}
}
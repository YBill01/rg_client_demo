using UnityEngine;
using System.Collections;
using Unity.Entities;
using Legacy.Client;
using Legacy.Database;

public class PlayPauseButtonBehaviour : MonoBehaviour
{
	EntityQuery _query_battle_pause;

	private void Start()
	{
		_query_battle_pause = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<BattlePause>());
	}

	public void Play()
	{
		Debug.Log("Resume sandbox game");
		var _pause_entity = _query_battle_pause.GetSingletonEntity();
		ClientWorld.Instance.EntityManager.DestroyEntity(_pause_entity);
	}

	public void Pause()
	{
		Debug.Log("Pause sandbox game");
		var _entity = ClientWorld.Instance.EntityManager.CreateEntity();
		ClientWorld.Instance.EntityManager.AddComponent<BattlePause>(_entity);
	}
}

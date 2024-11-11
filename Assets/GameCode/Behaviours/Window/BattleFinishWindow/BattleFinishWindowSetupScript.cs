using Legacy.Client;
using Legacy.Network;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class BattleFinishWindowSetupScript : MonoBehaviour
{
	public byte Stars1;
	public byte Stars2;
	public byte winnerSide;
	[SerializeField]
	private Image side1image;
	[SerializeField]
	private Image side2image;
	[SerializeField]
	private TextMeshProUGUI side1name;
	[SerializeField]
	private TextMeshProUGUI side2name;
	[SerializeField]
	private GameObject lootboxContainer;
	[SerializeField]
	private Image lootboxImage;

	[System.Serializable]
	public class KeyValue
	{
		public int key;
		public int value;
	}
	public KeyValue[] rewards = new KeyValue[] { };
	[SerializeField]
	private Animator animator;
	void Start()
	{
		animator.SetBool("Win", winnerSide == 0);
		animator.SetBool("Loose", winnerSide != 0);
	}

	private void OnEnable()
	{
		animator.SetBool("Win", winnerSide == 0);
		animator.SetBool("Loose", winnerSide != 0);
	}

	public void PreviewPhaseDone()
	{
		ShowStars();
	}

	private void ShowStars()
	{
		var players = GetComponentsInChildren<PlayerHeroWinItem>(true);
		players[0].stars = Stars1;
		players[1].stars = Stars2;
		players[winnerSide].winner = true;

		players[0].ResetView();
		players[1].ResetView();
	}

	private void ShowLootbox()
	{
		lootboxContainer.SetActive(true);
	}

	private void ShowRewards()
	{
		var rewItems = GetComponentsInChildren<SimpleRewardItem>(true);
		for (int i = 0; i < rewards.Length; i++)
		{
			rewItems[i].ResetData(rewards[i].key, rewards[i].value);
			rewItems[i].gameObject.SetActive(true);
		}
	}

	private StateMachineSystem stateMachineSystem;
	public void GoHome()
	{

		/*var _query_connection = World.Active.EntityManager.CreateEntityQuery(
				ComponentType.ReadOnly<PlayerGameClient>(),
				ComponentType.ReadOnly<GameConnectRequest>(),
				ComponentType.ReadOnly<NetworkAuthorization>()
			);
		var connection = _query_connection.GetSingletonEntity();

		World.Active.EntityManager.AddComponentData(connection, default(PlayerGameDisconnect));*/
		FaderCanvas.Instance.CompleteStateEvent.AddListener(OnFaderDone);
		FaderCanvas.Instance.Hide = true;
	}

	void OnDestroy()
	{
		FaderCanvas.Instance.CompleteStateEvent.RemoveListener(OnFaderDone);
		if (stateMachineSystem == null) return;
		stateMachineSystem.HomeLoaded.RemoveListener(HomeLoaded);
	}

	private void OnFaderDone()
	{
		FaderCanvas.Instance.CompleteStateEvent.RemoveListener(OnFaderDone);
		if (stateMachineSystem != null) return;
		stateMachineSystem = ClientWorld.Instance.GetExistingSystem<StateMachineSystem>();
		stateMachineSystem.ForceExitBattle();
		stateMachineSystem.HomeLoaded.AddListener(HomeLoaded);
	}

	private void HomeLoaded()
	{
		stateMachineSystem.HomeLoaded.RemoveListener(HomeLoaded);
		FaderCanvas.Instance.Hide = false;
	}
}

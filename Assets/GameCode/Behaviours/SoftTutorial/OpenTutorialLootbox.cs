using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// Показываем игроку открой сундук
	/// </summary>
	class OpenTutorialLootbox : SoftTutorialBehaviour
	{
		[SerializeField]
		RectTransform LootboxContainer;

		public override int Priority => (int)MainWindowPriority.OpenTutorialLootbox;

		public override ushort TutorialState => (ushort)SoftTutorial.SoftTutorialState.OpenFirstLootbox;

		public override bool CanBeRestarted => true;

		private int LootboxToOpen;
		private bool FirstTutorLootbox = false;

		public override bool CanStartTutorial()
		{
			if (SoftTutorialManager.Instance.ClickedOnBattleWith6Cards)
			{
				if (FindTutorialLootboxFromBattle(3))
					return true;
			}

			if (profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.UpgradeCard))
				return false;

			if (FindTutorialLootboxFromBattle(1))
			{
				FirstTutorLootbox = true;
				return true;
			}

			if (FindTutorialLootboxFromBattle(2))
				return true;

			if (!SoftTutorialManager.Instance.ClickedOnBattleWith6Cards)
			{
				if (FindTutorialLootboxFromBattle(3))
					return true;
			}

			return false;
		}

		private bool FindTutorialLootboxFromBattle(ushort tutorialBattleNumber)
		{
			var lootboxIndex = GetTutorialLootboxIndex(tutorialBattleNumber);

			for (int i = 0; i < 4; ++i)
			{
				var lootbox = profile.loot.boxes[i];
				if (lootbox.index == lootboxIndex && lootbox.started)
				{
					LootboxToOpen = i;
					return true;
				}
			}

			return false;
		}

		public override void StartTutorial()
		{
			StartCoroutine(DelayedStart());
		}

		private IEnumerator DelayedStart()
		{
			//Первый сундук - особый случай он не знает что открыт :(
			if (!FirstTutorLootbox)
				yield return new WaitWhile(() => !profile.loot.boxes[LootboxToOpen].isOpenedForUI);

			TutorialLogic();
		}

		private void TutorialLogic()
		{
			var target = LootboxContainer.GetChild(LootboxToOpen).GetChild(0).GetComponent<RectTransform>();
			var button = LootboxContainer.GetChild(LootboxToOpen).GetComponent<LegacyButton>();

			SoftTutorialManager.Instance.MenuTutorialPointer.PointerToRect(target, button, OnOpen);
		}

		private void OnOpen()
		{
			FirstTutorLootbox = false;
			StopTutorial();
			SoftTutorialManager.Instance.OnTutotorComplite(this);
		}

		public static ushort GetTutorialLootboxIndex(ushort tutorialBattleNumber)
		{
			if (!Tutorial.Instance.Get(tutorialBattleNumber, out BinaryTutorial tutorial))
				throw new Exception($"Недоступен туториальный бой. Возможно он теперь имеет индекс не {tutorialBattleNumber}. Наверное появился механизм порядка туториальныйх боев и его стоит учесть");

			if (!Missions.Instance.Get(tutorial.mission, out BinaryMission mission))
				throw new Exception("Mission not found. Index " + tutorial.mission);

			if (!Rewards.Instance.Get(mission.reward, out BinaryReward reward))
				throw new Exception("Reward not found. Index " + mission.reward);

			return reward.lootbox;
		}
	}
}


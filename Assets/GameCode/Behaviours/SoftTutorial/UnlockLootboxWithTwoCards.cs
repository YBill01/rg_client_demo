﻿using Legacy.Database;
using System;
using System.Collections;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// Показываем игроку разблокируй сундук в котором две карты
	/// </summary>
	class UnlockLootboxWithTwoCards : SoftTutorialBehaviour
	{
		[SerializeField]
		RectTransform LootboxContainer;

		public override int Priority => (int)MainWindowPriority.UnlockLootboxFor8Cards;

		private int LootboxToOpen;

		public override bool CanStartTutorial()
		{
			if (!profile.IsBattleTutorial)
				return false;

			if (!SoftTutorialManager.Instance.ClickedOnBattleWith6Cards)
				return false;

			if (profile.DecksCollection.In_deck.Length == 8)
				return false;

			var lootboxIndex = GetTutorialLootboxIndex(3);

			for (int i = 0; i < 4; ++i)
			{
				var lootbox = profile.loot.boxes[i];
				if (lootbox.index == lootboxIndex && !lootbox.started)
				{
					LootboxToOpen = i;
					return true;
				}
			}

			return false;
		}

		public override void StartTutorial()
		{
			var lootbox = profile.loot.boxes[LootboxToOpen];

			if (lootbox.arrived)
				TutorialLogic();
			else
				StartCoroutine(DelayedStart());
		}

		private IEnumerator DelayedStart()
		{
			//Даем лутбоксу время на появление arrive
			yield return new WaitForSeconds(0.7f);

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


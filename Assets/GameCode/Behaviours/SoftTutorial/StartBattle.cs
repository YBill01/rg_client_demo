using Legacy.Database;
using System;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// До первого рейтингового боя, если нет дургих туториалов - показываем - иди в бой
	/// </summary>
	class StartBattle : SoftTutorialBehaviour
	{
		[SerializeField]
		RectTransform StartBattleButton;

		public override int Priority => (int)MainWindowPriority.StartBattle;

		public override bool CanStartTutorial()
		{
			if (profile.IsBattleTutorial)
				return true;

			if (profile.battleStatistic.battles == 0)
				return true;

			return false;
		}

		public override void StartTutorial()
		{
			var button = StartBattleButton.GetComponent<LegacyButton>();

			SoftTutorialManager.Instance.MenuTutorialPointer.PointerToRect(StartBattleButton, button);
		}
	}
}


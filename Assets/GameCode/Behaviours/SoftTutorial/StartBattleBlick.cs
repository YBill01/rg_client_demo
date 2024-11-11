using Legacy.Database;
using System;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// До первых трех рейтинговых боев, если нет дургих туториалов - показываем - иди в бой
	/// </summary>
	class StartBattleBlick : SoftTutorialBehaviour
	{
		[SerializeField]
		LegacyButton StartBattleButton;

		public override int Priority => (int)MainWindowPriority.StartBattle + 1;

		public override bool CanStartTutorial()
		{
			if (profile.IsBattleTutorial)
				return false;

			if (profile.battleStatistic.battles > 3 || profile.battleStatistic.battles == 0)
				return false;

			return true;
		}

		public override void StartTutorial()
		{
			StartBattleButton.EnableBlick();
		}
	}
}


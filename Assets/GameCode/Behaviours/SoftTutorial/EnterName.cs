using Legacy.Database;
using System;
using System.Collections;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// Показываем игроку как открывается сундук
	/// </summary>
	class EnterName : SoftTutorialBehaviour
	{
		public override int Priority => (int)MainWindowPriority.EnterName;

		public override bool CanStartTutorial()
		{
			if (profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.EnterName))
				return false;

			if (!profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.OpenArena))
				return false;

			if (profile.IsBattleTutorial)
				return false;

			return true;
		}

		public override void StartTutorial()
		{
			WindowManager.Instance.MainWindow.ChangeName();
			SoftTutorialManager.Instance.MenuTutorialPointer.ReleasePointer();
			SoftTutorialManager.Instance.MenuTutorialPointer.UnhidePointer();
		}
	}
}

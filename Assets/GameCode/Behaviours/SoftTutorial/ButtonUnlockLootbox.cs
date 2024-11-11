using Legacy.Database;
using System;
using System.Collections;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// Показываем игроку как ставить сундук на открытие
	/// </summary>
	class ButtonUnlockLootbox : SoftTutorialBehaviour
	{
		[SerializeField]
		RectTransform UnlockButton;

		public override ushort TutorialState => (ushort)SoftTutorial.SoftTutorialState.UnlockLootbox;

		public override int Priority => 0;

		public override bool CanStartTutorial()
		{
			if (profile.HasSoftTutorialState(TutorialState))
				return false;

			return true;
		}

		public override void StartTutorial()
		{
			var target = UnlockButton.GetComponent<RectTransform>();
			var button = UnlockButton.GetComponent<LegacyButton>();

			SoftTutorialManager.Instance.MenuTutorialPointer.PointerToRect(target, button, OnOpen);
		}

		private void OnOpen()
		{
			StopTutorial();
			SoftTutorialManager.Instance.OnTutotorComplite(this);
		}
	}
}

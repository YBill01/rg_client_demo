using Legacy.Database;
using System;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// Показываем игроку на кнопку улучшить карту
	/// </summary>
	class UpgradeCard : SoftTutorialBehaviour
	{
		[SerializeField]
		RectTransform UpgradeButton;

		// Нам не нужно отмечать выполнение этого тутора.
		// Это сделает кнопка улучшить карту, так как игрок может улучшить карту, до того как этот туториал успеет отработать
		// По факту оно оказалось в методе CardUpgradeWindowBehavior.SelfOpen
		//public override ushort TutorialState => (ushort)SoftTutorial.SoftTutorialState.UpgradeCard;

		public override int Priority => 0;

		public override bool CanStartTutorial()
		{
			if (profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.UpgradeCard))
				return false;

			var cardData = (WindowManager.Instance.CurrentWindow as CardWindowBehaviour).ClickedCard.GetPlayerCard();
			if (!cardData.CanUpgrade || cardData.SoftToUpgrade > profile.Stock.GetCount(Legacy.Database.CurrencyType.Soft))
				return false;

			return true;
		}

		public override void StartTutorial()
		{
			var button = UpgradeButton.GetComponent<LegacyButton>();

			SoftTutorialManager.Instance.MenuTutorialPointer.PointerToRect(UpgradeButton, button, OnClick);
		}

		private void OnClick()
		{
			SoftTutorialManager.Instance.OnTutotorComplite(this);
		}
	}
}



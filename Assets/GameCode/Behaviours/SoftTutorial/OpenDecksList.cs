using Legacy.Database;
using System;
using UnityEngine;

namespace Legacy.Client
{
	/// <summary>
	/// Если у игрока есть карта для обновления - предлагаем перейти в окно колод
	/// </summary>
	class OpenDecksList : SoftTutorialBehaviour
	{
		[SerializeField]
		RectTransform DecksButton;

		//public override ushort TutorialState => (ushort)SoftTutorial.SoftTutorialState.UpgradeCard;

		public override int Priority => (int)MainWindowPriority.UpgradeCard;

		public override bool CanStartTutorial()
		{
			if (profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.UpgradeCard))
				return false;

			if (!HasCardsForUpgrade(profile))
				return false;

			return true;
		}

		public override void StartTutorial()
		{
			var button = DecksButton.GetComponent<LegacyButton>();

			SoftTutorialManager.Instance.MenuTutorialPointer.PointerToRect(DecksButton, button);
		}

		public static bool HasCardsForUpgrade(ProfileInstance profile)
		{
			var cards = profile.DecksCollection.ActiveSet.Cards;
			foreach (var card in cards)
			{
				var cardData = profile.Inventory.GetCardData(card);
				if (cardData.CanUpgrade && cardData.SoftToUpgrade <= profile.Stock.GetCount(Legacy.Database.CurrencyType.Soft))
				{
					return true;
				}
			}
			return false;
		}
	}
}



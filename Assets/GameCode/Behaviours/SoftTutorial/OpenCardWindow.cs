using Legacy.Database;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Legacy.Client
{
	/// <summary>
	/// В окне колоды, если у игрока есть карта для обновления предлагаем открыть окно карты для ее улучшения
	/// </summary>
	class OpenCardWindow : SoftTutorialBehaviour
	{
		[SerializeField]
		RectTransform CardsContainer;

		[SerializeField]
		EventTrigger Fader;

		/// <summary>
		/// Кнопка Улучшить/Информация для карты. Так как туториал "Нажми улучшить карту" может начаться позже чем игрок закроет окно Карты
		/// Мы останавливаем туториал как только нажали на эту кнопку. И сохраняем на нее ссылку - что бы отписаться от событий
		/// </summary>
		private LegacyButton upgradeButton;
		private EventTrigger.Entry faderEventEntry;

		private bool PointingOnCard = false; // Палец показывает на карту, а не на кнопку Улучшить

		//public override ushort TutorialState => (ushort)SoftTutorial.SoftTutorialState.UpgradeCard;

		public override int Priority => 1;

		public override bool CanStartTutorial()
		{
			if (profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.UpgradeCard))
				return false;

			if (!OpenDecksList.HasCardsForUpgrade(profile))
				return false;

			return true;
		}

		public override void StartTutorial()
		{
			PointerToCardToUpgrade();

			PointingOnCard = true;

			//Подписываем метод OnAnyCardClick на нажатие любой из карт
			//Ведь игрок может нажать на другую карту, которую тоже можно улучшить
			Subscribe(true);

			faderEventEntry = Fader.triggers.First();
			faderEventEntry.callback.AddListener(OnFaderClick);
		}

		public override void StopTutorial()
		{
			faderEventEntry.callback.RemoveListener(OnFaderClick);
			SoftTutorialManager.Instance.MenuTutorialPointer.ReleasePointer();
		}

		private void PointerToCardToUpgrade()
		{
			var cardRect = GetCardToUpgrade();
			var button = cardRect.GetComponent<LegacyButton>();

			SoftTutorialManager.Instance.MenuTutorialPointer.PointerToRect(cardRect, button);
		}

		private void OnFaderClick(BaseEventData eventData)
		{
			Debug.Log("OnFaderClick");

			//Похоже перед тем как нажать на фейдер, игрок надал на карту которую нельзя улучшить, и палец до сих пор показывает куда нужно
			//Так что нам ничего не нужно делать
			if (PointingOnCard)
			{
				upgradeButton.onClick.RemoveListener(OnOpenCardWindow);
				upgradeButton = null;
				return;
			}

			PointerToCardToUpgrade();
			Subscribe(true);
		}

		private void Subscribe(bool enabled)
		{
			foreach (RectTransform card in CardsContainer)
			{
				var button = card.GetComponent<LegacyButton>();

				if (enabled)
					button.onClick.AddListener(OnAnyCardClick);
				else
					button.onClick.RemoveListener(OnAnyCardClick);
			}
		}

		private void OnAnyCardClick()
		{
			var card = (WindowManager.Instance.CurrentWindow as DecksWindowBehaviour).ClickedCard;
			var cardData = (WindowManager.Instance.CurrentWindow as DecksWindowBehaviour).ClickedCard.GetPlayerCard();

			Debug.Log("OnAnyCardClick " + card.binaryCard.title);

			var upgradeRect = card.UpgradeButton.GetComponent<RectTransform>();
			upgradeButton = upgradeRect.GetComponent<LegacyButton>();

			//Нажали на карту которую нельзя улучшить. Палец продолжает показывать на карту которую можно улучшить 
			if (!cardData.CanUpgrade || cardData.SoftToUpgrade > profile.Stock.GetCount(Legacy.Database.CurrencyType.Soft))
			{
				upgradeButton = cardData.CanUpgrade ? upgradeButton : card.InfoButton.GetComponent<LegacyButton>();
				upgradeButton.onClick.AddListener(OnOpenCardWindow);
				return;
			}

			PointingOnCard = false;
			Subscribe(false);

			SoftTutorialManager.Instance.MenuTutorialPointer.PointerToRect(upgradeRect, upgradeButton, OnUpgradeClick);//По сути на одну кнопку подписываем два метода
			upgradeButton.onClick.AddListener(OnOpenCardWindow);//По сути на одну кнопку подписываем два метода
		}

		private void OnOpenCardWindow()
		{
			StopTutorial();
			SoftTutorialManager.Instance.TutorialSelfStoped(this);
			upgradeButton.onClick.RemoveListener(OnOpenCardWindow);
		}

		private void OnUpgradeClick()
		{
			PointingOnCard = false;
		}

		private RectTransform GetCardToUpgrade()
		{
			var profile = ClientWorld.Instance.Profile;
			var cards = profile.DecksCollection.ActiveSet.Cards;
			ushort cardToUpgrade = 0;

			foreach (var card in cards)
			{
				var cardData = profile.Inventory.GetCardData(card);

				if (!cardData.CanUpgrade || cardData.SoftToUpgrade > profile.Stock.GetCount(Legacy.Database.CurrencyType.Soft))
					continue;

				cardToUpgrade = card;
				break;
			}

			if (cardToUpgrade == 0)
				throw new Exception("For some reason, we did not find a card for upgrade, although we checked that it exists");

			foreach (RectTransform card in CardsContainer)
			{
				var behaviour = card.GetComponent<DeckCardBehaviour>();
				if (behaviour == null)
					continue;

				if (behaviour.binaryCard.index == cardToUpgrade)
				{
					return card;
					//UpgradeCardButtonPoint = behaviour.UpgradeButton.GetComponent<RectTransform>();
				}
			}

			throw new Exception("For some reason, we did not find a card rect with index as card for upgrade");
		}
	}
}



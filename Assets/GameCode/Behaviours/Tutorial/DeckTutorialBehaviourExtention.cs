using UnityEngine;
using Legacy.Client;
using Legacy.Database;
using Unity.Entities;
using System;
/*
public class DeckTutorialBehaviourExtention : BaseMenuTutorialExtention
{
	[SerializeField]
	private RectTransform CardsContainer;

	[SerializeField]
	private RectTransform CloseCardPoint;

	[SerializeField]
	private RectTransform UpgradeCardPoint;

	[SerializeField]
	private RectTransform UpgradeCardButtonPoint;

	[SerializeField]
	private MenuTutorialPointerBehaviour MenuTutorialPointer;

	private RectTransform CardToUpgradePoint;
	private ushort cardToUpgrade;

	public override bool ProcessMessage(string message, ref RectTransform buttonForPointer)
	{
		if (message == "OpenCard")
		{
			buttonForPointer = CardToUpgradePoint;
		} 
		else if (message == "UpgradeCardButton")
		{
			buttonForPointer = UpgradeCardButtonPoint;
		}
		else if (message == "UpgradeCard")
		{
			buttonForPointer = UpgradeCardPoint;
		}
		else if (message == "CloseCard")
		{
			buttonForPointer = CloseCardPoint;
		}

		MenuTutorialPointer.SetClickBlockerEnabled(true);

		return false;
	}

	protected void OnEnable()
	{
		var profile = ClientWorld.Instance.Profile;
		if ((profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.Deck) != 0)
			return;

		var cards = profile.DecksCollection.ActiveSet.Cards;

		foreach (var card in cards)
		{
			var cardData = profile.Inventory.GetCardData(card);

			if (!cardData.CanUpgrade)
				continue;

			cardToUpgrade = card;
			break;
		}

		if (cardToUpgrade == 0)
			return;

		StopOtherTutorials();

		FoundCardToUpgradePoint();

		var tutorial = new MenuTutorialInstance
		{
			currentTrigger = 0,
			softTutorialState = SoftTutorial.SoftTutorialState.Deck,
			_timer_start = int.MaxValue
		};

		var entity = ClientWorld.Instance.EntityManager.CreateEntity();
		ClientWorld.Instance.EntityManager.AddComponentData(entity, tutorial);

		MenuTutorialPointer.Init(this);
	}

	private void FoundCardToUpgradePoint()
	{
		foreach (RectTransform card in CardsContainer)
		{
			var behaviour = card.GetComponent<DeckCardBehaviour>();
			if (behaviour == null)
				continue;

			if (behaviour.binaryCard.index == cardToUpgrade)
			{
				CardToUpgradePoint = card;
				UpgradeCardButtonPoint = behaviour.UpgradeButton.GetComponent<RectTransform>();
			}
		}
	}
}
*/
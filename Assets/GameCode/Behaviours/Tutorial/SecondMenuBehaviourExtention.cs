/*using UnityEngine;
using Legacy.Client;
using Legacy.Database;
using Unity.Entities;

public class SecondMenuBehaviourExtention : BaseMenuTutorialExtention
{
	[SerializeField]
	private MenuTutorialPointerBehaviour MenuTutorialPointer;

	[SerializeField]
	private RectTransform UpgradeCardPoint; // Кнопка в окне карты

	[SerializeField]
	RectTransform OpenLootboxPoint;

	[SerializeField]
	private RectTransform CardsContainer;
	[SerializeField]
	private RectTransform BackButton;
	private RectTransform UpgradeCardButtonPoint;
	
	private RectTransform CardToUpgradePoint;
	[SerializeField] private RectTransform DescButton;
	[SerializeField] private RectTransform BattleButton;
	[SerializeField] private RectTransform CloseCardWindowButton;
	private ushort cardToUpgrade;
	private DecksWindowBehaviour deckWin;
	public override bool ProcessMessage(string message, ref RectTransform buttonForPointer)
	{
		if (message == "CloseCardFlip")
        {
			GetComponent<MenuTutorialPointerBehaviour>().SetFlipHandVariant1();
			buttonForPointer = BackButton;
			buttonForPointer.GetComponentInChildren<BlickControl>().Enable();
		}else
        if (message == "DeckButton" && ClientWorld.Instance.Profile.HardTutorialState==2)//тутор 2 
        {
			HomeTutorialHelper.Instance.HardHomeTutorStep = 22; //1 бой 1 шаг.
		}
		if (message == "CloseCardWindow")
        {
			if(deckWin is DecksWindowBehaviour)
					deckWin.isFaderClick = true;
			WindowManager.Instance.ShowBack(true);
			buttonForPointer = CloseCardWindowButton; 
			CloseCardWindowButton.GetComponent<ButtonHideAnimation>().Enable(true);
			//buttonForPointer = CloseCardWindowButton;
			//buttonForPointer = null;
			//CloseCardWindowButton.GetComponentInChildren<BlickControl>().Enable();
			//buttonForPointer = null;
			//	return true;
		}
		else
		if (message == "OpenNewCard" )
        {
			WindowManager.Instance.ShowBack(false);
			FoundNewCardPoint();
			buttonForPointer = CardToUpgradePoint;
			if (WindowManager.Instance.CurrentWindow is DecksWindowBehaviour)
			{
				var window = WindowManager.Instance.CurrentWindow as DecksWindowBehaviour;
				window.CardTutorUpdate = cardToUpgrade;
			}
		}
		else
		if (message == "OpenCard")
		{
			WindowManager.Instance.ShowBack(false);
			FoundCardToUpgradePoint();
			buttonForPointer = CardToUpgradePoint;
			if(WindowManager.Instance.CurrentWindow is DecksWindowBehaviour)
            {
				var window = WindowManager.Instance.CurrentWindow as DecksWindowBehaviour;
				window.CardTutorUpdate=cardToUpgrade;
			}
		}
		else

		if (message == "OpenLootbox")
		{
			buttonForPointer = OpenLootboxPoint;
		}
		else if (message == "InfoCardButton")
		{
			if(WindowManager.Instance.CurrentWindow is DecksWindowBehaviour)
            {
				deckWin = WindowManager.Instance.CurrentWindow as DecksWindowBehaviour;
				deckWin.isFaderClick = false;
			}
			buttonForPointer = UpgradeCardButtonPoint;
		}	else if (message == "UpgradeCardButton")
		{
			buttonForPointer = UpgradeCardButtonPoint;
			MenuTutorialPointer.popupMessage.ShowTextAtBottomFrom(Locales.Get("locale:1180"), UpgradeCardButtonPoint);
		}
		else if (message == "PopupMessageUpCard1") // Desc2
		{
			buttonForPointer = DescButton;
			MenuTutorialPointer.popupMessage.ShowTextAtLeftFrom(Locales.Get("locale:2119"), DescButton);
		}
		else if (message == "PopupMessageUpCard") // 
		{
			buttonForPointer = DescButton;
			MenuTutorialPointer.popupMessage.ShowTextAtLeftFrom(Locales.Get("locale:2122"), DescButton);
		}
		else if (message == "PopupMessageNewCardHide") // 
		{
		//	buttonForPointer = DescButton;
			MenuTutorialPointer.popupMessage.Hide();
		}
		
		else if (message == "PopupMessageNewCard") //1 tutor - new Card
		{
			buttonForPointer = BackButton;
			MenuTutorialPointer.popupMessage.ShowTextAtLeftFrom(Locales.Get("locale:2119"), DescButton);
		}
		else if (message == "UpgradeCard")
		{
			buttonForPointer = UpgradeCardPoint;
		}
		/ *	if (message == "OpenLootbox")
			{
				buttonForPointer = OpenLootboxPoint;
			}* /
		else if (message == "StartBattle2")
		{
			//buttonForPointer.GetComponentInChildren<BlickControl>().Enable();
			buttonForPointer = BattleButton;
		}
		else if (message == "StartBattleMessage")
        {
			MenuTutorialPointer.popupMessage.ShowTextAtLeftFrom(Locales.Get("locale:1162"), BattleButton);
		}
		else if (message == "StartBattleMessage2")
		{
			MenuTutorialPointer.popupMessage.ShowTextAtLeftFrom(Locales.Get("locale:2128"), BattleButton);
		}
		else if (message == "StartBattle")
		{
			buttonForPointer.GetComponentInChildren<BlickControl>().Enable();
			buttonForPointer = null;
			return true;
		}
		else if (message == "CloseCard")
		{
			buttonForPointer.GetComponentInChildren<BlickControl>().Enable();
			buttonForPointer = BackButton;
			return true;
		}
		else if (message == "StopBattleBlick")
		{
			buttonForPointer = BackButton;
			buttonForPointer.GetComponentInChildren<BlickControl>().Disable();
			buttonForPointer = null;
			return true;
		}

		return false;
	}

	private void OnEnable()
	{
	}

	void Start()
	{
		MenuTriggerSystem.MenuTutorialFinishEvent.AddListener(OnOtherTutorialFinish);
	}

	private void OnDestroy()
	{
		MenuTriggerSystem.MenuTutorialFinishEvent.RemoveListener(OnOtherTutorialFinish);
	}

	private void OnOtherTutorialFinish(SoftTutorial.SoftTutorialState state)
	{
	}

	

	private byte GetCurrentTriger(ProfileInstance profile)
	{
		byte result = 0;
		PlayerProfileLootBox box;
		if (profile.loot.boxes[0].index > 0)
			box = profile.loot.boxes[0];
		else
			box = profile.loot.boxes[1];

		if (box.started)
			result = 3;

		return result;
	}

	private void FoundNewCardPoint()
    {
		var profile = ClientWorld.Instance.Profile;
		var cards = profile.DecksCollection.ActiveSet.Cards;

		foreach (var card in cards)
		{
			var cardData = profile.Inventory.GetCardData(card);
			if (cardData.isNew)
			{
				cardToUpgrade = card;
				break;
			}
		}
		foreach (RectTransform card in CardsContainer)
		{
			var behaviour = card.GetComponent<DeckCardBehaviour>();
			if (behaviour == null)
				continue;

			if (behaviour.binaryCard.index == cardToUpgrade)
			{
				CardToUpgradePoint = card;
				UpgradeCardButtonPoint = behaviour.InfoButton.GetComponent<RectTransform>();
			}
		}
	}
	private void FoundCardToUpgradePoint()
	{
		var profile = ClientWorld.Instance.Profile;
		var cards = profile.DecksCollection.ActiveSet.Cards;

		foreach (var card in cards)
		{
			var cardData = profile.Inventory.GetCardData(card);

			if (!cardData.CanUpgrade)
				continue;

			cardToUpgrade = card;
			break;
		}

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
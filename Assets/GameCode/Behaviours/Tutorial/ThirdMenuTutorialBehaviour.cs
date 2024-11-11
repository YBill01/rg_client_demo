/*using UnityEngine;
using Legacy.Client;
using Legacy.Database;
using Unity.Entities;


// У нас два туторила после третьего боя. Один на случай если игрок прокачал до третьего боя карту и второй если нет
// Этот работает когда игрок Не прокачал карту. Определяется по уровню игрока
//AfterBattle3_1
public class ThirdMenuTutorialBehaviour : BaseMenuTutorialExtention
{
	[SerializeField]
	private MenuTutorialPointerBehaviour MenuTutorialPointer;

	void TryStartTutorial()
	{
		//var em = ClientWorld.Instance.EntityManager;
		//var queryTutorial = em.CreateEntityQuery(ComponentType.ReadOnly<MenuTutorialInstance>());
		//if (!queryTutorial.IsEmptyIgnoreFilter)
		//	return;

		//var profile = ClientWorld.Instance.Profile;
		//var openTutorial = false;
		//MenuTutorialInstance tutorial = new MenuTutorialInstance { currentTrigger = 0, _timer_start = int.MaxValue }; ;

		//if (profile.Level.level == 1 && 
		//	profile.HardTutorialState == 3 && 
		//	(profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.AfterBattle3_1) == 0)
		//{
		//	tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterBattle3_1;
		//	openTutorial = true;
		//}

		//if (!openTutorial)
		//	return;

		//tutorial.currentTrigger = 0;
		//var entity = ClientWorld.Instance.EntityManager.CreateEntity();
		//ClientWorld.Instance.EntityManager.AddComponentData(entity, tutorial);
		//MenuTutorialPointer.Init(this);
	}

	public override bool ProcessMessage(string message, ref RectTransform buttonForPointer)
	{
		return false;
	}

	void Start()
	{
		TryStartTutorial();
	}

	public static void CreateStartBattleWith6CardsEvent()
	{
		var em = ClientWorld.Instance.EntityManager;
		var trigger = em.CreateEntity();
		em.AddComponentData(trigger, new MenuEventInstance { trigger = MenuTutorialEventTrigger.OnStartBattleWith6Cards });
	}
}

*/
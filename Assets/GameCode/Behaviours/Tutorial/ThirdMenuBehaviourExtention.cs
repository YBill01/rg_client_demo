/*using UnityEngine;
using Legacy.Client;
using Legacy.Database;
using Unity.Entities;
using System.Linq;

// У нас два туторила после третьего боя. Один на случай если игрок прокачал до третьего боя карту и второй если нет
// Этот работает когда игрок Прокачал карту. Определяется по уровню игрока
//AfterBattle3_2
public class ThirdMenuBehaviourExtention : BaseMenuTutorialExtention
{
    [SerializeField]
    private MenuTutorialPointerBehaviour MenuTutorialPointer;

    [SerializeField]
    private RectTransform HeroesContainer;


    [SerializeField]
    private RectTransform UpgradeHeroButton;

    [SerializeField]
    private RectTransform HomeButton;

    void TryStartTutorial()
    {

    }

    public override bool ProcessMessage(string message, ref RectTransform buttonForPointer)
    {
        var profile = ClientWorld.Instance.Profile;

        if (message == "OpenGalahard")
        {
            buttonForPointer = HeroesContainer.GetChild(0).GetComponentInChildren<LegacyButton>().GetComponent<RectTransform>();
        }
        else if (message == "UpgradeHero")
        {
            buttonForPointer = UpgradeHeroButton;
            MenuTutorialPointer.popupMessage.ShowTextAtLeftFrom(Locales.Get("locale:1201")/ *"Upgrade Hero to  increase it`s power!"* /, UpgradeHeroButton);
        }
        else if (message == "Home")
        {
            buttonForPointer = HomeButton;
        }
        else if (message == "GoHomeBlick")
        {
            GetComponent<MenuTutorialPointerBehaviour>().SetFlipHandVariant1();
            buttonForPointer = HomeButton;
            buttonForPointer.GetComponentInChildren<BlickControl>().Enable();

        }
        else if (message == "HandTap")
        {
            buttonForPointer = UpgradeHeroButton;

        }
        else if (message == "StopBattleBlick")
        {
            buttonForPointer = HomeButton;
            buttonForPointer.GetComponentInChildren<BlickControl>().Enable();
            buttonForPointer = null;
        }

        else if (message == "ShowHeroMessage")
        {
            HeroesContainer.gameObject.SetActive(true);
        }
        else if (message == "HideHeroMessage")
        {
            HeroesContainer.gameObject.SetActive(false);
        }
        else if (message == "HidePopupMessage")
        {
            GetComponent<MenuTutorialPointerBehaviour>().SetFlipHandVariant1();
            MenuTutorialPointer.popupMessage.Hide();
            buttonForPointer = HomeButton;
        }
        else if (message == "StartBattle")
        {
            buttonForPointer.GetComponentInChildren<BlickControl>().Enable();
            buttonForPointer = null;
            return true;
        }

        return false;
    }

    void Start()
    {
        TryStartTutorial();
        MenuTriggerSystem.MenuTutorialFinishEvent.AddListener(OnOtherTutorialFinish);
        ClientWorld.Instance.Profile.PlayerProfileUpdated.AddListener(TryStartTutorial);
    }

    private void OnDestroy()
    {
        MenuTriggerSystem.MenuTutorialFinishEvent.RemoveListener(OnOtherTutorialFinish);
        ClientWorld.Instance.Profile.PlayerProfileUpdated.RemoveListener(TryStartTutorial);
    }

    void OnOtherTutorialFinish(SoftTutorial.SoftTutorialState state)
    {
        TryStartTutorial();
    }

    public static void CreateStartBattleWith6CardsEvent()
    {
        HomeTutorialHelper.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.AfterBattle1,true);

        var em = ClientWorld.Instance.EntityManager;
        var trigger = em.CreateEntity();
        em.AddComponentData(trigger, new MenuEventInstance { trigger = MenuTutorialEventTrigger.OnStartBattleWith6Cards });
    }
}

*/
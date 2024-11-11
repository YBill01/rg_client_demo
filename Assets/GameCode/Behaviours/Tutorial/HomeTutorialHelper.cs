/*using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

public class HomeTutorialHelper : BaseMenuTutorialExtention
{
    public static HomeTutorialHelper Instance;
    [SerializeField]
    private MenuTutorialPointerBehaviour MenuTutorialPointer;
    public MenuTutorialInstance tutorial;
    public static Entity tutorialEntity;
    private ProfileInstance profile;
    private UnityEvent update = new UnityEvent();
    public UnityEvent Update { get => update; }

    public int HardHomeTutorStep=default;
    private void Awake()
    {
        profile = ClientWorld.Instance.Profile;

        Instance = this;

    }

    public bool IsLockTutorialStart = false;
    / *
    public void OnTutorialStart()
    {
		if (IsLockTutorialStart)
		{
            return;
		}

        // запуск Home Tutor
        if (profile.IsBattleTutorial) 
        {
            StartHomeTutor();
        }
        else
        {
            StartPostTutorial();
        }
        #region old_tutior
        / *  NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.GoIntoBattle,true);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.Deck);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.AfterBattle1);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.AfterBattle2);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.UpgradeHero);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.EnterName);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.AfterBattle3_1);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.Deck2);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.NewCard);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattle1);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattle2);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattle3);
           NonRequiredTutorials.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattleAll);* /
        #endregion
    }

    #region Post Tutor
    private void StartPostTutorial()
    {
        switch (profile.battleStatistic.battles)
        {
            case 0: // после тутора  // сюда добавить и туторы Арены, Имени...
                if (!TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattle4))
                {
                    if (!TryStartTutorial(SoftTutorial.SoftTutorialState.EnterName))
                    {
                        TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattle1);
                    }
                }

                / *if (!TryStartTutorial(SoftTutorial.SoftTutorialState.EnterName))
				{
					if (!TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattle4))
					{
                        if (ClientWorld.Instance.Profile.HardTutorialState == Tutorial.Instance.TotalCount() && ClientWorld.Instance.Profile.battleStatistic.battles == 0)
                        {
                            WindowManager.Instance.MainWindow.GetLeftContainer.SetActive(true);
                        }
                        TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattle1);
                    }
				}* /
                break;
            case 1: // после 1 боя
                TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattle2);
                break;
            case 2:// после 2  боя
               TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattle3);
                break;
            default:
               TryStartTutorial(SoftTutorial.SoftTutorialState.AfterTutorBattleAll);
                break;
        }
    }
    #endregion* /
    / *
    #region Hard Home Tutor
    private void StartHomeTutor()
    {
        switch (profile.HardTutorialState)
        {
            case 1: // после 1 туторного боя
                {
                    if (!TryStartTutorial(SoftTutorial.SoftTutorialState.AfterBattle1))
                        if (!TryStartTutorial(SoftTutorial.SoftTutorialState.AfterBattle1_2))
                        //    if (!TryStartTutorial(SoftTutorial.SoftTutorialState.NewCard))
                        {
                            TryStartTutorial(SoftTutorial.SoftTutorialState.GoIntoBattle, true);
                        }
                    break;
                }
            case 2: // после 2 туторного боя
                {
                    if (!TryStartTutorial(SoftTutorial.SoftTutorialState.AfterBattle2))
                        if (!TryStartTutorial(SoftTutorial.SoftTutorialState.Deck))
                        {
                            TryStartTutorial(SoftTutorial.SoftTutorialState.GoIntoBattle, true);
                        }
                    break;
                }
            case 3: // после 3 туторного боя
                if (!TryStartTutorial(SoftTutorial.SoftTutorialState.AfterBattle3_1))
                    if (!TryStartTutorial(SoftTutorial.SoftTutorialState.Deck2))
                        if (!TryStartTutorial(SoftTutorial.SoftTutorialState.UpgradeHero))
                        {
                            TryStartTutorial(SoftTutorial.SoftTutorialState.GoIntoBattle, true);
                        }
                break;
        }
    }
    #endregion* /

    public bool IsDescTutor()
    {
        if(tutorial.softTutorialState == SoftTutorial.SoftTutorialState.Deck || 
            tutorial.softTutorialState == SoftTutorial.SoftTutorialState.Deck2 || 
                tutorial.softTutorialState == SoftTutorial.SoftTutorialState.NewCard)
        {
            return true;
        }
        return false;
    }
    / *
    public bool TryStartTutorial(SoftTutorial.SoftTutorialState softTutorialState, bool isForcible = false)
    {
        var openTutorial = false;
        var profile = ClientWorld.Instance.Profile;
        UnityEngine.Debug.Log($"on open this window enable softTutorialState <color=blue>{softTutorialState}</color>");
        switch (softTutorialState)
        {
            case SoftTutorial.SoftTutorialState.UpgradeCard://   если, после игрок пропустил Upgrade карты после второго боя, то должен улучшить её после 3го. 
                openTutorial = UpgradeCardTutorial(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                MenuTutorialPointer.Init(GetComponent<SecondMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.AfterTutorBattle1://+
                openTutorial = AfterTutorBattle1(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                MenuTutorialPointer.Init(GetComponent<SecondMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.AfterTutorBattle2://+
                openTutorial = AfterTutorBattle2(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                MenuTutorialPointer.Init(GetComponent<SecondMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.AfterTutorBattle3://+
                openTutorial = AfterTutorBattle3(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                MenuTutorialPointer.Init(GetComponent<SecondMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.AfterTutorBattleAll://+
                openTutorial = AfterTutorBattleAll(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                MenuTutorialPointer.Init(GetComponent<SecondMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.AfterBattle4://+
                openTutorial = OpenBattlePass(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                MenuTutorialPointer.Init(GetComponent<FourthMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.Deck://+
                openTutorial = UpgradeCardsTutorial(profile, ref tutorial);
                if (!openTutorial)
                    return false;
               
                MenuTutorialPointer.Init(GetComponent<SecondMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.EnterName://+
                openTutorial = EnterNameTutorial(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                MenuTutorialPointer.Init(GetComponent<FourthMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.OpenArena://+
                openTutorial = OpenArenaTutorial(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                MenuTutorialPointer.Init(GetComponent<FourthMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.AfterBattle1_2://+
                openTutorial = TryStartLootBoxTutorial2(profile, ref tutorial, isForcible);
                if (!openTutorial)
                    return false;
                HardHomeTutorStep = 11; //1 бой 1 шаг.
                MenuTutorialPointer.Init(GetComponent<FirstMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.AfterBattle1://+
                openTutorial = TryStartLootBoxTutorial(profile, ref tutorial, isForcible);
                if (!openTutorial)
                    return false;
                HardHomeTutorStep = 11; //1 бой 1 шаг.
                MenuTutorialPointer.Init(GetComponent<FirstMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.AfterTutorBattle4://+
                openTutorial = TryStartLootBoxTutorial3(profile, ref tutorial, isForcible);
                if (!openTutorial)
                    return false;
                //HardHomeTutorStep = 11; //1 бой 1 шаг.
                MenuTutorialPointer.Init(GetComponent<FirstMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.NewCard://+
                openTutorial = TryStartNewCardTutorial(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                HardHomeTutorStep = 12; //1 бой 1 шаг.
                MenuTutorialPointer.Init(GetComponent<SecondMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.AfterBattle3_1://+
                openTutorial = TryStartAfterBattle3Tutorial(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                MenuTutorialPointer.Init(GetComponent<FirstMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.Deck2://+
                openTutorial = UpgradeDesk2Tutorial(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                MenuTutorialPointer.Init(GetComponent<SecondMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.AfterBattle2://+
                openTutorial = TryStartOpenCardTutorial(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                HardHomeTutorStep = 21; //1 бой 1 шаг.
                MenuTutorialPointer.Init(GetComponent<SecondMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.UpgradeHero://+
                openTutorial = UpgradeHeroTutorial(profile, ref tutorial);
                if (!openTutorial)
                    return false;
                HardHomeTutorStep = 14; //1 бой 1 шаг.
                Update.Invoke();
                MenuTutorialPointer.Init(GetComponent<ThirdMenuBehaviourExtention>());
                break;
            case SoftTutorial.SoftTutorialState.GoIntoBattle://+
                openTutorial = GoToBattleTutorial(profile, ref tutorial, isForcible);
                if (!openTutorial)
                    return false;
                if(profile.HardTutorialState==1)
                   HardHomeTutorStep = 13; //1 бой 1 шаг.
                MenuTutorialPointer.Init(GetComponent<FirstMenuBehaviourExtention>());
                break;
            default:
                break;
        }

        if (!openTutorial)
            return false;

        var scenarioIndex = SoftTutorial.GetScenarioIndex(tutorial.softTutorialState);
        var trigger = profile.TutorialsSteps.ContainsKey(scenarioIndex) && !isForcible ?
            (byte)profile.TutorialsSteps[scenarioIndex] : 0;

        tutorial.currentTrigger = (byte)trigger;
        tutorial._timer_start = int.MaxValue;
        UnityEngine.Debug.Log($"switch softTutorialState <color=purple>{softTutorialState}</color> with step {tutorial.currentTrigger}");
        TutorialEntityClass.getInstance().SetDataToTutorialEntity(tutorial);

        return true;
    }* /
  
    public bool TryStartLootBoxTutorial2(ProfileInstance profile, ref MenuTutorialInstance tutorial, bool isForcible)
    {
        var openTutorial = false;
        bool hasLootbox = false;
        foreach (var box in profile.loot.boxes)
        {
            if (box.index > 0)
            {
                hasLootbox = true;
                break;
            }
        }
        if (hasLootbox)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterBattle1_2;
            openTutorial = true;
        }

        return openTutorial;
    }
    public bool TryStartLootBoxTutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial, bool isForcible)
    {
        var openTutorial = false;
        bool hasLootbox = false;
        foreach (var box in profile.loot.boxes)
        {
            if (box.index > 0)
            {
                hasLootbox = true;
                break;
            }
        }
        if (hasLootbox && (profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.AfterBattle1) == 0
              || isForcible)
          {
              tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterBattle1;
              openTutorial = true;
          }

        return openTutorial;

    }

    private bool AfterTutorBattle4IsActive = false;
    public bool TryStartLootBoxTutorial3(ProfileInstance profile, ref MenuTutorialInstance tutorial, bool isForcible)
    {
        var openTutorial = false;
        / *bool hasLootbox = false;
        foreach (var box in profile.loot.boxes)
        {
            if (box.index > 0)
            {
                hasLootbox = true;
                break;
            }
        }* /
        if (!AfterTutorBattle4IsActive / *&& hasLootbox* / && (profile.MenuTutorialState & (int)SoftTutorial.SoftTutorialState.AfterTutorBattle4) == 0 || isForcible)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterTutorBattle4;
            openTutorial = true;
        }

        AfterTutorBattle4IsActive = true;

        return openTutorial;

    }
    private bool TryStartOpenCardTutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {

        var openTutorial = false;
        bool hasLootbox = false;
        foreach (var box in profile.loot.boxes)
        {
            if (box.index > 0)
            {
                hasLootbox = true;
                break;
            }
        }
        if (hasLootbox && profile.HardTutorialState ==2 && (profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.AfterBattle2) == 0)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterBattle2;
            openTutorial = true;
        }


        return openTutorial;
    }
    
     private bool UpgradeCardTutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        var openTutorial = false;
        var canUpgrade = HasCardsForUpgrade(profile);
        if (profile.HardTutorialState == 3 && ((profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.Deck2) > 0) && ((profile.MenuTutorialState & (int)SoftTutorial.SoftTutorialState.UpgradeCard) == 3))
        {
            if (canUpgrade)
            {
                UnityEngine.Debug.Log("UpgradeCards");

                tutorial.softTutorialState = SoftTutorial.SoftTutorialState.UpgradeCard;
                openTutorial = true;
            }
        }

        return openTutorial;
    }
    private bool UpgradeDesk2Tutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        var openTutorial = false;
        var canUpgrade = HasCardsForUpgrade(profile);
        if (profile.HardTutorialState == 3 && (profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.Deck2) == 0)
        {
            if (canUpgrade)
            {
                UnityEngine.Debug.Log("UpgradeCards2Tutorial");

                tutorial.softTutorialState = SoftTutorial.SoftTutorialState.Deck2;
                openTutorial = true;
            }
        }

        return openTutorial;
    }
    private bool  TryStartNewCardTutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        var openTutorial = false;
        if (profile.HardTutorialState > 1)
            return openTutorial;


        bool hasNewCard = false;
       
        foreach (var index in profile.DecksCollection.In_deck)
        {
           if(profile.Inventory.GetCardData(index).isNew)
            {
                hasNewCard = true;
                break;
            }
        }
        if (!hasNewCard)
        {
            foreach (var index in profile.DecksCollection.In_collection)
            {
                if (profile.Inventory.GetCardData(index).isNew) 
                {
                    hasNewCard = true;
                    break;
                }
            }
        }
        if (hasNewCard &&  (profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.NewCard) == 0)
        {
            UnityEngine.Debug.Log("NewCardTutorial");
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.NewCard;
            openTutorial = true;
        }

         return openTutorial;
    }

    private bool TryStartAfterBattle3Tutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {

        var openTutorial = false;
        bool hasLootbox = false;
        foreach (var box in profile.loot.boxes)
        {
            if (box.index > 0)
            {
                UnityEngine.Debug.Log("Start After Battle 3 Tutorial");
                hasLootbox = true;
                break;
            }
        }

        if (hasLootbox && profile.HardTutorialState == 3 && (profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.AfterBattle3_1) == 0)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterBattle3_1;
            UnityEngine.Debug.Log("Start After Battle 3 Tutorial");
            openTutorial = true;
        }


        return openTutorial;
    }
    
    private bool AfterTutorBattleAll(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        bool openTutorial = false;
        if (profile.IsBattleTutorial == false && profile.battleStatistic.battles >2)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterTutorBattleAll;
            openTutorial = true;
        }
        return openTutorial;
    }
    private bool AfterTutorBattle3(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        bool openTutorial = false;
        if (profile.IsBattleTutorial == false && profile.battleStatistic.battles == 2)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterTutorBattle3;
            openTutorial = true;
        }
        return openTutorial;
    }

    private bool AfterTutorBattle2(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        bool openTutorial = false;
        if (profile.IsBattleTutorial == false && profile.battleStatistic.battles == 1)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterTutorBattle2;
            openTutorial = true;
        }
        return openTutorial;
    }
    private bool AfterTutorBattle1(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        bool openTutorial = false;
        if(profile.IsBattleTutorial==false && profile.battleStatistic.battles == 0)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterTutorBattle1;
            openTutorial = true;
        }
        return openTutorial;
    }
    private bool UpgradeCardsTutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        bool openTutorial = false;
        var canUpgrade = HasCardsForUpgrade(profile);
        if ((profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.Deck) == 0)
        {
            if (canUpgrade)
            {
                UnityEngine.Debug.Log("UpgradeCardsTutorial");

                tutorial.softTutorialState = SoftTutorial.SoftTutorialState.Deck;
                openTutorial = true;
            }
        }

        return openTutorial;
    }


    private static bool HasCardsForUpgrade(ProfileInstance profile)
    {
        var cards = profile.DecksCollection.ActiveSet.Cards;
        var canUpgrade = false;
        foreach (var card in cards)
        {
            var cardData = profile.Inventory.GetCardData(card);
            if (cardData.CanUpgrade)
            {
                canUpgrade = true;
                break;
            }
        }
        return canUpgrade;
    }

    private bool EnterNameTutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        bool openTutorial = false;
        //var isCollectedFirstReward = GetComponent<FourthMenuBehaviourExtention>().IsCollectedfirstReward();
        if (/ *isCollectedFirstReward && * /profile.HardTutorialState == Tutorial.Instance.TotalCount() && ((profile.MenuTutorialState & (int)SoftTutorial.SoftTutorialState.EnterName) == 0))
        {
            UnityEngine.Debug.Log("EnterNameTutorial");
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.EnterName;
            openTutorial = true;
        }
        return openTutorial;
    }
    private bool OpenBattlePass(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        bool openTutorial = false;
        if ((profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.AfterBattle4) == 0)
        {
            UnityEngine.Debug.Log("OpenBattlePass");
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterBattle4;
            openTutorial = true;

        }
        return openTutorial;
    }

    private bool OpenArenaTutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        bool openTutorial = false;
        if ((profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.OpenArena) == 0)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.OpenArena;
            openTutorial = true;
        }
        return openTutorial;
    }

    private bool GoToBattleTutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial, bool isForcible)
    {
        bool openTutorial = false;
        bool hasLootbox = false;
        foreach(var box in profile.loot.boxes)
        {
            if (box.index > 0)
            {
                hasLootbox = true;
                break;
            }
        }
        if (!hasLootbox && profile.HardTutorialState < 3 && ((profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.GoIntoBattle) == 0 || isForcible))
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.GoIntoBattle;
            openTutorial = true;
        }
        if (!openTutorial && profile.HardTutorialState == 3 && ((profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.UpgradeHero) > 0)){
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.GoIntoBattle;
            openTutorial = true;
        }
        if (!openTutorial && profile.HardTutorialState == Tutorial.Instance.TotalCount() && profile.battleStatistic.battles == 0 && (profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.AfterBattle4) > 0)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.GoIntoBattle;
            openTutorial = true;
        }
        return openTutorial;
    }

    private bool UpgradeHeroTutorial(ProfileInstance profile, ref MenuTutorialInstance tutorial)
    {
        var heroes = profile.heroes;
        var canUpgrade = false;
        foreach (var hero in heroes)
        {
            canUpgrade = hero.Value.level < profile.Level.level;
            if (!profile.Stock.CanTake(CurrencyType.Soft, hero.Value.UpdatePrice)) //Not Soft 
                canUpgrade = false;
            if (canUpgrade)
                break;
        }
        bool openTutorial = false;
        if (profile.Level.level >= 2 && canUpgrade && (profile.MenuTutorialState & (ushort)SoftTutorial.SoftTutorialState.UpgradeHero) == 0)
        {
            tutorial.softTutorialState = SoftTutorial.SoftTutorialState.UpgradeHero;
            UnityEngine.Debug.Log("UpgradeHeroTutorial");
            openTutorial = true;
        }
        return openTutorial;
    }

    public override bool ProcessMessage(string message, ref RectTransform buttonForPointer)
    {
        return false;
    }

}
*/
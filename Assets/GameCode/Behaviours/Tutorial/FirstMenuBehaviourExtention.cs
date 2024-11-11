/*using UnityEngine;
using Legacy.Client;
using Legacy.Database;
using Unity.Entities;
using System.Collections.Generic;
using System.Collections;

public class FirstMenuBehaviourExtention : BaseMenuTutorialExtention
{
    [SerializeField]
    RectTransform FirstLootboxPoint;

    [SerializeField]
    RectTransform SecondLootboxPoint;

    [SerializeField]
    RectTransform OpenLootboxPoint;

    [SerializeField]
    RectTransform DeckButtonPoint;

    [SerializeField]
    private RectTransform CardsContainer;

    [SerializeField]
    private RectTransform CloseCardPoint;

    [SerializeField]
    private List<RectTransform> ButtonsWithBlick;

    [SerializeField]
    private MenuTutorialPointerBehaviour MenuTutorialPointer;

    public override bool ProcessMessage(string message, ref RectTransform buttonForPointer)
    {
        if (message == "OpenLootbox")
        {
            buttonForPointer = OpenLootboxPoint;
        }
        if (message == "HideHand")
        {
            buttonForPointer = null;
            GetComponent<MenuTutorialPointerBehaviour>().SetFlipHandVariant1();
        }
        if (message == "ArenaButton")
        {
            GetComponent<MenuTutorialPointerBehaviour>().SetFlipHandVariant1();
        }
        if (message == "OpenArena")
        {
            //StartCoroutine(OpenArenaWindowWait());
        }
        if (message == "ShowBattlePassVFX")
        {
            //WindowManager.Instance.MainWindow.GetBattlePassButton.SetCollectEffect(true);
            //MainMenuArenaChangeBehaviour.Instance.IsTutorial = false;
            //WindowManager.Instance.MainWindow.GetBattlePassButton.gameObject.SetActive(true);
            //WindowManager.Instance.MainWindow.GetLeftContainer.SetActive(true);
            //HomeTutorialHelper.Instance.IsLockTutorialStart = false;
            //HomeTutorialHelper.Instance.OnTutorialStart();
        }
        if (message == "GoHomeBlick")
        {
            foreach (var bttnWithBlick in ButtonsWithBlick)
            {
                buttonForPointer = bttnWithBlick;
                buttonForPointer.GetComponentInChildren<BlickControl>().Enable();
            }
        }
        if (message == "DisableBlicks")
        {
            foreach (var bttnWithBlick in ButtonsWithBlick)
            {
                buttonForPointer = bttnWithBlick;
                buttonForPointer.GetComponentInChildren<BlickControl>().Disable();
                buttonForPointer = null;

            }
        }

        //if (message != "StartBattle" && buttonForPointer != null)
//			MenuTutorialPointer.SetupClickFrame(buttonForPointer);

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

    private IEnumerator OpenArenaWindowWait()
    {
        yield return new WaitForSeconds(0.01f);

        WindowManager.Instance.MainWindow.Arena();
    }


    public void TryStartLootBoxTutorial()
    {
        //var profile = ClientWorld.Instance.Profile;
        //var openTutorial = false;
        //MenuTutorialInstance tutorial = new MenuTutorialInstance { currentTrigger = 0, _timer_start = int.MaxValue }; ;

        //      if (profile.MenuTutorialState < (ushort)SoftTutorial.SoftTutorialState.AfterBattle1)
        //      {
        //          tutorial.softTutorialState = SoftTutorial.SoftTutorialState.AfterBattle1;
        //          openTutorial = true;
        //      }

        //tutorial.currentTrigger = GetCurrentTriger(profile);
        //return openTutorial;

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

}
*/
/*using UnityEngine;
using Legacy.Client;
using Legacy.Database;
using Unity.Entities;
using System.Linq;

public class FourthMenuBehaviourExtention : BaseMenuTutorialExtention
{
    [SerializeField]
    private MenuTutorialPointerBehaviour MenuTutorialPointer;
    [SerializeField]
    private RectTransform HeroesContainer;
    [SerializeField]
    private RectTransform BattlePassRewards;

    RectTransform ActiveRewardRect;
    void TryStartTutorial()
    {

    }
    public override bool ProcessMessage(string message, ref RectTransform buttonForPointer)
    {
        if (message == "ChangeName")
        {
            WindowManager.Instance.MainWindow.ChangeName();
            buttonForPointer = null;
            return true;
        }
        if (message == "HideHand")
        {
            buttonForPointer = null;
        }
        else if (message == "OpenArena")
        {
            WindowManager.Instance.MainWindow.Arena();
            return true;
        }
        else if (message == "OpenBattlePass")
        {
            WindowManager.Instance.MainWindow.BattlePass();
            return true;
        }
        else if (message == "ActiveReward")
        {
            if(ActiveRewardRect == null)
            {
                ActiveRewardRect = GetActiveReward();
            }
            buttonForPointer = ActiveRewardRect;
        }

        return false;
    }

    public RectTransform GetActiveReward()
    {
        var rewards = BattlePassRewards.gameObject.GetComponentsInChildren<BattlePassBasicRewardBehaviour>().ToList();
        var openedReward = rewards.Where(x => x.rewardState == BattlePassRewardState.Active && !x.GetComponent<LegacyButton>().isLocked).FirstOrDefault();

        return openedReward?.GetComponent<RectTransform>() ?? null;
    }

    public bool IsCollectedfirstReward()
    {
        var rewards = BattlePassRewards.gameObject.GetComponentsInChildren<BattlePassBasicRewardBehaviour>().ToList();
        var isCollected = rewards.Where(x => x.rewardState == BattlePassRewardState.Collected).Count() > 0;
        return isCollected;
    }
    void Start()
    {
        TryStartTutorial();
    }
}
*/
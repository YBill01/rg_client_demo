using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassLevelBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private List<TMP_Text> levelTexts;
        [SerializeField] 
        private GameObject currentLevel;
        [SerializeField] 
        private GameObject lockedLevel;
        [SerializeField]
        private GameObject firstLevel;
        [SerializeField] 
        private GameObject previousLevel;
        [SerializeField] 
        private BattlePassPremiumRewardBehaviour premiumRewardContent;
        [SerializeField] 
        private BattlePassBasicRewardBehaviour freeRewardContent;

        private BinaryBattlePassTreasure battlePassData;
        private ProfileInstance profile;
        private BattlePassWindowBehaviour battlePassWindow;
        private int levelID;
        private DateTime timeForLevel;
        private float positionX;
        private bool isSeen;

        private BattlePassRewardState freeState;
        private BattlePassRewardState premiumState;
        private BinaryBattlePass binaryBattlePass;

        public bool IsActiveToCollected { get { return freeState == BattlePassRewardState.Active || premiumState == BattlePassRewardState.Active; } }

        public void Init(ProfileInstance profileInstance, BinaryBattlePassTreasure data, int level, int playerLevel,
            DateTime timeStart, BattlePassWindowBehaviour battlePassWindowBehaviour, BinaryBattlePass binaryBattlePass)
        {
            profile = profileInstance;
            battlePassData = data;
            levelID = level;
            battlePassWindow = battlePassWindowBehaviour;
            this.binaryBattlePass = binaryBattlePass;


            currentLevel.SetActive(levelID == playerLevel + 1);
            lockedLevel.SetActive(levelID > playerLevel + 1);
            previousLevel.SetActive(levelID <= playerLevel && levelID > 0);
            firstLevel.SetActive(levelID == 0);

            for (int i = 0; i < levelTexts.Count; i++)
            {
                levelTexts[i].text = level == 0 ? "" : level.ToString();
            }

            freeState = BattlePassRewardState.Locked;
            premiumState = BattlePassRewardState.Locked;

            if (profile.battlePass.GetBattlePass((ushort) levelID, out PlayerBattlePassItem levelData))
            {
                if (levelData.free)
                    freeState = BattlePassRewardState.Collected;

                if (levelData.premium)
                    premiumState = BattlePassRewardState.Collected;
            }
            
            timeForLevel = timeStart.AddDays(level);

            if (playerLevel >= levelID)
            {
                if (freeState != BattlePassRewardState.Collected)
                {
                    freeState = BattlePassRewardState.Active;
                }
                if (premiumState != BattlePassRewardState.Collected)
                {
                    if (profile.battlePass.isPremiumBought)
                    {
                        premiumState = BattlePassRewardState.Active;
                    }
                    else
                    {
                        premiumState = BattlePassRewardState.LockedPremium;
                    }
                }
                if (timeForLevel.ToLocalTime() > DateTime.Now)
                {
                    var state = playerLevel == levelID ? BattlePassRewardState.ActiveTimer : BattlePassRewardState.CurrentTimer;
                    freeState = state;
                    premiumState = state;
                    freeRewardContent.TimerUp += OnTimerUp;
                }
                
            }

            premiumRewardContent.Init(battlePassData.pay, premiumState, timeForLevel, true , battlePassWindow,this);
            freeRewardContent.Init(battlePassData.free, freeState, timeForLevel, false, battlePassWindow, this);
            
            premiumRewardContent.SetPremiumLockedState(profileInstance.battlePass.isPremiumBought);
        }

        private void OnTimerUp()
        {
            freeRewardContent.TimerUp -= OnTimerUp;
            battlePassWindow.OnRewardTimerUp();
        }

        public void PremiumButtonClick()
        {
            SendBPAnalytics(true);
            profile.ClaimBattlePassReward(false, levelID);
            premiumRewardContent.SetCollected();
            var reward = premiumRewardContent.GetItem();
            
            if (reward is BattlePassLootBoxItemBehaviour lootBoxReward)
            {
                battlePassWindow.OpenLootBox(lootBoxReward);
            }
            else
            {
                premiumRewardContent.DropRewardParticles();
            }
        }

        public void onFreeShow()
        {
            if (battlePassWindow.isTutor)
            {
             //   PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:1483"));
                return;
            }
            var reward = freeRewardContent.GetItem();

            if (reward is BattlePassLootBoxItemBehaviour lootBoxReward)
            {
                battlePassWindow.ShowLootBox(lootBoxReward);
            }
        }

        public void onPremShow()
        {
            if (battlePassWindow.isTutor)
            {
          //      PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:1483"));
                return;
            }
            var reward = premiumRewardContent.GetItem();

            if (reward is BattlePassLootBoxItemBehaviour lootBoxReward)
            {
                battlePassWindow.ShowLootBox(lootBoxReward);
            }
        }
        public void FreeButtonClick()
        {
            if (battlePassWindow.isTutor && levelID>0)
            {
                PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:1483"));
                return;
            }
            SendBPAnalytics(false);
            profile.ClaimBattlePassReward(true, levelID);
            freeRewardContent.SetCollected();
            var reward = freeRewardContent.GetItem();
            
            if (reward is BattlePassLootBoxItemBehaviour lootBoxReward)
            {
                battlePassWindow.OpenLootBox(lootBoxReward);
            }
            else
            {
                freeRewardContent.DropRewardParticles();
            }
        }

        public void ActivatePremiumReward()
        {
            premiumRewardContent.SetPremiumLockedState(true);
        }

        public void EnablePremiumReward()
        {
            premiumRewardContent.EnablePremiumReward();
        }

        public void SetPreviousState()
        {
            currentLevel.SetActive(false);
            previousLevel.SetActive(true);
            if (timeForLevel.ToLocalTime() > DateTime.Now)
            {
                freeRewardContent.ChangeToPreviousStateWithTimer(timeForLevel);
                premiumRewardContent.ChangeToPreviousStateWithTimer(timeForLevel);
            }
            else
            {
                freeRewardContent.ChangeToPreviousState();
                premiumRewardContent.ChangeToPreviousState();
            }
        }

        public void SetCurrentState()
        {
            currentLevel.SetActive(true);
            lockedLevel.SetActive(false);
            if (timeForLevel.ToLocalTime() > DateTime.Now)
            {
                freeRewardContent.ChangeToCurrentStateWithTimer(timeForLevel);
                premiumRewardContent.ChangeToCurrentStateWithTimer(timeForLevel);
            }
            else
            {
                freeRewardContent.ChangeToCurrentState();
                premiumRewardContent.ChangeToCurrentState();
            }
        }

        public void SetPosition(float pos)
        {
            positionX = pos;
        }

        public float GetPosition()
        {
            return positionX;
        }

        public void SetInitialScale()
        {
            isSeen = false;
            freeRewardContent.SetInitialScale();
            premiumRewardContent.SetInitialScale();
        }

        public void SetInitialNoAnimation()
        {
            isSeen = true;
            freeRewardContent.SetInitialNoAnimation();
            premiumRewardContent.SetInitialNoAnimation();
        }

        public bool HadFirstAppearance()
        {
            return isSeen;
        }

        public void PlayAnimationOnFirstAppearance()
        {
            isSeen = true;
            freeRewardContent.PlayAnimationOnFirstAppearance();
            premiumRewardContent.PlayAnimationOnFirstAppearance();
        }

        private void OnDestroy()
        {
            freeRewardContent.TimerUp -= OnTimerUp;
        }

        private void SendBPAnalytics(bool isPremium) 
        {
            var rewardId = isPremium ? battlePassData.pay : battlePassData.free;
            AnalyticsManager.Instance.BattlePassReward(binaryBattlePass.index, rewardId, levelID, isPremium);
        }
    }
}
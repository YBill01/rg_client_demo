using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class BattlePassWindowBehaviour : WindowBehaviour
    {
        public event Action RewardTimerUp;
        public bool isTutor = false;
        public LootBoxViewBehaviour BoxToOpen;
        public BattlePassLootBoxItemBehaviour ShowBox;

        [SerializeField]
        private BattlePassContentBehaviour contentBehaviour;
        [SerializeField]
        private ScrollRect scrollRect;

        private ProfileInstance profile;
        private bool openingLootBox;
        private BattlePassLootBoxItemBehaviour lootBoxReward;
        private Canvas _canvasUpPanel;
        public override void Init(Action callback)
        {
            profile = ClientWorld.Instance.Profile;
            profile.PlayerProfileUpdated.AddListener(OnPlayerProfileUpdated);
            contentBehaviour.Init(profile, this);
            callback();
        }
        
        public void ShowLootBox(BattlePassLootBoxItemBehaviour reward)
        {
            System.Collections.Generic.Dictionary<string, string> settings = new System.Collections.Generic.Dictionary<string, string>();

            ShowBox = reward;
            BoxToOpen = reward.GetLootBox();
            openingLootBox = true;
            settings.Add("battlePass", "close");
  
            WindowManager.Instance.OpenWindow(WindowManager.Instance.Windows[8], settings);
        }

        public bool GetClickedShowBox(out LootBoxViewBehaviour clickedBox)
        {
            clickedBox = BoxToOpen;
            return clickedBox != null;
        }

        public void OpenLootBox(BattlePassLootBoxItemBehaviour reward)
        {
            lootBoxReward = reward;

            BoxToOpen = reward.GetLootBox();
            var reset = BoxToOpen.gameObject.GetComponent<RectScaleToBehaviour>();
            if (reset)
            {
                reset.Reset();
                Debug.Log("Reset RectScaleToBehaviour Scale");
            }
            openingLootBox = true;
            WindowManager.Instance.OpenWindow(childs_windows[0]);
        }

        public bool GetClickedBox(out LootBoxViewBehaviour clickedBox)
        {
            clickedBox = BoxToOpen;
            return clickedBox != null;
        }

        public void OpenShop()
        {
            WindowManager.Instance.OpenWindow(childs_windows[1]);
        }

        public void OnRewardTimerUp()
        {
            RewardTimerUp?.Invoke();
        }

        private void OnPlayerProfileUpdated()
        {
            if (gameObject.activeInHierarchy)
            {
                SelfClose();
                SelfOpen();
            }
        }

        protected override void SelfOpen()
        {
            EnableThisGameObject(true);

            if (openingLootBox)
            {
                if(lootBoxReward)
                     lootBoxReward.ResetAfterOpening();
                openingLootBox = false;
            }
            else
                contentBehaviour.OpenWindow();
            /*if (HomeTutorialHelper.Instance.TryStartTutorial(SoftTutorial.SoftTutorialState.AfterBattle4))
            {
                BattlePassContentBehaviour bpc = GetComponent<BattlePassContentBehaviour>();
                if (bpc)
                {
                    Transform leftPanel = bpc.GetLeft();
                    leftPanel.GetComponent<Canvas>().sortingLayerName = "Fader";
                    Canvas upPanel = WindowManager.Instance.GetUpPanel().GetComponent<Canvas>();
                    _canvasUpPanel = upPanel;
                    upPanel.sortingLayerName = "Fader";
                    isTutor = true;
                    if (scrollRect)
                        scrollRect.enabled = false;
                }
                else isTutor = false;
            }
            else
            {
                
            }*/

            isTutor = false;
            if (scrollRect)
                scrollRect.enabled = true;

        }

        
        private void resetCanvasUpPanel()
        {
            if (_canvasUpPanel)
            {
                _canvasUpPanel.sortingLayerName = "TopLayer";
                BattlePassContentBehaviour bpc = GetComponent<BattlePassContentBehaviour>();
                if (bpc)
                {
                    Transform leftPanel = bpc.GetLeft();
                    leftPanel.GetComponent<Canvas>().sortingLayerName = "TopLayer";
                }
            }
        }
        private void EnableThisGameObject(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        protected override void SelfClose()
        {
            resetCanvasUpPanel();
            EnableThisGameObject(false);
        }
    }
}
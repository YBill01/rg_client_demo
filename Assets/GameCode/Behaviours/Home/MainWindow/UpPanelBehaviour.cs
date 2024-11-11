using Legacy.Database;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    [Serializable]
    public enum UpPanelItem
    {
        Account = 1 << 0,
        Rating = 1 << 1,
        Soft = 1 << 2,
        Hard = 1 << 3,
        Settings = 1 << 4,
        BackButton = 1 << 5,
        HomeButton = 1 << 6,
        //Shards = 1 << 7,
        PlusButtons = 1 << 8
    }
    public class UpPanelBehaviour : MonoBehaviour
    {
        [SerializeField]
        private AccountBehaviour Account;
        [SerializeField]
        private NextRewardBehaviour NextReward;
        [SerializeField]
        private CurrenciesBehaviour Currencies;
        [SerializeField]
        private LegacyButton SettingsButton;
        [SerializeField]
        private LegacyButton BackButton;
        [SerializeField]
        private LegacyButton HomeButton;
        [SerializeField]
        private CanvasScaler CanvasScaler;

        const int defaultConfig = 31;

        int currentConfig = 0;

        public bool IsItemEnabled(UpPanelItem type)
        {
            return (currentConfig & (int)type) > 0;
        }
        public void OnArenaWindow()
        {
            NextReward.OnArenaWindow();
        }
        public void UpdateCurrencies()
        {
            Currencies.UpdateValue();
        }

        public void Init()
        {
            CanvasScaler.matchWidthOrHeight = Camera.main.aspect > 1.777f ? 1.0f : 0.0f;
            ClientWorld.Instance.Profile.PlayerProfileUpdated.AddListener(UpdateCurrencies);
        }

        private void OnDestroy()
        {
            ClientWorld.Instance.Profile?.PlayerProfileUpdated.RemoveListener(UpdateCurrencies);
        }
        internal void AccountUpdateLevel()
        {
            Account.NextLevel();
        }
        public ushort minNewRewardRating = 0;
       /* public void NewReward( ushort index,bool isArena=false)
        {
            NextReward.InitReward(index, isArena); 
        }*/
        internal void Setup(int config = defaultConfig)
        {
            currentConfig = config;
            Account.Init();
            Account.Enable(IsItemEnabled(UpPanelItem.Account));
            Currencies.EnableSoft(IsItemEnabled(UpPanelItem.Soft));
            Currencies.EnableHard(IsItemEnabled(UpPanelItem.Hard));
            //Currencies.EnableShards(IsItemEnabled(UpPanelItem.Shards));
            //BackButton.gameObject.SetActive(IsItemEnabled(UpPanelItem.BackButton));
            SettingsButton.gameObject.SetActive(IsItemEnabled(UpPanelItem.Settings));
            HomeButton.gameObject.SetActive(IsItemEnabled(UpPanelItem.HomeButton));
            Currencies.EnablePlusButtons(IsItemEnabled(UpPanelItem.PlusButtons));
            BackButton.gameObject.SetActive(IsItemEnabled(UpPanelItem.BackButton));
        }
        internal void ShowArena(bool v)
        {
            Account.Enable(v);
        }
        internal void ShowNewReward(bool v)
        {
            Account.EnableNeReward(v);
        }
        public void Back()
        {
            var profile = ClientWorld.Instance.Profile;
            WindowManager.Instance.Back();
  
            /*if (profile.HardTutorialState == 4 && (profile.MenuTutorialState & (int)SoftTutorial.SoftTutorialState.AfterBattle4) == 0)
            {
                MenuTutorialPointerBehaviour.CreateOnTapEntity();
            }*/

            if (BackButton.GetComponentInChildren<BlickControl>().active)
			{
                BackButton.GetComponentInChildren<BlickControl>().Disable();
            }
        }

        public void Home()
        {
            WindowManager.Instance.Home();
        }

        internal void Show()
        {
            Setup();
        }
        internal void ShowBack(bool v = true)
        {
            BackButton.gameObject.SetActive(v);
        }

        public void BackButtonBlick()
        {
            BackButton.GetComponentInChildren<BlickControl>().Enable();
        }

        internal void Hide()
        {
            Setup(0);
        }
    }
}
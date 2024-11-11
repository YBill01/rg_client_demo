using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassContentBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private BattlePassScrollContentBehaviour scrollContentBehaviour;
        [SerializeField] 
        private BattlePassTopPanelBehaviour topPanelBehaviour;
        [SerializeField] 
        private BattlePassPremiumPassPanelBehaviour premiumPassPanelBehaviour;
        
        private ProfileInstance profile;
        
        public Transform GetLeft()
        {
            return premiumPassPanelBehaviour.transform;
        }
        public void Init(ProfileInstance profileInstance, BattlePassWindowBehaviour battlePassWindow)
        {
            profile = profileInstance;
            var battlePassData = Shop.Instance.BattlePass.GetCurrent();

            if (battlePassData != null)
            {
                var currentLevel = (int) Mathf.Floor(profileInstance.battlePass.stars / 10);
                var starsInCurrentLevel = profileInstance.battlePass.stars - currentLevel * 10;
                var nextLevel = currentLevel + 1;
            
                if (nextLevel >= battlePassData.tresures.Count)
                    nextLevel = battlePassData.tresures.Count - 1;

                if (currentLevel >= battlePassData.tresures.Count)
                {
                    currentLevel = battlePassData.tresures.Count - 1;
                    starsInCurrentLevel = 10;
                }
                
                scrollContentBehaviour.Init(battlePassData, battlePassWindow);
                topPanelBehaviour.Init(battlePassData, nextLevel, starsInCurrentLevel);
                premiumPassPanelBehaviour.Init(profile, battlePassWindow);
            }
        }

        public void OpenWindow()
        {
            scrollContentBehaviour.OpenWindow();
        }
    }
}
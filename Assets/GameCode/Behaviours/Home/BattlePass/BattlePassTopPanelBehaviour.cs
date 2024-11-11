using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassTopPanelBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text seasonTitleText;
        [SerializeField] 
        private TMP_Text buttonPriceText;
        [SerializeField] 
        private TMP_Text buttonTitleText;
        [SerializeField] 
        private UITimerBehaviour timer;
        [SerializeField] 
        private TMP_Text nextLevelText;
        [SerializeField] 
        private ProgressBarChangeValueBehaviour progressBar;

        public void Init(BinaryBattlePass battlePass, int nextLevel, int starsInCurrentLevel)
        {
            buttonPriceText.text = "25";
            timer.SetFinishedTime(battlePass.timeEnd);

            buttonTitleText.text = "Buy level " + nextLevel;
            nextLevelText.text = "<size=50%>"+Locales.Get("locale:775")+"</size>"+ nextLevel;
            progressBar.Set((uint) starsInCurrentLevel, true, 10);
            seasonTitleText.text = Locales.Get(battlePass.title);
        }

        public void BuyLevelButtonClick()
        {
            
        }
    }
}
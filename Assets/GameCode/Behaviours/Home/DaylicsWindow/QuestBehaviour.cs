using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestBehaviour : MonoBehaviour
{
    [SerializeField] LootCurrencyBehaviour RegularReward;
    [SerializeField] LootCurrencyBehaviour ShardsReward;
    [SerializeField] ProgressBarChangeValueBehaviour ProgressBar;

    [SerializeField] LegacyButton ChangeButton;
    [SerializeField] LegacyButton infoButton;
    [SerializeField] LegacyButton CollectButton;
    [SerializeField] LegacyButton SkipButton;
    [SerializeField] ButtonWithPriceViewBehaviour SkipButtonWithPrice;

    [SerializeField] GameObject TimerLayout;
    [SerializeField] GameObject QuestLayout;

    [SerializeField] UITimerBehaviour Timer;

    [SerializeField] TMP_Text QuestDescription;
    [SerializeField] Image HeroQuestIcon;

    BinaryDaylic binaryDaylic;
    PlayerDaylic playerDaylic;
    public void InitData(PlayerDaylic playerDaylic)
    {
        this.playerDaylic = playerDaylic;
        if (Daylics.Instance.Get(playerDaylic.db_index, out BinaryDaylic binaryData))
        {
            binaryDaylic = binaryData;
            if (!playerDaylic.isFinished())
            {
                ProgressBar.Set(playerDaylic.completed, true, binaryDaylic.need);
            }
            else
            {

            }
        }
    }
}

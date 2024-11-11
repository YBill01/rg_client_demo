using DG.Tweening;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PopupAlertBehaviour;

public enum AlretPosition
{
    Center,
    OffTop
}
public class AlertsBehaviour : MonoBehaviour
{
    private int timeToCountSeconds = 9;
    private const int seconds = 9;
    public static AlertsBehaviour Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void ShowAlertQueue(string[] locales, int[] values, float delay)
    {
        if (battle.status <= BattleInstanceStatus.Prepare || battle.status >= BattleInstanceStatus.Pause)
            return;
        StartCoroutine(Cor(locales, values, delay));
    }

    private IEnumerator Cor(string[] locales, int[] values, float delay)
    {
        for (int i = 0; i < locales.Length; i++)
        {
            // if (values[i] == 0)
            PopupAlertBehaviour.ShowBattlePopupAlert(AlertPosition(AlretPosition.OffTop), Locales.Get(locales[i]));
            //else
            //    PopupAlertBehaviour.ShowBattlePopupAlert(AlertPosition(AlretPosition.OffTop), Locales.Get(locales[i], values[i]));

            if (battle.status <= BattleInstanceStatus.Prepare || battle.status >= BattleInstanceStatus.Pause)
                yield break;
            else
                yield return new WaitForSeconds(delay);
        }
    }
    string str = "";
    BattleInstance battle;
    public void GetBattle(BattleInstance battle)
    {
        this.battle = battle;
        if (battle.status <= BattleInstanceStatus.Prepare || battle.status >= BattleInstanceStatus.Pause)
        {
            StopInvokeTextAnimation();
            StopInvokeTextMessageAnimation();
        }
    }
    public void ShowAlert(string locale, ref BattleInstance battle)
    {
        this.battle = battle;
        var time = Int32.Parse(locale);
        if (battle.status <= BattleInstanceStatus.Prepare || battle.status >= BattleInstanceStatus.Pause)
        {
            StopInvokeTextAnimation();
            StopInvokeTextMessageAnimation();
            return;
        }
        if (time >= seconds)
        {
            timeToCountSeconds = seconds;
            str = "locale:1117";
            if (!IsInvoking("InvoreRepeatingAlertMessage"))
                InvokeRepeating("InvoreRepeatingAlertMessage", 0f, 5);
        }
        if (time < seconds)
        {
            StopInvokeTextMessageAnimation();

            str = (timeToCountSeconds).ToString();
            if (!IsInvoking("InvoreRepeatingAlert"))
                InvokeRepeating("InvoreRepeatingAlert", 0f, 1);
        }
        if (time == 0)
            StopInvokeTextAnimation();
    }

    private void InvoreRepeatingAlert()
    {
        PopupAlertBehaviour.ShowBattlePopupAlert(AlertPosition(AlretPosition.OffTop), Locales.Get(str), 1);
        timeToCountSeconds--;
    }
    private void InvoreRepeatingAlertMessage()
    {
        PopupAlertBehaviour.ShowBattlePopupAlert(AlertPosition(AlretPosition.OffTop), Locales.Get(str), 2f);
    }

    public void StopInvokeTextAnimation()
    {
        CancelInvoke("InvoreRepeatingAlert");
    }

    public void StopInvokeTextMessageAnimation()
    {
        CancelInvoke("InvoreRepeatingAlertMessage");
    }

    public void StopAlerts()
    {
        StopAllCoroutines();
        StopInvokeTextMessageAnimation();
        StopInvokeTextAnimation();
    }
    private static Vector2 AlertPosition(AlretPosition position)
    {
        Vector2 pos = Vector2.zero;
        switch (position)
        {
            case AlretPosition.Center:
                var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                pos = screenCenter;
                break;
            case AlretPosition.OffTop:
                var screenOffTop = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                pos = screenOffTop;
                break;
            default:
                var screenCenterDefault = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                pos = screenCenterDefault;
                break;
        }
        return pos;
    }
}
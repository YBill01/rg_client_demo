using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameLegacy/LoadingTips", fileName = "LoadingTips")]
public class LoadingTips : SettingObject
{
    public static LoadingTips Instance;

    public List<string> tips;
    public override void Init()
    {
        //Instance = this;
    }

    public string GetRandomTips()
    {
        return Locales.Get(tips[UnityEngine.Random.Range(0, tips.Count)]);
    }
}

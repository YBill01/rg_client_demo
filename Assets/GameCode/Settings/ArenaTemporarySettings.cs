using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameLegacy/ArenaTemporarySettings", fileName = "ArenaTemporarySettings")]
public class ArenaTemporarySettings : SettingObject
{
    public static ArenaTemporarySettings Instance;

    public uint RealArenasCount;
    public override void Init()
    {
        Instance = this;
    }

}

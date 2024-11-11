using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "GameLegacy/SkillParamsConfig", fileName = "SkillParamsConfig")]
public class SkillParamsConfig : SettingObject
{
    public static SkillParamsConfig Instance;
    [Serializable]
    public struct SkillStaticParam
    {
        public SkillParamType type;
        public Sprite icon;
        public string ShortTitle;
        public string FullTitle;
        public bool needDelails;
        public string additionalTailToValue;
    }


    [SerializeField] private List<SkillStaticParam> skillData;

    public override void Init()
    {
        Instance = this;
    }
    public SkillStaticParam GetSkillParamByType(SkillParamType type)
    {
        return skillData.LastOrDefault(x => x.type == type);
    }

}

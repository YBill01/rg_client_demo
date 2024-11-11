using UnityEngine;
using System;
using Legacy.Database;
using System.Collections.Generic;
using System.Linq;
using HeroSkillsConfig;

namespace HeroSkillsConfig
{
    public enum ClientSkillParamType
    {
        Heal,
        HealDuration,
        HellDuration,
        Damage,
        Radius,
        Width,
        RazorStun,
        Root,
        Knockback,
        Slow,
        MagmaBallDuration,
        RazorDamage,
        RazorRadius,
        TrapDamage,
        TrapRoot,
        TrapLifeDuration,
        TotemDamage,
        TotemSlow,
        TotemHeal,
        PiercingArrowKnockback,
        PiercingArrowStun,
        PlagueDamage,
       PlagueDuration,
        TouchDeathDamage,
        TouchDeathRadius,
        BlizzardDamage,
        Quantity,
        Summon,
        GrimvaldSlow,
        MagmaBallWidth,
        MagmaBallDistance,
        HellFireDamage,
        HellFireDuration,
        HellFireWidth


    }
}

[CreateAssetMenu(menuName = "GameLegacy/HeroSkillsParamConfig", fileName = "HeroSkillsParamConfig")]
public class HeroSkillsParamConfig : SettingObject
{
    public static HeroSkillsParamConfig Instance;
    [Serializable]
    public struct HeroSkillParams
    {
        public uint skillIndex;
        /// <summary>
        /// Только для читаемости в инспекторе юнити
        /// </summary>
        public string name;
        public List<ClientSkillParamType> parameters;
    }

    [SerializeField] private List<HeroSkillParams> skillData;

    public override void Init()
    {
        Instance = this;
    }

    public Dictionary<SkillParamType, string> GetSkillParamValues(BinarySkill bSkill, byte level)
    {
        var parameters = skillData.FirstOrDefault(x => x.skillIndex == bSkill.index).parameters;
        var result = new Dictionary<SkillParamType, string>();

        foreach (var type in parameters)
        {
            result.Add(HeroSkillsHelper.ClientToSkillParamType(type), HeroSkillsHelper.GetSkillValue(type, bSkill, level));
        }

        return result;
    }
}

public class HeroSkillsHelper
{
    public static SkillParamType ClientToSkillParamType(ClientSkillParamType type)
    {
        switch (type)
        {
            case ClientSkillParamType.Heal:
            case ClientSkillParamType.TotemHeal:
                return SkillParamType.Heal;

            case ClientSkillParamType.HellFireDuration:
            case ClientSkillParamType.PlagueDuration:
            case ClientSkillParamType.HealDuration:
            case ClientSkillParamType.HellDuration:
            case ClientSkillParamType.MagmaBallDuration:
            case ClientSkillParamType.TrapLifeDuration:
                return SkillParamType.Duration;

            case ClientSkillParamType.HellFireDamage:
            case ClientSkillParamType.BlizzardDamage:
            case ClientSkillParamType.TouchDeathDamage:
            case ClientSkillParamType.PlagueDamage:
            case ClientSkillParamType.Damage:
            case ClientSkillParamType.RazorDamage:
                return SkillParamType.Damage;

            case ClientSkillParamType.HellFireWidth:
            case ClientSkillParamType.MagmaBallDistance:
            case ClientSkillParamType.TouchDeathRadius:
            case ClientSkillParamType.Radius:
            case ClientSkillParamType.RazorRadius:
                return SkillParamType.Radius;

            case ClientSkillParamType.MagmaBallWidth:
            case ClientSkillParamType.Width:
                return SkillParamType.Width;

            case ClientSkillParamType.RazorStun:
            case ClientSkillParamType.PiercingArrowStun:
                return SkillParamType.Stun;

            case ClientSkillParamType.Root:
            case ClientSkillParamType.TrapRoot:
                return SkillParamType.Root;

            case ClientSkillParamType.Knockback:
            case ClientSkillParamType.PiercingArrowKnockback:
                return SkillParamType.Knockback;

            case ClientSkillParamType.GrimvaldSlow:
            case ClientSkillParamType.Slow:
            case ClientSkillParamType.TotemSlow:
                return SkillParamType.Slow;

            case ClientSkillParamType.TrapDamage:
            case ClientSkillParamType.TotemDamage:
                return SkillParamType.DamagePerSecond;

            case ClientSkillParamType.Quantity:
                return SkillParamType.Quantity;
            case ClientSkillParamType.Summon:
                return SkillParamType.Summon;

        }
        return SkillParamType.Lvl;
    }

    public static string GetSkillValue(ClientSkillParamType type, BinarySkill bSkill, byte level)
    {
        var result = -1f;
        switch (type)
        {
            case ClientSkillParamType.Heal:
                result = GetDamage(bSkill, level);
                break;
            case ClientSkillParamType.HealDuration:
                result = GetHealDuration(bSkill);
                break;
            case ClientSkillParamType.Damage:
                result = GetDamage(bSkill, level);
                break;
            case ClientSkillParamType.Radius:
                result = GetRadius(bSkill);
                break;
            case ClientSkillParamType.Width:
                break;
            case ClientSkillParamType.PiercingArrowStun:
            case ClientSkillParamType.RazorStun:
                result = GetRazorStun(bSkill, level);
                break;
            case ClientSkillParamType.Root:
                break;
            case ClientSkillParamType.Knockback:
                result = GetKnockback(bSkill);
                break;
            case ClientSkillParamType.Slow:
                break;
            case ClientSkillParamType.HellDuration:
                break;
            case ClientSkillParamType.MagmaBallDuration:
                break;
            case ClientSkillParamType.RazorRadius:
                result = GetRazorRadius(bSkill, level);
                break;
            case ClientSkillParamType.RazorDamage:
                result = GetRazorDamage(bSkill, level);
                break;
            case ClientSkillParamType.TrapDamage:
                result = GetTrapDamage(bSkill, level);
                break;
            case ClientSkillParamType.TrapRoot:
                result = GetTrapRoot(bSkill);
                break;
            case ClientSkillParamType.TrapLifeDuration:
                result = GetTrapDuration(bSkill);
                break;
            case ClientSkillParamType.TotemDamage:
                result = GetTotemDamage(bSkill, level);
                break;
            case ClientSkillParamType.TotemSlow:
                result = GetTotemSlow(bSkill, level);
                break;
            case ClientSkillParamType.TotemHeal:
                result = GetTotemHeal(bSkill, level);
                break;
            case ClientSkillParamType.PiercingArrowKnockback:
                result = GetPiercingArrowKnockback(bSkill, level);
                break;
            /////////////////////
            case ClientSkillParamType.PlagueDamage:
                result = GetPlagueDamage(bSkill, level);
                break;
            case ClientSkillParamType.PlagueDuration:
                result = GetPlagueDuratione(bSkill, level);
                break;
            case ClientSkillParamType.TouchDeathRadius:
                result = GetTouchDeathRadius(bSkill, level);
                break;
            case ClientSkillParamType.TouchDeathDamage:
                result = GetTouchDeathDamage(bSkill, level);
                break;
            case ClientSkillParamType.BlizzardDamage:
                result = GetBlizzardDamage(bSkill, level);
                break;
            case ClientSkillParamType.Quantity:
                result = GetQuantity(bSkill, level);
                break;
            case ClientSkillParamType.Summon:
                result = GetSummon(bSkill, level);
                break;

            case ClientSkillParamType.GrimvaldSlow:
                result = GetGrimvaldSlow(bSkill, level);
                return "x" + result.ToString();

            case ClientSkillParamType.MagmaBallWidth:
                result = GetMagmaBallWidth(bSkill, level);
                break;

            case ClientSkillParamType.MagmaBallDistance:
                result = GetMagmaBallDistance(bSkill, level);
                break;
            case ClientSkillParamType.HellFireDamage:
                result = GetHellFireDamage(bSkill, level);
                break;
            case ClientSkillParamType.HellFireDuration:
                result = GetHellFireDuration(bSkill, level);
                break;
            case ClientSkillParamType.HellFireWidth:
                result = GetHellFireWidth(bSkill, level);
                break;
        }



        var skillParamType = ClientToSkillParamType(type);

        switch (skillParamType)
        {

            case SkillParamType.Slow:
                return "x" + Mathf.RoundToInt(100 / result).ToString();
            case SkillParamType.Summon:
                {
                    Entities.Instance.Get((ushort)result, out BinaryEntity binary);
                    return Locales.Get("entities:" + binary.index + ":title"); ;
                }
            default:
                return result == Mathf.RoundToInt(result) || result > 10 ? Mathf.RoundToInt(result).ToString() : string.Format("{0:F1}", result);
        }
    }
    private static float GetHellFireWidth(BinarySkill bSkill, byte level)
    {
        float width = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {

                var effects = Components.Instance.Get<EffectMassCreateEffect>();
                if (effects.TryGetValue(e, out EffectMassCreateEffect dData))
                {
                    if (dData.effect.length > 0)
                    {
                        for (byte i = 0; i < dData.effect.length; i++)
                        {
                            var e2 = dData.effect[i];
                            var effects2 = Components.Instance.Get<EffectRadius>();
                            if (effects2.TryGetValue(e2, out EffectRadius dData2))
                            {
                                width = dData2.radius;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }
        return width;
    }

    private static float GetHellFireDuration(BinarySkill bSkill, byte level)
    {
        float duration = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {

                var effects = Components.Instance.Get<EffectMassCreateEffect>();
                if (effects.TryGetValue(e, out EffectMassCreateEffect dData))
                {
                    if (dData.effect.length > 0)
                    {
                        for (byte i = 0; i < dData.effect.length; i++)
                        {
                            var e2 = dData.effect[i];
                            var effects2 = Components.Instance.Get<EffectFollowTarget>();
                            if (Effects.Instance.Get(e2, out BinaryEffect beffect2))
                            {
                                duration = beffect2.duration.value / 1000;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }
        return duration;
    }
    private static float GetHellFireDamage(BinarySkill bSkill, byte level)
    {
        float damage = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectMassCreateEffect>();
                if (effects.TryGetValue(e, out EffectMassCreateEffect dData))
                {
                    if (dData.effect.length > 0)
                    {
                        for (byte i = 0; i < dData.effect.length; i++)
                        {
                            var e2 = dData.effect[i];
                            var effects2 = Components.Instance.Get<EffectDamage>();
                            if (effects2.TryGetValue(e2, out EffectDamage dData2))
                            {
                                damage = dData2.damage._value(level);
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }
        return damage;
    }

    private static float GetMagmaBallDistance(BinarySkill bSkill, byte level)
    {
        float distance = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectLine>();
                if (effects.TryGetValue(e, out EffectLine dData))
                {
                    distance = dData.distance;//
                    break;
                }
            }
        }
        return distance;
    }

    private static float GetMagmaBallWidth(BinarySkill bSkill, byte level)
    {
        float width = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectWave>();
                if (effects.TryGetValue(e, out EffectWave dData))
                {
                    width = dData.size;//
                    break;
                }
            }
        }
        return width;
    }

    private static float GetGrimvaldSlow(BinarySkill bSkill, byte level)
    {
        float damage = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {

                var effects = Components.Instance.Get<EffectMassCreateEffect>();
                if (effects.TryGetValue(e, out EffectMassCreateEffect dData))
                {
                    if (dData.effect.length > 0)
                    {
                        for (byte i = 0; i < dData.effect.length; i++)
                        {
                            var e2 = dData.effect[i];
                            var effects2 = Components.Instance.Get<EffectMinionMovement>();
                            if (effects2.TryGetValue(e2, out EffectMinionMovement dData2))
                            {
                                damage = dData2.speed.rate.value;
                                break;
                            }
                        }
                    }
                }
            }
        }
        return damage;
    }

    private static float GetSummon(BinarySkill bSkill, byte level)
    {
        float count = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectSpawnMinionCustomPos>();
                if (effects.TryGetValue(e, out EffectSpawnMinionCustomPos dData))
                {
                    count = dData.entity[0];
                    break;
                }
            }
        }
        return count;
    }

    private static float GetQuantity(BinarySkill bSkill, byte level)
    {
        float count = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectSpawnMinionCustomPos>();
                if (effects.TryGetValue(e, out EffectSpawnMinionCustomPos dData))
                {
                    count = dData.entity.length;
                    if (count == 1) // костыль для гринвальда... убрать как заполнится в админке.
                    {
                        count = 6;  
                    }
                    break;
                }
            }
        }
        return count;
    }
    private static float GetBlizzardDamage(BinarySkill bSkill, byte level)
    {
        float damage = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {

              //  foreach (var  com in beffect.components)
              //  {
                 //  \\ if(com == 62)
                 //   {
                        var effects = Components.Instance.Get<EffectMassCreateEffect>();
                        if (effects.TryGetValue(e, out EffectMassCreateEffect dData))
                        {
                            if (dData.effect.length > 0)
                            {
                                for (byte i = 0; i < dData.effect.length; i++)
                                {
                                    var e2 = dData.effect[i];
                                    var effects2 = Components.Instance.Get<EffectDamage>();
                                    if (effects2.TryGetValue(e2, out EffectDamage dData2))
                                    {
                                        damage = dData2.damage._value(level);
                                        break;
                                    }
                                }
                            }
                           }

                   //// }
              //  }
                
            }
        }
        return damage;
    }
    private static float GetTouchDeathRadius(BinarySkill bSkill, byte level)
    {
        float radius = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<TouchOfDeathEffect>();
                if (effects.TryGetValue(e, out TouchOfDeathEffect dData))
                {
                    radius= dData.radius;
                    break;
                }
            }
        }
        return radius;
    }
    private static float GetTouchDeathDamage(BinarySkill bSkill, byte level)
    {
        float damage = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectDamage>();
                if (effects.TryGetValue(e, out EffectDamage dData))
                {
                    damage = dData.damage._value(level);
                }
            }
        }
        return damage;
    }

    private static float GetPlagueDuratione(BinarySkill bSkill, byte level)
    {
        float duration = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {

                duration = beffect.duration.value/1000;
            }
        }
        return duration;
    }

    private static float GetPlagueDamage(BinarySkill bSkill, byte level)
    {
        float damage = 0;
        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectMassCreateEffect>();
                if (effects.TryGetValue(e, out EffectMassCreateEffect dData))
                {
                    if (dData.effect.length > 0)
                    {
                        var e2 = dData.effect[0];
                        var effects2 = Components.Instance.Get<EffectDamage>();
                        if (effects2.TryGetValue(e2, out EffectDamage dData2))
                        {
                            damage = dData2.damage._value(level);
                            break;
                        }
                    }
                    break;
                }
            }
        }
        return damage;
    }

	private static float GetDamage(BinarySkill bSkill, byte level)
    {
        float damage = 0;

        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectDamage>();
                if (effects.TryGetValue(e, out EffectDamage dData))
                {
                    damage = dData.damage._value(level);
                    break;
                }
            }
        }

        return damage;
    }

    private static float GetKnockback(BinarySkill bSkill)
    {
        float damage = 0;

        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectPush>();
                if (effects.TryGetValue(e, out EffectPush dData))
                {
                    damage = dData.distance;
                    break;
                }
            }
        }

        return damage;
    }

    private static float GetSlow(BinarySkill bSkill, byte level)
    {
        float result = 0;

        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectMinionMovement>();
                if (effects.TryGetValue(e, out EffectMinionMovement dData))
                {
                    result = dData.speed.rate._value(level);
                }
            }
        }

        return result;
    }

    private static float GetRadius(BinarySkill bSkill)
    {
        float result = 0;

        foreach (var e in bSkill.effects)
        {
            if (Effects.Instance.Get(e, out BinaryEffect beffect))
            {
                var effects = Components.Instance.Get<EffectRadius>();
                if (effects.TryGetValue(e, out EffectRadius dData))
                {
                    result = dData.radius;
                }
            }
        }

        return result;
    }

    private static float GetRazorStun(BinarySkill bSkill, byte level)
    {
        var effectIndex = bSkill.effects[0];

        var eees = Components.Instance.Get<EffectEventEffects>();
        if (!eees.TryGetValue(effectIndex, out var eee))
            return -3;

        if (!Effects.Instance.Get(eee.effect1.effect, out BinaryEffect razorParalize))
            return -4;

        return razorParalize.duration._value(level) / 1000f;
    }

    private static float GetRazorDamage(BinarySkill bSkill, byte level)
    {
        var effectIndex = bSkill.effects[0];

        var effects = Components.Instance.Get<EffectBloodyRazor>();
        if (!effects.TryGetValue(effectIndex, out var effect))
            return -3;

        return effect.power._value(level);
    }

    private static float GetTrapSkills(BinarySkill trapSkill, out MinionSkills skills)
    {
        var effectIndex = trapSkill.effects[0];
        skills = default;

        var effects = Components.Instance.Get<EffectSpawnMinion>();
        if (!effects.TryGetValue(effectIndex, out var effect))
            return -3;

        var minionIndex = effect.entity[0];

        if (!Components.Instance.Get<MinionSkills>().TryGetValue(minionIndex, out skills))
            return -5;

        return 0;
    }

    private static float GetTrapDamage(BinarySkill bSkill, byte level)
    {
        var found = GetTrapSkills(bSkill, out var skills);
        if (found < 0)
            return found;

        if (!Skills.Instance.Get(skills.skill2, out var trapDamageSkill))
            return -6;

        var damageEffects = Components.Instance.Get<EffectDamage>();
        if (!damageEffects.TryGetValue(trapDamageSkill.effects[0], out var effectDamage))
            return -7;

        return effectDamage.damage._value(level);
    }

    private static float GetTrapRoot(BinarySkill bSkill)
    {
        var found = GetTrapSkills(bSkill, out var skills);
        if (found < 0)
            return found;

        if (!Skills.Instance.Get(skills.skill1, out var trapDamageSkill))
            return -6;

        if (!Effects.Instance.Get(trapDamageSkill.effects[0], out var binaryEffect))
            return -7;

        return binaryEffect.duration.value / 1000f;
    }

    private static float GetTrapDuration(BinarySkill bSkill)
    {
        var found = GetTrapSkills(bSkill, out var skills);
        if (found < 0)
            return found;

        if (!Skills.Instance.Get(skills.skill3, out var trapDeathSkill))
            return -6;

        return trapDeathSkill.cooldown.value / 1000f;
    }

    private static float GetRazorRadius(BinarySkill bSkill, byte level)
    {
        var effectIndex = bSkill.effects[0];

        var effects = Components.Instance.Get<EffectBloodyRazor>();
        if (!effects.TryGetValue(effectIndex, out var effect))
            return -3;

        return effect.radius;
    }

    private static float GetHealDuration(BinarySkill bSkill)
    {
        var effectIndex = bSkill.effects[0];

        if (!Effects.Instance.Get(effectIndex, out BinaryEffect effect))
            return -2;

        return effect.iteration * effect.iterationsCount / 1000f;
    }

    private static float GetTotemDamage(BinarySkill bSkill, byte level)
    {
        var effectIndex = bSkill.effects[0];

        var effects = Components.Instance.Get<EffectSpawnMinionsByField>();
        if (!effects.TryGetValue(effectIndex, out var effect))
            return -2;

        if (!Components.Instance.Get<MinionSkills>().TryGetValue(effect.allyZoneEntity[0], out var skills))
            return -3;

        if (!Skills.Instance.Get(skills.skill1, out var damageSkill))
            return -4;

        return GetDamage(damageSkill, level);
    }

    private static float GetTotemHeal(BinarySkill bSkill, byte level)
    {
        var effectIndex = bSkill.effects[0];

        var effects = Components.Instance.Get<EffectSpawnMinionsByField>();
        if (!effects.TryGetValue(effectIndex, out var effect))
            return -2;

        if (!Components.Instance.Get<MinionSkills>().TryGetValue(effect.enemyZoneEntity[0], out var skills))
            return -3;

        if (!Skills.Instance.Get(skills.skill1, out var damageSkill))
            return -4;

        return GetDamage(damageSkill, level);
    }



    private static float GetTotemSlow(BinarySkill bSkill, byte level)
    {
        var effectIndex = bSkill.effects[0];

        var effects = Components.Instance.Get<EffectSpawnMinionsByField>();
        if (!effects.TryGetValue(effectIndex, out var effect))
            return -2;

        if (!Components.Instance.Get<MinionSkills>().TryGetValue(effect.bridgeZoneEntity[0], out var skills))
            return -3;

        if (!Skills.Instance.Get(skills.skill1, out var damageSkill))
            return -4;

        return GetSlow(damageSkill, level);
    }
   
    private static float GetPiercingArrowKnockback(BinarySkill bSkill, byte level)
    {
        var effectIndex = bSkill.effects[0];

        var eees = Components.Instance.Get<EffectEventEffects>();
        if (!eees.TryGetValue(effectIndex, out var eee))
            return -2;

        var effects = Components.Instance.Get<EffectPush>();
        if (!effects.TryGetValue(eee.effect2.effect, out var effect))
            return -3;

        return effect.distance;
    }
}

using Legacy.Database;
using Unity.Mathematics;
using UnityEngine;
using static Legacy.Client.HeroParamBehaviour;
using System;


namespace Legacy.Client
{
    public class CardParams : MonoBehaviour
    {
        private BinaryCard binaryCard;
        private ClientCardData PlayerCard;
        private BaseBattleSettings settings;

        public BinaryEntity BinaryMinion { get; private set; }
        public MinionOffence Offence { get; private set; }
        public MinionDefence Defence { get; private set; }
        public MinionSkills Skills { get; private set; }
        public MinionMovement Movement { get; private set; }
        public static string GetAttackSpeedPrefix(float value)
        {
            if (value > 2f)
                return Locales.Get("locale:1456");
            else if (value > 1.5f)
                return Locales.Get("locale:994");
            else if (value > 1f)
                return Locales.Get("locale:997");
            else if (value > 0.5f)
                return Locales.Get("locale:1000");
            else
                return Locales.Get("locale:1003");
        }

        public static string GetMovementSpeedPrefix(float value)
        {
            if (value > 1.7f)
                return Locales.Get("locale:1456");
            else if (value > 1.2f)
                return Locales.Get("locale:994");
            else if (value > 1f)
                return Locales.Get("locale:997");
            else if (value > 0.6f)
                return Locales.Get("locale:1000");
            else
                return Locales.Get("locale:1003");
        }

        public static string GetAttackTypePrefix(AttackType type)
        {
            switch (type)
            {
                case AttackType.Mellee:
                    return Locales.Get("locale:925");
                case AttackType.Range:
                    return Locales.Get("locale:928");
                default:
                    return "";
            }
        }

        public static string GetMovementTypePrefix(MinionLayerType type)
        {
            switch (type)
            {
                case MinionLayerType.Ground:
                    return Locales.Get("locale:913");
                case MinionLayerType.Fly:
                    return Locales.Get("locale:991");
                default:
                    return "";
            }
        }

        public static string GetTargetTypeLocales(MinionLayerType targetType)
        {
            switch (targetType)
            {
                case MinionLayerType.Hero:
                    return Locales.Get("locale:916");
                case MinionLayerType.Ground:
                    return Locales.Get("locale:913");
                case MinionLayerType.Fly:
                    return Locales.Get("locale:991");
                default:
                    return "";
            }
        }


        private void GetEntityComponents(ushort id)
        {
            BinaryMinion = default;
            Offence = default;
            Movement = default;
            Defence = default;
            Skills = default;
            if (Entities.Instance.Get(id, out BinaryEntity binaryMinion))
            {
                BinaryMinion = binaryMinion;
                if (Components.Instance.Get<MinionOffence>().TryGetValue(BinaryMinion.index, out MinionOffence offence))
                {
                    Offence = offence;
                }
                if (Components.Instance.Get<MinionDefence>().TryGetValue(BinaryMinion.index, out MinionDefence defence))
                {
                    Defence = defence;
                }
                if (Components.Instance.Get<MinionMovement>().TryGetValue(BinaryMinion.index, out MinionMovement movement))
                {
                    Movement = movement;
                }
                if (Components.Instance.Get<MinionSkills>().TryGetValue(BinaryMinion.index, out MinionSkills skills))
                {
                    Skills = skills;
                }
            }
        }
        private ProfileInstance profile;

        public string GetAttackType(ushort indexUnit)
        {
            GetEntityComponents(indexUnit);
            return  GetAttackTypePrefix(Offence.type);
        }

        public string GetMovement(ushort indexUnit)
        {
            GetEntityComponents(indexUnit);
            var type = MinionLayerType.Ground;
            if (BinaryMinion.type == MinionLayerType.Building)
            {
                type = MinionLayerType.Building;
            }

            if (BinaryMinion.type == MinionLayerType.Fly)
            {
                type = MinionLayerType.Fly;
            }
            var stringValue = GetMovementTypePrefix(type) + " : " + GetMovementSpeedPrefix(Movement.speed);
            if (BinaryMinion.type == MinionLayerType.Building)
                stringValue = GetMovementTypePrefix(type);

            return stringValue;
        }
        public float SetEntetyParams(ushort indexUnit , UnitParamType unitType, byte _level = 0,float dValue=0f)
        {
            float value = 0.0f;
            if (profile == null)
            {
                profile = ClientWorld.Instance.GetExistingSystem<HomeSystems>().UserProfile;
                settings = Settings.Instance.Get<BaseBattleSettings>();
            }
            GetEntityComponents(indexUnit);

            float damage = Offence._damage(
                settings.minions.damage,
                _level
            );

            switch (unitType)
            {
                case UnitParamType.AttackSpeed:
                    value = (float)Offence.duration / 1000;
                    break;
                case UnitParamType.AttackType:
                    break;
                case UnitParamType.Movement:
                    break;
                case UnitParamType.DMG:
                    value = damage;
                    break;
                case UnitParamType.Splash:
                    var newValue = GetSplashValue(value);
                    value = Offence._customDamage(
                          newValue,
                          settings.minions.damage,
                         _level
                     );
                    //           Params[i].gameObject.SetActive(value > 0);
                    break;
                case UnitParamType.Piercing_DMG:
                    newValue = GetSkillEffect("Piercing");
                    value = Offence._customDamage(
                         newValue,
                         settings.minions.damage,
                        _level
                    );
                    //           Params[i].gameObject.SetActive(value > 0);
                    break;
                case UnitParamType.DPS:
                    if (damage > 0)
                    {
                        value = math.round(damage * 1000 / Offence.duration);
                    }
                    else
                    {
                        value = math.round(dValue * 1000 / Offence.duration);
                    }
                    break;
                case UnitParamType.HP:
                    value = Defence._health(settings.minions.health, _level);
                    break;
                case UnitParamType.AreaDamage:
                    break;
                case UnitParamType.Targets:
                    value = (float)Offence.target;
                    break;
                case UnitParamType.Quantity:
                    break;
                case UnitParamType.Knockback:
                    value = GetSkillEffect("EffectPushBack");
                    break;
                case UnitParamType.Summon:
                    value = GetSkillEffect("EffectSpawnUnit");
                    break;
                case UnitParamType.Sturm_Damage:
                    value = GetSkillEffect("EffectHill");
                    if (value > 0)
                    {
                        value = Offence._customDamage(
                         value,
                         settings.minions.damage,
                         _level
                     );
                    }
                    break;
                case UnitParamType.Life_Time:
                    value = GetSkillEffect("EffectLife");
                    break;
                default:
                    break;
            }
            return value;
        }
        public float SetParams(BinaryCard bc, UnitParamType unitType,float dValue=0f, byte _level = 0)
        {
            float value = 0.0f;
            if (profile == null)
            {
                profile = ClientWorld.Instance.GetExistingSystem<HomeSystems>().UserProfile;
                settings = Settings.Instance.Get<BaseBattleSettings>();
            }
            PlayerCard = profile.Inventory.GetCardData(bc.index);
            if (_level == 0)
                _level = PlayerCard.level;
            GetEntityComponents(bc.entities[0]);

                    float damage = Offence._damage(
                        settings.minions.damage,
                        _level
                    );

                switch (unitType)
                {
                    case UnitParamType.AttackSpeed:
                        value = (float)Offence.duration / 1000;
                        break;
                    case UnitParamType.AttackType:
                        break;
                    case UnitParamType.Movement:
                        break;
                    case UnitParamType.DMG:
                        value = damage;
                        break;
                    case UnitParamType.Splash:
                        var newValue = GetSplashValue(value);
                        value = Offence._customDamage(
                              newValue,
                              settings.minions.damage,
                             _level
                         );
             //           Params[i].gameObject.SetActive(value > 0);
                        break;
                    case UnitParamType.Piercing_DMG:
                        newValue = GetSkillEffect("Piercing");
                         value = Offence._customDamage(
                              newValue,
                              settings.minions.damage,
                             _level
                         );
                        //           Params[i].gameObject.SetActive(value > 0);
                        break;
                    case UnitParamType.DPS:
                        if (damage > 0)
                        {
                            value = math.round(damage * 1000 / Offence.duration);
                        }
                        else
                        {
                            value = math.round(dValue * 1000 / Offence.duration);
                        }
                        break;
                    case UnitParamType.HP:
                        value = Defence._health(settings.minions.health, _level);
                        break;
                    case UnitParamType.AreaDamage:
                        break;
                    case UnitParamType.Targets:
                        break;
                    case UnitParamType.Quantity:
                        value = (byte)bc.entities.Count;
                        break;
                    case UnitParamType.Knockback:
                        value = GetSkillEffect("EffectPushBack");
                        break;
                    case UnitParamType.Summon:
                        value = GetSkillEffect("EffectSpawnUnit");
                    break;
                    case UnitParamType.Sturm_Damage:
                        value = GetSkillEffect("EffectHill");
                        if (value > 0)
                        {
                            value = Offence._customDamage(
                             value,
                             settings.minions.damage,
                             _level
                         );
                        }
                        break;
                case UnitParamType.Life_Time:
                    value = GetSkillEffect("EffectLife");
                    break;
                default:
                        break;
                }
                return value;
        }

        private float GetSplashValue(float damage)
        {
            if (damage == 0)
            {
                if (Skills.skill1 == 3 || Skills.skill2 == 3 || Skills.skill3 == 3)
                {
                    if (Legacy.Database.Skills.Instance.Get(3, out BinarySkill bskill))
                    {
                        foreach (var e in bskill.effects)
                        {
                            if (Legacy.Database.Effects.Instance.Get(e, out BinaryEffect beffect))
                            {
                                if (beffect.components.Contains(36) && beffect.components.Contains(66) && bskill.effects.Count == 2)
                                {
                                    //Legacy.Database.Components.Instance.Get(36,out bin);
                                    var effects = Components.Instance.Get<EffectDamage>();
                                    if (effects.TryGetValue(e, out EffectDamage dData))
                                    {
                                        return dData.damage.value;

                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                for (byte k = 0; k < MinionSkills.Count; ++k)
                {
                    if (Skills.Get(k, out ushort skill))
                    {
                        damage = EffectSlashDamdge(skill);
                        if (damage > 0)
                            return damage;
                    }
                }

            }
            return damage;
        }


        private float EffectSlashDamdge(ushort index)
        {
            if (Legacy.Database.Skills.Instance.Get(index, out BinarySkill bskill))
            {
                foreach (var e in bskill.effects)
                {
                    if (Legacy.Database.Effects.Instance.Get(e, out BinaryEffect beffect))
                    {
                        if (beffect.components.Count > 0)
                        {
                            var effects = Components.Instance.Get<EffectSlash>();
                            if (effects.TryGetValue(e, out EffectSlash dData))
                            {
                                return dData.damage.value;
                            }
                        }
                    }
                }
            }
            return 0f;
        }

        private float GetSkillEffect(String effect)
        {
            float rez = 0f;
            for (byte k = 0; k < MinionSkills.Count; ++k)
            {
                if (Skills.Get(k, out ushort skill))
                {

                    switch (effect) 
                    {
                        case "EffectPushBack":
                            if (skill == 6)
                                rez = EffectPushBack(skill); break;
                        case "EffectHill":
                            if (skill == 65)
                                rez = EffectHill(skill); break; 
                        case "EffectLife":
                            if(skill == 80 || skill == 78 || skill == 79 || skill == 81)
                                 rez = EffectLife(skill); break;
                        case "Piercing":
                            if (skill == 83)
                                rez = EffectPiercing(skill); break; 
                           case "EffectSpawnUnit":
                            if (skill == 74 || skill == 67 )
                                  rez =  EffectSpawnUnit(skill);
                            break;
                    }
                }
                if (rez > 0) break;

            }
            return rez;
        }
        private float EffectSpawnUnit(ushort index)
        {
            if (Legacy.Database.Skills.Instance.Get(index, out BinarySkill bskill))
            {
                foreach (var e in bskill.effects)
                {
                    if (Legacy.Database.Effects.Instance.Get(e, out BinaryEffect beffect))
                    {
                        if (beffect.components.Count > 0)
                        {
                            var effects = Components.Instance.Get<EffectSpawnMinion>();
                            if (effects.TryGetValue(e, out EffectSpawnMinion dData))
                            {
                                return (float)dData.entity[0];
                            }
}
                    }
                }
            }
            return 0f;
        }
        private float EffectPiercing(ushort index)
        {
            if (Legacy.Database.Skills.Instance.Get(index, out BinarySkill bskill))
            {
                foreach (var e in bskill.effects)
                {
                    if (Legacy.Database.Effects.Instance.Get(e, out BinaryEffect beffect))
                    {
                        if (beffect.components.Count > 0)
                        {
                            var effects = Components.Instance.Get<EffectDamage>();
                            if (effects.TryGetValue(e, out EffectDamage dData))
                            {
                                return dData.damage.value;
                            }
                        }
                    }
                }
            }
            return 0f;
        }

        private float EffectLife(ushort index)
        {
            if (Legacy.Database.Skills.Instance.Get(index, out BinarySkill bskill))
            {
                foreach (var e in bskill.effects)
                {
                    if (Legacy.Database.Effects.Instance.Get(e, out BinaryEffect beffect))
                    {
                        if (beffect.components.Count > 0 )
                        {
                            var effects = Components.Instance.Get<BuildingDamage>();
                            if (effects.TryGetValue(e, out BuildingDamage dData))
                            {
                                return dData.duration/1000;
                            }
                        }
                    }
                }
            }
            return 0f;
        }
        private float EffectHill(ushort index)
        {
            if (Legacy.Database.Skills.Instance.Get(index, out BinarySkill bskill))
            {
                foreach (var e in bskill.effects)
                {
                    if (Legacy.Database.Effects.Instance.Get(e, out BinaryEffect beffect))
                    {
                        if (beffect.components.Count > 0)
                        {
                            var effects = Components.Instance.Get<EffectDamage>();
                            if (effects.TryGetValue(e, out EffectDamage dData))
                            {
                                return dData.damage.value;
                            }
                        }
                    }
                }
            }
            return 0f;
        }

        private float EffectPushBack(ushort index)
        {
            if (Legacy.Database.Skills.Instance.Get(index, out BinarySkill bskill))
            {
                foreach (var e in bskill.effects)
                {
                    if (Legacy.Database.Effects.Instance.Get(e, out BinaryEffect beffect))
                    {
                        if (beffect.components.Count > 0)
                        {
                            var effects = Components.Instance.Get<EffectPush>();
                            if (effects.TryGetValue(e, out EffectPush dData))
                            {
                                return dData.distance;
                            }
                        }
                    }
                }
            }
            return 0f;
        }
    }

}
using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CardGridUpBehaviour : MonoBehaviour
{
    [SerializeField] GameObject ParamPrefab;
    [SerializeField] List<HeroParamBehaviour> cardParams = new List<HeroParamBehaviour>();

    public List<HeroParamBehaviour> AddParamsToSkill(BinaryCard bCard)
    {
        GetEntityComponents(bCard.entities[0], bCard);
        return cardParams;
    }

    public BinaryEntity BinaryMinion { get; private set; }
    public MinionOffence Offence { get; private set; }
    public MinionDefence Defence { get; private set; }
    public MinionSkills Skills { get; private set; }
    public MinionMovement Movement { get; private set; }
    private ClientCardData PlayerCard;
    private BaseBattleSettings settings;
    private CardParams _cardParams = new CardParams();
    private void GetEntityComponents(ushort id, BinaryCard bCard)
    {
        BinaryMinion = default;
        Offence = default;
        Movement = default;
        Defence = default;
        Skills = default;
        PlayerCard = ClientWorld.Instance.Profile.Inventory.GetCardData(bCard.index);
        settings = Settings.Instance.Get<BaseBattleSettings>();
        var newLevel = PlayerCard.level + 1;
        float canUpgrade = 0.0f;
        var paramIndex = 0;
        if (Entities.Instance.Get(id, out BinaryEntity binaryMinion))
        {
            float value = 0.0f;
            BinaryMinion = binaryMinion;
            if (Components.Instance.Get<MinionSkills>().TryGetValue(BinaryMinion.index, out MinionSkills skills))
            {
                Skills = skills;
            }
            if (Components.Instance.Get<MinionDefence>().TryGetValue(BinaryMinion.index, out MinionDefence defence))
            {
                Defence = defence;
                //float hp = CountHp();
                float hp = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.HP);
                canUpgrade = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.HP, 0, (byte)newLevel);
                //hp
                cardParams[paramIndex].UpdateParamView(HeroParamBehaviour.UnitParamType.HP);
               
                cardParams[paramIndex].SetValue(hp, true, canUpgrade);
                cardParams[paramIndex].gameObject.SetActive(hp > 0);
            }
            HeroParamBehaviour.UnitParamType typeDamage = HeroParamBehaviour.UnitParamType.DMG;
            if (Components.Instance.Get<MinionOffence>().TryGetValue(BinaryMinion.index, out MinionOffence offence))
            {
                Offence = offence;
                float damage = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.DMG); //CountDamage();
                if (damage == 0)
                {
                    typeDamage = HeroParamBehaviour.UnitParamType.Splash;
                    damage = _cardParams.SetParams(bCard, typeDamage);
                }

                if (damage == 0)
                {
                    typeDamage = HeroParamBehaviour.UnitParamType.Piercing_DMG;
                    damage = _cardParams.SetParams(bCard, typeDamage);
                }
                // if (damage != 0) value = damage;

                //dmg
                float dmgUpd = 0f;
                ++paramIndex;
                /*  if (!isSplah)
                  {
                      cardParams[paramIndex].UpdateParamView(HeroParamBehaviour.UnitParamType.DMG);
                      canUpgrade = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.DMG, 0, (byte)newLevel);
                  }
                  else
                  {
                      cardParams[paramIndex].UpdateParamView(HeroParamBehaviour.UnitParamType.Splash);
                      canUpgrade = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.Splash, 0, (byte)newLevel);
                  }*/
                cardParams[paramIndex].UpdateParamView(typeDamage);
                canUpgrade = _cardParams.SetParams(bCard, typeDamage, 0, (byte)newLevel);

                dmgUpd = canUpgrade;
                cardParams[paramIndex].SetValue(damage, true, canUpgrade);

              //  value = CountMinionDamage(damage);
               // value = CountMinionDamage(value);
               value = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.DPS, damage);
                cardParams[paramIndex].gameObject.SetActive(damage > 0);
                //dps
                ++paramIndex;
                /*  if (!isSplah)
                  {
                      cardParams[paramIndex].UpdateParamView(HeroParamBehaviour.UnitParamType.DPS);
                      canUpgrade = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.DPS, dmgUpd, (byte)newLevel);
                  }
                  else
                  {
                      cardParams[paramIndex].UpdateParamView(HeroParamBehaviour.UnitParamType.DPS_splash);
                      canUpgrade = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.DPS, dmgUpd, (byte)newLevel);
                  }*/
                cardParams[paramIndex].UpdateParamView(HeroParamBehaviour.UnitParamType.DPS);
                canUpgrade = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.DPS, dmgUpd, (byte)newLevel);

                cardParams[paramIndex].SetValue(value, true, canUpgrade);
                cardParams[paramIndex].gameObject.SetActive(value > 0);
                //Sturm damage
                value = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.Sturm_Damage);
                ++paramIndex;
                if (value > 0)
                {
                    cardParams[paramIndex].UpdateParamView(HeroParamBehaviour.UnitParamType.Sturm_Damage);
                    canUpgrade = _cardParams.SetParams(bCard, HeroParamBehaviour.UnitParamType.Sturm_Damage, 0, (byte)newLevel);
                    cardParams[paramIndex].SetValue(value, true, canUpgrade);
                    cardParams[paramIndex].gameObject.SetActive(true);

                }
                else
                {
                    cardParams[paramIndex].gameObject.SetActive(false);
                }
            }
            else
            {
                cardParams[1].gameObject.SetActive(false);
                cardParams[2].gameObject.SetActive(false);
                cardParams[3].gameObject.SetActive(false);
            }

        }
    }

    //вынес в CardParams
    #region delete 
    /*
    private float CountMinionDamage(float damage)
    {
        return math.round(damage * 1000 / Offence.duration);
    }

    private float CountHp()
    {
        return Defence._health(settings.minions.health, PlayerCard.level);
    }

    private float CountDmgInSkill(ref float value)
    {
        var splashValue = GetSplashValue(value);

        return Offence._customDamage(
            splashValue,
            settings.minions.damage,
            PlayerCard.level
        );
    }

    private float CountDamage()
    {
        return Offence._damage(
                 settings.minions.damage,
                 PlayerCard.level);
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
                                var effects = Components.Instance.Get<Legacy.Database.EffectDamage>();
                                if (effects.TryGetValue(e, out Legacy.Database.EffectDamage dData))
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
                if (Effects.Instance.Get(e, out BinaryEffect beffect))
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
    */
    #endregion delete
}

using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using Legacy.Database;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static Legacy.Client.HeroParamBehaviour;

public class CardWindowDataBehaviour : MonoBehaviour
{
    [SerializeField]
    private CardWindowBehaviour WindowBehaviour;

    private ProfileInstance profile;

    private BinaryCard binaryCard;
    private ClientCardData PlayerCard;
    private BaseBattleSettings settings;
    [SerializeField]
    private TextMeshProUGUI CardName;

    [SerializeField]
    private CardWindowRarityPanelBehaviour RarityPanel;

    [SerializeField]
    private HeroParamBehaviour[] Params;

    [SerializeField]
    private GameObject CardPrefab;

    [SerializeField]
    private TextMeshProUGUI CardDescription;

    [SerializeField]
    private TextMeshProUGUI CardRarityText;

    [SerializeField]
    private RectTransform CardContainer;
    [SerializeField]
    private GameObject NotEnaughCoinsObject;
    [SerializeField]
    private GameObject NotEnaughCardsObject;
    [SerializeField]
    private GameObject UpgradeGivesObject;
    [SerializeField]
    private TextMeshProUGUI ExpValue;
    [SerializeField]
    private ButtonWithPriceViewBehaviour PriceButton;
    [SerializeField]
    private GameObject PriceButtonUpEffect;
    [SerializeField]
    private LegacyButton PriceLegacyButton;

    [SerializeField]
    private GameObject UseButtonObject;
    [SerializeField]
    private GameObject cardObject = null;

    public BinaryEntity BinaryMinion { get; private set; }
    public MinionOffence Offence { get; private set; }
    public MinionDefence Defence { get; private set; }
    public MinionSkills Skills { get; private set; }
    public MinionMovement Movement { get; private set; }

    public static string GetAttackSpeedPrefix(float value)
    {
        if (value >= 2f)
            return Locales.Get("locale:1456");
        else if (value >= 1.5f)
            return Locales.Get("locale:994");
        else if (value >= 1f)
            return Locales.Get("locale:997");
        else if (value >= 0.5f)
            return Locales.Get("locale:1000");
        else
            return Locales.Get("locale:1003");
    }

    public static string GetMovementSpeedPrefix(float value)
    {
        if (value > 1.8f)
            return Locales.Get("locale:1003"); 
        else if (value > 1.2f)
            return Locales.Get("locale:1000");
        else if (value > .9f)
            return Locales.Get("locale:997");
        else if (value > .7f)
            return Locales.Get("locale:994");
        else
            return Locales.Get("locale:1456");
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
            case MinionLayerType.Building:
                return Locales.Get("locale:2389");
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

    void SetParams(/*byte quantity =0*/)
    {
        float value = 0.0f;
        float canUpgrade = 0.0f;
        float dmg = 0f;
        CardParams cardParams = new CardParams();
        var  newLevel = PlayerCard.level + 1;
        byte j = 0;

        for (byte i = 0; i < Params.Length; i++)
        {
            Params[i].SetDefaultValue();
            Params[i].gameObject.SetActive(false);
            Params[j].SetAdditionalValue("");
            Params[i].CloseInfoButtom();
        }
        foreach (UnitParamType valueType in Enum.GetValues(typeof(UnitParamType)))
        {
         //   UnitParamType valueType = (UnitParamType)i;
            Params[j].UpdateParamView(valueType);

            switch (valueType)
            {
                case UnitParamType.AttackSpeed:
                    value = cardParams.SetParams(binaryCard, valueType);
                    canUpgrade = cardParams.SetParams(binaryCard, valueType,0,(byte)newLevel);
                    if(value>0)
                        Params[j].SetAdditionalValue(GetAttackSpeedPrefix(value));
                    Params[j].gameObject.SetActive(value > 0);
                    break;
                case UnitParamType.AttackType:
                   // value = cardParams.SetParams(binaryCard, valueType);
                   // value = Offence.radius;
                   // Params[i].SetAdditionalValue(GetAttackTypePrefix(Offence.type));
                  
                    Params[j].SetStringValue(GetAttackTypePrefix(Offence.type));
                    if (GetAttackTypePrefix(Offence.type) != "")
                    {
                        Params[j].gameObject.SetActive(true);
                    }
                    break;
                case UnitParamType.Movement:
                    var type = MinionLayerType.Ground;
                    if (BinaryMinion.type == MinionLayerType.Building)
                    {
                        type = MinionLayerType.Building;
                      /*  Params[i].gameObject.SetActive(false);
                        break;*/
                    }
                    
                    if (BinaryMinion.type == MinionLayerType.Fly)
                    {
                        type = MinionLayerType.Fly;
                    }
                    var stringValue = GetMovementTypePrefix(type) + " : " + GetMovementSpeedPrefix(Movement.speed);
                    if (BinaryMinion.type == MinionLayerType.Building)
                        stringValue = GetMovementTypePrefix(type);

                     Params[j].SetStringValue(stringValue);
                     Params[j].gameObject.SetActive(true);
                    break;
                case UnitParamType.DMG:
                    // value = damage;
                    /*  value = Offence._customDamage(
                          value,
                          settings.minions.damage,
                          PlayerCard.level
                      );*/
                       value = cardParams.SetParams(binaryCard, valueType);
                    //   Params[i].gameObject.SetActive(value>0);
                    if (value == 0)
                    {
                        value = cardParams.SetParams(binaryCard, UnitParamType.Splash);
                        Params[j].UpdateParamView(UnitParamType.Splash);
                        //Params[i].gameObject.SetActive(value > 0);
                        canUpgrade = cardParams.SetParams(binaryCard, UnitParamType.Splash, 0, (byte)newLevel);
                    }
                    if(value == 0)
                    {
                        value = cardParams.SetParams(binaryCard, UnitParamType.Piercing_DMG);
                        Params[j].UpdateParamView(UnitParamType.Piercing_DMG);
                        canUpgrade = cardParams.SetParams(binaryCard, UnitParamType.Piercing_DMG, 0, (byte)newLevel);
                    }
                       canUpgrade = cardParams.SetParams(binaryCard, valueType, 0, (byte)newLevel);
                    Params[j].gameObject.SetActive(value > 1);
                    dmg = value;
                    break;
                  case UnitParamType.Piercing_DMG:
                    value = 0f;
                    break;
                case UnitParamType.Splash:
                    value = 0f;
                    // Params[j].gameObject.SetActive(false);
                    break;
                  case UnitParamType.DPS:
                    /* value = math.round(damage * 1000 / Offence.duration);
                     if (value == 0)
                     {
                         value = Params[2].GetLvlValue() * 1000 / Offence.duration;
                     }*/
                   
                        value = cardParams.SetParams(binaryCard, valueType);
                        if (value == 0)
                        {
                          //  Params[j].UpdateParamView(UnitParamType.DPS_splash);
                            value = cardParams.SetParams(binaryCard, valueType, dmg /*Params[1].GetLvlValue()*/);
                            canUpgrade = cardParams.SetParams(binaryCard, valueType, dmg /*Params[1].GetPlusLvlValue()*/, (byte)newLevel);
                        }else
                           canUpgrade = cardParams.SetParams(binaryCard, valueType, cardParams.SetParams(binaryCard, valueType, 0, (byte)newLevel), (byte)newLevel);

                       Params[j].gameObject.SetActive(value > 1);
                    break;
                  case UnitParamType.HP:
                     // value = Defence._health(settings.minions.health, PlayerCard.level);
                      value = cardParams.SetParams(binaryCard, valueType);
                      canUpgrade = cardParams.SetParams(binaryCard, valueType, 0, (byte)newLevel);

                       Params[j].gameObject.SetActive(value > 0);
                    break;
                  case UnitParamType.AreaDamage:  
                   //   Params[i].gameObject.SetActive(!Skills.Equals(default));                    
                     // value = 2;
                     // Params[j].gameObject.SetActive(false);
                      //TODO: DifferentSkillsBehaviour;
                      break;
                  case UnitParamType.Targets:
                      value = Offence.target;
                       Params[j].gameObject.SetActive(value>0);
                    break;
                  case UnitParamType.Quantity: 
                      value = cardParams.SetParams(binaryCard, valueType);
                       Params[j].gameObject.SetActive(value > 1);
                      break;
                  case UnitParamType.Knockback:
                    //  value = GetSkillEffect("EffectPushBack");
                      value =  cardParams.SetParams(binaryCard, valueType);
                      Params[j].gameObject.SetActive(value>0);
                      break;
                  case UnitParamType.Sturm_Damage:

                      value = cardParams.SetParams(binaryCard, valueType);
                    if (value > 0)
                    {
                        canUpgrade = cardParams.SetParams(binaryCard, valueType, 0, (byte)newLevel);
                    }
                    Params[j].gameObject.SetActive(value > 0);
                    break;
                case UnitParamType.DPS_splash:
                    Params[j].gameObject.SetActive(false);
                    break;
                case UnitParamType.Life_Time:
                    value = cardParams.SetParams(binaryCard, valueType);
                    Params[j].gameObject.SetActive(value > 0);
                    break;
                case UnitParamType.Summon:
                    value = cardParams.SetParams(binaryCard, valueType);
                    summonId =(int) value;
                    Params[j].gameObject.SetActive(value > 0);
                    break;
                default:
                      break;
              }


              if (value>0 && valueType != UnitParamType.Movement && valueType != UnitParamType.AttackType)
                  Params[j].SetValue(value,/* PlayerCard.count > 0*/ true, canUpgrade);

              if (Params[j].gameObject.activeSelf)
                {
                    j++;
                }

        }
    }

    private int summonId;
    public bool GetSummon(out byte level, out int index)
    {
            level = PlayerCard.level;
            index = summonId;
            return true;
    }
    //вынес в CardParams
    #region delete
    /*
            float damage = Offence._damage(
                            settings.minions.damage,
                            PlayerCard.level
                        );

            for (byte i = 0; i < Params.Length; i++)
            {
                UnitParamType valueType = (UnitParamType)i;
                Params[i].UpdateParamView(valueType);
                float value = 0.0f;
                switch (valueType)
                {
                    case UnitParamType.AttackSpeed:
                        value = (float)Offence.duration / 1000;
                        Params[i].SetAdditionalValue(GetAttackSpeedPrefix(value));
                        break;
                    case UnitParamType.AttackType:
                        value = Offence.radius;
                        Params[i].SetAdditionalValue(GetAttackTypePrefix(Offence.type));
                        break;
                    case UnitParamType.Movement:
                        var type = MinionLayerType.Ground;
                        if (BinaryMinion.type == MinionLayerType.Fly)
                        {
                            type = MinionLayerType.Fly;
                        }
                        var stringValue = GetMovementTypePrefix(type) + " : " + GetMovementSpeedPrefix(Movement.speed);
                        Params[i].SetStringValue(stringValue);
                        break;
                    case UnitParamType.DMG:
                        value = damage;
                      /*  value = Offence._customDamage(
                            value,
                            settings.minions.damage,
                            PlayerCard.level
                        );
                        Params[i].gameObject.SetActive(value>0);
                        break;
                    case UnitParamType.Splash:
                          var newValue = GetSplashValue(value);
                           value = Offence._customDamage(
                                 newValue,
                                 settings.minions.damage,
                                 PlayerCard.level
                            );
                        Params[i].gameObject.SetActive(value > 0);
                        break;
                    case UnitParamType.DPS:
                        value = math.round(damage * 1000 / Offence.duration);
                        if (value == 0)
                        {
                            value = Params[2].GetLvlValue() * 1000 / Offence.duration;
                        }
                        break;
                    case UnitParamType.HP:
                        value = Defence._health(settings.minions.health, PlayerCard.level);
                        break;
                    case UnitParamType.AreaDamage:  
                        Params[i].gameObject.SetActive(!Skills.Equals(default));                    
                       // value = 2;
                        Params[i].gameObject.SetActive(false);
                        //TODO: DifferentSkillsBehaviour;
                        break;
                    case UnitParamType.Targets:
                        value = Offence.target;
                        break;
                    case UnitParamType.Quantity: 
                        value = (byte)binaryCard.entities.Count;
                        Params[i].gameObject.SetActive(value > 1);
                        break;
                    case UnitParamType.Knockback:
                        value = GetSkillEffect("EffectPushBack");

                        Params[i].gameObject.SetActive(value>0);
                        break;
                    case UnitParamType.Storm_Damage:
                        value = GetSkillEffect("EffectHill"); 
                        if(value > 0)
                        {
                           value = Offence._customDamage(
                            value,
                            settings.minions.damage,
                            PlayerCard.level
                        );
                        }
                         Params[i].gameObject.SetActive(value > 0);
                        break;
                    default:
                        break;
                }
                if (valueType != UnitParamType.Movement )
                    Params[i].SetValue(value, PlayerCard.count > 0);
            }*/
    //     }
    /*
        private float GetSkillEffect(String effect )
        {
            float rez = 0f;
            for (byte k = 0; k < MinionSkills.Count; ++k)
            {
                if (Skills.Get(k, out ushort skill))
                {
                    switch (effect)
                    {
                        case "EffectPushBack":
                            rez = EffectPushBack(skill); break;
                        case "EffectHill":
                            rez = EffectHill(skill); break;
                    }

                }
                if (rez > 0) break;

            }
            return rez;
        }
    */
    /*
        private float EffectHill(ushort index)
        {
            if (Legacy.Database.Skills.Instance.Get(index, out BinarySkill bskill))
            {
                foreach (var e in bskill.effects)
                {
                    if (Effects.Instance.Get(e, out BinaryEffect beffect))
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
    */
    /*   private float EffectPushBack(ushort index)
       {
           if (Legacy.Database.Skills.Instance.Get(index, out BinarySkill bskill))
           {
               foreach (var e in bskill.effects)
               {
                   if (Effects.Instance.Get(e, out BinaryEffect beffect))
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
       }*/
    /*
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
                           if (Effects.Instance.Get(e, out BinaryEffect beffect))
                           {
                               if(beffect.components.Contains(36) && beffect.components.Contains(66) && bskill.effects.Count == 2)
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

       *//*
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

    internal void Init(DeckCardBehaviour currentDeckCard)
    {
        if (profile == null)
        {
            profile = ClientWorld.Instance.GetExistingSystem<HomeSystems>().UserProfile;
        }

        binaryCard = currentDeckCard.binaryCard;
        PlayerCard = currentDeckCard.GetPlayerCard();        
        settings = Settings.Instance.Get<BaseBattleSettings>();
        
        if (binaryCard.entities.Count > 1)
        {
            var isDifferentUnitsInSquad = false;
            {
                ushort firstUnit = 0;
                for (byte i = 0; i < binaryCard.entities.Count; i++)
                {
                    if (i == 0)
                    {
                        firstUnit = binaryCard.entities[i];
                        continue;
                    }
                    if(firstUnit != binaryCard.entities[i])
                    {
                        isDifferentUnitsInSquad = true;
                        break;
                    }
                }
            }
            if (isDifferentUnitsInSquad)
            {
                //TODO: DifferrentSquadScenario();
            }
            else
            {
                GetEntityComponents(binaryCard.entities[0]);
                SetParams(/*(byte)binaryCard.entities.Count*/);
            }
        }
        else
        {            
            GetEntityComponents(binaryCard.entities[0]);
            SetParams();
        }
        
        UpdateAll();
        profile.PlayerProfileUpdated.AddListener(UpdateAll);
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

    public void UpdateAll()
    {
        //if(cardObject != null)
        //{
        //    DestroyImmediate(cardObject);
        //}
        SetParams();
        PlayerCard = profile.Inventory.GetCardData(binaryCard.index);
   //     cardObject = Instantiate(CardPrefab, CardContainer);
        cardObject.GetComponent<CardViewBehaviour>().Init(binaryCard);
        var cardText = cardObject.GetComponent<CardTextDataBehaviour>();
        cardText.SetLevel(PlayerCard.level);
        cardText.SetManaCost(binaryCard.manaCost);
        cardText.SetCount(
            PlayerCard.count,
            PlayerCard.CardsToUpgrade
        );
        var can_upgrade = PlayerCard.CanUpgrade;
        bool EnaughCoins = profile.Stock.CanTake(CurrencyType.Soft, PlayerCard.SoftToUpgrade);
        CardName.text = Locales.Get(binaryCard.title);
        CardRarityText.text = binaryCard.GetRarityString();
        CardRarityText.color = VisualContent.Instance.GetRarityState(CardGlowState.Rarity,binaryCard.rarity);

        CardDescription.text = Locales.Get(binaryCard.description);
      //  PriceButton.isCloseLook = false;
        PriceButton.SetPrice(PlayerCard.SoftToUpgrade);
        PriceButton.SetGrayMaterial(!can_upgrade);

        PriceLegacyButton.interactable = EnaughCoins && can_upgrade;
        //
        uint need_count = PlayerCard.CardsToUpgrade - (uint)PlayerCard.count;


        if (!can_upgrade)
        {
            PriceLegacyButton.isLocked = !can_upgrade;
            PriceButton.IsNotEnoughtCoins(false);
            PriceLegacyButton.localeAlert = Locales.Get("locale:1006", (need_count).ToString());
        }
        else
        {
            PriceLegacyButton.localeAlert = "";
            PriceButton.IsNotEnoughtCoins();
        }
  
        PriceButtonUpEffect.SetActive(can_upgrade);
        UseButtonObject.SetActive(!profile.DecksCollection.IsCardInDeck(binaryCard.index));
        ExpValue.text = PlayerCard.ExpToUpgrade.ToString();

        NotEnaughCardsObject.SetActive(!can_upgrade);
        NotEnaughCoinsObject.SetActive(!EnaughCoins && can_upgrade);
        UpgradeGivesObject.SetActive(EnaughCoins && can_upgrade);
    }

    private void OnDestroy()
    {
        profile?.PlayerProfileUpdated?.RemoveListener(UpdateAll);
    }
}

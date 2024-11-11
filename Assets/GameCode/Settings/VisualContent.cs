using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using static Legacy.Client.LootBoxWindowBehaviour;

[CreateAssetMenu(menuName = "GameLegacy/VisualContent", fileName = "VisualContent")]
public class VisualContent : SettingObject
{
    public static VisualContent Instance;

    public SpriteAtlas CardIconsAtlas; 
    public SpriteAtlas BankCurrenciesAtlas; 
    public SpriteAtlas SkillsIconsAtlas; 

    public Material GrayScaleMaterial;
    public Material SpawnUnitMaterial;
    public Material[] DamageMaterialsEnemy;
    public Material[] DamageMaterialsEnemyHero;
    public Material[] DamageMaterialsAlly;
    public Material[] DamageMaterialsAllyHero;

    public static Color PlayerColor = new Color(4f / 255f, 181f / 255f, 254f / 255f);
    public static Color EnemyColor = new Color(208f / 255f, 66f / 255f, 19f / 255f);

    [System.Serializable]
    public struct RewardParticlesMaterials
    {
        public Material Soft;
        public Material Hard;
        public Material Shards;
        public Material Cards;
        public Material Exp;
        public Material HeroExp;
        public Material Rating;
    }

    public RewardParticlesMaterials RewardMaterials;
    public override void Init()
    {
        Instance = this;
    }

    [System.Serializable]
    public struct RarityColors
    {
        public Color32 common;
        public Color32 rare;
        public Color32 epic;
        public Color32 legendary;
    }

    [System.Serializable]
    public struct CardGlowStates//updateColor struct
    {
        //public RarityColors rarity;
        //public Color32 tremor;
        public Color32 update;
    }

    public Color32 GetRarityState(CardGlowState currentState, CardRarity rarity)
    {
        Color32 color = new Color32(0, 0, 0, 0);
        switch (currentState)
        {
            case CardGlowState.Rarity:
                color = GetRarityColor(rarity);
                break;
            case CardGlowState.Tremor:
                color = GetRarityColor(rarity);
                break;
            case CardGlowState.Update:
                color = glowStates.update;
                break;
            default:
                color = GetRarityColor(rarity);
                break;
        }

        return color;
    }

    public Color32 GetRarityColor(CardRarity rarity)
    {
        Color32 color = new Color32(0,0,0,0);
        switch (rarity)
        {
            case CardRarity.Common:
                color = rarityColors.common;
                break;
            case CardRarity.Rare:
                color = rarityColors.rare;
                break;
            case CardRarity.Epic:
                color = rarityColors.epic;
                break;
            case CardRarity.Legendary:
                color = rarityColors.legendary;
                break;
            default:
                break;
        }
        return color;
    }

    internal Material GetRewardParticleMaterial(LootCardType type)
    {
        Material material = RewardMaterials.Soft;
        switch (type)
        {
            case LootCardType.Hard:
                material = RewardMaterials.Hard;
                break;
            case LootCardType.Soft:
                material = RewardMaterials.Soft;
                break;
            case LootCardType.Shards:
                material = RewardMaterials.Shards;
                break;
            case LootCardType.Cards:
                material = RewardMaterials.Cards;
                break;
            case LootCardType.Exp:
                material = RewardMaterials.Exp;
                break;
            case LootCardType.HeroExp:
                material = RewardMaterials.HeroExp;
                break;
            case LootCardType.Rating:
                material = RewardMaterials.Rating;
                break;
        }
        return material;
    }

    [System.Serializable]
    public struct SliderBasicContent
    {
        public Sprite background;
        public Sprite slider;
        public Sprite cover;
    }


    public CustomVisualData customVisualData;

    [System.Serializable]
    public struct CustomVisualData
    {
        public GameObject BasicAlertPrefab;
        public GameObject CollectionCardPrefab;
        public GameObject BasicDragPrefab;
        public GameObject BasicWindowPrefab;
        public GameObject InfoWindowPrefab;
        public GameObject StatsContentPrefab;
        public GameObject StatsItemPrefab;
        public GameObject CardAchievementsContentPrefab;
        public GameObject CardDescriptionContentPrefab;
        public GameObject UnitTimerPrefab;
        public GameObject SpendManaPrefab;
        public GameObject PopupAlertPrefab;
        public TMP_FontAsset DefaultFont;
        public Sprite DefaultButtonImage;
        public SliderBasicContent SliderContent;
    }


    [System.Serializable]
    public struct HeroVisualData
    {
        public ushort id;
        public GameObject MenuModelPrefab;
        public Sprite HeroListIcon;
        public Sprite StartBattleIcon;
        public ArenaHeroBehaviour ArenaPrefab;
    }

    public List<HeroVisualData> Heroes;

    public List<HeroVisualData> MissionHeroes;

    [System.Serializable]
    public struct ArenaVisualData
    {
        public ushort id;
        public GameObject MenuPrefab;
        public Sprite TitleImage;
    }

    public List<ArenaVisualData> Arenas;

    public ArenaVisualData GetArenaVisualData(ushort id)
    {
        foreach (ArenaVisualData arena in Arenas)
        {
            if (arena.id != id) continue;
            return arena;
        }
        return Arenas[0];
    }

    public GameObject TouchPrefab;

    public RarityColors rarityColors;
    public CardGlowStates glowStates;
    public HeroVisualData GetHeroVisualData(ushort id)
    {
        foreach (HeroVisualData hero in Heroes)
        {
            if (hero.id != id) continue;
            return hero;
        }
        return GetMissionHeroVisualData(id);
    }

    public HeroVisualData GetMissionHeroVisualData(ushort id)
    {
        foreach (HeroVisualData hero in MissionHeroes)
        {
            if (hero.id != id) continue;
            return hero;
        }
        return Heroes[0];
    }

    [Serializable]
    public class CurrencyIcon
    {
        public Sprite sprite;
        public CurrencyType type;
    }

    [SerializeField]
    public List<CurrencyIcon> CurrenciesSprites = new List<CurrencyIcon>();

    public Sprite GetCurrencyIcon(CurrencyType type)
    {
        Sprite sprite = CurrenciesSprites[0].sprite;
        CurrenciesSprites.ForEach((icon) =>
        {
            if (icon.type == type)
            {
                sprite = icon.sprite;
            }
        });
        return sprite;
    }

    [Serializable]
    public class CardMiniRarityIcon
    {
        public Sprite sprite;
        public CardRarity rarity;
    }

    [SerializeField]
    public List<CardMiniRarityIcon> CardsMiniRaritySprites = new List<CardMiniRarityIcon>();

    internal Sprite GetMiniCardIconRarity(CardRarity rarity)
    {
        Sprite sprite = CardsMiniRaritySprites[0].sprite;
        CardsMiniRaritySprites.ForEach((icon) =>
        {
            if (icon.rarity == rarity)
            {
                sprite = icon.sprite;
            }
        });
        return sprite;
    }


    [System.Serializable]
    public struct LootBoxIcons
    {
        public string name;
        public Sprite sprite;
    }

    public List<LootBoxIcons> listIcon;

    internal Sprite GetLootBoxIcon(String name)
    {
        Sprite sprite = null;
        listIcon.ForEach((icon) =>
        {
            if (icon.name == name)
            {
                sprite = icon.sprite;
            }
        });
        return sprite;
    }
}

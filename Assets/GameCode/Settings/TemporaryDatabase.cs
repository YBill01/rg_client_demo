using System;
using UnityEngine;
using System.Collections.Generic;
using Legacy.Database;
using Legacy.Client;

public abstract class SettingObject : ScriptableObject
{
	abstract public void Init();
}

[CreateAssetMenu(menuName = "GameLegacy/TemporaryDatabase", fileName = "TemporaryDatabase")]
public class TemporaryDatabase : SettingObject
{
	public static TemporaryDatabase Instance;

	public override void Init()
	{
		Instance = this;
	}

	[Serializable]
	public struct STOCKITEMDATA
	{
		public ushort ID;
		public string Name;
		public Sprite activeImage;
		public Sprite nonActiveImage;
		public bool IsForSale;
		public StockItemType Type;
	}
	public List<STOCKITEMDATA> Stock;

	public STOCKITEMDATA StockItemData(ushort ID)
	{
		return StockItemData(ID, StockItemType.Simple);
	}

	public STOCKITEMDATA StockItemData(ushort ID, StockItemType type)
	{
		foreach (STOCKITEMDATA im in Stock)
		{
			if (im.ID != ID) continue;
			if (im.Type != type) continue;
			return im;
		}
		return Stock[0];
	}

	[Serializable]
	public class CARDITEMDATA
	{
		public ushort sid;
		public CardRarity cardClass;
		public byte CurrentLevel;
		public List<LEVELDATA> Levels;
	}

	[Serializable]
	public class CARDCLASSDATA
	{
		public CardRarity cardClass;
		public Color MainColor;
		public Sprite LevelImageBig;
		public Sprite BarContentImageBig;
		public Sprite LevelImageSmall;
		public Sprite BarContentImageSmall;
		public Sprite RarityImage;
		public Color TextColor;
		public string Text;
	}

	public List<CARDITEMDATA> CardsStock;
	public List<ParamStaticData> ParamStaticDatas;
	public List<CARDCLASSDATA> CardClassData;

	public ParamStaticData GetStaticParamData(StatType statType)
	{
		foreach(var p in ParamStaticDatas)
		{
			if (p.Param != statType) continue;
			return p;
		}
		return ParamStaticDatas[0];
	}

	[Serializable]
	public class LEVELDATA
	{
		public byte Level;
		[SerializeField]
		public List<ParamData> Params;
	}

	[Serializable]
	public enum StatType
	{
		Damage,
		AttackSpeed,
		DamagePerSecond,
		MelleeDamage,
		RangedDamage,
		HitPoints,
		Evasion,
		LandUnit,
		FlyUnit,
		Count,
		AnyTargets,
		LandTargets,
		AirTartets,
		MoveSpeed,
		Heroes
	}

	[Serializable]
	public enum StatParam
	{
		Image,
		Name,
		Option,
		Prefix,
		Value,
		Ending,
		Growth
	}

	[Serializable]
	public class ParamStaticData
	{
		public StatType Param;
		public Sprite Image;
		public string ParamName;
		public string ValuePrefix;
		public bool HasValue;
		public bool Upgradeable;
		public string Option;
		public string Ending;
	}

	[Serializable]
	public class ParamData
	{
		[SerializeField]
		public StatType Param;
		[SerializeField]
		public float ParamValue;
	}

	[Serializable]
	public class RegularParamData : ParamData
	{
		public float Value;
	}

	[Serializable]
	public class MovementTypeData : ParamData
	{
		public MovementType MovementType;
		public float Value;
	}

	[Serializable]
	public class AttackTypeData : ParamData
	{
		public AttackType MovementType;
		public float Value;
	}

	[Serializable]
	public class UnitsCountData : ParamData
	{
		public byte Value;
	}

	public enum AttackType
	{
		Mellee,
		Range
	}

	public enum MovementType
	{
		Land,
		Fly
	}

	public CARDCLASSDATA GetClassData(ushort sid)
	{
		CARDITEMDATA cid = GetCardData(sid);
		foreach(CARDCLASSDATA ccd in CardClassData)
		{
			if (cid.cardClass != ccd.cardClass) continue;
			return ccd;
		}
		return CardClassData[0];
	}

	public CARDCLASSDATA GetClassData(CardRarity rarity)
	{foreach (CARDCLASSDATA ccd in CardClassData)
		{
			if (rarity != ccd.cardClass) continue;
			return ccd;
		}
		return CardClassData[0];
	}

	public CARDITEMDATA GetCardData(ushort sid)
	{
		foreach (CARDITEMDATA c in CardsStock)
		{
			if (c.sid != sid) continue;
			return c;
		}
		return CardsStock[0];
	}

	public enum CardSizeType
	{
		Small,
		Big
	}

	public Sprite FilledBarImage;
	public Sprite FilledBarImageSmall;

	public CARDITEMDATA CardItemData(ushort ID)
	{
		foreach (CARDITEMDATA im in CardsStock)
		{
			if (im.sid != ID) continue;
			return im;
		}
		return CardsStock[0];
	}

	[Serializable]
	public class CardItemImages
	{
		public ushort sid;
		public Sprite small;
		public Sprite big;
	}

	public List<CardItemImages> cardImages;

	public CardItemImages GetCardImages(ushort sid)
	{
		foreach (CardItemImages im in cardImages)
		{
			if (im.sid != sid) continue;
			return im;
		}
		return cardImages[0];
	}

	[Serializable]
	public class CardClassFrame
	{
		public CardSizeType cardSizeType;
		public Sprite RegularImage;
		public Sprite SelectedImage;
	}

	public List<CardClassFrame> frameImages;

	public CardClassFrame GetCardFrame(CardSizeType cType)
	{
		foreach (CardClassFrame im in frameImages)
		{
			if (im.cardSizeType != cType) continue;
			return im;
		}
		return frameImages[0];
	}
}

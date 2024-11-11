using UnityEngine;

[CreateAssetMenu(menuName = "GameLegacy/BattleSettings", fileName = "BattleSettings")] 
public class BattleSettings : ScriptableObject
{
	static BattleSettings _instance = null;
	public static BattleSettings Instance
	{
		get
		{
			if (!_instance)
				_instance = (BattleSettings)Resources.Load("Settings/BattleSettings");
			return _instance;
		}
	}

	[System.Serializable]
	public struct GridSettings
	{
		public float TileSize;
	}

	public GridSettings Grid;

	public ushort MaxUnitCount = 128;
	public byte MaxHistoryFrame = 128;
	public uint MinionSpawnTime = 1000u;
	public bool PlayerSideRight = false;
	public uint Seed = 0x1E124EB1u;
	public float ManaRestoreSpeed = 0.5f;
	public ushort StartMana = 4;
	public ushort MaxMana = 10;
}

using Legacy.Database;
using Unity.Entities;
using UnityEngine;

public struct ArenaZoneHighlight : IComponentData
{
	public bool allZone;
}
public struct StartDragBattleCard : IComponentData
{
	public Vector3 dragPosition;
	public byte state;
	public float agroRadius;
}

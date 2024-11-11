using Unity.Entities;

namespace Legacy.Client
{

	public struct DragApplyTimeData : IComponentData
	{
		public float StartTime;
		public float WaitTime;
	}

	public enum CardState
	{
		CollectionDefault,
		Requesting,
		Touched,
		DeckDefault,
		Unavailable,
		UseState,
		Hidden,
	}
	public struct CardStateChangeRequest: IComponentData
	{
		public CardState State;
		public Entity StateEntity;
	}
	public struct CardStateComponent : IComponentData
	{
		public CardState State;
	}

	public struct ReplaceCardRequest : IComponentData
	{
		public Entity ReplacingEntity;
		public Entity ReplaceableEntity;
	}

	public struct InitClickReplaceRequest : IComponentData
	{
		public ushort sid;
	}

	public struct ApplyClickReplaceCardRequest : IComponentData
	{
		public Entity ReplacingEntity;
	}
	public struct RequestingStateUpdateRequest : IComponentData
	{
		public Entity ReplacingEntity;
	}

	public struct WaitChooseStateRequest : IComponentData
	{
		public Entity ReplacingEntity;
	}

	public struct ClickExchangeCancelRequest : IComponentData { }
	public struct NextCardsSortRequest : IComponentData { }
	public struct AvgValueTag : IComponentData { }
	public struct UpdateAvgValueRequest : IComponentData { }
	public struct RebuildCollectionRequest : IComponentData { }
	public struct RequestingChangeTag : IComponentData { }
	public struct HideWaitChangeTag : IComponentData { }
	public struct WaitChangeTag : IComponentData { }
	public struct DeckTabTag : IComponentData { }
	public struct ReplaceableTag : IComponentData { }
	public struct InitReturnBackTag : IComponentData { }
	public struct ReturnBackTag : IComponentData { }
	public struct StopDragTag : IComponentData { }
	public struct DragInitTag : IComponentData { }
	public struct DragTag : IComponentData { }
	public struct DeckCardTag : IComponentData { }
	public struct RetouchRequest : IComponentData
	{
		public Entity TouchedEntity;
	}

	public struct UnholdRequest : IComponentData
	{
		public Entity TouchedEntity;
	}
	public struct SetDefaultParentTag : IComponentData { }
	public struct StopDragRequest : IComponentData { }

	public struct CollectionCardTagData : IComponentData{ }

	public struct ChangeDeckRequest : IComponentData
	{
		public byte NewDeckID;
	}

	public struct PlaceCardRequest : IComponentData
	{
		public uint PlaceCardPosition;
		public uint PlacePosition;
		public uint DeckID;
	}

	public struct CardSetData : IComponentData
	{
		public ushort sid;
	}

	public struct PlayerDeckMenuComponent : IComponentData
	{

	}

	public struct PlayerCardsMenuComponent : IComponentData
	{

	}

}
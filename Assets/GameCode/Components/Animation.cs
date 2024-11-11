using Unity.Entities;

namespace Legacy.Client
{

	public enum AnimationType
	{
		None,
		Spawn,
		Stand,
		Walk,
		Run,
		Attack,
		Death
	}

	public struct AnimationRequest : IComponentData
	{
		public Entity entity;
		public AnimationType type;
	}

	public struct MinionAnimation : ISystemStateComponentData
	{

	}

}


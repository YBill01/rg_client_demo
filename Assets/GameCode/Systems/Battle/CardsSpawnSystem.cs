using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Legacy.Network;
using UnityEngine;

namespace Legacy.Client
{
	/*public class SetCardsPropertySystem : JobComponentSystem
	{
		private EntityQuery _player_query;
		private EntityQuery _set_cards_property_query;
		private BeginInitializationEntityCommandBufferSystem _barrier;

		protected override void OnCreate()
		{
			_barrier = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
			_player_query = GetEntityQuery(
				ComponentType.ReadOnly<BattlePlayer>(),
				ComponentType.ReadOnly<BattlePlayerOwner>()
			);
			_set_cards_property_query = GetEntityQuery(
				ComponentType.ReadOnly<NetworkReplicated>(),
				ComponentType.ReadOnly<CardDataComponent>(),
				//ComponentType.Exclude<PropertyOfMe>(),
				//ComponentType.Exclude<PropertyOfSomeoneElse>(),
				ComponentType.Exclude<Transform>(),
				ComponentType.Exclude<RectTransform>()
			);
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			if (_player_query.IsEmptyIgnoreFilter) return inputDeps;
			if (_set_cards_property_query.IsEmptyIgnoreFilter) return inputDeps;

			var _playerEntity = _player_query.GetSingletonEntity();
			var _player = EntityManager.GetComponentData<BattlePlayer>(_playerEntity);
			if (_player.side == BattlePlayerSide.None) return inputDeps;

			var _set_prop_job = new SetCardsProperty
			{
				playerSide = _player.side,
				buffer = _barrier.CreateCommandBuffer()
			}.Schedule(_set_cards_property_query, inputDeps);
			_set_prop_job.Complete();
			return inputDeps;
		}

		[Unity.Burst.BurstCompile]
		[ExcludeComponent(typeof(Transform))]
		struct SetCardsProperty : IJobForEachWithEntity<NetworkReplicated, CardDataComponent>
		{
			public BattlePlayerSide playerSide;
			[ReadOnly] public EntityCommandBuffer buffer;
			public void Execute(Entity entity, int index, [ReadOnly] ref NetworkReplicated repl, [ReadOnly] ref CardDataComponent card)
			{
				if (repl.snapshottime <= 0) return;
				if (card.side == BattlePlayerSide.None) return;
				if (card.side == playerSide)
				{
					//buffer.AddComponent(entity, default(PropertyOfMe));
				}
				else
				{
					//buffer.AddComponent(entity, default(PropertyOfSomeoneElse));
				}
			}
		}
	}*/
}
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Jobs;
using Legacy.Database;

namespace Legacy.Client
{
	[UpdateInGroup(typeof(BattlePresentation))]

	public class EffectTransformSystem : JobComponentSystem
	{

		private EntityQuery _query_effects;
		private NativeHashMap<int, float2> _changes;

		protected override void OnCreate()
		{
            _query_effects = GetEntityQuery(
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<Transform>(),
				ComponentType.Exclude<CustomSTransform>()
			);

			_changes = new NativeHashMap<int, float2>(128, Allocator.Persistent);

			RequireForUpdate(_query_effects);
			RequireSingletonForUpdate<BattleInstance>();
		}

		protected override void OnDestroy()
		{
			_changes.Dispose();
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var _battle = GetSingleton<BattleInstance>();
			var _player = _battle.players[_battle.players.player];

			_changes.Clear();

			inputDeps = new CollectPositions
			{
				changes = _changes.AsParallelWriter(),
				side = _player.side == BattlePlayerSide.Right ? -1 : 1
			}.Schedule(_query_effects, inputDeps);

			var _transforms = _query_effects.GetTransformAccessArray();

			inputDeps = new TransformPositionsJob
			{
				changes = _changes,
				delta = Time.DeltaTime
			}.Schedule(_transforms, inputDeps);

			return inputDeps;
		}

		[Unity.Burst.BurstCompile]
		struct CollectPositions : IJobForEachWithEntity<EffectData>
		{
			public NativeHashMap<int, float2>.ParallelWriter changes;
			public int side;

			public void Execute(Entity entity, int index, [ReadOnly] ref EffectData effect)
			{
				changes.TryAdd(index, new float2(effect.position.x * side, effect.position.y));
			}
		}

		[Unity.Burst.BurstCompile]
		struct TransformPositionsJob : IJobParallelForTransform
		{
			[ReadOnly] public NativeHashMap<int, float2> changes;
			internal float delta;

			public void Execute(int index, TransformAccess transform)
			{
				if (changes.TryGetValue(index, out float2 position))
				{
					transform.localPosition = new float3(position.x, transform.localPosition.y, position.y);
				}
			}
		}

	}
}


using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using UnityEngine.Jobs;
using Legacy.Database;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattleInitialization))]

	public class BattleBucketsSystem : ComponentSystem
	{
		private EntityQuery _query_minions;
		private EntityQuery _query_effects;

		private EntityQueryMask _mask_hero;
		private EntityQueryMask _mask_nonTargeted;

		private NativeHashMap<byte, MinionClientBucket> _minions;
		public NativeHashMap<byte, MinionClientBucket> Minions => _minions;

		private NativeHashMap<byte, EffectClientBucket> _effects;
		public NativeHashMap<byte, EffectClientBucket> Effects => _effects;

		protected override void OnCreate()
		{
			_query_minions = GetEntityQuery(
				ComponentType.ReadOnly<MinionData>(),
				ComponentType.ReadOnly<EntityDatabase>()
			);

			_query_effects = GetEntityQuery(
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<EntityDatabase>()
			);

			_mask_hero = EntityManager.GetEntityQueryMask(GetEntityQuery(typeof(MinionHeroTag)));

			_minions = new NativeHashMap<byte, MinionClientBucket>(256, Allocator.Persistent);
			_effects = new NativeHashMap<byte, EffectClientBucket>(256, Allocator.Persistent);
		}

		protected override void OnDestroy()
		{
			EntityManager.CompleteAllJobs();
			_minions.Dispose();
			_effects.Dispose();
		}

		protected override void OnUpdate()
		{
			_minions.Clear();
			_effects.Clear();

			var inputDeps = new FindMinions
			{
				minions = _minions.AsParallelWriter(),
				hero = _mask_hero,
			}.Schedule(_query_minions);

			if(!_query_effects.IsEmptyIgnoreFilter)
			{
				inputDeps = new FindEffects
				{
					effects = _effects.AsParallelWriter()
				}.Schedule(_query_effects, inputDeps);
			}

			inputDeps.Complete();
		}

	    [Unity.Burst.BurstCompile]
		struct FindEffects : IJobForEachWithEntity<EffectData, EntityDatabase>
		{
			public NativeHashMap<byte, EffectClientBucket>.ParallelWriter effects;

			public void Execute(
				Entity entity,
				int index,
				[ReadOnly] ref EffectData effect,
				[ReadOnly] ref EntityDatabase database
			)
			{
				effects.TryAdd(database.index, new EffectClientBucket
				{
					entity = entity,
					effect = effect,
					database = database
				});
			}
		}

		[Unity.Burst.BurstCompile]
		struct FindMinions : IJobForEachWithEntity<MinionData, EntityDatabase>
		{
			public NativeHashMap<byte, MinionClientBucket>.ParallelWriter minions;
			public EntityQueryMask hero;

			public void Execute(
				Entity entity,
				int index,
				[ReadOnly] ref MinionData minion,
				[ReadOnly] ref EntityDatabase repl
			)
			{
				minions.TryAdd(repl.index, new MinionClientBucket
				{
					entity = entity,
					minion = minion,
					repl = repl,
					state = new MinionBucketState
					{
						isHero = hero.Matches(entity),
						isEnable = true,
					}
				});;
			}
		}

	}
}
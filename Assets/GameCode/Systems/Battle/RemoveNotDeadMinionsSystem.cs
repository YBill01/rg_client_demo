using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

using Legacy.Database;
using UnityEngine;
using System.Collections.Generic;

namespace Legacy.Client
{
	// Эта система призвана решить проблему с не умирающими миньонами при кратковременных разрывах соединения
	// Если Во время разрыва соединения миньон умирает, то для него перестают приходить снапшоты и сущность остается зависшей. 
	// Точно также висит и миньон в игре, привязанный к зависшей сущности

	// Эта система возвращает миньонов в пул и удаляет сущности для которых долго не приходят снапшоты
	[UpdateInGroup(typeof(BattleInitialization))]
	[UpdateBefore(typeof(SnapshotMinionsSystem))]
	public class RemoveNotDeadMinionsSystem : JobComponentSystem
	{
		private EntityQuery snapshotsQuery;
		private EntityQuery replicatedQuery;
		private EntityQuery battleQuery;

		private Dictionary<ushort, long> snapshotTimers = new Dictionary<ushort, long>();
		private const int minionDataExpireTime = 1000;


		protected override void OnCreate()
		{
			snapshotsQuery = GetEntityQuery(
				ComponentType.ReadOnly<MinionSnapshot>()
			);

			replicatedQuery = GetEntityQuery(
				ComponentType.ReadWrite<MinionData>(),
				ComponentType.ReadWrite<MinionInitBehaviour>(),
				ComponentType.ReadOnly<EntityDatabase>()
			);

			battleQuery = GetEntityQuery(ComponentType.ReadWrite<BattleInstance>());

			RequireForUpdate(battleQuery);
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var snapshotsCount = snapshotsQuery.CalculateEntityCount();
			var minionsCount = replicatedQuery.CalculateEntityCount();

			var snapshotsForMinions = new NativeHashMap<ushort, byte>(snapshotsCount, Allocator.TempJob);
			var minionsWithoutSnapshots = new NativeHashMap<ushort, Entity>(minionsCount, Allocator.TempJob);

			var battle = battleQuery.GetSingleton<BattleInstance>();

			inputDeps = new CollectSnapshotsJob
			{
				snapshotsForMinions = snapshotsForMinions.AsParallelWriter()
			}.Schedule(snapshotsQuery, inputDeps);

			inputDeps.Complete();

			inputDeps = new CheckMinionsListJob
			{
				snapshotsForMinions = snapshotsForMinions,
				minionsWithoutSnapshots = minionsWithoutSnapshots.AsParallelWriter()
			}.Schedule(replicatedQuery, inputDeps);

			inputDeps.Complete();

			var newSnapshotTimers = new Dictionary<ushort, long>();
			var minionIndexes = minionsWithoutSnapshots.GetKeyArray(Allocator.TempJob);
			foreach (var index in minionIndexes)
			{
				if (snapshotTimers.ContainsKey(index))
				{
					var value = snapshotTimers[index];
					if (value - battle.timer > minionDataExpireTime)
					{
						minionsWithoutSnapshots.TryGetValue(index, out Entity minion);
						var mib = EntityManager.GetComponentObject<MinionInitBehaviour>(minion);
						var mp = mib.GetComponent<MinionPanel>();
						mib.MakeForceDisposing();
						mp.Delete();
						EntityManager.DestroyEntity(minion);

						Debug.LogError($"RemoveNotDeadMinionsSystem MakeForceDisposing {mib.name}");
					}
					else
					{
						newSnapshotTimers.Add(index, value);
					}
				}
				else
				{
					newSnapshotTimers.Add(index, battle.timer);
				}

			}

			snapshotTimers = newSnapshotTimers;
			
			snapshotsForMinions.Dispose();
			minionsWithoutSnapshots.Dispose();
			minionIndexes.Dispose();

			return inputDeps;
		}

#if !UNITY_EDITOR
		[Unity.Burst.BurstCompile]
#endif
		struct CollectSnapshotsJob : IJobForEachWithEntity<MinionSnapshot>
		{
			public NativeHashMap<ushort, byte>.ParallelWriter snapshotsForMinions;

			public void Execute(Entity entity, int index, [ReadOnly] ref MinionSnapshot snapshot)
			{
				snapshotsForMinions.TryAdd(snapshot.repl.index, 0);
			}
		}


#if !UNITY_EDITOR
		[Unity.Burst.BurstCompile]
#endif
		struct CheckMinionsListJob : IJobForEachWithEntity<MinionData, EntityDatabase>
		{
			[ReadOnly] public NativeHashMap<ushort, byte> snapshotsForMinions;
			public NativeHashMap<ushort, Entity>.ParallelWriter minionsWithoutSnapshots;

			public void Execute(
				Entity entity,
				int index,
				ref MinionData minion,
				[ReadOnly] ref EntityDatabase repl
				)
			{
				if (snapshotsForMinions.ContainsKey(repl.index))
					return;

				minionsWithoutSnapshots.TryAdd(repl.index, entity);
			}
		}
	}
}
using Unity.Collections;
using Unity.Entities;
using Legacy.Database;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattleInitialization))]
	[UpdateAfter(typeof(SnapshotEffectsSystem))]

	public class SpawnEffectsSystem : ComponentSystem
	{

		private EntityQuery _query_snapshots;
		private BattleBucketsSystem _buckets;
		private NativeHashMap<byte, EffectSnapshot> _create_effects;

		protected override void OnCreate()
		{
			_query_snapshots = GetEntityQuery(
				ComponentType.ReadOnly<EffectSnapshot>()
			);

			_buckets = World.GetOrCreateSystem<BattleBucketsSystem>();
			_create_effects = new NativeHashMap<byte, EffectSnapshot>(32, Allocator.Persistent);

			RequireForUpdate(_query_snapshots);
		}

		protected override void OnDestroy()
		{
			_create_effects.Dispose();
		}

		protected override void OnUpdate()
		{
			_create_effects.Clear();

			var inputDeps = new DetectNewJob
			{
				buckets = _buckets.Effects,
				created = _create_effects.AsParallelWriter(),
				buffer = PostUpdateCommands.AsParallelWriter()
			}.Schedule(_query_snapshots);

			inputDeps.Complete();
		}

		struct DetectNewJob : IJobForEachWithEntity<EffectSnapshot>
		{
			public NativeHashMap<byte, EffectSnapshot>.ParallelWriter created;

			public EntityCommandBuffer.ParallelWriter buffer;
			[ReadOnly] internal NativeHashMap<byte, EffectClientBucket> buckets;

			public void Execute(Entity entity, int index, [ReadOnly] ref EffectSnapshot snapshot)
			{
				if (snapshot.effect.state > EffectState.Expire) return;
				if (buckets.ContainsKey(snapshot.database.index)) return;

				if (created.TryAdd(snapshot.database.index, snapshot))
				{
					if (Database.Effects.Instance.Get(snapshot.database.db, out BinaryEffect binary))
					{
						if (snapshot.database.index > 0)
						{
							var _entity = buffer.CreateEntity(index);
							for (byte i = 0; i < binary.components.Count; ++i)
							{
								Components.Instance.Attach(
									binary.components[i],
									snapshot.database.db,
									index,
									buffer,
									_entity
								);
							}
							buffer.AddComponent(index, _entity, snapshot.effect);
							buffer.AddComponent(index, _entity, snapshot.database);

							UnityEngine.Debug.Log($"SpawnEffectSystem >> Index:{snapshot.database.index} Effect: {snapshot.database.db}");
						}
					}
				}
			}
		}

	}
}

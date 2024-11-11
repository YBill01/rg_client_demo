using Unity.Collections;
using Unity.Entities;
using Legacy.Database;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattleInitialization))]
    [UpdateAfter(typeof(SnapshotMinionsSystem))]
    [UpdateBefore(typeof(BattleBucketsSystem))]

    public class SpawnMinionsSystem : ComponentSystem
    {

        private EntityQuery _query_snapshots;
        private BattleBucketsSystem _buckets;

        protected override void OnCreate()
        {
            _query_snapshots = GetEntityQuery(
                ComponentType.ReadOnly<MinionSnapshot>()
            );

            _buckets = World.GetOrCreateSystem<BattleBucketsSystem>();

            RequireForUpdate(_query_snapshots);
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnUpdate()
        {
            var snapshots = _query_snapshots.ToComponentDataArray<MinionSnapshot>(Allocator.TempJob);

            for(int i = 0; i < snapshots.Length; i++)
            {
                var snapshot = snapshots[i];
                if (snapshot.minion.state < MinionState.Death/* && snapshot.minion.state != MinionState.Paralize*/)
                {
                    if (!_buckets.Minions.ContainsKey(snapshot.repl.index))
                    {
                        if (snapshot.repl.index > 0)
                        {
                            var _entity = EntityManager.CreateEntity();
                            EntityManager.AddComponentData(_entity, snapshot.minion);
                            EntityManager.AddComponentData(_entity, snapshot.repl);
                            _buckets.Minions.Add(snapshot.repl.index, default);
                        }

                        UnityEngine.Debug.Log($"SpawnSystem >> replicated :{snapshot.repl} ; snapshot number i = {i} ; Minion: {snapshot.minion}");
                    }
                }
            }

            snapshots.Dispose();
        }

        struct DetectNewJob : IJobForEachWithEntity<MinionSnapshot>
        {

            public EntityCommandBuffer buffer;
            [ReadOnly] internal NativeHashMap<byte, MinionClientBucket> buckets;

            public void Execute(Entity entity, int index, [ReadOnly] ref MinionSnapshot snapshot)
            {
                if (snapshot.minion.state < MinionState.Death/* && snapshot.minion.state != MinionState.Paralize*/)
                {
                    if (!buckets.ContainsKey(snapshot.repl.index))
                    {
                        if (snapshot.repl.index > 0)
                        {
                            var _entity = buffer.CreateEntity();
                            buffer.AddComponent(_entity, snapshot.minion);
                            buffer.AddComponent(_entity, snapshot.repl);
                        }

                        UnityEngine.Debug.Log($"SpawnSystem >> replicated :{snapshot.repl} Minion: {snapshot.minion}");
                    }
                }
            }
        }

    }
}

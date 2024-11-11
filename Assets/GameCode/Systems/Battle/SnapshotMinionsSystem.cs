using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{

    [UpdateInGroup(typeof(BattleSimulation))]

    public class SnapshotMinionsSystem : SystemBase
    {
        private EntityQuery _query_snapshots;

        private BattleSystems _battle;

        private EndSimulationEntityCommandBufferSystem _barrier;
        protected override void OnCreate()
        {
            _query_snapshots = GetEntityQuery(ComponentType.ReadOnly<MinionSnapshot>());

            _barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            _battle = World.GetOrCreateSystem<BattleSystems>();

            RequireForUpdate(_query_snapshots);
        }

        protected override void OnUpdate()
        {
            var _interpolate_time = _battle.CurrentTime - AppInitSettings.Instance.snapshotExpireTime;

            var snapshots = new NativeMultiHashMap<ushort, MinionSnapshot>(1024, Allocator.Persistent);

            var expire_time = _interpolate_time - AppInitSettings.Instance.snapshotDiffTime;
            var buffer = _barrier.CreateCommandBuffer();
            var death = GetComponentDataFromEntity<StateDeath>(true);

            Entities
               .ForEach((Entity entity, ref MinionSnapshot snapshot) =>
               {
                   if (expire_time > snapshot.time)
                   {
                       buffer.DestroyEntity(entity);
                       return;
                   }
                   snapshots.Add(snapshot.repl.index, snapshot);
               }).Run();

            Entities
                .WithReadOnly(death)
                .WithReadOnly(snapshots)
                .ForEach(
                (Entity _entity, ref MinionData minion, in EntityDatabase repl) =>
                {
                    //if (minion.state == MinionState.Death) return;

                    var _length = snapshots.CountValuesForKey(repl.index);
                    if (_length > 1)
                    {
                        var _values = snapshots.GetValuesForKey(repl.index);
                        var _snapshots = new NativeArray<MinionSnapshot>(_length, Allocator.Temp);
                        var _index = 0;
                        foreach (var _info in _values)
                        {
                            _snapshots[_index++] = _info;
                        }
                        _snapshots.Sort();

                        for (int i = 0; i < _length; ++i)
                        {
                            if (i > 0)
                            {
                                if (_snapshots[i].time < _interpolate_time)
                                {
                                    var _diff_first = (float)(_interpolate_time - _snapshots[i].time);
                                    var _diff_second = (float)(_snapshots[i - 1].time - _snapshots[i].time);

                                    minion.Interpolate(
                                        _snapshots[i].minion,
                                        _snapshots[i - 1].minion,
                                        _diff_first / _diff_second
                                    );

                                    if (minion.state == MinionState.Death)
                                    {
                                        if (!death.HasComponent(_entity))
                                        {
                                            buffer.AddComponent(_entity, new StateDeath { expire = _interpolate_time + 500u });
                                        }
                                    }

                                    return;
                                }

                            }
                        }
                    }
                    //else if (_length == 1)
                    //{
                    //    var _value = snapshots.GetValuesForKey(repl.index).Current;
                    //    minion.Interpolate(_value.minion, _value.minion, 0);
                    //}
                }).Run();

            snapshots.Dispose();
        }


    }
}


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

    public class MinionTransformSystem : JobComponentSystem
    {

        struct MinionMoveData
        {
            public bool isHero;
            public bool isComplex;
            public float2 position;
            public short rotation;
        }

        private EntityQuery _query_minions;
        private EntityQueryMask _complex;
        private NativeHashMap<int, MinionMoveData> _changes;
        private BattleSystems _battle;

        protected override void OnCreate()
        {
            _query_minions = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.ReadOnly<Transform>()
            );

            _changes = new NativeHashMap<int, MinionMoveData>(128, Allocator.Persistent);
            _battle = World.GetOrCreateSystem<BattleSystems>();
            _complex = EntityManager.GetEntityQueryMask(GetEntityQuery(ComponentType.ReadOnly<ComplexUnit>()));
            RequireForUpdate(_query_minions);
            RequireSingletonForUpdate<BattleInstance>();
        }

        protected override void OnDestroy()
        {
            _changes.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var _battle_instance = GetSingleton<BattleInstance>();
            var _player = _battle_instance.players[_battle_instance.players.player];

            _changes.Clear();


            inputDeps = new CollectPositions
            {
                changes = _changes.AsParallelWriter(),
                complex = _complex,
                side = _player.side == BattlePlayerSide.Right ? -1 : 1
            }.Schedule(_query_minions, inputDeps);


            var _transforms = _query_minions.GetTransformAccessArray();
            for (int i = 0; i < _transforms.length; i++)
            {
                if (_transforms[i].GetComponent<ComplexUnit>())
                {
                    _transforms[i] = _transforms[i].GetComponent<ComplexUnit>().RotatedObject;
                }
            }

            inputDeps = new TransformPositionsJob
            {
                changes = _changes,
                delta = _battle.DeltaTime
            }.Schedule(_transforms, inputDeps);

            return inputDeps;
        }

        [Unity.Burst.BurstCompile]
        struct CollectPositions : IJobForEachWithEntity<MinionData>
        {
            public NativeHashMap<int, MinionMoveData>.ParallelWriter changes;
            public int side;
            public EntityQueryMask complex;

            public void Execute(Entity entity, int index, [ReadOnly] ref MinionData minion)
            {
                var position = complex.Matches(entity) ? float2.zero : new float2(minion.mposition.x * side, minion.mposition.y);
                changes.TryAdd(index, new MinionMoveData
                {
                    position = position,
                    rotation = (short)(minion.mrotation * side),
                    isHero = minion.layer == MinionLayerType.Hero
                });
            }
        }

        [Unity.Burst.BurstCompile]
        struct TransformPositionsJob : IJobParallelForTransform
        {
            [ReadOnly] public NativeHashMap<int, MinionMoveData> changes;
            internal float delta;

            public void Execute(int index, TransformAccess transform)
            {
                if (changes.TryGetValue(index, out MinionMoveData movement))
                {
                    transform.localPosition = new float3(movement.position.x, transform.localPosition.y, movement.position.y);
                    if (movement.isHero) transform.position = new Vector3(transform.localPosition.x, 0.4f, transform.localPosition.z);
                    var r = transform.rotation;
                    var e = r.eulerAngles;
                    e.x = 0;
                    e.z = 0;
                    r.eulerAngles = e;
                    transform.rotation = r;
                    transform.rotation = Quaternion.Lerp(
                        transform.rotation,
                        Quaternion.Euler(0, movement.rotation, 0),
                        delta * 5.0f
                    );
                }
            }
        }

    }
}


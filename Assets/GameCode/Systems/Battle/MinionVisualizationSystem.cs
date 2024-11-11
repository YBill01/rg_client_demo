using Legacy.Database;
using Legacy.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]

    public class MinionVisualizationSystem : JobComponentSystem
    {
        private struct minion_info
        {
            public int index;
            public Entity minionEntity;
            public MinionData minion;
            public EntityDatabase database;
        }
        private struct changes_info
        {
            public int index;
        }

        private EntityQuery _query_minions;
        private EntityQuery _spawn_removed;
        private EntityQuery _spawn_added;

        private NativeHashMap<Entity, MinionState> _states;
        private NativeArray<ComponentType> _previous_states;

        private NativeList<changes_info> _changes;
        private NativeQueue<Entity> _updated;

        private EndSimulationEntityCommandBufferSystem _barrier;

        protected override void OnCreate()
        {
            _query_minions = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.ReadOnly<Animator>(),
                ComponentType.ReadOnly<EntityDatabase>()
                );

            _spawn_removed = GetEntityQuery(
                  ComponentType.ReadOnly<MinionAnimation>(),
                  ComponentType.Exclude<MinionData>()
              );


            _spawn_added = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>()
            );

            _states = new NativeHashMap<Entity, MinionState>(256, Allocator.Persistent);
            _previous_states = new NativeArray<ComponentType>(128, Allocator.Persistent);//length constrained by max units

            _changes = new NativeList<changes_info>(Allocator.Persistent);
            _updated = new NativeQueue<Entity>(Allocator.Persistent);

            _barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var buffer = _barrier.CreateCommandBuffer().AsParallelWriter();
            var changes = new NativeList<changes_info>(Allocator.TempJob);
            var states = _states;

            inputDeps = Entities
                .WithReadOnly(states)
                .WithNativeDisableParallelForRestriction(changes)
                .WithAll<Animator>()
                .ForEach((Entity entity, int entityInQueryIndex, ref MinionData minion, ref EntityDatabase database) =>
                {
                    if (states.TryGetValue(entity, out MinionState state))
                    {
                        if (minion.state != state)
                        {
                            changes.Add(new changes_info { index = database.index });
                        }
                    }
                })
                .Schedule(inputDeps);

            inputDeps.Complete();

            //------------------------------------------------------------

            var _state = _states;
            var previous_states = _previous_states;

            inputDeps = Entities
                .WithNativeDisableParallelForRestriction(changes)
                .WithNativeDisableParallelForRestriction(_state)
                .WithNativeDisableParallelForRestriction(previous_states)
                .WithAll<Animator>()
              .ForEach((Entity entity, int entityInQueryIndex, ref MinionData minion, ref EntityDatabase database) =>
              {
                  if (changes.Length > 0)
                  {
                      for (int i = 0; i < changes.Length; i++)
                      {
                          if (changes[i].index == database.index)
                          {
                              var state = minion.state;
                              _state[entity] = state;
                              SetStates(entity, entityInQueryIndex, minion, database, ref buffer, ref previous_states);
                          }
                      }
                  }
              })
              .Schedule(inputDeps);

            inputDeps.Complete();

            //----------------------------------------------------------------

            if (!_spawn_added.IsEmptyIgnoreFilter)
            {
                var addStatedParallel = _states.AsParallelWriter();
                var updatedParallel = _updated.AsParallelWriter();
                inputDeps = Entities
                    .WithNone<MinionAnimation>()
                   .ForEach((Entity entity, int entityInQueryIndex, ref MinionData minion) =>
                   {
                       var state = MinionState.Undefined;
                       if (addStatedParallel.TryAdd(entity, state))
                       {
                           updatedParallel.Enqueue(entity);
                           buffer.AddComponent(entityInQueryIndex, entity, ComponentType.ReadOnly<MinionAnimation>());
                       }
                   })
                   .Schedule(inputDeps);
                inputDeps.Complete();
            }

            if (!_spawn_removed.IsEmptyIgnoreFilter)
            {
                var updatedParallelRemove = _updated.AsParallelWriter();
                var _statesParallelRemove = _states;

                inputDeps = Entities
                    .WithNone<MinionData>()
                    .WithNativeDisableParallelForRestriction(_statesParallelRemove)
                    .ForEach((Entity entity, int entityInQueryIndex, ref MinionAnimation minion) =>
                    {
                        updatedParallelRemove.Enqueue(entity);
                        _statesParallelRemove.Remove(entity);
                        buffer.RemoveComponent(entityInQueryIndex, entity, ComponentType.ReadOnly<MinionAnimation>());
                    })
                    .Schedule(inputDeps);

            }

            _barrier.AddJobHandleForProducer(inputDeps);
            changes.Dispose();
            return inputDeps;
        }

        private static void SetStates(Entity entity, int entityInQueryIndex, MinionData minion, EntityDatabase database, ref EntityCommandBuffer.ParallelWriter buffer, ref NativeArray<ComponentType> previous_states)
        {
            ComponentType component = ComponentType.ReadOnly<SpawnWaitState>();
            //var _animator = EntityManager.GetComponentObject<Animator>(entity);
            //var minionSoundManager = _animator.gameObject.GetComponent<MinionSoundManager>();
            switch (minion.state)
            {
                case MinionState.Spawn:
                    component = ComponentType.ReadOnly<StateSpawning>();//+
                    break;
                case MinionState.Idle:
                    component = ComponentType.ReadOnly<StateWaiting>();//+
                    break;
                case MinionState.Run:
                case MinionState.Move:
                    component = ComponentType.ReadOnly<StateNavigate>();//+
                    break;
                case MinionState.Attack:
                    //  minionSoundManager.PlayHit();
                    component = ComponentType.ReadOnly<StateAttack>();//+
                    break;
                case MinionState.Charge:
                    component = ComponentType.ReadOnly<StateCharge>();//+ charge played once
                    break;
                case MinionState.Paralize:
                    component = ComponentType.ReadOnly<StateParalize>();//
                    break;
                case MinionState.SkillPoint://?
                    break;
                case MinionState.Skill1:
                    component = ComponentType.ReadOnly<StateSkill1>();//+
                    break;
                case MinionState.Skill2:
                    component = ComponentType.ReadOnly<StateSkill2>();//+
                    break;
                case MinionState.Celebrating:
                    component = ComponentType.ReadOnly<StateCelebrating>();
                    break;
                case MinionState.Death:
                    //minionSoundManager.PlayDie();
                    //MinionsSoundsManager.RemoveSourceFromList(minionSoundManager, minion.side == BattlePlayerSide.Right);
                    component = ComponentType.ReadOnly<StateDeath>();//+
                    break;
                case MinionState.Undefined:
                    break;
                default:
                    break;
            }

            if (previous_states[database.index] != component)
            {
                buffer.AddComponent(entityInQueryIndex, entity, component);

                if (previous_states[database.index] != default)
                {
                    var _component = previous_states[database.index];
                    buffer.RemoveComponent(entityInQueryIndex, entity, _component);
                }

                previous_states[database.index] = component;
            }
        }


        [Unity.Burst.BurstCompile]
        struct AddedJob : IJobForEachWithEntity<MinionData>
        {
            public NativeHashMap<Entity, MinionState>.ParallelWriter states;
            public NativeQueue<Entity>.ParallelWriter updated;

            public void Execute(Entity entity, int index, [ReadOnly] ref MinionData minion)
            {
                var state = MinionState.Undefined;
                if (states.TryAdd(entity, state))
                {
                    updated.Enqueue(entity);
                }
            }
        }

        [Unity.Burst.BurstCompile]
        struct RemovedJob : IJobForEachWithEntity<MinionAnimation>
        {
            public NativeQueue<Entity>.ParallelWriter updated;
            public void Execute(Entity entity, int index, [ReadOnly] ref MinionAnimation minion)
            {
                updated.Enqueue(entity);
            }
        }



        [Unity.Burst.BurstCompile]
        struct UpdateJob : IJobForEachWithEntity<MinionData, EntityDatabase>
        {
            [ReadOnly] public NativeHashMap<Entity, MinionState> states;
            public NativeQueue<Entity>.ParallelWriter changes;

            public void Execute(Entity entity, int index, [ReadOnly] ref MinionData minion, [ReadOnly] ref EntityDatabase database)
            {
                if (states.TryGetValue(entity, out MinionState state))
                {
                    if (minion.state != state)
                    {
                        changes.Enqueue(entity);
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            _states.Dispose();
            _previous_states.Dispose();
            _changes.Dispose();
            _updated.Dispose();
        }

    }
}
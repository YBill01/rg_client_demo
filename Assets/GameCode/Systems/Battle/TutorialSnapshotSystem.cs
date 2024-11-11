using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Legacy.Database;
using Legacy.Server;
using UnityEngine.Events;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattleInitialization))]
    public class TutorialStepInvokeEvent : UnityEvent<BinaryTutorialEvent> { };

    public class TutorialSnapshotSystem : ComponentSystem
    {
        private static TutorialStepInvokeEvent _tutorialStepEvent = new TutorialStepInvokeEvent();
        public static TutorialStepInvokeEvent TutorialStepEvent => _tutorialStepEvent;
        private EntityQuery _query_snapshot;
        private EntityQuery _query_tutorial;
        private NativeQueue<BinaryTutorialEvent> _events;

        protected override void OnCreate()
        {
            _tutorialStepEvent = new TutorialStepInvokeEvent();

            _query_snapshot = GetEntityQuery(ComponentType.ReadOnly<TutorialSnapshot>());
            _query_tutorial = GetEntityQuery(ComponentType.ReadOnly<TutorialInstance>());
            _events = new NativeQueue<BinaryTutorialEvent>(Allocator.Persistent);
            RequireSingletonForUpdate<BattleInstance>();
        }

        protected override void OnDestroy()
        {
            _events.Dispose();
        }
        protected override void OnUpdate()
        {
            var _buffer = PostUpdateCommands;


            if (_query_tutorial.IsEmptyIgnoreFilter)
            {
                if (!_query_snapshot.IsEmptyIgnoreFilter)
                {
                    var _snapshots = _query_snapshot.ToComponentDataArray<TutorialSnapshot>(Allocator.TempJob);
                    if (_snapshots.Length > 0)
                    {
                        var tutorialEntity = _buffer.CreateEntity();
                        var _tutorial = new TutorialInstance
                        {
                            index = _snapshots[0].instance.index,
                            currentTrigger = _snapshots[0].instance.currentTrigger
                        };
                        _buffer.AddComponent(tutorialEntity, _tutorial);

                        _snapshots = CreateFirtsEventInstance(_buffer, _snapshots);

                        PlayPreviousTutorialsTriggersIfReconnected(_buffer, _tutorial);

                    }
                    _snapshots.Dispose();
                }
            }
            if (!_query_tutorial.IsEmptyIgnoreFilter)
            {
                var _tutorial = GetSingleton<TutorialInstance>();
                var _tutorialEntity = GetSingletonEntity<TutorialInstance>();

                var inputDeps = new UpdateTutorialInstanceJob
                {
                    buffer = _buffer.AsParallelWriter(),
                    tutorial = _tutorial,
                    tutorialEntity = _tutorialEntity,
                    events = _events.AsParallelWriter()

                }.Schedule(_query_snapshot);
                inputDeps.Complete();

                while (_events.TryDequeue(out BinaryTutorialEvent tutorialEvent))
                {
                    _tutorialStepEvent.Invoke(tutorialEvent);
                }

                inputDeps.Complete();
            }

        }

        private static NativeArray<TutorialSnapshot> CreateFirtsEventInstance(EntityCommandBuffer _buffer, NativeArray<TutorialSnapshot> _snapshots)
        {
            var tutorial_event = _snapshots[0].instance.GetCurrentTriggerEvent();
            var event_entity = _buffer.CreateEntity();
            var event_capture = new EventCaptureInstance { _event = tutorial_event.type };
            _buffer.AddComponent(event_entity, event_capture);
            return _snapshots;
        }

        private void PlayPreviousTutorialsTriggersIfReconnected(EntityCommandBuffer _buffer, TutorialInstance _tutorial)
        {
            if (_tutorial.TryGetPreviousCurrentTriggerEvent(out (BinaryTutorialEvent, int) _event_index))
            {
                var _event = _event_index.Item1;
                var index = _event_index.Item2;
                _events.Enqueue(_event);

                var unblockCardsIndex = -1;
                var blockCardsIndex = -1;
                var unblockSkillIndex = -1;
                var blockSkillIndex = -1;
                //cards
                for (int i = index - 1; i >= 0; i--)
                {
                    var eventType = _tutorial.GetPreviousCurrentTriggerEventByIndex(i).type;
                    if (eventType == TutorialEvent.UnblockCards)
                    {
                        unblockCardsIndex = i;

                        if (unblockCardsIndex >= blockCardsIndex)
                        {
                            var _event_entity = _buffer.CreateEntity();
                            var _event_capture = new EventCaptureInstance { _event = eventType };
                            _buffer.AddComponent(_event_entity, _event_capture);
                        }
                    }
                    if (eventType == TutorialEvent.BlockCards)
                    {
                        blockCardsIndex = i; 
                        if (unblockCardsIndex <= blockCardsIndex)
                        {
                            var _event_entity = _buffer.CreateEntity();
                            var _event_capture = new EventCaptureInstance { _event = eventType };
                            _buffer.AddComponent(_event_entity, _event_capture);
                        }
                    }
                }
                //skills
                for (int i = index - 1; i >= 0; i--)
                {
                    var eventType = _tutorial.GetPreviousCurrentTriggerEventByIndex(i).type;
                    if (eventType == TutorialEvent.UnblockSkills || eventType == TutorialEvent.UnblockSkill_1 || eventType == TutorialEvent.UnblockSkill_2)
                    {
                        unblockSkillIndex = i;

                        if (unblockSkillIndex >= blockSkillIndex)
                        {
                            unblockSkillIndex = i;
                            var _event_entity = _buffer.CreateEntity();
                            var _event_capture = new EventCaptureInstance { _event = eventType };
                            _buffer.AddComponent(_event_entity, _event_capture);
                        }
                    }
                    if (eventType == TutorialEvent.BlockSkills)
                    {
                        blockSkillIndex = i;
                        if (unblockSkillIndex <= blockSkillIndex)
                        {
                            var _event_entity = _buffer.CreateEntity();
                            var _event_capture = new EventCaptureInstance { _event = eventType };
                            _buffer.AddComponent(_event_entity, _event_capture);
                        }
                    }
                }
            }
        }

        struct UpdateTutorialInstanceJob : IJobForEachWithEntity<TutorialSnapshot>
        {
            internal EntityCommandBuffer.ParallelWriter buffer;
            internal TutorialInstance tutorial;
            internal Entity tutorialEntity;
            internal NativeQueue<BinaryTutorialEvent>.ParallelWriter events;
            public void Execute(
                Entity entity,
                int index,
                [ReadOnly] ref TutorialSnapshot snapshot
            )
            {

                if (snapshot.instance.currentTrigger > tutorial.currentTrigger)
                {
                    buffer.SetComponent(index, tutorialEntity, snapshot.instance);

                    var _event = tutorial.GetCurrentTriggerEvent();
                    var _entity = buffer.CreateEntity(index);
                    var event_capture = new EventCaptureInstance { _event = _event.type };
                    buffer.AddComponent(index, _entity, event_capture);
                    events.Enqueue(_event);
                }
                if (snapshot.instance.index != tutorial.index)
                {
                    buffer.SetComponent(index, tutorialEntity, snapshot.instance);
                }

                buffer.DestroyEntity(index, entity);

            }
        }
    }
}
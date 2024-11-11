/*using Legacy.Database;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class MenuTriggerSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _barrier;

        private EntityQuery _query_event;
        private ProfileInstance profile;

        private static MenuTutorialStepInvokeEvent _tutorialStepEvent = new MenuTutorialStepInvokeEvent();
        public static MenuTutorialStepInvokeEvent MenuTutorialStepEvent => _tutorialStepEvent;

        private static MenuTutorialFinishInvokeEvent _tutorialFinishEvent = new MenuTutorialFinishInvokeEvent();
        public static MenuTutorialFinishInvokeEvent MenuTutorialFinishEvent => _tutorialFinishEvent;

        protected override void OnCreate()
        {
            _barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            _query_event = GetEntityQuery(ComponentType.ReadOnly<MenuEventInstance>());
            profile = ClientWorld.Instance.GetExistingSystem<HomeSystems>().UserProfile;
          //  RequireForUpdate(_query_event);   // ломает тутор, после первого тригера, остальные уже не срабатывают
            RequireSingletonForUpdate<MenuTutorialInstance>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (TutorialEntityClass.getInstance().TutorialEntity == Entity.Null) return inputDeps;
            var _buffer = _barrier.CreateCommandBuffer().AsParallelWriter();

            //если не туториал - просто удаляем все тригеры
            var _job = new DestroyTriggersJob() { buffer = _buffer };
            inputDeps = _job.Schedule(_query_event, inputDeps);
            inputDeps.Complete();

            var _tutorial = GetSingleton<MenuTutorialInstance>();
            if (_tutorial.softTutorialState > 0)
            {
                var _tutorialEnitity = GetSingletonEntity<MenuTutorialInstance>();

                //Debug.Log("GetScenarioindex: " + _tutorial.softTutorialState);
                var scenarioIndex = SoftTutorial.GetScenarioIndex(_tutorial.softTutorialState);

                MenuTutorial.Instance.Get(scenarioIndex, out var binaryTutorial);

                //Если все события закончились - просто удаляем все тригеры
                if (!profile.TutorialsSteps.ContainsKey(scenarioIndex))
                {
                    profile.TutorialsSteps.Add(scenarioIndex, 0);
                }

                if (binaryTutorial.events.Count <= _tutorial.currentTrigger)
                {
                    var _job2 = new DestroyTriggersJob() { buffer = _buffer };
                    inputDeps = _job2.Schedule(_query_event, inputDeps);
                    inputDeps.Complete();

                    return inputDeps;
                }

                var _event = binaryTutorial.events[_tutorial.currentTrigger];
                var currentTime = (int)(UnityEngine.Time.time * 1000f);

                // Если мы Не ждем окончания таймера для запуска события для ранее пойманного триггера
                if (_tutorial._timer_start == int.MaxValue)
                {
                    //Следующий тригер - окончание таймера - так что мы считаем что уже поймали нужный тригер и заводим таймер
                    if (_event.trigger == MenuTutorialEventTrigger.OnInactivityTimer)
                    {
                        _tutorial._timer_start = currentTime;
                        WindowManager.Instance.playerActivity.isStart();
                        WindowManager.Instance.playerActivity.AddTarget(_event.timer, finishActicity);
                        EntityManager.SetComponentData(_tutorialEnitity, _tutorial);
                    }
                    else
                    if (_event.trigger == MenuTutorialEventTrigger.OnTimerEnd)
                    {
                        _tutorial._timer_start = currentTime;
                        EntityManager.SetComponentData(_tutorialEnitity, _tutorial);
                    }
                    else
                    {
                        var _result = new NativeArray<bool>(1, Allocator.TempJob);

                        var _job1 = new TriggerJob
                        {
                            buffer = _buffer,
                            tutorial_event = _event,
                            result = _result
                        };
                        inputDeps = _job1.Schedule(_query_event, inputDeps);

                        inputDeps.Complete();

                        //Мы поймали нужный триггер
                        if (_result[0])
                        {
                            _tutorial._timer_start = currentTime;
                            EntityManager.SetComponentData(_tutorialEnitity, _tutorial);
                        }
                        _result.Dispose();
                    }
                }

                //Таймер закончился
                if (currentTime - _tutorial._timer_start >= _event.timer && _event.trigger != MenuTutorialEventTrigger.OnInactivityTimer)
                {
                    finishActicity();
                }

                void finishActicity()
                {
                    UnityEngine.Debug.Log($"Catch menu trigger { _event.trigger } for event { _event.type } with message {_event.message}");

                    MenuTutorialStepEvent.Invoke(_event);

                    _tutorial.currentTrigger++;
                    _tutorial._timer_start = int.MaxValue;
                    EntityManager.SetComponentData(_tutorialEnitity, _tutorial);

                    ushort newMenuState = (ushort)(profile.MenuTutorialState | (ushort)_tutorial.softTutorialState);
                    ushort newMenuStep = _tutorial.currentTrigger;

                    if (_event.save_state)
                    {
                        profile.MenuTutorialState = newMenuState;
                        UnityEngine.Debug.Log($"<color=green> save tutor {_tutorial.softTutorialState}</color> with step {newMenuStep}");
                        NetworkMessageHelper.UpdateTutorialState(profile.HardTutorialState, 0, newMenuState, scenarioIndex, newMenuStep, profile.index);
                    }
                }
            }

            return inputDeps;
        }

        struct TriggerJob : IJobForEachWithEntity<MenuEventInstance>
        {
            public NativeArray<bool> result;
            public EntityCommandBuffer.ParallelWriter buffer;
            public BinaryMenuTutorialEvent tutorial_event;

            public void Execute(
                Entity entity,
                int index,
                [ReadOnly] ref MenuEventInstance eventInstance
            )
            {
                if (tutorial_event.trigger == eventInstance.trigger)
                {
                    result[0] = true;
                }

                buffer.DestroyEntity(index, entity);
            }
        }

        struct DestroyTriggersJob : IJobForEachWithEntity<MenuEventInstance>
        {
            public EntityCommandBuffer.ParallelWriter buffer;

            public void Execute(
                Entity entity,
                int index,
                [ReadOnly] ref MenuEventInstance eventInstance
            )
            {
                buffer.DestroyEntity(index, entity);
            }
        }
    }

    public class MenuTutorialStepInvokeEvent : UnityEvent<BinaryMenuTutorialEvent> { };
    public class MenuTutorialFinishInvokeEvent : UnityEvent<SoftTutorial.SoftTutorialState> { };
}*/
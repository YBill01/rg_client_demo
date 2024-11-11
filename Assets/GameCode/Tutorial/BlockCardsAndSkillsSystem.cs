using Legacy.Database;
using Legacy.Client;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Legacy.Client
{
    // Задача системы блокировать и разблокировать карты на основе системы тригеров и событий для туториалов
    // Эта система работает очень любопытно:
    // В OnUpdate мы запускаем джобу по отслеживанию событий и результат записываем в список из одного элемента - action
    // А на следующем OnUpdate мы обрабатываем это действие и помечаем его как выполненое и снова ищем новые события
    // Таким образом система не выполняет Complete() для того что бы начать работать с картами и их BattleCardDragBehaviour

    [UpdateInGroup(typeof(BattleSimulation))]
    public class BlockCardsAndSkillsSystem : JobComponentSystem
    {
        EntityQuery eventsQuery;
        private EndSimulationEntityCommandBufferSystem barrier;

        private NativeQueue<WhatToDo> actions;



        enum WhatToDo
        {
            Nothing,
            BlockCards,
            UnblockCards,
            BlockSkills,
            UnblockSkills,
            UnblockSkill_1,
            UnblockSkill_2
        }

        protected override void OnCreate()
        {
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            actions = new NativeQueue<WhatToDo>(Allocator.Persistent);

            eventsQuery = GetEntityQuery(ComponentType.ReadWrite<EventCaptureInstance>());
        RequireSingletonForUpdate<TutorialInstance>();
            RequireSingletonForUpdate<BattleInstance>();
        }

        protected override void OnDestroy()
        {
            actions.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var buffer = barrier.CreateCommandBuffer().AsParallelWriter();

            DoAction();

            inputDeps = new EventJob
            {
                buffer = buffer,
                action = actions.AsParallelWriter()
            }.Schedule(eventsQuery, inputDeps);

            //TODO решить эту ошибку или понянять почему элегантное решение этой системы не состоятельно. И убрать Complete
            //ArgumentException: The previously scheduled job BlockCardsAndSkillsSystem:EventJob writes to the UNKNOWN_OBJECT_TYPE EventJob.Data.buffer. 
            //You must call JobHandle.Complete() on the job BlockCardsAndSkillsSystem:EventJob, before you can write to the UNKNOWN_OBJECT_TYPE safely
            inputDeps.Complete();

            return inputDeps;
        }

        private void DoAction()
        {
            if (actions.Count > 0)
            {
                while (actions.TryDequeue(out WhatToDo action))
                {
                    if (action == WhatToDo.BlockCards || action == WhatToDo.UnblockCards)
                    {
                        foreach (var card in BattleInstanceInterface.instance.Cards)
                        {
                            card.IsBlockedByTutorial = action == WhatToDo.BlockCards;
                        }
                    }
                    else if(action == WhatToDo.BlockSkills || action == WhatToDo.UnblockSkills)
                    {
                        BattleInstanceInterface.instance.Skill1.IsBlockedByTutorial = action == WhatToDo.BlockSkills;
                        BattleInstanceInterface.instance.Skill2.IsBlockedByTutorial = action == WhatToDo.BlockSkills;
                    }
                    else if (action == WhatToDo.UnblockSkill_1)
                    {
                        BattleInstanceInterface.instance.Skill1.IsBlockedByTutorial = false;
                    }
                    else if (action == WhatToDo.UnblockSkill_2)
                    {
                        BattleInstanceInterface.instance.Skill2.IsBlockedByTutorial = false;
                    }
                }
            }
        }

		struct EventJob : IJobForEachWithEntity<EventCaptureInstance>
        {
            internal EntityCommandBuffer.ParallelWriter buffer;
            internal NativeQueue<WhatToDo>.ParallelWriter action;

            public void Execute(
                Entity entity,
                int index,
                ref EventCaptureInstance eventInstance
            )
            {
                if (eventInstance._event == TutorialEvent.BlockCards)
                {
                    action.Enqueue(WhatToDo.BlockCards);
                    buffer.DestroyEntity(index, entity);
                }

                if (eventInstance._event == TutorialEvent.UnblockCards)
                {
                    action.Enqueue(WhatToDo.UnblockCards);
                    buffer.DestroyEntity(index, entity);
                }

                if (eventInstance._event == TutorialEvent.BlockSkills)
                {
                    action.Enqueue(WhatToDo.BlockSkills);
                    buffer.DestroyEntity(index, entity);
                }

                if (eventInstance._event == TutorialEvent.UnblockSkills)
                {
                    action.Enqueue(WhatToDo.UnblockSkills);
                    buffer.DestroyEntity(index, entity);
                }

                if (eventInstance._event == TutorialEvent.UnblockSkill_1)
                {
                    action.Enqueue(WhatToDo.UnblockSkill_1);
                    buffer.DestroyEntity(index, entity);
                }

                if (eventInstance._event == TutorialEvent.UnblockSkill_2)
                {
                    action.Enqueue(WhatToDo.UnblockSkill_2);
                    buffer.DestroyEntity(index, entity);
                }
            }
        }
    }
}

//using Legacy.Database;
//using Legacy.Client;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;

//namespace Legacy.Server
//{
//    [UpdateInGroup(typeof(ServerInitializationSystemGroup))]

//    public class GiveCustomManaSystem : JobComponentSystem
//    {
//        EntityQuery eventsQuery;
//        private EndSimulationEntityCommandBufferSystem barrier;

//        protected override void OnCreate()
//        {
//            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

//            eventsQuery = GetEntityQuery(ComponentType.ReadWrite<EventCaptureInstance>());

//            RequireForUpdate(eventsQuery);
//            RequireSingletonForUpdate<TutorialInstance>();
//            RequireSingletonForUpdate<BattleInstance>();
//        }


//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            var buffer = barrier.CreateCommandBuffer().AsParallelWriter();
//            var battle = GetSingleton<BattleInstance>();
//            var battleEntity = GetSingletonEntity<BattleInstance>();
//            //var tutorial = GetSingleton<TutorialInstance>();

//            //Tutorial.Instance.Get(tutorial.index, out var binaryTutorial);
//            //var catchedEvent = binaryTutorial.events[tutorial.currentTrigger - 1];

//            var _job = new EventJob
//            {
//                buffer = buffer,
//                battle = battle,
//                battleEntity = battleEntity
//            };

//            inputDeps = _job.Schedule(eventsQuery, inputDeps);
//            inputDeps.Complete();

//            return inputDeps;
//        }

//        struct EventJob : IJobForEachWithEntity<EventCaptureInstance>
//        {
//            internal EntityCommandBuffer.ParallelWriter buffer;
//            internal BattleInstance battle;
//            internal Entity battleEntity;
//            //internal int manaToSet;

//            public void Execute(
//                Entity entity,
//                int index,
//                ref EventCaptureInstance eventInstance
//            )
//            {
//                if (eventInstance._event == TutorialEvent.SetMana)
//                {
//                    buffer.DestroyEntity(index, entity);

//                    var player = battle.players[battle.players.player];
//                    player.mana = 10;
//                    battle.players[battle.players.player] = player;

//                    buffer.SetComponent(index, battleEntity, battle);
//                }
//            }
//        }
//    }
//}

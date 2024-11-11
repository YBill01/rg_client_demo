//using Unity.Entities;
//using Legacy.Database;

//namespace Legacy.Server
//{
//    [UpdateBefore(typeof(BattlePrepareSystem))]
//    [UpdateInGroup(typeof(ServerInitializationSystemGroup))]
//    public class TutorialBattleStartSystem : SystemBase
//    {
//        private EndInitializationEntityCommandBufferSystem _barrier;
//        protected override void OnCreate()
//        {
//            _barrier = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
//        }

//        protected override void OnUpdate()
//        {
//            var buffer = _barrier.CreateCommandBuffer();
//            Entities
//                .ForEach((Entity entity, ref BattleInstance _battle, in BaseBattleSettings _settings,in TutorialBattleReadyRequest request) =>
//               {
//                    _battle.status = BattleInstanceStatus.Playing;
//                    _battle.timer = _settings.duration;

//                    buffer.RemoveComponent<TutorialBattleReadyRequest>(entity);
//              })
//                .Run();

//        }
//    }
//}


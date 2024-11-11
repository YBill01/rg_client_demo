//using Unity.Entities;

//using Legacy.Database;
//using Legacy.Client;

//namespace Legacy.Server
//{
//    [UpdateInGroup(typeof(ServerInitializationSystemGroup))]
//    public class BattlePlayerReadySystem : ComponentSystem
//    {
//        EntityQuery tutorialQuery;

//        protected override void OnCreate()
//        {
//            tutorialQuery = GetEntityQuery(ComponentType.ReadOnly<TutorialInstance>());

//            RequireSingletonForUpdate<BattleInstance>();
//            RequireSingletonForUpdate<BattlePlayerReadyRequest>();
//        }

//        protected override void OnUpdate()
//        {
//            var _entity = GetSingletonEntity<BattleInstance>();
//            var _battle = EntityManager.GetComponentData<BattleInstance>(_entity);
//            var _settings = EntityManager.GetComponentData<BaseBattleSettings>(_entity);
//            _battle.status = BattleInstanceStatus.Prepare;
//            _battle.timer = tutorialQuery.IsEmptyIgnoreFilter ? (int)(_settings.preprare) : (int)(60000);
//            _battle.players.player = 0;
//            PostUpdateCommands.SetComponent(_entity, _battle);

//            PostUpdateCommands.DestroyEntity(GetSingletonEntity<BattlePlayerReadyRequest>());

//            // create message instance
//            var _message = default(NetworkMessageRaw);
//            _battle.players.Serialize(ref _message);
//            _message.size = 0;

//            var _event = ClientWorld.Instance.EntityManager.CreateEntity();
//            ClientWorld.Instance.EntityManager.AddComponentData(_event, new NetworkMessageHelper
//            {
//                protocol = (byte)PlayerGameMessage.BattleInstance,
//                data = _message
//            });

//        }
//    }
//}


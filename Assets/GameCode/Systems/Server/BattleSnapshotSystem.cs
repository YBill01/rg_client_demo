//using Unity.Collections;
//using Unity.Entities;

//using Legacy.Database;
//using Legacy.Client;

//namespace Legacy.Server
//{
//    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
//    [UpdateAfter(typeof(BattlePauseSystem))]

//    public class BattleSnapshotSystem : ComponentSystem
//	{
//		private BattleBucketsSystem _buckets;
//        EntityQuery _query_tutorial;


//        protected override void OnCreate()
//		{
//            _buckets = World.GetOrCreateSystem<BattleBucketsSystem>();

//            var _query_battles = GetEntityQuery(
//                ComponentType.ReadWrite<BattleInstance>()
//            );

//            _query_tutorial = GetEntityQuery(ComponentType.ReadOnly<TutorialInstance>());

//            RequireForUpdate(_query_battles);
//        }

//        protected override void OnUpdate()
//		{
//            var _battle_instance = GetSingleton<BattleInstance>();
//            var _time = ServerInitializationSystemGroup.ElapsedMilliseconds;
//            var _client_manager = ClientWorld.Instance.EntityManager;            

//            if (_battle_instance.status > BattleInstanceStatus.Waiting && _battle_instance.status < BattleInstanceStatus.Close)
//            {
//                if (_time > _battle_instance.snapshot)
//                {
//                    _battle_instance.snapshot = _time + ServerSettings.SnapshotTime.IntValue;

//                    #region Battle
//                    var _entity = _client_manager.CreateEntity();
//                    _client_manager.AddComponentData(_entity, new BattleSnapshot
//                    {
//                        instance = _battle_instance,
//                        player = _battle_instance.players[0],
//                        time = _time
//                    });
//                    #endregion

               

//                    #region Collect Snapshots
//                    var _base_offset = _battle_instance.index * ServerSettings.MaxUnits.IntValue;
//                    // minions
//                    for (byte i = 0; i < _battle_instance.minions.point; i++)
//                    {
//                        var _original_index = _battle_instance.minions._get(i);
//                        var _unit_offset = _base_offset + _original_index;
//                        var _bucket = _buckets.Minions[_unit_offset];

//                        if (_bucket.database.index > 0)
//                        {
//                            var _minion_snapshot = _client_manager.CreateEntity();
//                            _client_manager.AddComponentData(_minion_snapshot, new MinionSnapshot
//                            {
//                                minion = _buckets.Minions[_unit_offset].minion,
//                                repl = _buckets.Minions[_unit_offset].database,
//                                time = _time
//                            });
//                        }
//                    }

//                    // effects
//                    for (byte i = 0; i < _battle_instance.effects.point; i++)
//                    {
//                        var _original_index = _battle_instance.effects._get(i);
//                        var _unit_offset = _base_offset + _original_index;

//                        var _effect_bucket = _buckets.Effects[_unit_offset];
//                        if (_effect_bucket.database.replicated)
//                        {
//                            var _minion_snapshot = _client_manager.CreateEntity();
//                            _client_manager.AddComponentData(_minion_snapshot, new EffectSnapshot
//                            {
//                                time = _time,
//                                database = _effect_bucket.database,
//                                effect = _effect_bucket.effect
//                            });
//                        }
//                    }
//                    #endregion
//                }
//            }
//        }
//	}
//}


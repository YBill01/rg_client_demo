//using Unity.Entities;
//using Legacy.Database;
//using Unity.Mathematics;
//using UnityEngine;
//using Legacy.Client;
//using System.Collections.Generic;
//using Unity.Collections;

//namespace Legacy.Server
//{
//    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
//    public class BattleInstantiateSystem : ComponentSystem
//    {

//        protected override void OnCreate()
//        {
//            RequireSingletonForUpdate<ObserverBattle>();
//        }

//        protected override void OnUpdate()
//        {
//            UnityEngine.Debug.Log("BattleInstantiateSystem");

//            var entity = GetSingletonEntity<ObserverBattle>();
//            var battle = EntityManager.GetComponentData<ObserverBattle>(entity);
//            PostUpdateCommands.RemoveComponent<ObserverBattle>(entity);

//            // campaign or tutorial
//            //if (battle.campaign.index != 0)
//            //    InstantiateCampaign(entity, battle);

//            ////if (battle.tutorial.index != 0)
//            ////    InstantiateTutorial(entity, battle);

//            if (battle.isSandbox == 1)
//                InstantiateSandbox(entity, battle);
//        }

//        private void InstantiateCampaign(Entity battleEntity, ObserverBattle battle)
//        {
//            var mission_index = battle.campaign.mission;

//            if (!Missions.Instance.Get(mission_index, out BinaryMission mission))
//            {
//                Debug.LogError($"Can not load mission {mission_index}");
//                return;
//            }

//            BattleInstance _instance = CreateBattleInstance(battle, mission);

//            PreparePlayers(battleEntity, battle, mission, ref _instance);
//            PrepareEntity(battleEntity, mission, _instance);
//            PrepareMinionsPool(_instance);
//        }

//        private void InstantiateTutorial(Entity battleEntity, ObserverBattle battle)
//        {
//            if (!Tutorial.Instance.Get(battle.tutorial.index, out BinaryTutorial binaryTutorial))
//            {
//                Debug.LogError($"Can not load tutorial {battle.tutorial.index}");
//                return;
//            }
//            var mission_index = binaryTutorial.mission;

//            foreach (var e in binaryTutorial.events)
//            {
//                if (e.type == TutorialEvent.SpawnUnit)
//                {
//                    if (Cards.Instance.Get(e.param_0, out BinaryCard card))
//                    {
//                        for (var j = 0; j < card.entities.Count; j++)
//                        {
//                            ObjectPooler.instance.InitMinion(card.entities[j]);
//                        }
//                    }
//                }
//            }

//            if (!Missions.Instance.Get(mission_index, out BinaryMission mission))
//            {
//                Debug.LogError($"Can not load mission {mission_index}");
//                return;
//            }

//            BattleInstance _instance = CreateBattleInstance(battle, mission, true);

//            PreparePlayers(battleEntity, battle, mission, ref _instance, battle.tutorial.index);//s
//            PrepareEntity(battleEntity, mission, _instance);//
//            PrepareMinionsPool(_instance);//

//            var _tutorialEnitity = PostUpdateCommands.CreateEntity();
//            PostUpdateCommands.AddComponent(_tutorialEnitity, new TutorialInstance { mission_index = mission.index, index = battle.tutorial.index });//?here
//            CreateObstaclesEntities();//remove
//        }

//        private void InstantiateSandbox(Entity battleEntity, ObserverBattle battle)
//        {
//            var mission_index = (ushort)17;

//            if (!Missions.Instance.Get(mission_index, out BinaryMission mission))
//            {
//                Debug.LogError($"Can not load mission {mission_index}");
//                return;
//            }

//            BattleInstance _instance = CreateBattleInstance(battle, mission);

//            PreparePlayers(battleEntity, battle, mission, ref _instance);
//            PrepareEntity(battleEntity, mission, _instance);
//            ServerWorld.Instance.LocalBattlePlayerReady();

//            //var _tutorialEnitity = PostUpdateCommands.CreateEntity();
//            PostUpdateCommands.AddComponent(battleEntity, new SandboxInstance());


//            //Prepare spesical sandbox deck
//            var deck = new NativeList<BinaryBattleCard>(Allocator.TempJob);
//            foreach (var card in Cards.Instance.List)
//            {
//                deck.Add(new BinaryBattleCard() { index = card.index, level = 1 });
//            }

//            SandboxPlayerDeck sandboxDeck = SandboxPlayerDeck.PrepareDeck(deck);
//            PostUpdateCommands.AddComponent(battleEntity, sandboxDeck);
//            deck.Dispose();
//            CreateObstaclesEntities();
//        }
//        private void CreateObstaclesEntities()
//        {
//            var tiles = BinaryGrid.Instance.Tiles.GetValueArray(Allocator.Persistent);
//            UnityEngine.Debug.Log("CreateObstaclesEntities");
//            for (int i = 0; i < tiles.Length; i++)
//            {
//                var _tile = tiles[i];
//                if (_tile.status == TileStatus.Blocked)
//                {
//                    var newEntity = PostUpdateCommands.CreateEntity();
//                    PostUpdateCommands.AddComponent(newEntity, new ObstacleComponent
//                    {
//                        index = _tile.index,//max units
//                        colliderRadius = BinaryGrid.Instance.TileSize / 2,
//                        battle = byte.MaxValue,
//                        layer = MinionLayerType.Ground,
//                        mass = ushort.MaxValue,
//                        position = _tile.position,
//                    });
//                }
//                if (_tile.status == TileStatus.Waypoint)
//                {
//                    var newEntity = PostUpdateCommands.CreateEntity();
//                    PostUpdateCommands.AddComponent(newEntity, new ObstacleComponent
//                    {
//                        index = _tile.index,//max units
//                        colliderRadius = BinaryGrid.Instance.TileSize / 3,
//                        battle = byte.MaxValue,
//                        layer = MinionLayerType.Ground,
//                        mass = ushort.MaxValue,
//                        position = _tile.position,

//                    });
//                }
//            }
//            tiles.Dispose();


//        }

//        private void PrepareEntity(Entity entity, BinaryMission mission, BattleInstance _instance)
//        {
//            ///
//            PostUpdateCommands.AddComponent(entity, _instance);
//            PostUpdateCommands.AddComponent(entity, mission.settings);
//            PostUpdateCommands.AddComponent<BattleInstancePrepare>(entity);
//            ///
//        }

//        private void PreparePlayers(Entity entity, ObserverBattle battle, BinaryMission mission, ref BattleInstance _instance, int tutorialNumber = 0)
//        {
//            //var _settings = Database.Settings.Instance.Get<HeroRageSettings>();

//            ///
//            for (byte k = 0; k < _instance.players.count; ++k)
//            {
//                var _observer_player = battle[k];

//                if (_observer_player.profile.is_bot)
//                {
//                    ///
//                    var _bot_entity = PostUpdateCommands.CreateEntity();
//                    PostUpdateCommands.AddComponent(_bot_entity, new BattlePlayerBot
//                    {
//                        player = k,
//                        battle = entity
//                    });
//                    ///
//                    if (Bots.Instance.Get(mission.enemy, out BinaryBot bot))
//                    {
//                        if (battle.isSandbox == 1)
//                        {
//                            PostUpdateCommands.AddComponent<BotDisabled>(_bot_entity);
//                        }
//                    }
//                    ///

//                }

//                ///
//                var _settings_player = mission.settings.players[k];
//                /*var start_cooldown = _settings.cooldown;
//				if (tutorialNumber == 2)
//					start_cooldown -= 10000;
//				if (tutorialNumber == 3)
//					start_cooldown -= (int)(_settings.skill2_cooldown * 0.9f);*/

//                var _player = new BattlePlayer
//                {
//                    player = _observer_player.playerID,
//                    profile = _observer_player.profile,
//                    status = _observer_player.profile.is_bot ? BattlePlayerStatus.Ready : BattlePlayerStatus.Disconnected,
//                    deck = _observer_player.deck,
//                    mana = mission.settings.mana.start,
//                    position = new float2(_settings_player.x, _settings_player.z),
//                    side = _settings_player.side,
//                    //skillTimer = start_cooldown
//                };

//                for (byte h = 0; h < BattlePlayerHand.length; ++h)
//                {
//                    _player.hand[h] = _player.deck._next();
//                }

//                _instance.players[k] = _player;
//                ///
//            }
//        }

//        private void PrepareMinionsPool(BattleInstance instance)
//        {
//            for (byte i = 0; i < instance.players.count; ++i)
//            {
//                var _player = instance.players[i];

//                if (i < instance.players.count - 1)
//                    ObjectPooler.instance.InitDeck(_player.deck);//here
//                else
//                    ObjectPooler.instance.InitDeck(_player.deck, () =>
//                    {
//                        ///
//                        ServerWorld.Instance.LocalBattlePlayerReady();//server
//                        ///
//                    });
//            }
//        }

//        private BattleInstance CreateBattleInstance(ObserverBattle battle, BinaryMission mission, bool shortWaiting = false)
//        {
//            return new BattleInstance
//            {
//                battlefield = mission.battlefield,
//                index = 0,
//                status = BattleInstanceStatus.Waiting,
//                timer = shortWaiting ? 60 * 2 * 1000 : 60 * 5 * 1000,
//                minions = new IndexSequence(_max_units),
//                effects = new IndexSequence(_max_units),
//                players = new BattlePlayers
//                {
//                    count = battle.group.need
//                }
//            };
//        }

//        byte _max_units => (byte)ServerSettings.MaxUnits.IntValue;

//    }
//}


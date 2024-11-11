//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Collections;
//using Unity.Jobs;

//using Legacy.Database;
//using UnityEngine;
//using Legacy.Client;

//namespace Legacy.Server
//{
//    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
//    public class SandboxPlayerActionSystem : JobComponentSystem
//    {
//        private EntityQuery _query_actions;
//        private EntityQuery _query_battles;

//        private EndSimulationEntityCommandBufferSystem _barrier;
//        private BattleBucketsSystem _buckets;

//        protected override void OnCreate()
//        {
//            _query_actions = GetEntityQuery(
//                ComponentType.ReadOnly<BattlePlayerAction>()
//            );

//            _query_battles = GetEntityQuery(
//                ComponentType.ReadWrite<BattleInstance>(),
//                ComponentType.ReadOnly<BattleInstanceComplete>(),
//                ComponentType.ReadOnly<BaseBattleSettings>(),
//                ComponentType.ReadOnly<SandboxPlayerDeck>(),
//                ComponentType.ReadOnly<SandboxInstance>()
//            );

//            _barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//            _buckets = World.GetOrCreateSystem<BattleBucketsSystem>();

//            RequireForUpdate(_query_actions);
//            RequireForUpdate(_query_battles);
//        }

//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            var _buffer = _barrier.CreateCommandBuffer();
//            _buffer.DestroyEntity(_query_actions);

//            inputDeps = new ActionCardJob
//            {
//                requests = _query_actions.ToComponentDataArray<BattlePlayerAction>(Allocator.TempJob),
//                buffer = _buffer.AsParallelWriter(),
//                minions = _buckets.Minions,
//                cardsSettings = Settings.Instance.Get<CardsSettings>(),
//                gameSettings = Settings.Instance.Get<BaseGameSettings>(),
//                max_units = _buckets.MaxUnits,
//                tiles = BinaryGrid.Instance.Tiles,
//                tileSize = BinaryGrid.Instance.TileSize,
//                mapWidth = BinaryGrid.Instance.MapWidth,
//                mapHeight = BinaryGrid.Instance.MapHeight
//            }.Schedule(_query_battles, inputDeps);

//            _barrier.AddJobHandleForProducer(inputDeps);

//            return inputDeps;
//        }


//        struct ActionCardJob : IJobForEachWithEntity<BattleInstance, BaseBattleSettings, SandboxPlayerDeck>
//        {
//            [ReadOnly, DeallocateOnJobCompletion] internal NativeArray<BattlePlayerAction> requests;
//            [ReadOnly] internal NativeArray<BattleMinionBucket> minions;
//            internal EntityCommandBuffer.ParallelWriter buffer;
//            internal BaseGameSettings gameSettings;
//            internal CardsSettings cardsSettings;
//            public byte max_units;
//            [ReadOnly] public NativeHashMap<int, TileData> tiles;
//            public float tileSize;
//            public float mapWidth;
//            public float mapHeight;

//            public void Execute(
//                Entity entity,
//                int index,
//                ref BattleInstance battle,
//                [ReadOnly] ref BaseBattleSettings settings,
//                [ReadOnly] ref SandboxPlayerDeck deck
//            )
//            {
//                if (battle.status != BattleInstanceStatus.Playing && battle.status != BattleInstanceStatus.Pause)
//                    return;

//                for (int i = 0; i < requests.Length; ++i)
//                {
//                    var _request = requests[i];
//                    if (battle.index != _request.player.battle) continue;

//                    var _player = battle.players[_request.player.index];
//                    BattlePlayerSide side = _player.side;

//                    if (_request.position.x > 0)
//                        side = BattlePlayerSide.Right;

//                    switch (_request.type)
//                    {
//                        case BattlePlayerActionType.Card:
//                            var _hand_card = deck._get(_request.index);

//                            if (Cards.Instance.Get(_hand_card.index, out BinaryCard binary))
//                            {
//                                if(_player.mana>= binary.manaCost && ClientWorld.Instance.Profile.IsTutorial || ServerWorld.Instance.isSandbox)
//                                {
//                                    _player.stats.manaspend += binary.manaCost;

//                                    var _length = (byte)binary.entities.Count;
//                                    var _collider = 0.4f;
//                                    if(Database.Entities.Instance.Get(binary.entities[0], out BinaryEntity binaryEntity))
//                                    {
//                                        _collider = binaryEntity.collider;
//                                    }
//                                    //Это нужно так как у некоторых ботов в колодах эпические карты 1-го уровня, и уровень получается "отрицательный"
//                                    var startLevel = cardsSettings.GetStartLevel(_hand_card.index);
//                                    var cardLevel = math.clamp((uint)_hand_card.level, startLevel, gameSettings.maxLevel);
//                                    var realLevel = (byte)(cardLevel - startLevel + 1);
//                                    var squadPositionType = binary.squadPositionType;
//                                    var succesPosition = GetSquadPosition(ref _request);

//                                    for (byte k = 0; k < _length; ++k)
//                                    {
//                                        var customPos = SquadPosUtils.GetCustomSquadUnitPosition(_length, k, squadPositionType, side, _collider);

//                                        var _entity = buffer.CreateEntity(index);
//                                        buffer.AddComponent(index, _entity, new MinionSpawnRequest
//                                        {
//                                            battle = battle.index,
//                                            db = binary.entities[k],
//                                            position = succesPosition + customPos,
//                                            side = side,
//                                            //level = _hand_card.level,
//                                            level = 10,
//                                            card_index = _hand_card.index
//                                        });
//                                    }

//                                    // Mission event
//                                    var _mission_event = buffer.CreateEntity(index);
//                                    buffer.AddComponent(index, _mission_event, new EventInstance
//                                    {
//                                        trigger = _player.side == BattlePlayerSide.Right ? TutorialEventTrigger.OnBotMinionSpawn : TutorialEventTrigger.OnPlayerMinionSpawn
//                                    });

//                                    battle.players[_request.player.index] = _player;

//                                }
//                                if (_player.mana < binary.manaCost && ClientWorld.Instance.Profile.IsTutorial)
//                                {
//                                    var e = buffer.CreateEntity(index);
//                                    buffer.AddComponent(
//                                            index,
//                                            entity,
//                                            new BattlePlayerActionFailedData
//                                            {
//                                                playerAction = _request
//                                            });
//                                    BattleInstanceInterface.instance.hand.handObjects[_request.index].FailedCardToNormalState();
//                                }
//                            }
//                            else
//                            {
//                                var e = buffer.CreateEntity(index);
//                                buffer.AddComponent(
//                                        index,
//                                        entity,
//                                        new BattlePlayerActionFailedData
//                                        {
//                                            playerAction = _request
//                                        });
//                                BattleInstanceInterface.instance.hand.handObjects[_request.index].FailedCardToNormalState();
//                            }
//                            break;

//                        case BattlePlayerActionType.Skill:
//                            {
//                                _player.stats.skillused++;
//                                if (_player.side == BattlePlayerSide.Right)
//                                {
//                                    _request.position.x *= -1;
//                                }
//                                battle.players[_request.player.index] = _player;

//                                var _offset = max_units * battle.index;
//                                var _hero_bucket = minions[_offset + _player.minion_index];

//                                var _entity = buffer.CreateEntity(index);
//                                buffer.AddComponent(index, _entity, new EffectTriggerData
//                                {
//                                    index = _request.index,
//                                    type = TriggerEvent.OnPlayerRequest,
//                                    forward = _request.position - _hero_bucket.minion.mposition,
//                                    position = _request.position,
//                                    source = _player.minion_index,
//                                    esource = _hero_bucket.entity,
//                                    side = _player.side,
//                                    battle = battle.index,
//                                    level = _hero_bucket.minion.level
//                                });
//                                break;
//                            }
//                    }
//                }
//            }

//            private float2 GetSquadPosition(ref BattlePlayerAction _request)
//            {
//                float2 succesPosition = _request.position;
//                var _minion_tile_index = TileData.Float2Int2Tile(succesPosition, tileSize);

//                var _minion_tile_hash = new TileData { index = _minion_tile_index }.Hash;
//                if (tiles.TryGetValue(_minion_tile_hash, out TileData _minion_tile))
//                {
//                    if ((_minion_tile.status == TileStatus.Blocked )||
//                        (_minion_tile.position.x + tileSize / 2 >= mapWidth / 2 )||
//                         (       _minion_tile.position.x - tileSize / 2 <= -mapWidth / 2 )||
//                          (      _minion_tile.position.y + tileSize / 2 >= mapHeight / 2 )||
//                           (     _minion_tile.position.y - tileSize / 2 <= -mapHeight / 2))
//                    {
//                        var availableTiles = GetAvailableTiles(_minion_tile);
//                        if (availableTiles.Count <= 0) return succesPosition;

//                        succesPosition = FindNeighbours(availableTiles, _minion_tile);
//                    }
//                }
//                return succesPosition;
//            }
//            private NativeQueue<int2> GetAvailableTiles(TileData _minion_tile)
//            {
//                var availableTiles = new NativeQueue<int2>(Allocator.Temp);
//                var step = 1;

//                for (int l = -step; l <= step; l++)//соседи в радиусе step
//                {
//                    for (int k = -step; k <= step; k++)
//                    {
//                        if (l == 0 && k == 0) continue;//пропускаем текущую клетку

//                        int2 neighbourTileIndex = new int2(_minion_tile.index.x + l, _minion_tile.index.y + k);
//                        var neighborTile_hash = new TileData { index = neighbourTileIndex }.Hash;
//                        bool blocked = false;
//                        if (tiles.TryGetValue(neighborTile_hash, out TileData _neighbour_minion_tile))
//                        {
//                            if (_neighbour_minion_tile.status == TileStatus.Blocked)//success
//                            {
//                                blocked = true;
//                            }
//                        }
//                        //чтоб не выходило за границы
//                            var pos = TileData.Float2Int2(neighbourTileIndex,tileSize);

//                        if ((pos.x + tileSize / 2 >= mapWidth / 2) ||
//                                (pos.x - tileSize / 2 <= -mapWidth / 2) ||
//                                (pos.y + tileSize/2 >= mapHeight/ 2) ||
//                                (pos.y - tileSize /2<= -mapHeight / 2))
//                            blocked = true;

//                        if (!blocked)
//                            availableTiles.Enqueue(neighbourTileIndex);//вернем позицию
//                    }
//                }
//                return availableTiles;
//            }

//            private float2 FindNeighbours(NativeQueue<int2> availableTiles, TileData startTile)
//            {
//                var clothestIndex = FindClothestTileIndex(availableTiles, startTile);
//                var pos = TileData.Float2Int2(clothestIndex, tileSize);
//                return pos;
//            }

//            private int2 FindClothestTileIndex(NativeQueue<int2> availableTilesIndexes, TileData startTile)
//            {
//                var stepX = 10;
//                var stepY = 10;
//                var startTileIndex = startTile.index;
//                var resultTileIndex = startTile.index;
//                stepX = stepY = availableTilesIndexes.Count;
//                while (availableTilesIndexes.TryDequeue(out int2 availableIndex))
//                {

//                    var tileindex = availableIndex;
//                    var moduleX = math.abs(tileindex.x);
//                    var moduleY = math.abs(tileindex.y);
//                    //находим модуь индекса
//                    //сделать еще проверку на ближайший к (0,0) индекс
//                    var difOfIndexX = tileindex.x - startTileIndex.x;
//                    var difOfIndexY = tileindex.y - startTileIndex.y;
//                    if (math.abs(difOfIndexX) <= math.abs(stepX) && math.abs(difOfIndexY) <= math.abs(stepY))
//                    {
//                        stepX = difOfIndexX;
//                        stepY = difOfIndexY;
//                    }
//                }
//                resultTileIndex.x = startTile.index.x + stepX;
//                resultTileIndex.y = startTile.index.y + stepY;

//                return resultTileIndex;
//            }
//        }
//    }
//}


using Unity.Collections;
using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;
using Legacy.Database;
using System.Collections.Generic;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]
    [UpdateAfter(typeof(SpawnMinionsSystem))]
    public class MinionGameObjectInitializationSystem : ComponentSystem
    {

        private EntityQuery _spawn_prefabs;
        private BattleBucketsSystem _buckets;
        private Dictionary<ushort, List<GameObject>> _spawned;
        private List<byte> _awaiting;
        private Dictionary<ushort, Vector3> minionCenters;

        public void ClearMinionCenters()
        {
            minionCenters.Clear();
        }

        protected override void OnCreate()
        {
            _spawn_prefabs = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.ReadOnly<EntityDatabase>(),
                ComponentType.Exclude<Transform>()
            );

            _buckets = World.GetOrCreateSystem<BattleBucketsSystem>();
            _spawned = new Dictionary<ushort, List<GameObject>>();
            _awaiting = new List<byte>();
            minionCenters = new Dictionary<ushort, Vector3>();

            RequireSingletonForUpdate<BattleInstance>();
        }

        internal void Spawned(ushort index, GameObject prefab)
        {
            if (!_spawned.ContainsKey(index))
            {
                _spawned[index] = new List<GameObject>();
            }
            _spawned[index].Add(prefab);
        }
        internal void Unspawn(GameObject prefab)
        {
            foreach (var s in _spawned)
            {
                if (s.Value.Contains(prefab))
                {
                    s.Value.Remove(prefab);
                    return;
                }
            }
        }

        private void InitGameObject(GameObject _object, MinionClientBucket bucket, bool isEnemy, byte numberInSquad)
        {
            var proxy = _object.GetComponent<EntityProxyBehaviour>();
            if (proxy != null)
            {
                proxy.Entity = bucket.entity;
            }
            else
            {
                proxy = _object.AddComponent<EntityProxyBehaviour>();
                proxy.Entity = bucket.entity;
            }

            _object.transform.position = new float3(
                bucket.minion.mposition.x,
                _object.transform.position.y,
                bucket.minion.mposition.y
            );

            GameObjectEntity.AddToEntity(EntityManager, _object, bucket.entity);
            _object.SetActive(true);
            var mib = _object.GetComponent<MinionInitBehaviour>();
            mib.isHero = Heroes.Instance.Hero_minions_list.Contains(bucket.repl.db);
            if (isEnemy)
            {
                var m = _object.transform.Find("Main") ? _object.transform.Find("Main") : _object.transform.Find("Group/Main");
                var daX = m.localScale.x;
                m.localScale = new Vector3(-daX, m.localScale.y, m.localScale.z);
            }

            MakeStarted(_object, isEnemy, numberInSquad, ref bucket.minion, bucket.repl.db);
        }

        private void MakeStarted(GameObject _object, bool isEnemy, byte numberInSquad, ref MinionData minionData, ushort db)
        {
            MinionInitBehaviour mib = _object.GetComponent<MinionInitBehaviour>();
            if (mib != null)
            {
                mib.MakeStarted(numberInSquad, isEnemy, ref minionData, db);
                Debug.Log("Minion started state inited");
                return;
            }
            Debug.Log("Minion started state failed");
        }

        protected override void OnUpdate()
        {
            var _battle = GetSingleton<BattleInstance>();
            var _player = _battle.players[_battle.players.player];
            var _replicated = _spawn_prefabs.ToComponentDataArray<EntityDatabase>(Allocator.TempJob);

            // Считаем количество миньонов в отряде вызванном одной картой
            // Мне очень не хотелось делать два словаря для левых и правых карт, потому 
            // В этом словаре Все Левые миньоны сохраняются с родным индексом карты
            // Все правые миньоны сохраняются с индексом карты + 10000
            // Навряд ли когда то у нас будет более 10000 карт
            var minionsCount = new Dictionary<ushort, byte>(_replicated.Length / 2);

            // А тут мы считаем центр группы юнитов вызванных одной картой
            // И тут та же самая магия с +10000
            var minionPositions = new Dictionary<ushort, float2>(_replicated.Length / 2);

            // А тут мы сохраняем список первых миньонов которым нужно передать этот самый центр группы
            var firstMinions = new Dictionary<ushort, GameObject>(_replicated.Length / 2);

            for (int i = 0; i < _replicated.Length; ++i)
            {
                GameObject _game_object = null;
                var _repl = _replicated[i];
                //var isHero = Heroes.Instance.Hero_minions_list.Contains(_repl.db);
                if (_buckets.Minions.TryGetValue(_repl.index, out MinionClientBucket bucket))
                {
                    //Иногдав туториале у нас юнит умирал, но как то все же попадал сюда. Теперь мы не будем обрабатывать мертвых миньонов

                    //Debug.Log($"--- health = {bucket.minion.health} state is = {bucket.minion.state}");
                    if (bucket.minion.health <= 0 || bucket.minion.state == MinionState.Death)
                        continue;

                    if (Database.Entities.Instance.Get(_repl.db, out BinaryEntity minion))
                    {
                      //  if (bucket.minion.side == _player.side || ServerWorld.Instance.isSandbox)
                        {
                            if (_spawned.ContainsKey(_repl.db))
                            {
                                if (_spawned[_repl.db].Count > 0)
                                {
                                    _game_object = _spawned[_repl.db][0];
                                    _spawned[_repl.db].RemoveAt(0);
                                }
                            }
                        }

                        if (_player.side == BattlePlayerSide.Right)
                        {
                            bucket.minion.mposition.x *= -1;
                        }

                        //Считаем номера юнитов и центр отрада
                        ushort cardIndex = bucket.minion.side == BattlePlayerSide.Left ? bucket.minion.card_index : (ushort)(bucket.minion.card_index + 10000);
                        if (minionsCount.ContainsKey(cardIndex))
                        {
                            minionsCount[cardIndex]++;
                            minionPositions[cardIndex] += bucket.minion.mposition;
                        }
                        else
                        {
                            minionsCount.Add(cardIndex, 1);
                            minionPositions[cardIndex] = bucket.minion.mposition;
                        }

                        byte number = (byte)(minionsCount[cardIndex] - 1);

                        if (_game_object != null)
                        {
                            if (number == 0)
                                firstMinions.Add(cardIndex, _game_object);

                            InitGameObject(_game_object, bucket, bucket.minion.side != _player.side, number);
                        }
                        else
                        {
                            if (!_awaiting.Contains(bucket.repl.index))
                            {
                                _awaiting.Add(bucket.repl.index);
                                var minion_object = ObjectPooler.instance.GetMinion(minion, (GameObject _object) =>
                                {
                                    InitGameObject(_object, bucket, bucket.minion.side != _player.side, number);

                                    _awaiting.Remove(bucket.repl.index);

                                    // Если миньон загрузится сразу, то попав сюда мы не найдем нужного minionCenters
                                    // Если же миньон загрузится позже - minionCenters будет его ждать
                                    if (number == 0 && minionCenters.ContainsKey(cardIndex))
                                    {
                                        var pos = minionCenters[cardIndex];
                                        _object.GetComponent<MinionInitBehaviour>().SetTimerPosition(new Vector3(pos.x, 0, pos.y));


                                        minionCenters.Remove(cardIndex);
                                    }
                                });

                                // Миньен загружен сразу
                                // Нулеого добавляем к списку первых миньенов, что бы сказать где будет таймер
                                if (minion_object != null && number == 0)
                                {
                                    firstMinions.Add(cardIndex, minion_object);
                                }
                            }
                        }
                    }
                }
            }

            //Теперь когда мы обработали всех новых миньонов, мы можем посчитать центры отрядов и сказать первым миньонам куда ставить таймер
            foreach (var card in minionsCount.Keys)
            {
                if (card == 0 || card == 10000)
                    continue;

                var pos = minionPositions[card] / minionsCount[card];

                if (firstMinions.ContainsKey(card))
                    firstMinions[card].GetComponent<MinionInitBehaviour>().SetTimerPosition(new Vector3(pos.x, 0, pos.y));
                else
                {
                    // Для тех миньонов которые дребуют дозагрузки - мы сохраняем позиции для таймера
                    if (!minionCenters.ContainsKey(card))
                        minionCenters.Add(card, new Vector3(pos.x, 0, pos.y));
                }
            }

            _replicated.Dispose();
        }

    }


 

}

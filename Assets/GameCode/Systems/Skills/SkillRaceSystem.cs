//using Unity.Collections;
//using Unity.Entities;
//using Legacy.Effects;
//using Legacy.Database;

//namespace Legacy.Client
//{
////    [UpdateBefore(typeof(AnimationSystem))]
//    public class SkillRaceSystem : ComponentSystem
//    {
//        private EntityQuery _query_added;
//        private EntityQuery _query_deleted;
//        private NativeHashMap<byte, Entity> _hidden;
//        private NativeQueue<byte> _new_added;
//        private BattleBucketsSystem _buckets;
//        protected override void OnCreate()
//        {
//            _query_added = GetEntityQuery(
//                ComponentType.ReadOnly<EffectData>(),
//                ComponentType.ReadOnly<EffectRacing>(),
//                ComponentType.Exclude<StateDeath>()
//            );

//            _query_deleted = GetEntityQuery(
//                ComponentType.ReadOnly<EffectData>(),
//                ComponentType.ReadOnly<EffectRacing>(),
//                ComponentType.ReadOnly<StateDeath>()
//            );

//            _hidden = new NativeHashMap<byte, Entity>(16, Allocator.Persistent);
//            _new_added = new NativeQueue<byte>(Allocator.Persistent);
//            _buckets = World.GetOrCreateSystem<BattleBucketsSystem>();
//        }

//        protected override void OnDestroy()//
//        {
//            _hidden.Dispose();
//            _new_added.Dispose();
//        }

//        protected override void OnUpdate()
//        {
//            if (!_query_deleted.IsEmptyIgnoreFilter)
//            {
//                var _effects = _query_deleted.ToComponentDataArray<EffectData>(Allocator.TempJob);
//                for (int i = 0; i < _effects.Length; ++i)
//                {
//                    var _effect = _effects[i];
//                    _hidden.Remove(_effect.source);
//                    if (_buckets.Minions.TryGetValue(_effect.source, out MinionClientBucket bucket))
//                    {
//                        if (EntityManager.HasComponent<CentaurRacingScript>(bucket.entity))
//                        {
//                            var _raceEffect = EntityManager.GetComponentObject<CentaurRacingScript>(bucket.entity);
//                             // _raceEffect.ResetSkill1Animation();
             
//                        }
//                    }
//                }
//                _effects.Dispose();
//            }

//            if (!_query_added.IsEmptyIgnoreFilter)
//            {
//                var _added_job = new _new_added_job
//                {
//                    hidden = _hidden.AsParallelWriter(),
//                    added = _new_added.AsParallelWriter()
//                }.Schedule(_query_added);

//                _added_job.Complete();

//                while (_new_added.TryDequeue(out byte index))
//                {
//                    if (_buckets.Minions.TryGetValue(index, out MinionClientBucket bucket))
//                    {
//                        if (EntityManager.HasComponent<CentaurRacingScript>(bucket.entity))
//                        {
//                            var _raceEffect = EntityManager.GetComponentObject<CentaurRacingScript>(bucket.entity);
//                          //  if (!_raceEffect.IsStartAttack)
//                              //  _raceEffect.SetSkill1Animation();
//                        }
//                    }
//                }
//            }

//        }

//        [Unity.Burst.BurstCompile]
//        struct _new_added_job : IJobForEachWithEntity<EffectData>
//        {
//            internal NativeHashMap<byte, Entity>.ParallelWriter hidden;
//            internal NativeQueue<byte>.ParallelWriter added;

//            public void Execute(Entity entity, int index, [ReadOnly] ref EffectData effect)
//            {
//                if (hidden.TryAdd(effect.source, entity))
//                {
//                    added.Enqueue(effect.source);
//                }
//            }
//        }
//    }
//}

//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using UnityEngine;
//using UnityEngine.Jobs;
//using Legacy.Effects;
//using Legacy.Database;

//namespace Legacy.Client
//{
//    [UpdateInGroup(typeof(BattlePresentation))]
//    [UpdateAfter(typeof(MinionTransformSystem))]

//    public class AnimationSystem : ComponentSystem
//    {
//        public struct StatesAndMoveSpeed
//        {
//            public MinionState state;
//            public float mspeed;
//        }

//        private BattleSystems _battle;
//        private BattleBucketsSystem _buckets;
//        private MinionState previousMinionState = MinionState.Idle;
//        private NativeHashMap<Entity, StatesAndMoveSpeed> _states;
//        private NativeQueue<Entity> _changes;
//        private NativeQueue<Entity> _updated;
//        private EntityQuery _spawn_added;
//        private EntityQuery _spawn_removed;
//        private EntityQuery _spawn_updated;

//        protected override void OnCreate()
//        {
//            _spawn_added = GetEntityQuery(
//                ComponentType.ReadOnly<MinionData>(),
//                ComponentType.Exclude<MinionAnimation>(),
//                ComponentType.Exclude<AnimationSystemBlocker>(),
//                ComponentType.Exclude<ContiniousAtackBehaviour>()
//            );

//            _spawn_removed = GetEntityQuery(
//                ComponentType.ReadOnly<MinionAnimation>(),
//                ComponentType.Exclude<MinionData>(),
//                ComponentType.Exclude<AnimationSystemBlocker>(),
//                ComponentType.Exclude<ContiniousAtackBehaviour>()
//            );

//            _spawn_updated = GetEntityQuery(
//                ComponentType.ReadOnly<MinionData>(),
//                ComponentType.ReadOnly<EntityDatabase>(),
//                ComponentType.ReadOnly<Animator>(),
//                ComponentType.ReadOnly<MinionAnimation>(),
//                ComponentType.Exclude<AnimationSystemBlocker>()
//            );

//            _battle = World.GetOrCreateSystem<BattleSystems>();
//            _states = new NativeHashMap<Entity, StatesAndMoveSpeed>(128, Allocator.Persistent);
//            _changes = new NativeQueue<Entity>(Allocator.Persistent);
//            _updated = new NativeQueue<Entity>(Allocator.Persistent);

//            _buckets = World.GetOrCreateSystem<BattleBucketsSystem>();
//        }

//        protected override void OnDestroy()
//        {
//            _states.Dispose();
//            _changes.Dispose();
//            _updated.Dispose();
//        }

//        protected override void OnUpdate()
//        {
//            if (!_spawn_updated.IsEmptyIgnoreFilter)
//            {
//                var inputDeps = new UpdateJob
//                {
//                    changes = _changes.AsParallelWriter(),
//                    states = _states
//                }.Schedule(_spawn_updated);

//                inputDeps.Complete();

//                if (_changes.Count > 0)
//                {
//                    UnityEngine.Debug.LogError("_states.Length " + _states.Length);
//                    while (_changes.TryDequeue(out Entity entity))
//                    {
//                        var _minion = EntityManager.GetComponentData<MinionData>(entity);
//                        var stateAndMove = new StatesAndMoveSpeed { state = _minion.state, mspeed = _minion.mspeed };
//                        _states[entity] = stateAndMove;

//                        var _animator = EntityManager.GetComponentObject<Animator>(entity);
//                        var dbdata = EntityManager.GetComponentData<EntityDatabase>(entity);
//                        if (EntityManager.HasComponent<ContiniousAtackBehaviour>(entity))
//                        {
//                            ApplyContingousChanges(_animator, _minion, dbdata);
//                        }
//                        else
//                        {
//                            ApplyChanges(_animator, _minion, dbdata);
//                        }

//                        if (EntityManager.HasComponent<ConnectedUnitBehaviour>(entity))
//                        {
//                            var cub = EntityManager.GetComponentObject<ConnectedUnitBehaviour>(entity);
//                            ApplyChanges(cub.connectedAnimator, _minion, dbdata);
//                        }
//                        previousMinionState = _minion.state;
//                    }
//                }
//            }

//            if (!_spawn_added.IsEmptyIgnoreFilter)
//            {
//                var inputDeps = new AddedJob
//                {
//                    updated = _updated.AsParallelWriter(),
//                    states = _states.AsParallelWriter(),
//                }.Schedule(_spawn_added);

//                inputDeps.Complete();

//                while (_updated.TryDequeue(out Entity entity))
//                {
//                    PostUpdateCommands.AddComponent(entity, default(MinionAnimation));
//                }
//            }

//            if (!_spawn_removed.IsEmptyIgnoreFilter)
//            {
//                var inputDeps = new RemovedJob
//                {
//                    updated = _updated.AsParallelWriter()
//                }.Schedule(_spawn_removed);

//                inputDeps.Complete();

//                while (_updated.TryDequeue(out Entity entity))
//                {
//                    _states.Remove(entity);
//                    if (EntityManager.Exists(entity))
//                    {
//                        PostUpdateCommands.RemoveComponent<MinionAnimation>(entity);
//                    }
//                }
//            }
//        }

//        void ResetBools(Animator animator, string exclude = "")
//        {

//            string[] bools = new string[] { "Landing", "Stand", "Attack", "Walk", "Death", "Skill1", "Skill2" };

//            for (byte i = 0; i < bools.Length; i++)
//            {
//                if (bools[i] != exclude)
//                {
//                    animator.SetBool(bools[i], false);
//                }
//            }
//        }

//        private void ApplyChanges(Animator animator, MinionData minion, EntityDatabase dbData)
//        {

//            if (animator)
//            {

//                animator.enabled = true;
//                animator.speed = 1;

//                var _info = animator.GetCurrentAnimatorClipInfo(0);
//                if (_info.Length > 0)
//                {
//                    var hitRange = animator.gameObject.GetComponent<RangeHitEffect>();
//                    var minionSoundManager = animator.gameObject.GetComponent<MinionSoundManager>();
//                    UnityEngine.Debug.LogError(minion.state);
//                    switch (minion.state)
//                    {
//                        case MinionState.Spawn:
//                            ResetBools(animator, "Landing");
//                            animator.SetBool("Landing", true);
//                            break;

//                        case MinionState.Paralize:
//                            ResetBools(animator);
//                            animator.enabled = false;
//                            break;

//                        case MinionState.Skill1:
//                            ResetBools(animator, "Skill1");
//                            animator.Play("Skill1");
//                            break;

//                        case MinionState.Skill2:
//                            ResetBools(animator, "Skill2");
//                            animator.Play("Skill2");
//                            break;

//                        case MinionState.Idle:
//                            if (hitRange != null)
//                            {
//                                hitRange.Charge(false);
//                            }
//                            ResetBools(animator, "Stand");
//                            animator.SetBool("Stand", true);
//                            break;

//                        case MinionState.Charge:


//                            if (hitRange != null)
//                            {
//                                var targetIndex = minion.atarget;
//                                var _offence_list = Database.Components.Instance.Get<MinionOffence>();
//                                if (_offence_list.TryGetValue(dbData.db, out MinionOffence offence))
//                                {
//                                    var bulletSpeed = offence.bulletSpeed;
//                                    if (_buckets.Minions.TryGetValue(targetIndex, out MinionClientBucket bucket))
//                                    {
//                                        if (EntityManager.HasComponent<Transform>(bucket.entity))
//                                        {
//                                            //is previous state != skill1 || skill2
//                                            if (previousMinionState != MinionState.Skill1 && previousMinionState != MinionState.Skill2)
//                                            {
//                                                hitRange.Hit(EntityManager.GetComponentObject<Transform>(bucket.entity), bulletSpeed);
//                                            }
//                                        }
//                                    }
//                                }
//                            }

//                            var _offence = Database.Components.Instance.Get<MinionOffence>();
//                            if (_offence.TryGetValue(dbData.db, out MinionOffence ñoffence))
//                            {
//                                var duration = ñoffence.duration;
//                                var charge = minion.acharge;

//                                if (charge >= duration)
//                                {
//                                    ResetBools(animator, "Stand");
//                                    animator.SetBool("Stand", true);
//                                }
//                                else
//                                {
//                                    ResetBools(animator, "Attack");
//                                    animator.SetBool("Attack", true);
//                                }

//                            }
//                            //ResetBools(animator, "Stand");
//                            //animator.SetBool("Stand", true);
//                            //Debug.Log($"Charge Charge: {minion.acharge}");
//                            animator.speed = minion.aspeed * 0.01f;
//                            break;

//                        case MinionState.Move:
//                            if (hitRange != null)
//                            {
//                                hitRange.Charge(false);
//                            }
//                            if (animator.GetComponent<CentaurRacingScript>() != null)
//                            {
//                                if (animator.GetComponent<CentaurRacingScript>().RaceStarted)
//                                {
//                                    animator.speed = 1;
//                                    break;
//                                }
//                            }
//                            ResetBools(animator, "Walk");
//                            animator.SetBool("Walk", true);
//                            //if (dbData.db != 50)
//                            //    animator.Play("Walk", 0, Random.Range(0f, 1f));

//                            animator.speed = minion.mspeed * 0.01f;

//                            break;

//                        case MinionState.Death:
//                            if (minionSoundManager)
//                            {
//                                minionSoundManager.PlayDie();
//                                MinionsSoundsManager.RemoveSourceFromList(minionSoundManager, minion.side == BattlePlayerSide.Right);
//                            }
//                            if (hitRange != null)
//                            {
//                                hitRange.Charge(false);
//                            }
//                            animator.gameObject.transform.Find("shadow").gameObject.SetActive(false);
//                            //var panel = animator.gameObject.GetComponent<MinionPanel>();
//                            //if (panel != null && panel.panel != null)
//                            //{
//                            //    panel.panel.SetActive(false);
//                            //}
//                            ResetBools(animator, "Death");
//                            animator.SetBool("Death", true);
//                            break;

//                        case MinionState.Attack:
//                            if (minionSoundManager)
//                            {
//                                minionSoundManager.PlayHit();
//                            }

//                            var _charge_time = minion.aspeed * 0.001f;
//                            var targetIndex1 = minion.atarget;
//                            if (_buckets.Minions.TryGetValue(targetIndex1, out MinionClientBucket bucketAttack))
//                            {
//                                if (EntityManager.HasComponent<Transform>(bucketAttack.entity))
//                                {
//                                    if (hitRange)
//                                        hitRange.Target = EntityManager.GetComponentObject<Transform>(bucketAttack.entity);
//                                }
//                            }

//                            var _offence_ = Database.Components.Instance.Get<MinionOffence>();
//                            bool _over_time = false;
//                            if (_offence_.TryGetValue(dbData.db, out MinionOffence aoffence))
//                            {
//                                var duration = aoffence.duration;
//                                var charge = minion.acharge;

//                                if (charge >= duration)
//                                {
//                                    ResetBools(animator, "Stand");
//                                    animator.SetBool("Stand", true);
//                                    _over_time = true;
//                                }
//                                else
//                                {
//                                    ResetBools(animator, "Attack");
//                                    animator.SetBool("Attack", true);
//                                }

//                            }
//                            //for (int i = 0; i < _clips.Length; ++i)
//                            //{
//                            //    if (_clips[i].name == "Attack")
//                            //    {
//                            //        UnityEngine.Debug.LogError("_charge_time "+ _charge_time);
//                            //        UnityEngine.Debug.LogError("_clips[i].length" + _clips[i].length);
//                            //      //  if (_charge_time > _clips[i].length)
//                            //        {
//                            //            animator.SetBool("Stand", true);
//                            //            _over_time = true;
//                            //        }
//                            //        break;
//                            //    }
//                            //}
//                            //if (!_over_time)
//                            //{
//                            //    Debug.Log($"attack Charge: {minion}");

//                            //    //animator.SetFloat("_multiplier_attack", minion.aspeed / 100f);
//                            //    //  animator.SetFloat("_multiplier_attack", 1);
//                            //    animator.speed = minion.aspeed * 0.01f;
//                            //    ResetBools(animator, "Attack");
//                            //    animator.SetBool("Attack", true);

//                            //    if (!animator.speed.AboutEquals(1, 0.01f))
//                            //        minionSoundManager.ResetPitch(animator.speed);
//                            //    //  animator.Play("Attack", 0, _charge_time);
//                            //}
//                            if (hitRange != null)
//                            {
//                                hitRange.Charge(true);
//                            }
//                            break;
//                    }
//                }
//            }
//        }

//        private void ApplyContingousChanges(Animator animator, MinionData minion, EntityDatabase dbData)
//        {
//            if (animator)
//            {
//                animator.SetBool("Landing", false);
//                animator.SetBool("Stand", false);
//                animator.SetBool("Walk", false);
//                animator.SetBool("Death", false);
//                animator.SetBool("Skill1", false);
//                animator.SetBool("Skill2", false);
//                animator.enabled = true;
//                animator.speed = 1;

//                var _info = animator.GetCurrentAnimatorClipInfo(0);
//                if (_info.Length > 0)
//                {
//                    var hitRange = animator.gameObject.GetComponent<RangeHitEffect>();
//                    var continuousBehaviour = animator.gameObject.GetComponent<ContiniousAtackBehaviour>();
//                    if (minion.state != MinionState.Attack)
//                    {
//                        continuousBehaviour.StopAttack();
//                    }
//                    switch (minion.state)
//                    {
//                        case MinionState.Spawn:
//                            animator.SetBool("Landing", true);
//                            break;

//                        case MinionState.Paralize:
//                            animator.enabled = false;
//                            break;

//                        case MinionState.Skill1:
//                            animator.Play("Skill1");
//                            break;

//                        case MinionState.Skill2:
//                            animator.Play("Skill2");
//                            break;

//                        case MinionState.Idle:
//                            animator.SetBool("Stand", true);
//                            if (_buckets.Minions.TryGetValue(minion.atarget, out MinionClientBucket bucket))
//                            {
//                                if (EntityManager.HasComponent<Transform>(bucket.entity) && bucket.minion.state != MinionState.Death)
//                                {

//                                    //var t = EntityManager.GetComponentObject<Transform>(bucket.entity);
//                                    //continuousBehaviour.StartAttack(t);
//                                    continuousBehaviour.ResetTarget(EntityManager.GetComponentObject<Transform>(bucket.entity));
//                                }
//                                else
//                                {
//                                    animator.SetBool("Attack", false);
//                                    continuousBehaviour.StopAttack();
//                                }
//                            }
//                            else
//                            {
//                                animator.SetBool("Attack", false);
//                                continuousBehaviour.StopAttack();
//                            }
//                            break;

//                        case MinionState.Move:
//                            if (hitRange != null)
//                            {
//                                hitRange.Charge(false);
//                            }
//                            animator.SetBool("Walk", true);
//                            animator.SetBool("Attack", false);
//                            animator.speed = minion.mspeed * 0.01f;
//                            break;

//                        case MinionState.Death:
//                            if (hitRange != null)
//                            {
//                                hitRange.Charge(false);
//                            }
//                            animator.gameObject.transform.Find("shadow").gameObject.SetActive(false);
//                            //var panel = animator.gameObject.GetComponent<MinionPanel>();
//                            //if (panel != null && panel.panel != null)
//                            //{
//                            //    panel.panel.SetActive(false);
//                            //}
//                            animator.SetBool("Death", true);
//                            break;

//                        case MinionState.Charge:
//                        case MinionState.Attack:
//                            //continuousBehaviour.StartAttack();
//                            if (_buckets.Minions.TryGetValue(minion.atarget, out MinionClientBucket b))
//                            {
//                                if (EntityManager.HasComponent<Transform>(b.entity) && b.minion.state != MinionState.Death)
//                                {
//                                    var t = EntityManager.GetComponentObject<Transform>(b.entity);
//                                    continuousBehaviour.StartAttack(t);
//                                }
//                                else
//                                {
//                                    continuousBehaviour.StopAttack();
//                                }
//                            }
//                            else
//                            {
//                                Debug.Log("No Ghost attack target");
//                            }
//                            var _charge_time = minion.aspeed * 0.001f;
//                            animator.speed = minion.aspeed * 0.01f;
//                            animator.SetFloat("_multiplier_attack", 1);
//                            //animator.SetFloat("_multiplier_attack", minion.aspeed / 100f);
//                            animator.SetBool("Attack", true);
//                            if (hitRange != null)
//                                hitRange.Charge(true);
//                            break;
//                    }
//                }
//            }
//        }

//        [Unity.Burst.BurstCompile]
//        struct UpdateJob : IJobForEachWithEntity<MinionData>
//        {
//            [ReadOnly] public NativeHashMap<Entity, StatesAndMoveSpeed> states;
//            public NativeQueue<Entity>.ParallelWriter changes;

//            public void Execute(Entity entity, int index, [ReadOnly] ref MinionData minion)
//            {
//                if (states.TryGetValue(entity, out StatesAndMoveSpeed stateAndMove))
//                {
//                    if (minion.state != stateAndMove.state || minion.mspeed != stateAndMove.mspeed)
//                    {
//                        changes.Enqueue(entity);
//                    }
//                }
//            }
//        }


//        [Unity.Burst.BurstCompile]
//        struct RemovedJob : IJobForEachWithEntity<MinionAnimation>
//        {
//            public NativeQueue<Entity>.ParallelWriter updated;
//            public void Execute(Entity entity, int index, [ReadOnly] ref MinionAnimation c0)
//            {
//                updated.Enqueue(entity);
//            }
//        }

//        [Unity.Burst.BurstCompile]
//        struct AddedJob : IJobForEachWithEntity<MinionData>
//        {
//            public NativeHashMap<Entity, StatesAndMoveSpeed>.ParallelWriter states;
//            public NativeQueue<Entity>.ParallelWriter updated;

//            public void Execute(Entity entity, int index, [ReadOnly] ref MinionData minion)
//            {
//                var stateAndMove = new StatesAndMoveSpeed { state = MinionState.Undefined, mspeed = 100 };
//                if (states.TryAdd(entity, stateAndMove))
//                {
//                    updated.Enqueue(entity);
//                }
//            }
//        }

//    }
//}


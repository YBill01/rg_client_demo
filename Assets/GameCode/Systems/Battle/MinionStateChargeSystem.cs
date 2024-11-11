
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]
    public class MinionStateChargeSystem : ComponentSystem, IStateSystemInterface
    {
        private EntityQuery _query_minions;
        private EntityQueryMask chargedMask;
        private BattleBucketsSystem _buckets;

        protected override void OnCreate()
        {
            _query_minions = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                 ComponentType.ReadOnly<EntityDatabase>(),
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<Animator>(),
                ComponentType.ReadOnly<StateCharge>());

            chargedMask = EntityManager.GetEntityQueryMask(GetEntityQuery(typeof(StateCharged)));
            _buckets = World.GetOrCreateSystem<BattleBucketsSystem>();
        }
        protected override void OnUpdate()
        {
            var _entities = _query_minions.ToEntityArray(Allocator.TempJob);
            var _animators = _query_minions.ToComponentArray<Animator>();
            var _minions = _query_minions.ToComponentDataArray<MinionData>(Allocator.TempJob);
            var _databases = _query_minions.ToComponentDataArray<EntityDatabase>(Allocator.TempJob);

            for (int i = 0; i < _animators.Length; i++)
            {
                var animator = _animators[i];
                var minion = _minions[i];
                var database = _databases[i];
                var _offence = Database.Components.Instance.Get<MinionOffence>();

                if (_offence.TryGetValue(database.db, out MinionOffence offence))//dublicate
                {
                    var mainLayer = _animators[i].GetLayerIndex("Base Layer");
                    var isAttack = _animators[i].GetCurrentAnimatorStateInfo(mainLayer).IsName("Attack");
                    var isWalk = _animators[i].GetCurrentAnimatorStateInfo(mainLayer).IsName("Walk");
                    var animNormalizeTime = _animators[i].GetCurrentAnimatorStateInfo(mainLayer).normalizedTime;
                    _animators[i].speed = _minions[i].aspeed * 0.01f;
                    if (animNormalizeTime >= 1 && minion.acharge < offence.duration && isAttack)
                    {
                        _animators[i].ResetBools("Stand");
                        _animators[i].SetBool("Stand", true);
                    }
                  //  это чтоб если чардж == 25с, то он потом не чарджился в ходьбе на месте, а выходи в стенд
                    if (isWalk && minion.acharge < offence.duration)
                    {
                        _animators[i].ResetBools("Stand");
                        _animators[i].SetBool("Stand", true);
                    }

                    if (animator.GetComponent<RangeHitEffect>())//если дальник то стреляем пулей и удаляем копонет
                    {
                        var targetIndex = minion.atarget;
                        var hitRange = animator.GetComponent<RangeHitEffect>();
                        if (_buckets.Minions.TryGetValue(targetIndex, out MinionClientBucket bucket))
                        {
                            if (EntityManager.HasComponent<Transform>(bucket.entity))
                            {
                                if (isAttack && animNormalizeTime < 1 && !chargedMask.Matches(_entities[i]))
                                {
                                    ClientWorld.Instance.EntityManager.AddComponentData<StateCharged>(_entities[i],new StateCharged { });

                                    hitRange.Target = EntityManager.GetComponentObject<Transform>(bucket.entity);
                                    hitRange.Hit(EntityManager.GetComponentObject<Transform>(bucket.entity), offence.bulletSpeed);
                                }
                            }
                        }
                    }
                }
            }
            _entities.Dispose();
            _minions.Dispose();
            _databases.Dispose();
        }
        public void PlayClip()
        {
            throw new NotImplementedException();
        }

        public void SetAnimatorSpeed(float typedSpeedValue)
        {
            throw new NotImplementedException();
        }
    }
}
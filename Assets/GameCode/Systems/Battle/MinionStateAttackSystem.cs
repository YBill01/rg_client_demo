
using Legacy.Database;
using Legacy.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]
    public class MinionStateAttackSystem : ComponentSystem, IStateSystemInterface
    {
        private EntityQuery _query_minions;
        private NativeArray<ushort> _previous_charges;
        private BattleBucketsSystem _buckets;


        protected override void OnCreate()
        {
            _query_minions = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.ReadOnly<EntityDatabase>(),
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<Animator>(),
                ComponentType.ReadOnly<MinionSoundManager>(),
                ComponentType.ReadOnly<StateAttack>());

            _previous_charges = new NativeArray<ushort>(264, Allocator.Persistent);
            _buckets = World.GetOrCreateSystem<BattleBucketsSystem>();
        }
        protected override void OnUpdate()
        {
            if (_query_minions.IsEmptyIgnoreFilter)
                return;

            var _entities = _query_minions.ToEntityArray(Allocator.TempJob);
            var _animators = _query_minions.ToComponentArray<Animator>();
            var _soundManagers = _query_minions.ToComponentArray<MinionSoundManager>();
            var _minions = _query_minions.ToComponentDataArray<MinionData>(Allocator.TempJob);
            var _databases = _query_minions.ToComponentDataArray<EntityDatabase>(Allocator.TempJob);
            var _offence = Database.Components.Instance.Get<MinionOffence>();

            for (int i = 0; i < _animators.Length; i++)
            {
                var minion = _minions[i];
                var database = _databases[i];

                var mainLayer = _animators[i].GetLayerIndex("Base Layer");
                var isAttack = _animators[i].GetCurrentAnimatorStateInfo(mainLayer).IsName("Attack");

                var prevCharge = _previous_charges[database.index];
                if (_offence.TryGetValue(database.db, out MinionOffence offence))
                {
                    float animLength = 0;
                    if (isAttack)
                    {
                        animLength = _animators[i].GetCurrentAnimatorClipInfo(mainLayer)[0].clip.length / _animators[i].GetCurrentAnimatorStateInfo(mainLayer).speed;
                    }
                    ClientWorld.Instance.EntityManager.RemoveComponent<StateCharged>(_entities[i]);

                    if (animLength < offence.duration * 0.001f)
                    {
                        if (!isAttack && minion.acharge < offence._hittime(minion.aspeed))
                        {
                            _animators[i].ResetBools("Attack");
                            _animators[i].SetBool("Attack", true);
                            _soundManagers[i].PlayHit();
                        }
                    }
                    //-----------------------------------------------------------------------------
                    else
                    {
                        _animators[i].ResetBools("Attack");
                        _animators[i].SetBool("Attack", true);

                        if (prevCharge < offence.duration && prevCharge > offence._hittime(minion.aspeed) &&
                           minion.acharge < offence._hittime(minion.aspeed))
                        {
                            _animators[i].SetBool("InstantAttack", true);
                        }
                        else
                        {
                            _animators[i].SetBool("InstantAttack", false);
                        }
                        prevCharge = minion.acharge;
                        _previous_charges[database.index] = prevCharge;
                    }
                }

                _animators[i].speed = minion.aspeed * 0.01f;


                var targetIndex1 = minion.atarget;
                if (_buckets.Minions.TryGetValue(targetIndex1, out MinionClientBucket bucketAttack))
                {
                    if (EntityManager.HasComponent<Transform>(bucketAttack.entity))
                    {
                        var hitRange = _animators[i].GetComponent<RangeHitEffect>();
                        if (hitRange)
                            hitRange.Target = EntityManager.GetComponentObject<Transform>(bucketAttack.entity);
                    }
                }
            }
            _entities.Dispose();
            _minions.Dispose();
            _databases.Dispose();
        }

        protected override void OnDestroy()
        {
            _previous_charges.Dispose();
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
    //[UpdateInGroup(typeof(BattlePresentation))]
    //public class MinionStateAttackInterruprSystem : ComponentSystem
    //{
    //    private EntityQuery _query_minions;
    //    private NativeArray<ushort> _previous_charges;
    //    private BattleBucketsSystem _buckets;


    //    protected override void OnCreate()
    //    {
    //        _query_minions = GetEntityQuery(
    //            ComponentType.ReadOnly<MinionData>(),
    //            ComponentType.ReadOnly<EntityDatabase>(),
    //            ComponentType.ReadOnly<Transform>(),
    //            ComponentType.ReadOnly<Animator>(),
    //            ComponentType.ReadOnly<StateAttack>());

    //        _buckets = World.GetOrCreateSystem<BattleBucketsSystem>();
    //    }
    //    protected override void OnUpdate()
    //    {
    //        if (_query_minions.IsEmptyIgnoreFilter)
    //            return;

    //        var _animators = _query_minions.ToComponentArray<Animator>();
    //        var _minions = _query_minions.ToComponentDataArray<MinionData>(Allocator.TempJob);
    //        var _databases = _query_minions.ToComponentDataArray<EntityDatabase>(Allocator.TempJob);
    //        var _offence = Database.Components.Instance.Get<MinionOffence>();

    //        for (int i = 0; i < _animators.Length; i++)
    //        {
    //            var minion = _minions[i];
    //            var database = _databases[i];

    //            if (_buckets.Minions.TryGetValue(minion.atarget, out MinionClientBucket bucket))
    //            {
    //                if (_offence.TryGetValue(database.db, out MinionOffence offence))
    //                {
    //                    var distance = math.distance(minion.mposition, bucket.minion.mposition);
    //                    if (distance > offence.radius + minion.collider + bucket.minion.collider)//огр с орком - не робят
    //                    {
    //                        _animators[i].speed = _minions[i].aspeed * 0.01f;
    //                        _animators[i].ResetBools("Stand");
    //                        _animators[i].SetBool("Stand", true);
    //                    }

    //                }
    //            }
    //        }
    //        _minions.Dispose();
    //        _databases.Dispose();
    //    }
    //}
}



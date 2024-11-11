
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
    public class MinionStateSkillsSystem : ComponentSystem, IStateSystemInterface
    {
        private EntityQuery _query_minions_skill_1;
        private EntityQuery _query_minions_skill_2;
        private BattleBucketsSystem _buckets;

        protected override void OnCreate()
        {
            _query_minions_skill_1 = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                 ComponentType.ReadOnly<EntityDatabase>(),
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<Animator>(),
                ComponentType.ReadOnly<StateSkill1>());

            _query_minions_skill_2 = GetEntityQuery(
                 ComponentType.ReadOnly<MinionData>(),
                  ComponentType.ReadOnly<EntityDatabase>(),
                 ComponentType.ReadOnly<Transform>(),
                 ComponentType.ReadOnly<Animator>(),
                 ComponentType.ReadOnly<StateSkill2>());

            _buckets = World.GetOrCreateSystem<BattleBucketsSystem>();
        }

        protected override void OnUpdate()
        {
            var _animators_skill_1 = _query_minions_skill_1.ToComponentArray<Animator>();
            var _animators_skill_2 = _query_minions_skill_2.ToComponentArray<Animator>();

            for (int i = 0; i < _animators_skill_1.Length; i++)
            {
                if (!_animators_skill_1[i].GetBool("Skill1"))
                {
                    _animators_skill_1[i].speed = 1;
                    _animators_skill_1[i].ResetBools("Skill1");
                    _animators_skill_1[i].SetBool("Skill1", true);
                }
            }

            for (int i = 0; i < _animators_skill_2.Length; i++)
            {
                if (!_animators_skill_2[i].GetBool("Skill2"))
                {
                    _animators_skill_2[i].speed = 1;
                    _animators_skill_2[i].ResetBools("Skill2");
                    _animators_skill_2[i].SetBool("Skill2", true);
                }
                Debug.Log("-=- g[pgpgpgppgpgppgpgpg");
            }
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
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
    public class MinionStateNavigateSystem : ComponentSystem, IStateSystemInterface
    {
        private EntityQuery _query_minions;

        protected override void OnCreate()
        {
            _query_minions = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<Animator>(),
                ComponentType.ReadOnly<StateNavigate>());
        }
        protected override void OnUpdate()
        {
            var _animators = _query_minions.ToComponentArray<Animator>();
            var _minions = _query_minions.ToComponentDataArray<MinionData>(Allocator.TempJob);
            for (int i = 0; i < _animators.Length; i++)
            {
                _animators[i].ResetBools("Walk");
                _animators[i].SetBool("Walk", true);

                _animators[i].speed = _minions[i].mspeed * 0.01f;
            }
            _minions.Dispose();
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

    public static class MyExtensionAnimator
    {
        public static void ResetBools(this Animator animator, string exclude = "")
        {
            string[] bools = new string[] { "Landing", "Stand", "Attack", "Walk", "Death", "Skill1", "Skill2" };

            for (byte i = 0; i < bools.Length; i++)
            {
                if (bools[i] != exclude)
                {
                    animator.SetBool(bools[i], false);
                }
            }
        }
    }

}

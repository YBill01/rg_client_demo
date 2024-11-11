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
    public class MinionStateWaitingSystem : ComponentSystem, IStateSystemInterface
    {
        private EntityQuery _query_minions;

        protected override void OnCreate()
        {
            _query_minions = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<Animator>(),
                ComponentType.ReadOnly<StateWaiting>());
        }
        protected override void OnUpdate()
        {
            var _animators = _query_minions.ToComponentArray<Animator>();

            for (int i = 0; i < _animators.Length; i++)
            {
                _animators[i].ResetBools("Stand");
                _animators[i].SetBool("Stand", true);
                _animators[i].speed = 1;
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
﻿using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]
    public class MinionStateDeathSystem : ComponentSystem, IStateSystemInterface
    {
        private EntityQuery _query_minions;

        protected override void OnCreate()
        {
            _query_minions = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<Animator>(),
                ComponentType.ReadOnly<StateDeath>());
        }
        protected override void OnUpdate()
        {
            var _animators = _query_minions.ToComponentArray<Animator>();

            for (int i = 0; i < _animators.Length; i++)
            {
                _animators[i].ResetBools("Death");
                _animators[i].Play("Death");
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
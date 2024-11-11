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
    public class MinionStateCelebratingSystem : ComponentSystem, IStateSystemInterface
    {
        private EntityQuery _query_minions;
        private EntityManager _entityManager;

        protected override void OnCreate()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _query_minions = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<Animator>(),
                ComponentType.ReadOnly<MinionSoundManager>(),
                ComponentType.ReadOnly<StateCelebrating>());
        }
        protected override void OnUpdate()
        {
            var _soundManagers = _query_minions.ToComponentArray<MinionSoundManager>();
            var _animators = _query_minions.ToComponentArray<Animator>();
            var entity = _query_minions.ToEntityArray(Allocator.TempJob);

            for (int i = 0; i < _soundManagers.Length; i++)
            {
                _soundManagers[i].StopStepsSFX();
            }

            for (int i = 0; i < _animators.Length; i++)
            {
                _animators[i].ResetBools();
                _animators[i].SetBool("Celebrating", true);
            }

            _entityManager.RemoveComponent<StateCelebrating>(entity[0]);
            _entityManager.RemoveComponent<StateAlive>(entity[0]);
            _entityManager.AddComponent<StateDispose>(entity[0]);
            entity.Dispose();
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

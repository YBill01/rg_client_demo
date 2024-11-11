using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
    public class IrvaTsunamyDragBehaviour : DragRadiusScale
    {
        private Transform hero = null;

        public Transform Point;
        public GameObject tsunami;
        private Vector3 heroPosition = new Vector3(-12,0,0);

        void OnEnable()
        {
        var manager = ClientWorld.Instance.EntityManager;
            var _battle_query = manager.CreateEntityQuery(ComponentType.ReadOnly<BattleInstance>());
            var _heroes_query = manager.CreateEntityQuery(
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.Exclude<PauseState>());
            var _battle = _battle_query.GetSingleton<BattleInstance>();
            var _heroes = _heroes_query.ToEntityArray(Allocator.TempJob);
            var _transforms = _heroes_query.ToComponentArray<Transform>();
            for (int i = 0; i < _heroes.Length; ++i)
            {
                var _hero = manager.GetComponentData<MinionData>(_heroes[i]);
                if (_battle.players[_battle.players.player].side == _hero.side)
                {
                    if (_transforms[i].gameObject.GetComponent<IrvaBehaviour>())
                    {
                        hero = _transforms[i];
                        heroPosition = hero.position;
                    }
                }
            }
            _heroes.Dispose();
            tsunami.transform.position = heroPosition;
        }

        private void Update()
        {
            tsunami.transform.position = heroPosition;
        }


        void OnDisable()
        {
        }
    }
}

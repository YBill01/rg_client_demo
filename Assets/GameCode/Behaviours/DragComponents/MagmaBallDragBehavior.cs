using Legacy.Client;
using Legacy.Database;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
public class MagmaBallDragBehavior : MonoBehaviour
{
    private Transform hero = null;

    public Transform Point;

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
                if (_transforms[i].gameObject.GetComponent<SonnelonBehaviour>())
                    hero = _transforms[i];
            }
        }
        _heroes.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        if (hero != null)
        {
            var sonnelon = hero.GetComponent<SonnelonBehaviour>();
            if (sonnelon != null)
            {
                //Point.transform.rotation = ascalia.RayContainer.transform.rotation;
                //ascalia.RayTo(Point);
            }
        }
    }

    void OnDisable()
    {
        if (hero != null)
        {
            var sonnelon = hero.GetComponent<SonnelonBehaviour>();
            if (sonnelon != null)
            {
                //ascalia.RayStop();
            }
        }
    }
}

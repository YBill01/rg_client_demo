using Legacy.Client;
using Legacy.Database;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class MerisTouchOfDeathDragBehaviour : MonoBehaviour
{
    private Vector3     lookDirection;
    private Transform   hero;

    private void Awake()
    {
        FindHero();
        transform.position = hero.position;
        transform.LookAt(lookDirection);
    }
    private void Start()
    {
        transform.position = hero.position;
        transform.LookAt(lookDirection);
    }

    public void OnEnable()
    {
        transform.position = hero.position;
        transform.LookAt(lookDirection);
    }
    public void Update()
    {
        transform.position = hero.position;
        transform.LookAt(lookDirection);
    }

    private void FindHero()
    {
        var manager = ClientWorld.Instance.EntityManager;
        var _battle_query = manager.CreateEntityQuery(ComponentType.ReadOnly<BattleInstance>());
        var _heroes_query = manager.CreateEntityQuery(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<MinionData>());
        var _battle = _battle_query.GetSingleton<BattleInstance>();
        var _heroes = _heroes_query.ToEntityArray(Allocator.TempJob);
        var _transforms = _heroes_query.ToComponentArray<Transform>();

        for (int i = 0; i < _heroes.Length; ++i)
        {
            var _hero = manager.GetComponentData<MinionData>(_heroes[i]);
            if (_hero.layer == MinionLayerType.Hero)
            {
                if (_battle.players[_battle.players.player].side == _hero.side)
                {
                    hero = _transforms[i];
                }
            }
        }
        _heroes.Dispose();

        if (hero.position.x < 0)
        {
            lookDirection = Vector3.right;
        }
        else
        {
            lookDirection = Vector3.left;
        }
    }
}

using Legacy.Client;
using Legacy.Database;
using Legacy.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TrapsFly : MonoBehaviour
{
    [SerializeField] private List<GameObject> traps;
    private Transform hero = null;
    private float heroCollider = 0;
    private EntityQuery battleInstanceQuery;
    void OnEnable()
    {
        battleInstanceQuery = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<BattleInstance>());

        var _proxy = GetComponent<EntityProxyBehaviour>();
        if (!_proxy || _proxy.Entity == Entity.Null) return;
        var _effectData = ClientWorld.Instance.EntityManager.GetComponentData<EffectData>(_proxy.Entity);

        var _battle = battleInstanceQuery.GetSingleton<BattleInstance>();
        var _player = _battle.players[_battle.players.player];

        var sideOffset = _player.side == BattlePlayerSide.Left ? 1 : -1;

        transform.position = new Vector3(_effectData.position.x * sideOffset, 0f, _effectData.position.y);

        GetHero();

        ushort db = GetTrapDb();

        if (Entities.Instance.Get(db, out BinaryEntity entity))
        {
            var mapWidth = BinaryGrid.Instance.MapWidth;
            var mapHeight = BinaryGrid.Instance.MapHeight;

            var sourcePosition = hero.position;
            var effectPosition = this.transform.position;

            var widthXMultiplier = math.distance(effectPosition, sourcePosition) / mapWidth * mapHeight;
            var lengthYMultiplier = widthXMultiplier / 2;
            var clampedX = math.clamp(widthXMultiplier + 1f, 1f, mapHeight / 2);
            var clampedY = math.clamp(lengthYMultiplier + 1f, 1f, mapWidth / mapHeight);

            for (int i = 0; i < traps.Count; i++)
            {
                var realPos = SquadPosUtils.GetPoligonSquadUnitPosition((byte)traps.Count, (byte)i, _effectData.side != _player.side, 1f);
                traps[i].transform.localPosition = new Vector3(realPos.x * clampedY, 0, realPos.y * clampedX);
            }
        }

        hero.GetComponent<ZacZarBehaviour>().SetEndTargetPosition(traps.Select(x => x.transform.position).ToList());
        // hero.GetComponent<ZacZarBehaviour>().TrapThrow();//fly for sandbox
    }

    private ushort GetTrapDb()
    {
        return 116;//найди индекс нормально
    }

    private void GetHero()
    {
        var _proxy = GetComponent<EntityProxyBehaviour>();
        if (_proxy == null) return;
        var _effectData = ClientWorld.Instance.EntityManager.GetComponentData<EffectData>(_proxy.Entity);
        var _buckets = ClientWorld.Instance.GetOrCreateSystem<Legacy.Client.BattleBucketsSystem>();
        if (_buckets.Minions.TryGetValue(_effectData.source, out MinionClientBucket bucket))
        {
            hero = ClientWorld.Instance.EntityManager.GetComponentObject<Transform>(bucket.entity);
            heroCollider = bucket.minion.collider;
        }
    }
}

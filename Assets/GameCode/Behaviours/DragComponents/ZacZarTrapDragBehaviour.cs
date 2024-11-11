using Legacy.Client;
using Legacy.Database;
using Legacy.Server;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ZacZarTrapDragBehaviour : DragRadiusScale
{
    private Transform hero = null;

    public Transform Point;
    public List<GameObject> traps;

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
                if (_transforms[i].gameObject.GetComponent<ZacZarBehaviour>())
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
            var ascalia = hero.GetComponent<ZacZarBehaviour>();
            if (ascalia != null)
            {
                for (byte i = 0; i < traps.Count; ++i)
                {
                    var mapWidth = BinaryGrid.Instance.MapWidth;
                    var mapHeight = BinaryGrid.Instance.MapHeight;

                    var sourcePosition = hero.position;
                    var effectPosition = this.transform.position;

                    var widthXMultiplier = math.distance(effectPosition, sourcePosition) / mapWidth * mapHeight;
                    var lengthYMultiplier = widthXMultiplier/2;
                    var clampedX = math.clamp(widthXMultiplier + 1f, 1f, mapHeight/2 );
                    var clampedY = math.clamp(lengthYMultiplier + 1f, 1f, mapWidth / mapHeight);
                    var realPos = SquadPosUtils.GetPoligonSquadUnitPosition((byte)traps.Count, i,false, 1f);
                    traps[i].transform.position = this.transform.position + new Vector3((realPos.x * clampedY), 0, realPos.y * clampedX);
                }
            }
        }
    }


    void OnDisable()
    {
        StaticColliders.instance.AllZone.enabled = false;
        if (hero != null)
        {
            var zaczar = hero.GetComponent<ZacZarBehaviour>();
            if (zaczar != null)
            {
             //   zaczar.SetEndTargetPosition(traps.Select(x => x.transform.position).ToList());
            }
        }
    }
}

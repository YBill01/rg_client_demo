using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Legacy.Database;

namespace Legacy.Client
{
    public class IrvaTotemsDragBehavior : DragRadiusScale
    {
        private Transform hero = null;

        public Transform Point;
        public List<GameObject> totems;

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
                        hero = _transforms[i];
                }
            }
            _heroes.Dispose();


        }
        private int previousIndex = int.MaxValue;
        private void Update()
        {
            var _minion_tile_index = TileData.Float2Int2Tile(new float2(this.transform.position.x, this.transform.position.z), BinaryGrid.Instance.TileSize);
            var _minion_tile_hash = new TileData { index = _minion_tile_index }.Hash;
            bool minionOnBridge = false;
            if (BinaryGrid.Instance.Tiles.TryGetValue(_minion_tile_hash, out TileData _minion_tile))
            {
                if (_minion_tile.status == TileStatus.BridgeDown || _minion_tile.status == TileStatus.BridgeUp)
                {
                    minionOnBridge = true;
                }
            }
            var index = minionOnBridge ? 0 : this.transform.position.x > 0 ? 2 : 1;
            if(index!= previousIndex)
            EnableTotem(index);

            previousIndex = index;
        }

        private void EnableTotem(int index)
        {
            totems.ForEach(x => x.SetActive(false));
            totems[index].SetActive(true);
        }

        void OnDisable()
        {
        }
    }
}

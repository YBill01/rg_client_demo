using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;

namespace Legacy.Client
{

    //https://www.redblobgames.com/grids/hexagons/#range

    public class GridBehaviour : MonoBehaviour {

		public GameObject TilePrefab;
        public int MapWidth;
        public int MapHeight;

		public void Generate() {
#if UNITY_EDITOR
			ClearGrid();
			int3 _Index = int3.zero;
			var _tile_size = BattleSettings.Instance.Grid.TileSize;
			for (int z = -MapHeight; z < MapHeight; z++)
            {
                for (int x = -MapWidth; x < MapWidth; x++)
                {
					var _tile_prefab = (GameObject)PrefabUtility.InstantiatePrefab(TilePrefab, transform);
					_tile_prefab.name = string.Format("Tile[{0},{1}]", x, z);
					_tile_prefab.transform.position = new float3(x * _tile_size + _tile_size * 0.5f, transform.position.y, z * _tile_size + _tile_size * 0.5f);

				}
            }
#endif
		}

		public void ClearGrid()
        {
            Debug.Log("Clearing grid...");
            Transform[] _Children = GetComponentsInChildren<Transform>();
            List<GameObject> _Removed = new List<GameObject>();
            foreach (Transform _child in _Children)
            {
                if (_child != transform)
                {
                    _Removed.Add(_child.gameObject);
                }
            }
            while (_Removed.Count > 0)
            {
                DestroyImmediate(_Removed[0], false);
                _Removed.RemoveAt(0);
            }
        }
	}
}
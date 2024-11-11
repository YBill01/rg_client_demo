using Legacy.Database;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Legacy.Client
{
    public class DebugNavigationScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDrawGizmos()
        {
            //var tiles = BinaryGrid.Instance.Tiles.GetValueArray(Allocator.TempJob);
            //for (int i = 0; i < tiles.Length; i++)
            //{
            //    {
            //        var pos = new Vector3(tiles[i].position.x - 1.65f / 2, tiles[i].position.y - 1.65f / 2);
            //        var size = new Vector3(BinaryGrid.Instance.TileSize, 0, BinaryGrid.Instance.TileSize);
            //        Gizmos.DrawWireCube(pos, size);
            //        PointF point = new PointF(pos.x, pos.z);
            //        SizeF sizef = new SizeF(BinaryGrid.Instance.TileSize, BinaryGrid.Instance.TileSize);
            //        RectangleF rectangle = new RectangleF(point, sizef);
            //        var bottomCenter = new Vector3(rectangle.Location.X, 0, rectangle.Location.Y);
            //        Gizmos.DrawSphere(bottomCenter, 0.2f);
            //    }
            //}
            //tiles.Dispose();
        }
    }


//    [ExecuteAlways, UpdateInGroup(typeof(BattlePresentation))]
//    public class DebugNameSystem : ComponentSystem
//    {
//        public static string PrefabPrefix = "☒ ";
//        static EntityQuery m_queryDesc;

//        EntityQuery m_query;
//        EntityQuery obstacles_query;
//        EntityQuery m_queryPrefabs;

//        protected override void OnCreate()
//        {
//            base.OnCreate();
//            m_query = GetEntityQuery(
//             ComponentType.ReadOnly<MinionData>(),
//             ComponentType.ReadOnly<Transform>()
//         );
//            m_query.SetChangedVersionFilter(typeof(Transform));

//        }

//        protected override void OnUpdate()
//        {
//#if UNITY_EDITOR
//            var transforms = m_query.ToComponentArray<Transform>();
//            var entities = m_query.ToEntityArray(Allocator.TempJob
//                );
//            for
//                (int i = 0; i < transforms.Length
//                ; i++)
//            {
//                EntityManager.SetName(entities[i], transforms[i].name);

//            }
//            entities.Dispose();
//#endif
//        }
//    }
}
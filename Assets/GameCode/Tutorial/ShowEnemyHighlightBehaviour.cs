using UnityEngine;
using System.Collections;
using Unity.Entities;
using Legacy.Database;
using Unity.Collections;

namespace Legacy.Client
{
	public class ShowEnemyHighlightBehaviour : MonoBehaviour
	{
		[SerializeField]
		private GameObject EnemyHighlightPrefab;

        private Transform enemy = null;
        private bool highlighted;

        void CheckEnemy()
		{
			var manager = ClientWorld.Instance.EntityManager;

			var _minions_query = manager.CreateEntityQuery(
				ComponentType.ReadOnly<Transform>(),
				ComponentType.ReadOnly<MinionData>(),
				ComponentType.ReadOnly<EntityDatabase>()
			);

			var _minions = _minions_query.ToEntityArray(Allocator.TempJob);
			var _transforms = _minions_query.ToComponentArray<Transform>();
			for (int i = 0; i < _minions.Length; ++i)
			{
				var _data = manager.GetComponentData<MinionData>(_minions[i]);
				if (_data.layer != MinionLayerType.Hero)
					continue;

				if (_data.side != BattlePlayerSide.Right)
					continue;

				enemy = _transforms[i];
				break;
			}

			_minions.Dispose();

            if (enemy != null)
            {
                var highlight = Instantiate(EnemyHighlightPrefab, ObjectPooler.instance.transform);            
                var npos = enemy.position;
                npos.y = 0;
                highlight.transform.position = npos;
            }
		}

        void Update()
        {
            if (!highlighted)
            {
                CheckEnemy();
                highlighted = enemy != null;
            }
        }
	}
}

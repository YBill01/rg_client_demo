using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Legacy.Database;

namespace Legacy.Client
{
    [UpdateBefore(typeof(EffectTransformSystem))]

	public class WaveTransformSystem : ComponentSystem
	{

		private EntityQuery _query_moved;
		private EntityQuery _query_wawes;
		private EntityQuery _query_minions;

		protected override void OnCreate()
		{
			_query_moved = GetEntityQuery(
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<Transform>(),
				ComponentType.ReadOnly<WaveEffectBehaviour>(),
				ComponentType.Exclude<CustomSTransform>()
			);

			_query_wawes = GetEntityQuery(
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<Transform>(),
				ComponentType.ReadOnly<WaveEffectBehaviour>(),
				ComponentType.ReadOnly<CustomSTransform>()
			);
			_query_minions = GetEntityQuery(
				ComponentType.ReadOnly<MinionData>(),
				ComponentType.ReadOnly<Transform>()
			);
		}

		protected override void OnUpdate()
		{
			UpdateCustomTransform();
			UpdateWaveProgress();
		}

		private void UpdateCustomTransform()
		{
			if (_query_moved.IsEmptyIgnoreFilter) return;

			var _battle = GetSingleton<BattleInstance>();
			var _player = _battle.players[_battle.players.player];

			var entities = _query_moved.ToEntityArray(Allocator.TempJob);
			var datas = _query_moved.ToComponentDataArray<EffectData>(Allocator.TempJob);
			var waves = _query_moved.ToComponentArray<WaveEffectBehaviour>();
			var transforms = _query_moved.ToComponentArray<Transform>();
			for (int i = 0; i < entities.Length; i++)
			{
				var e = entities[i];
				var d = datas[i];
				var w = waves[i];
				var t = transforms[i];
				
				t.localPosition = new Vector3(0,0.2f,0);
				w.SetDirection(d.side == _player.side ? 1 : -1);
				EntityManager.AddComponentData(e, default(CustomSTransform));
			}
			datas.Dispose();
			entities.Dispose();
		}

		private void UpdateWaveProgress()
		{
			if (_query_wawes.IsEmptyIgnoreFilter) return;

			var _battle = GetSingleton<BattleInstance>();
			var _player = _battle.players[_battle.players.player];

			var entities = _query_wawes.ToEntityArray(Allocator.TempJob);
			 var datas = _query_wawes.ToComponentDataArray<EffectData>(Allocator.TempJob);
			 var waves = _query_wawes.ToComponentArray<WaveEffectBehaviour>();

			var minions = _query_minions.ToComponentDataArray<MinionData>(Allocator.TempJob);
			var mTransforms = _query_minions.ToComponentArray<Transform>();
			for (int i = 0; i < entities.Length; i++)
			{
				var e = entities[i];
				var d = datas[i];
				var w = waves[i];
				w.SetPosition(d.position.x);

				for (int j = 0; j < minions.Length; j++)
				{
					var md = minions[j];
					if (md.side == _player.side) continue;
					w.AddEnemy(mTransforms[j]);
				}
			}
			minions.Dispose();
			datas.Dispose();
			entities.Dispose();
		}

	}
}


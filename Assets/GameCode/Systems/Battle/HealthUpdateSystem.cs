using Legacy.Database;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]
    [UpdateAfter(typeof(MinionTransformSystem))]

    public class HealthUpdateSystem : ComponentSystem
    {
		private EntityQuery _query_added;
		private EntityQuery _query_update;

		protected override void OnCreate()
        {
            _query_added = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
				ComponentType.ReadOnly<EntityDatabase>(),
				ComponentType.ReadOnly<Transform>(),
				ComponentType.ReadOnly<MinionPanel>(),
				ComponentType.Exclude<MinionIsSpawned>()
			);

			_query_update = GetEntityQuery(
				ComponentType.ReadOnly<MinionData>(),
				ComponentType.ReadOnly<EntityDatabase>(),
				ComponentType.ReadOnly<Transform>(),
				ComponentType.ReadOnly<MinionPanel>(),
				ComponentType.ReadOnly<MinionIsSpawned>()
			);

			RequireSingletonForUpdate<BattleInstance>();
        }
        
        protected override void OnUpdate()
        {
			var battleInstance = GetSingleton<BattleInstance>();
			if (battleInstance.status >= BattleInstanceStatus.Prepare && battleInstance.status != BattleInstanceStatus.Pause)
			{
				var _player = battleInstance.players[battleInstance.players.player];
				if (!_query_added.IsEmptyIgnoreFilter)
				{
					var _panels = _query_added.ToComponentArray<MinionPanel>();
					var _entities = _query_added.ToEntityArray(Allocator.TempJob);
					var _minions = _query_added.ToComponentDataArray<MinionData>(Allocator.TempJob);
					var _databases = _query_added.ToComponentDataArray<EntityDatabase>(Allocator.TempJob);

					var _defences = Database.Components.Instance.Get<MinionDefence>();
                    var _settings = Database.Settings.Instance.Get<BaseBattleSettings>();
					for (int i = 0; i < _panels.Length; ++i)
					{
						var _database = _databases[i];
						var _minion = _minions[i];

						if (_defences.TryGetValue(_database.db, out MinionDefence defence))
						{
                            _panels[i].Spawn(
								_minion.level, 
								(ushort)defence._health(_settings.minions.health, _minion.level),
								Database.Heroes.Instance.IsHero(_database.db),
								_minion.side != _player.side,
								_minion.layer
							);;
						}
						EntityManager.AddComponentData<MinionIsSpawned>(_entities[i], default);
					}
					_entities.Dispose();
					_minions.Dispose();
					_databases.Dispose();
				}

				if (!_query_update.IsEmptyIgnoreFilter)
				{
					var _panels = _query_update.ToComponentArray<MinionPanel>();
					var _minions = _query_update.ToComponentDataArray<MinionData>(Allocator.TempJob);
					for (int i = 0; i < _panels.Length; ++i)
					{
                        if (_minions[i].state != MinionState.Death)
                        {
                            _panels[i].SetSliderValue(_minions[i].health,_minions[i].layer);
                        }
                        else
                        {
                            _panels[i].SetSliderValue(0);
                        }
					}
					_minions.Dispose();
                }

			}
		}
    }
}


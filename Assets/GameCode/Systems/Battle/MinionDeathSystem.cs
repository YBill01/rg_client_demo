using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Legacy.Database;

namespace Legacy.Client
{
	[UpdateInGroup(typeof(BattlePresentation))]
	[UpdateAfter(typeof(HealthUpdateSystem))]

	public class MinionDeathSystem : ComponentSystem
	{
		private EntityQuery		_query_minions;
		private BattleSystems	_battle;
		private EntityQuery		_query_battles;

		protected override void OnCreate()
		{
			_query_battles = GetEntityQuery(
				ComponentType.ReadOnly<BattleInstance>()
			);

			_query_minions = GetEntityQuery(
				ComponentType.ReadOnly<MinionData>(),
				ComponentType.ReadOnly<Transform>(),
				ComponentType.ReadOnly<DeathEffect>(),
				ComponentType.ReadOnly<EntityDatabase>(),
				ComponentType.ReadOnly<StateDeath>(),
				ComponentType.Exclude<PauseState>()
			);
			_battle = World.GetOrCreateSystem<BattleSystems>();

			RequireForUpdate(_query_minions);
			RequireSingletonForUpdate<BattleInstance>();
		}

		protected override void OnUpdate()
		{
			var _battle_list = _query_battles.ToComponentDataArray<BattleInstance>(Allocator.TempJob);
			var _entities = _query_minions.ToEntityArray(Allocator.TempJob);
			var _db_List = _query_minions.ToComponentDataArray<EntityDatabase>(Allocator.TempJob);
			for (int i = 0; i < _entities.Length; ++i)
			{
				var mData = _db_List[i];
				bool next = false;
				foreach (var b in _battle_list)
				{
					if(b.status == BattleInstanceStatus.Pause)
					{
						next = true;
						break;
					}
				}
				if(next)
					continue;
				
				var _state = EntityManager.GetComponentData<StateDeath>(_entities[i]);
				//var _deletin = EntityManager.GetComponentObject<MinionInitBehaviour>(_entities[i]);
				//if (_deletin != null && !_deletin.isHero)
				//{
				//	_deletin.MakeForceDisposing();					
				//}

				var playDeathAnim = EntityManager.GetComponentObject<DeathEffect>(_entities[i]).PlayDeathAnimation;
                var _death_effect = EntityManager.GetComponentObject<DeathEffect>(_entities[i]);

				if (!playDeathAnim && !_state.isExpired)
                {
                    _death_effect.Die();
					_state.isExpired = true;
				}

				if (_battle.CurrentTime > _state.expire)
				{
					if(!_state.isExpired)
                    {
						_death_effect.Die();
						_state.isExpired = true;
					}

					PostUpdateCommands.DestroyEntity(_entities[i]);
                    UnityEngine.Debug.Log("I'm dying! " + _death_effect.gameObject.name + ". Entity: " + _entities[i]);
                    UnityEngine.Debug.Log("_state.expire " + _state.expire + ". _battle.CurrentTime: " + _battle.CurrentTime);
                }
            }
			_battle_list.Dispose();
			_db_List.Dispose();
			_entities.Dispose();
		}


	}
}
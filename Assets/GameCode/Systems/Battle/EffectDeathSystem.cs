using Unity.Entities;
using UnityEngine;
using Legacy.Database;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]

	public class EffectDeathSystem : ComponentSystem
	{
		private EntityQuery _query_minions;
		private BattleSystems _battle;

		protected override void OnCreate()
		{
			_query_minions = GetEntityQuery(
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<Transform>(),
				ComponentType.ReadOnly<StateDeath>(),
				ComponentType.Exclude<PauseState>()
			);
            _battle = World.GetOrCreateSystem<BattleSystems>();

			RequireForUpdate(_query_minions);
			RequireSingletonForUpdate<BattleInstance>();
		}

		protected override void OnUpdate()
		{
			var _transforms = _query_minions.ToComponentArray<Transform>();
			var _states		= _query_minions.ToComponentDataArray<StateDeath>(Unity.Collections.Allocator.TempJob);
			var _entities	= _query_minions.ToEntityArray(Unity.Collections.Allocator.TempJob);

			for (int i = 0; i < _transforms.Length; ++i)
			{
				var _transform = _transforms[i];
				var _state = _states[i];

				if (!_state.isExpired)
                {
					var vfxs = _transform.GetComponentsInChildren<ParticleSystem>();
                    foreach (var vfx in vfxs)
						vfx.Stop();
					_state.isExpired = true;
				}

				if (_battle.CurrentTime > _state.expire)
				{
					PostUpdateCommands.DestroyEntity(_entities[i]);
					_transform.gameObject.SetActive(false);
					//var _systems = _transform.GetComponents<ParticleSystem>();
					//int effectsLeft = _systems.Length;
					//for (int k = 0; k < _systems.Length; ++k)
					//{
					//	var s = _systems[k];
					//	if(s.isPlaying)
					//	{
					//		s.Stop(true);
					//	}
					//	else
					//	{
					//		if(s.particleCount == 0)
					//		{
					//			s.gameObject.SetActive(false);
					//			effectsLeft--;
					//		}
					//	}
					//}
					//if(effectsLeft == 0)
					{
						//	UnityEngine.Debug.LogError("effectsLeft " + effectsLeft);

						//_transform.gameObject.SetActive(false);
						//PostUpdateCommands.DestroyEntity(_entities[i]);
					}
				}		
			}
			_states.Dispose();
			_entities.Dispose();
		}
	}
}


using Unity.Collections;
using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;
using Legacy.Database;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]

	public class EffectVisualizationSystem : ComponentSystem
	{

		private EntityQuery _spawn_prefabs;
		private BattleBucketsSystem _buckets;

		protected override void OnCreate()
		{
			_spawn_prefabs = GetEntityQuery(
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<EntityDatabase>(),
				ComponentType.Exclude<Transform>()
			);

			_buckets = World.GetOrCreateSystem<BattleBucketsSystem>();

			RequireSingletonForUpdate<BattleInstance>();
		}

		protected override void OnUpdate()
		{
			var _battle = GetSingleton<BattleInstance>();
			var _player = _battle.players[_battle.players.player];

			var _replicated = _spawn_prefabs.ToComponentDataArray<EntityDatabase>(Allocator.TempJob);
			for (int i = 0; i < _replicated.Length; ++i)
			{
				GameObject _game_object = null;
				var _database = _replicated[i];
				if (_buckets.Effects.TryGetValue(_database.index, out EffectClientBucket bucket))
				{
					if (Database.Effects.Instance.Get(_database.db, out BinaryEffect effect))
					{
						if (!string.IsNullOrEmpty(effect.prefab))
						{
							_game_object = ObjectPooler.instance.GetEffect(effect.prefab);
							if (_game_object == null)
							{
								UnityEngine.Debug.Log("No " + effect.prefab + " in pull. Effect id " + _database.db.ToString());
							}
						}
					}

					if (_player.side == BattlePlayerSide.Right)
					{
						//bucket.effect.position.x *= -1;
					}

					if (_game_object != null)
					{
						var proxy = _game_object.GetComponent<EntityProxyBehaviour>();
						if (proxy != null)
						{
							proxy.Entity = bucket.entity;
						}
						else
						{
							proxy = _game_object.AddComponent<EntityProxyBehaviour>();
							proxy.Entity = bucket.entity;
						}
						_game_object.SetActive(true);

						if(_database.db == 28)
						{
							if(_buckets.Minions.TryGetValue(bucket.effect.target, out MinionClientBucket target))
							{
								if(_buckets.Minions.TryGetValue(bucket.effect.source, out MinionClientBucket source))
								{
									var sEntity = source.entity;
									var tEntity = target.entity;
									var sourcePos = EntityManager.GetComponentObject<Transform>(sEntity).position;
									var damagePoint = EntityManager.GetComponentObject<Transform>(tEntity).Find("DamagePoint");
									var targetPos = damagePoint.position;
									//var up = Vector3.up * 5;
									//var down = Vector3.down * 5;
									//Debug.DrawLine(sourcePos + up, targetPos + up, Color.white, 1);
									//Debug.DrawLine(sourcePos + down, sourcePos + up, Color.white, 1);
									//Debug.DrawLine(targetPos + down, targetPos + up, Color.white, 1);
									var md = EntityManager.GetComponentData<MinionData>(tEntity);
									Vector3 direction = (targetPos - sourcePos).normalized;
									float enemyRadius = md.collider;
									Vector3 vfxPosition = targetPos - direction * enemyRadius;
									var q = _game_object.transform.localRotation;
									q.SetLookRotation(direction);
									_game_object.transform.localRotation = q;
									vfxPosition.y = 1f;
									_game_object.transform.position = vfxPosition;
									_game_object.transform.localScale = damagePoint.localScale;
									bucket.effect.position = new float2(vfxPosition.x * (_player.side == BattlePlayerSide.Right ? -1 : 1), vfxPosition.z);
									//Debug.Log("Gamera Effect Position is " + _game_object.transform.position.ToString());

									EntityManager.AddComponentData(bucket.entity,default(CustomSTransform));
								}
							} 
						}
						else
						{
							_game_object.transform.position = new float3(
								bucket.effect.position.x,
								_game_object.transform.position.y,
								bucket.effect.position.y
							);
						}

						GameObjectEntity.AddToEntity(EntityManager, _game_object, bucket.entity);
					}
				}
				else
				{
					Debug.Log("No effect id " + _database.index.ToString() + "; SID " + _database.db.ToString() + " in bucket");
				}
				
			}
			_replicated.Dispose();
		}

	}

}

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

using Legacy.Database;
using UnityEngine;
using Legacy.Server;

namespace Legacy.Client
{
	[UpdateInGroup(typeof(BattleInitialization))]

	public class SnapshotBattleSystem : ComponentSystem
	{

		private BattleSystems _battle;
		private BattleInstance _battle_timer;

		private EntityQuery _query_snapshots;
		private EntityQuery _query_battle;

		private NativeHashMap<Entity, BattleSnapshot> _snapshots;

		protected override void OnCreate()
		{
            _battle = World.GetOrCreateSystem<BattleSystems>();

			_query_snapshots = GetEntityQuery(
				ComponentType.ReadOnly<BattleSnapshot>()
			);

			_query_battle = GetEntityQuery(
				ComponentType.ReadWrite<BattleInstance>()
			);

			_snapshots = new NativeHashMap<Entity, BattleSnapshot>(512, Allocator.Persistent);

			RequireForUpdate(_query_snapshots);
			RequireForUpdate(_query_battle);
		}

		protected override void OnDestroy()
		{
			_snapshots.Dispose();
		}

		protected override void OnUpdate()
		{
			var _interpolate_time = _battle.CurrentTime - AppInitSettings.Instance.snapshotExpireTime;
			var _expire_time = _interpolate_time - AppInitSettings.Instance.snapshotDiffTime;
			//UnityEngine.Debug.LogError("_battle.CurrentTime " + _battle.CurrentTime); 
			//UnityEngine.Debug.LogError(" ServerInitializationSystemGroup.ElapsedMilliseconds.CurrentTime " + ServerInitializationSystemGroup.ElapsedMilliseconds); 

			_snapshots.Clear();

			var inputDeps = new CollectJob
			{
				buffer = PostUpdateCommands.AsParallelWriter(),
				snapshots = _snapshots.AsParallelWriter(),
				expire = _expire_time
            }.Schedule(_query_snapshots);

			inputDeps = new InterpolateJob
			{
				snapshots = _snapshots,
				interpolate = _interpolate_time
			}.ScheduleSingle(_query_battle, inputDeps);

            inputDeps.Complete();
        }

		[Unity.Burst.BurstCompile]
		struct InterpolateJob : IJobForEachWithEntity<BattleInstance>
		{
			public long interpolate;
			[ReadOnly] public NativeHashMap<Entity, BattleSnapshot> snapshots;

			public void Execute(Entity entity, int index, ref BattleInstance battle)
			{
				var _snapshots = snapshots.GetValueArray(Allocator.Temp);
				_snapshots.Sort();
				var count = snapshots.Count();
				if (count > 2)
				{
					for (byte i = 0; i < _snapshots.Length; ++i)
					{
						if (i > 0)
						{
							var _first = _snapshots[i];
							var _second = _snapshots[i - 1];
							if (_first.time < interpolate)
							{
								float _alpha = (float)(interpolate - _first.time) / (float)(_second.time - _first.time);
								battle.Interpolate(_first.instance, _second.instance, _alpha);

								BattlePlayer _player = battle.players[battle.players.player];
								_player.Interpolate(_first.player, _second.player, _alpha);

								battle.players[battle.players.player] = _player;
								return;
							}
						}
					}
                }
                //else if (count == 1)
                //{
                //    battle.status = _snapshots[0].instance.status;
                //    battle.timer = _snapshots[0].instance.timer;
                //    battle.bridges = _snapshots[0].instance.bridges;

                //    var player = battle.players[battle.players.player];
                //    player.Interpolate(_snapshots[0].player, _snapshots[0].player, 0);

                //    battle.players[battle.players.player] = player;
                //}
            }
		}

		[Unity.Burst.BurstCompile]
		struct CollectJob : IJobForEachWithEntity<BattleSnapshot>
		{
			public long expire;
			public EntityCommandBuffer.ParallelWriter buffer;
			internal NativeHashMap<Entity, BattleSnapshot>.ParallelWriter snapshots;

			public void Execute(Entity entity, int index, [ReadOnly] ref BattleSnapshot snapshot)
			{
                if (snapshot.time < expire)
				{
					//UnityEngine.Debug.LogError("DestroyEntity BattleSnapshot");
					buffer.DestroyEntity(index, entity);
					return;
				}
				snapshots.TryAdd(entity, snapshot);
			}
		}
	}
}


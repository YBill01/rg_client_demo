using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattleInitialization))]

	public class SnapshotEffectsSystem : ComponentSystem
	{
		private NativeMultiHashMap<ushort, EffectSnapshot> _snapshots;

		private EntityQuery _query_snapshots;
		private EntityQuery _query_replicated;
		private BattleSystems _battle;

		protected override void OnCreate()
		{
			_snapshots = new NativeMultiHashMap<ushort, EffectSnapshot>(256, Allocator.Persistent);

			_query_snapshots = GetEntityQuery(
				ComponentType.ReadOnly<EffectSnapshot>()
			);

			_query_replicated = GetEntityQuery(
				ComponentType.ReadWrite<EffectData>(),
				ComponentType.ReadOnly<EntityDatabase>()
			);

			_battle = World.GetOrCreateSystem<BattleSystems>();

			RequireForUpdate(_query_snapshots);
		}

		protected override void OnDestroy()
		{
			_snapshots.Dispose();
		}

		protected override void OnUpdate()
		{
			var _interpolate_time = _battle.CurrentTime - AppInitSettings.Instance.snapshotDiffTime;
			// back in time for 200 ms

			_snapshots.Clear();

			var _buffer = PostUpdateCommands.AsParallelWriter();
			var inputDeps = new ExpireJob
			{
				expire_time = _interpolate_time - AppInitSettings.Instance.snapshotExpireTime,
				buffer = _buffer,				
				snapshots = _snapshots.AsParallelWriter()
			}.Schedule(_query_snapshots);

			if (!_query_replicated.IsEmptyIgnoreFilter)
			{
				inputDeps = new InterpolateJob
				{
					snapshots = _snapshots,
					interpolate_time = _interpolate_time,
					buffer = _buffer,
					expires = GetComponentDataFromEntity<StateDeath>(true)
				}.Schedule(_query_replicated, inputDeps);
			}

            inputDeps.Complete();
        }

		[Unity.Burst.BurstCompile]
		struct ExpireJob : IJobForEachWithEntity<EffectSnapshot>
		{
			public NativeMultiHashMap<ushort, EffectSnapshot>.ParallelWriter snapshots;
			public EntityCommandBuffer.ParallelWriter buffer;
			public long expire_time;

			public void Execute(Entity entity, int index, [ReadOnly] ref EffectSnapshot snapshot)
			{
				if (expire_time > snapshot.time)
				{
					buffer.DestroyEntity(index, entity);
					return;
				}
				snapshots.Add(snapshot.database.index, snapshot);
			}
		}

		struct InterpolateJob : IJobForEachWithEntity<EffectData, EntityDatabase>
		{
			public long interpolate_time;
			[ReadOnly] public NativeMultiHashMap<ushort, EffectSnapshot> snapshots;
			internal EntityCommandBuffer.ParallelWriter buffer;
			[ReadOnly] internal ComponentDataFromEntity<StateDeath> expires;

			public void Execute(
				Entity entity, 
				int index, 
				ref EffectData effect, 
				[ReadOnly] ref EntityDatabase repl
			)
			{
				//if (minion.state == MinionState.Death) return;

				var _length = snapshots.CountValuesForKey(repl.index);
				if (_length > 2)
				{
					var _values = snapshots.GetValuesForKey(repl.index);
					var _snapshots = new NativeArray<EffectSnapshot>(_length, Allocator.Temp);
					var _index = 0;
					foreach (var _info in _values)
					{
						_snapshots[_index++] = _info;
					}
					_snapshots.Sort();

					for (int i = 0; i < _length; ++i)
					{
						if (i > 0)
						{
							if (_snapshots[i].time < interpolate_time)
							{
								var _diff_first = (float)(interpolate_time - _snapshots[i].time);
								var _diff_second = (float)(_snapshots[i - 1].time - _snapshots[i].time);

								effect.Interpolate(
									_snapshots[i].effect,
									_snapshots[i - 1].effect,
									_diff_first / _diff_second
								);

								if (effect.state == EffectState.Expire)
								{
									if (!expires.HasComponent(entity))
									{
										buffer.AddComponent(index, entity, new StateDeath
										{
											expire = interpolate_time + 3200
										});
									}
								}

								return;
							}

						}
					}
                }
                //else if (_length == 1)
                //{
                //    var _value = snapshots.GetValuesForKey(repl.index).Current;
                //    effect.Interpolate(_value.effect, _value.effect, 0);
                //}
            }
		}

	}
}


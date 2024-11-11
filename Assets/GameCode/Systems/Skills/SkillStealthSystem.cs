
using Unity.Collections;
using Unity.Entities;

using Legacy.Effects;
using System.Collections.Generic;
using Legacy.Database;

namespace Legacy.Client
{
	//[DisableAutoCreation]
	//[AlwaysUpdateSystem]
	//[UpdateInGroup(typeof(BattleSimulation))]

	public class SkillStealthSystem : ComponentSystem
	{
		private EntityQuery _query_added;
		private EntityQuery _query_deleted;
		private NativeHashMap<byte, Entity> _hidden;
		private NativeQueue<byte> _new_added;
		private BattleBucketsSystem _buckets;

		protected override void OnCreate()
		{
			_query_added = GetEntityQuery(
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<EffectStealth>(),
				ComponentType.Exclude<StateDeath>()
			);

			_query_deleted = GetEntityQuery(
				ComponentType.ReadOnly<EffectData>(),
				ComponentType.ReadOnly<EffectStealth>(),
				ComponentType.ReadOnly<StateDeath>()
			);

			_hidden = new NativeHashMap<byte, Entity>(16, Allocator.Persistent);
			_new_added = new NativeQueue<byte>(Allocator.Persistent);

			_buckets = World.GetOrCreateSystem<BattleBucketsSystem>();
		}

		protected override void OnDestroy()
		{
			_hidden.Dispose();
			_new_added.Dispose();
		}

		private List<AssasinBehaviour> affected = new List<AssasinBehaviour>();
		protected override void OnUpdate()
		{
			UpdateAlive();

			UpdateDead();

			affected.Clear();
		}

		private void UpdateAlive()
		{
			if (!_query_added.IsEmptyIgnoreFilter)
			{
				var _added_job = new _new_added_job
				{
					hidden = _hidden.AsParallelWriter(),
					added = _new_added.AsParallelWriter()
				}.Schedule(_query_added);

				_added_job.Complete();

				while (_new_added.TryDequeue(out byte index))
				{
					if (_buckets.Minions.TryGetValue(index, out MinionClientBucket bucket))
					{
						if (EntityManager.HasComponent<AssasinBehaviour>(bucket.entity))
						{
							var _assasin = EntityManager.GetComponentObject<AssasinBehaviour>(bucket.entity);
							affected.Add(_assasin);
							_assasin.Hide();
						}
					}
				}
			}
		}

		private void UpdateDead()
		{
			if (!_query_deleted.IsEmptyIgnoreFilter)
			{
				var _effects = _query_deleted.ToComponentDataArray<EffectData>(Allocator.TempJob);
				for (int i = 0; i < _effects.Length; ++i)
				{
					var _effect = _effects[i];
					_hidden.Remove(_effect.source);
					if (_buckets.Minions.TryGetValue(_effect.source, out MinionClientBucket bucket))
					{
						if (EntityManager.HasComponent<AssasinBehaviour>(bucket.entity))
						{
							var _assasin = EntityManager.GetComponentObject<AssasinBehaviour>(bucket.entity);
							if(!affected.Contains(_assasin))
								_assasin.Unhide();
						}
						else if (EntityManager.HasComponent<StartEffect>(bucket.entity))
						{
							var _StartEffect = EntityManager.GetComponentObject<StartEffect>(bucket.entity);
							_StartEffect.SetWait(false);
						}
					}
				}
				_effects.Dispose();
			}
		}

		[Unity.Burst.BurstCompile]
		struct _new_added_job : IJobForEachWithEntity<EffectData>
		{
			internal NativeHashMap<byte, Entity>.ParallelWriter hidden;
			internal NativeQueue<byte>.ParallelWriter added;

			public void Execute(Entity entity, int index, [ReadOnly] ref EffectData effect)
			{
				if (hidden.TryAdd(effect.source, entity))
				{
					added.Enqueue(effect.source);
				}
			}
		}
	}
}

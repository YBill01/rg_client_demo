using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Legacy.Database;

namespace Legacy.Client
{
	//[UpdateInGroup(typeof(BattlePresentation))]
	//[UpdateBefore(typeof(AnimationSystem))]
	//[UpdateAfter(typeof(MinionTransformSystem))]
	[DisableAutoCreation]
	public class ContinuousAttackAnimationSystem : ComponentSystem
	{
        private BattleBucketsSystem _buckets;
        private NativeHashMap<Entity, MinionState> _states;
		private NativeQueue<Entity> _changes;
		private NativeQueue<Entity> _updated;
		private EntityQuery _spawn_added;
		private EntityQuery _spawn_removed;
		private EntityQuery _spawn_updated;

		protected override void OnCreate()
		{
			_spawn_added = GetEntityQuery(
				ComponentType.ReadOnly<MinionData>(),
				ComponentType.Exclude<MinionAnimation>()
			);

			_spawn_removed = GetEntityQuery(
				ComponentType.ReadOnly<MinionAnimation>(),
				ComponentType.Exclude<MinionData>()
			);

			_spawn_updated = GetEntityQuery(
				ComponentType.ReadOnly<MinionData>(),
				ComponentType.ReadOnly<EntityDatabase>(),
				ComponentType.ReadOnly<Animator>(),
				ComponentType.ReadOnly<MinionAnimation>(),
				ComponentType.ReadOnly<ContiniousAtackBehaviour>()
			);

			_states = new NativeHashMap<Entity, MinionState>(128, Allocator.Persistent);
			_changes = new NativeQueue<Entity>(Allocator.Persistent);
			_updated = new NativeQueue<Entity>(Allocator.Persistent);

			_buckets = World.GetOrCreateSystem<BattleBucketsSystem>();
			RequireForUpdate(_spawn_updated);
        }

		protected override void OnDestroy()
		{
			_states.Dispose();
			_changes.Dispose();
			_updated.Dispose();
		}

		protected override void OnUpdate()
		{
			if (!_spawn_updated.IsEmptyIgnoreFilter)
			{
				var inputDeps = new UpdateJob
				{
					changes = _changes.AsParallelWriter(),
					states = _states
				}.Schedule(_spawn_updated);

				inputDeps.Complete();
				Debug.Log("Ghost animation update");
				if (_changes.Count > 0)
				{
					Debug.Log("Ghost animation changes");
					while (_changes.TryDequeue(out Entity entity))
					{
						var _minion = EntityManager.GetComponentData<MinionData>(entity);
						_states[entity] = _minion.state;


						var _animator = EntityManager.GetComponentObject<Animator>(entity);
						if (_animator)
						{
							_animator.SetBool("Landing", false);
							_animator.SetBool("Stand", false);
							_animator.SetBool("Walk", false);
							_animator.SetBool("Death", false);
							_animator.SetBool("Skill1", false);
							_animator.SetBool("Skill2", false);
							_animator.enabled = true;
							_animator.speed = 1;

							var dbdata = EntityManager.GetComponentData<EntityDatabase>(entity);

							var _info = _animator.GetCurrentAnimatorClipInfo(0);
							if (_info.Length > 0)
							{
								var hitRange = _animator.gameObject.GetComponent<RangeHitEffect>();
								var continuousBehaviour = _animator.gameObject.GetComponent<ContiniousAtackBehaviour>();
								if(_minion.state != MinionState.Attack)
								{
									continuousBehaviour.StopAttack();
								}
								switch (_minion.state)
								{
									case MinionState.Spawn:
										_animator.SetBool("Landing", true);
										break;

									case MinionState.Paralize:
										_animator.enabled = false;
										break;

									case MinionState.Skill1:
										_animator.Play("Skill1");
										break;

									case MinionState.Skill2:
										_animator.Play("Skill2");
										break;

									case MinionState.Idle:
										_animator.SetBool("Stand", true);
										break;

									case MinionState.Move:
										if (hitRange != null)
										{
											hitRange.Charge(false);
										}
										_animator.SetBool("Walk", true);
										_animator.speed = _minion.mspeed * 0.01f;
										break;

									case MinionState.Death:
										if (hitRange != null)
										{
											hitRange.Charge(false);
										}
										_animator.gameObject.transform.Find("shadow").gameObject.SetActive(false);
										var panel = _animator.gameObject.GetComponent<MinionPanel>();
										if (panel != null && panel.panel != null)
										{
											panel.panel.SetActive(false);
										}
										_animator.SetBool("Death", true);
										break;

									case MinionState.Charge:
									case MinionState.Attack:
										Debug.Log("Ghost attack");
										if (_buckets.Minions.TryGetValue(_minion.atarget, out MinionClientBucket bucket))
										{
											Debug.Log("Ghost attack has target");
											if (EntityManager.HasComponent<Transform>(bucket.entity) && bucket.minion.state != MinionState.Death)
											{
												var t = EntityManager.GetComponentObject<Transform>(bucket.entity);
												continuousBehaviour.StartAttack(t);
												Debug.Log("Ghost attack target is alive");
											}
											else
											{
												continuousBehaviour.StopAttack();
												Debug.Log("Stop attack");
											}
										}
										else
										{
											Debug.Log("No Ghost attack target");
										}
										continuousBehaviour.StartAttack();
										var _charge_time = _minion.aspeed * 0.001f;

										//_animator.SetFloat("_multiplier_attack", _minion.aspeed / 100f);
										_animator.SetFloat("_multiplier_attack", 1);
										_animator.speed = _minion.aspeed * 0.01f;
										_animator.SetBool("Attack", true);
										if (hitRange != null)
											hitRange.Charge(true);
										break;
								}
							}
						}
					}
				}
			}

			if (!_spawn_added.IsEmptyIgnoreFilter)
			{
				var inputDeps = new AddedJob
				{
					updated = _updated.AsParallelWriter(),
					states = _states.AsParallelWriter(),
				}.Schedule(_spawn_added);

				inputDeps.Complete();

				while (_updated.TryDequeue(out Entity entity))
				{
					PostUpdateCommands.AddComponent(entity, default(MinionAnimation));
				}
			}

			if (!_spawn_removed.IsEmptyIgnoreFilter)
			{
				var inputDeps = new RemovedJob
				{
					updated = _updated.AsParallelWriter()
				}.Schedule(_spawn_removed);

				inputDeps.Complete();

				while (_updated.TryDequeue(out Entity entity))
				{
					_states.Remove(entity);
					if (EntityManager.Exists(entity))
					{
						PostUpdateCommands.RemoveComponent<MinionAnimation>(entity);
					}					
				}
			}
		}

		[Unity.Burst.BurstCompile]
		struct UpdateJob : IJobForEachWithEntity<MinionData>
		{
			[ReadOnly] public NativeHashMap<Entity, MinionState> states;
			public NativeQueue<Entity>.ParallelWriter changes;

			public void Execute(Entity entity, int index, [ReadOnly] ref MinionData minion)
			{
				if (states.TryGetValue(entity, out MinionState state))
				{
					if (minion.state != state)
					{
						changes.Enqueue(entity);
					}
				}
			}
		}


		[Unity.Burst.BurstCompile]
		struct RemovedJob : IJobForEachWithEntity<MinionAnimation>
		{
			public NativeQueue<Entity>.ParallelWriter updated;
			public void Execute(Entity entity, int index, [ReadOnly] ref MinionAnimation c0)
			{
				updated.Enqueue(entity);
			}
		}

		[Unity.Burst.BurstCompile]
		struct AddedJob : IJobForEachWithEntity<MinionData>
		{
			public NativeHashMap<Entity, MinionState>.ParallelWriter states;
			public NativeQueue<Entity>.ParallelWriter updated;

			public void Execute(Entity entity, int index, [ReadOnly] ref MinionData minion)
			{
				if (states.TryAdd(entity, MinionState.Undefined))
				{
					updated.Enqueue(entity);
				}
			}
		}

	}
}

